using RectifyPad;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WordPad.WordPadUI
{
    public sealed partial class Pageprop : ContentDialog
    {
        private Dictionary<string, (double Width, double Height)> paperSizes;
        private Thickness currentMargins;
        private bool isOrientationChanged = false;

        public Pageprop()
        {
            InitializeComponent();
            LoadUI();
        }

        private void LoadUI()
        {
            var settings = ApplicationData.Current.LocalSettings;

            // Define the dimensions for each paper size based on A4 as a reference
            paperSizes = new Dictionary<string, (double Width, double Height)>
            {
                { "A3", (165 * 297 / 210, 165 * 210 / 297) },
                { "A4", (165, 120) },
                { "B5_1", (120 * 176 / 250, 165 * 250 / 176) },
                { "B5_2", (120 * 176 / 250, 165 * 250 / 176) },
                { "Executive", (120 * 184 / 215, 165 * 215 / 184) },
                { "Legal", (120 * 216 / 356, 165 * 356 / 216) },
                { "Letter", (120 * 216 / 279, 165 * 279 / 216) },
                { "Tabloid", (120 * 279 / 432, 165 * 432 / 279) }
            };

            if (settings.Values.TryGetValue("papersize", out object value))
            {
                string orientation = (string)settings.Values["orientation"];
                string selectedPaperSize = value.ToString();

                if (paperSizes.TryGetValue(selectedPaperSize, out var dimensions))
                {
                    SetPaperSizeAndOrientation(dimensions.Width, dimensions.Height, orientation);
                    PaperTypeCombo.SelectedIndex = paperSizes.Keys.ToList().IndexOf(selectedPaperSize);
                }
            }

            // Load the saved margin values
            LoadMarginValues();

            // Load the Print Page Numbers setting
            LoadPrintPageNumbersSetting();
        }

        private void SetPaperSizeAndOrientation(double width, double height, string orientation)
        {
            // Swap width and height if orientation is Landscape
            if (orientation == "Portrait")
            {
                double temp = width;
                width = height;
                height = temp;
                isOrientationChanged = true;
            }

            Paper.Width = width;
            Paper.Height = height;

            // Update the exampletextgrid margin
            UpdateMarginPreview();

            // Update the orientation radio buttons
            orientationportait.IsChecked = orientation == "Landscape";
            orientationlandscape.IsChecked = orientation == "Portrait";
        }

        private void LoadMarginValues()
        {
            // Load margin values and set limits
            string unit = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["unitSetting"];
            double maxMargin = 100.0; // Set a maximum margin value

            LoadAndLimitMarginValue(LeftMarginTextBox, "pagesetupLmargin", unit, maxMargin);
            LoadAndLimitMarginValue(RightMarginTextBox, "pagesetupRmargin", unit, maxMargin);
            LoadAndLimitMarginValue(TopMarginTextBox, "pagesetupTmargin", unit, maxMargin);
            LoadAndLimitMarginValue(BottomMarginTextBox, "pagesetupBmargin", unit, maxMargin);

            // Update the exampletextgrid margin based on the loaded values
            UpdateMarginPreview();
        }

        private void LoadAndLimitMarginValue(TextBox textBox, string settingKey, string unit, double maxMargin)
        {
            var settings = ApplicationData.Current.LocalSettings;

            if (settings.Values.TryGetValue(settingKey, out object value))
            {
                double margin = Convert.ToDouble(value);

                // Limit the margin value
                margin = Math.Min(maxMargin, margin);
                textBox.Text = margin.ToString();
            }
        }

        private void UpdateMarginPreview()
        {
            try
            {
                double left = double.Parse(LeftMarginTextBox.Text);
                double right = double.Parse(RightMarginTextBox.Text);
                double top = double.Parse(TopMarginTextBox.Text);
                double bottom = double.Parse(BottomMarginTextBox.Text);

                double maxWidth = exampletextgrid.Width;
                double maxHeight = exampletextgrid.Height;

                double maxLeftMargin = maxWidth - right;
                double maxRightMargin = maxWidth - left;
                double maxTopMargin = maxHeight - bottom;
                double maxBottomMargin = maxHeight - top;

                double finalLeftMargin = Math.Max(0, Math.Min(left, maxLeftMargin));
                double finalRightMargin = Math.Max(0, Math.Min(right, maxRightMargin));
                double finalTopMargin = Math.Max(0, Math.Min(top, maxTopMargin));
                double finalBottomMargin = Math.Max(0, Math.Min(bottom, maxBottomMargin));

                currentMargins = new Thickness(
                    finalLeftMargin,
                    finalTopMargin,
                    finalRightMargin,
                    finalBottomMargin);

                exampletextgrid.Margin = currentMargins;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during parsing or margin calculation
                Debug.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void LoadPrintPageNumbersSetting()
        {
            string no = "no";
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.TryGetValue("is10ptenabled", out object value))
            {
                string yesorno = value.ToString();
                if (yesorno != null)
                {
                    if (yesorno != no)
                    {
                        printpagenumbers.IsChecked = true;
                    }
                }
            }
        }

        private void marginsname_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the saved unit value
            string unit = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["unitSetting"];
            marginsname.Text = "Margins (" + unit + ")";
        }

        private void orientationportait_Checked(object sender, RoutedEventArgs e)
        {
            if (isOrientationChanged)
            {
                // Swap width and height back to Portrait orientation
                double temp = Paper.Width;
                Paper.Width = Paper.Height;
                Paper.Height = temp;
                isOrientationChanged = false;
            }

            UpdateMarginPreview();
        }

        private void orientationlandscape_Checked(object sender, RoutedEventArgs e)
        {
            if (!isOrientationChanged)
            {
                // Swap width and height for Landscape orientation
                double temp = Paper.Width;
                Paper.Width = Paper.Height;
                Paper.Height = temp;
                isOrientationChanged = true;
            }

            UpdateMarginPreview();
        }

        private void PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Save the selected paper size and orientation
            var settings = ApplicationData.Current.LocalSettings;
            if (PaperTypeCombo.SelectedItem != null)
            {
                string selectedPaperSize = (PaperTypeCombo.SelectedItem as ComboBoxItem).Content.ToString();
                settings.Values["papersize"] = selectedPaperSize;
            }

            settings.Values["orientation"] = orientationportait.IsChecked == true ? "Portrait" : "Landscape";

            // Save margin values
            settings.Values["pagesetupLmargin"] = LeftMarginTextBox.Text;
            settings.Values["pagesetupRmargin"] = RightMarginTextBox.Text;
            settings.Values["pagesetupTmargin"] = TopMarginTextBox.Text;
            settings.Values["pagesetupBmargin"] = BottomMarginTextBox.Text;

            // Save Print Page Numbers setting
            settings.Values["is10ptenabled"] = printpagenumbers.IsChecked == true ? "yes" : "no";
        }

        private void PaperTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaperTypeCombo.SelectedItem != null)
                {
                    string selectedPaperSize = (PaperTypeCombo.SelectedItem as ComboBoxItem).Content.ToString();
                    if (paperSizes.TryGetValue(selectedPaperSize, out var dimensions))
                    {
                        double width = dimensions.Width;
                        double height = dimensions.Height;

                        // Swap width and height if the orientation is Landscape
                        if (orientationlandscape.IsChecked == true)
                        {
                            double temp = width;
                            width = height;
                            height = temp;
                        }

                        Paper.Width = width;
                        Paper.Height = height;
                    }
                }
        }
    }
}
