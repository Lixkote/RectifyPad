using RectifyPad;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Szablon elementu Kontrolka użytkownika jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234236

namespace WordPad.WordPadUI
{
    public sealed partial class Pageprop : ContentDialog
    {
        public Pageprop()
        {
            this.InitializeComponent();
            // Load the saved unit value


            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.TryGetValue("orientation", out object value))
            {
                string yesorno = value.ToString();
                if (yesorno != null)
                {
                    if (yesorno == "Portrait")
                    {
                        orientationportait.IsChecked = true;
                        orientationlandscape.IsChecked = false;
                    }
                    if (yesorno == "Landscape")
                    {
                        orientationportait.IsChecked = false;
                        orientationlandscape.IsChecked = true;
                    }
                }
            }
            // Load the saved unit value
            string valueb = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["pagesetupBmargin"];
            if (valueb != null)
            {
                BottomMarginTextBox.Text = valueb;
            }
            string valuet = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["pagesetupTmargin"];
            if (valuet != null)
            {
                TopMarginTextBox.Text = valuet;
            }
            string valuel = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["pagesetupLmargin"];
            if (valuel != null)
            {
                LeftMarginTextBox.Text = valuel;
            }
            string valuer = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["pagesetupRmargin"];
            if (valuer != null)
            {
                RightMarginTextBox.Text = valuer;
            }
        }

        private void exampletext_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the values from the textboxes
                double left = double.Parse(LeftMarginTextBox.Text);
                double right = double.Parse(RightMarginTextBox.Text);
                double top = double.Parse(TopMarginTextBox.Text);
                double bottom = double.Parse(BottomMarginTextBox.Text);

                // Load the saved unit value
                string unit = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["unitSetting"];

                // Default pixels per unit
                double pixelsPerUnit = 1.0; // Default to 1 pixel per unit (no scaling)

                if (unit != null)
                {
                    switch (unit)
                    {
                        case "Inches":
                            pixelsPerUnit = 96.0; // 96 pixels per inch
                            break;
                        case "Centimeters":
                            pixelsPerUnit = 37.8; // 37.8 pixels per centimeter
                            break;
                        case "Points":
                            pixelsPerUnit = 1.33; // 1.33 pixels per point
                            break;
                        case "Cicera":
                            pixelsPerUnit = 4.5; // 4.5 pixels per cicero
                            break;
                        default:
                            // Handle unexpected unit values here, e.g., set a default or show an error message.
                            break;
                    }
                }
                else
                {
                    // Handle the exception
                    Debug.WriteLine("Unit not found");
                }

                // Calculate the margin based on the unit and values
                double maxWidth = 117; // Width of your TextBlock
                double maxHeight = 343; // Height of your TextBlock

                double maxLeftMargin = maxWidth - right * pixelsPerUnit;
                double maxRightMargin = maxWidth - left * pixelsPerUnit;
                double maxTopMargin = maxHeight - bottom * pixelsPerUnit;
                double maxBottomMargin = maxHeight - top * pixelsPerUnit;

                double finalLeftMargin = Math.Max(0, Math.Min(left * pixelsPerUnit, maxLeftMargin));
                double finalRightMargin = Math.Max(0, Math.Min(right * pixelsPerUnit, maxRightMargin));
                double finalTopMargin = Math.Max(0, Math.Min(top * pixelsPerUnit, maxTopMargin));
                double finalBottomMargin = Math.Max(0, Math.Min(bottom * pixelsPerUnit, maxBottomMargin));

                Thickness margin = new Thickness(
                    finalLeftMargin,
                    finalTopMargin,
                    finalRightMargin,
                    finalBottomMargin);

                // Set the margin of the textblock
                exampletext.Margin = margin;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during parsing or margin calculation
                Debug.WriteLine("An error occurred: " + ex.Message);
            }

        }

        private void printpagenumbers_Loaded(object sender, RoutedEventArgs e)
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
    }
}
