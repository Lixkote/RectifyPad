using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WordPad.Helpers;

// Szablon elementu Kontrolka użytkownika jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234236

namespace WordPad.WordPadUI.Ribbon
{
    public sealed partial class ParagraphToolbar : UserControl
    {
        public RichEditBox Editor { get; set; }
        public ParagraphToolbar()
        {
            this.InitializeComponent();
        }

        public object ConvertString2Float(string value, Type targetType, object parameter, string language)
        {
            // Convert a string to a float.
            if (value is string stringValue)
            {
                if (float.TryParse(stringValue, out float result))
                {
                    return result;
                }
            }

            return 0.0f; // Default value if the conversion fails.
        }

        private void LoadSettings()
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
                        _10pt.IsChecked = true;
                    }
                }
            }


            string linespacingval = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["linespacing"];
            Debug.WriteLine("Detected Spacing: " + linespacingval);

            float lineSpacingValue = 1.0f; // Default value

            switch (linespacingval)
            {
                case "1.0":
                    lineSpacingValue = 1.0f;
                    spacingradio1.IsChecked = true;
                    break;
                case "1.15":
                    lineSpacingValue = 1.15f;
                    spacingradio2.IsChecked = true;
                    break;
                case "1.5":
                    lineSpacingValue = 1.5f;
                    spacingradio3.IsChecked = true;
                    break;
                case "2":
                    lineSpacingValue = 2.0f;
                    spacingradio4.IsChecked = true;
                    break;
                default:
                    break;
            }
            float halal = (float)ConvertString2Float(linespacingval, typeof(float), null, null);

            // Get the current document from the RichEditBox
            ITextDocument document = Editor.Document;

            // Get the current selection from the document
            ITextSelection selection = document.Selection;

            // Get the paragraph format of the selection
            ITextParagraphFormat paragraphFormat = selection.ParagraphFormat;

            // Set the line spacing rule to multiple
            paragraphFormat.SetLineSpacing(LineSpacingRule.Multiple, halal);
        }


        private void NoneNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.None;
            TextBulletingButton.IsChecked = false;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void DottedNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.Bullet;
            TextBulletingButton.IsChecked = true;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void BulletButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBullet = (Button)sender;
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.Bullet;

            TextBulletingButton.IsChecked = true;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void NumberNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.Arabic;
            TextBulletingButton.IsChecked = true;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void LetterSmallNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.LowercaseEnglishLetter;
            TextBulletingButton.IsChecked = true;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void LetterBigNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.UppercaseEnglishLetter;
            TextBulletingButton.IsChecked = true;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void SmalliNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.LowercaseRoman;
            TextBulletingButton.IsChecked = true;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void BigINumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.UppercaseRoman;
            TextBulletingButton.IsChecked = true;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void AlignJustifyButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.AlignSelectedTo(RichEditHelpers.AlignMode.Justify);
            editor_SelectionChanged(sender, e);
        }

        private void editor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AlignLeftButton.IsChecked = Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Left;
            AlignCenterButton.IsChecked = Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Center;
            AlignRightButton.IsChecked = Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Right;
            if (Editor.Document.Selection.CharacterFormat.Size > 0)
            {
                //font size is negative when selection contains multiple font sizes
                //FontSizeBox. = Editor.Document.Selection.CharacterFormat.Size;
            }
            //prevent accidental font changes when selection contains multiple styles
            // Get a reference to the RichEditBox control
            RichEditBox richEditBox = Editor;
        }


        private async void ParagraphButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the ParagraphDialog
            ParagraphDialog paragraphDialog = new ParagraphDialog();

            // Show the dialog and wait for the user's input
            ContentDialogResult result = await paragraphDialog.ShowAsync();

            // If the user clicked the OK button, adjust the properties of the RichEditBox
            if (result == ContentDialogResult.Primary)
            {
                // Get the values from the dialog's TextBoxes and ComboBoxes
                TextBox leftTextBox = (TextBox)paragraphDialog.FindName("LeftTextBox");
                TextBox rightTextBox = (TextBox)paragraphDialog.FindName("RightTextBox");
                TextBox firstLineTextBox = (TextBox)paragraphDialog.FindName("FirstLineTextBox");
                ComboBox lineSpacingComboBox = (ComboBox)paragraphDialog.FindName("LineSpacingComboBox");

                // Parse the values and set the properties of the RichEditBox
                double left = double.Parse(leftTextBox.Text);
                double right = double.Parse(rightTextBox.Text);
                double firstLine = double.Parse(firstLineTextBox.Text);
                double lineSpacing = double.Parse(lineSpacingComboBox.SelectedItem.ToString());

                Editor.Margin = new Thickness(left, 0, right, 0);
                Editor.Document.Selection.ParagraphFormat.SetIndents((float)firstLine, 0, 0);
                Editor.Document.Selection.ParagraphFormat.SetLineSpacing(Windows.UI.Text.LineSpacingRule.AtLeast, (float)lineSpacing);
            }
        }

        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.AlignSelectedTo(RichEditHelpers.AlignMode.Right);
            editor_SelectionChanged(sender, e);
        }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.AlignSelectedTo(RichEditHelpers.AlignMode.Left);
            editor_SelectionChanged(sender, e);
        }

        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.AlignSelectedTo(RichEditHelpers.AlignMode.Center);
            editor_SelectionChanged(sender, e);
        }

        private void IndentationIncreaseRight_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IndentationIncreaseLeft_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            string selectedspacing = ((RadioMenuFlyoutItem)sender)?.Tag?.ToString();
            // Save the selected theme in app data
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["linespacing"] = selectedspacing;

            float halal = (float)ConvertString2Float(selectedspacing, typeof(float), null, null);

            // Get the current document from the RichEditBox
            ITextDocument document = Editor.Document;

            // Get the current selection from the document
            ITextSelection selection = document.Selection;

            // Get the paragraph format of the selection
            ITextParagraphFormat paragraphFormat = selection.ParagraphFormat;

            // Set the line spacing rule to multiple
            paragraphFormat.SetLineSpacing(LineSpacingRule.Multiple, halal);

        }

        private void ParagraphSettingButton_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MenuFlyout_Closed(object sender, object e)
        {
            // Save the selected paper size and orientation
            var settings = ApplicationData.Current.LocalSettings;
            // Save Print Page Numbers setting
            settings.Values["isprintpagenumbers"] = _10pt.IsChecked == true ? "yes" : "no";
        }
    }
}