using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using WordPad.Helpers;

namespace WordPad.WordPadUI.Ribbon
{
    public sealed partial class ParagraphToolbar : UserControl
    {
        public RichEditBox Editor { get; set; }

        public ParagraphToolbar()
        {
            this.InitializeComponent();
        }

        private void LoadSettings()
        {
            string linespacingval = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["linespacing"];
            double lineSpacingValue = 1.0;

            if (linespacingval == "1,0")
                lineSpacingValue = 1.0;
            else if (linespacingval == "1,15")
                lineSpacingValue = 1.15;
            else if (linespacingval == "1,5")
                lineSpacingValue = 1.5;
            else if (linespacingval == "2")
                lineSpacingValue = 2.0;

            SetLineSpacing(lineSpacingValue);
        }

        private void SetLineSpacing(double lineSpacingValue)
        {
            ITextDocument document = Editor.Document;
            ITextSelection selection = document.Selection;
            ITextParagraphFormat paragraphFormat = selection.ParagraphFormat;
            paragraphFormat.SetLineSpacing(LineSpacingRule.Multiple, (float)lineSpacingValue);
        }

        // Handle the bullet and numbering buttons' click events here (similar to your existing code)

        private void AlignButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton alignButton)
            {
                RichEditHelpers.AlignMode alignMode = RichEditHelpers.AlignMode.Left;

                if (alignButton == AlignCenterButton)
                    alignMode = RichEditHelpers.AlignMode.Center;
                else if (alignButton == AlignRightButton)
                    alignMode = RichEditHelpers.AlignMode.Right;

                Editor.AlignSelectedTo(alignMode);
                editor_SelectionChanged(sender, e);
            }
        }

        private void IndentationIncreaseRight_Click(object sender, RoutedEventArgs e)
        {
            // Handle the increase right indentation here
        }

        private void IndentationIncreaseLeft_Click(object sender, RoutedEventArgs e)
        {
            // Handle the increase left indentation here
        }

        private void RadioMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioMenuFlyoutItem selectedItem)
            {
                string selectedItemText = selectedItem.Text;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["linespacing"] = selectedItemText;

                if (double.TryParse(selectedItemText, out double lineSpacingValue))
                {
                    SetLineSpacing(lineSpacingValue);
                }
            }
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

        private void NumberNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.Arabic;
            TextBulletingButton.IsChecked = true;
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

        private void NoneNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.None;
            TextBulletingButton.IsChecked = false;
            TextBulletingButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        // Add this method to the revised code I provided earlier.

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

        // Add this method to the revised code I provided earlier.




        private void ParagraphSettingButton_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private void editor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Handle selection changes here (similar to your existing code)
        }
    }
}
