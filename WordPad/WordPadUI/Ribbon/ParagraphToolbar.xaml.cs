using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    }
}
