using DocumentFormat.OpenXml.Drawing;
using Microsoft.UI.Xaml.Controls;
using RectifyPad;
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
        public MainPage MainPagea { get; set; }  
        public ParagraphToolbar()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Editor.SelectionChanged += editor_SelectionChanged;
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

        public string GetText(RichEditBox RichEditor)
        {
            RichEditor.Document.GetText(TextGetOptions.FormatRtf, out string Text);
            ITextRange Range = RichEditor.Document.GetRange(0, Text.Length);
            Range.GetText(TextGetOptions.FormatRtf, out string Value);
            return Value;
        }

        private void SetParagraphIndents(float leftIndent, float rightIndent, float firstLineIndent, bool applyToSelectionOnly = true)
        {
            // Get the ITextDocument interface for the RichEditBox's document
            ITextDocument document = Editor.Document;

            // Get the current selection's start and end positions
            int start = document.Selection.StartPosition;
            int end = document.Selection.EndPosition;

            // If applyToSelectionOnly is true, check if there's any selected text in the RichEditBox
            if (applyToSelectionOnly && start == end)
            {
                //return;
            }

            // Get the ITextRange interface for the selection or the entire document
            ITextRange textRange;
            if (applyToSelectionOnly)
            {
                textRange = document.Selection;
            }
            else
            {
                textRange = document.GetRange(0, GetText(Editor).Length);
            }

            // Get the ITextParagraphFormat interface for the text range
            ITextParagraphFormat paragraphFormat = textRange.ParagraphFormat;

            // Set the left and right indents for the current selection's paragraph(s)
            try
            {
                if (document.Selection.Length != 0)
                {
                    paragraphFormat.SetIndents(firstLineIndent, leftIndent, rightIndent);
                }
                else
                {
                    document.GetRange(document.Selection.StartPosition, document.Selection.EndPosition + 1);
                    paragraphFormat.SetIndents(firstLineIndent, leftIndent, rightIndent);
                }
            }
            catch
            {

            }

            // Apply the new paragraph format to the current selection or the entire document
            textRange.ParagraphFormat = paragraphFormat;

            // LeftIndent.Text = leftIndent.ToString();

            // RightIndent.Text = rightIndent.ToString();
        }
        public async void ShowParagraphDialog()
        {
            // Create an instance of the ParagraphDialog
            ParagraphDialog paragraphDialog = new ParagraphDialog();
            TextBox leftTextBox = (TextBox)paragraphDialog.FindName("LeftIndentBox");
            TextBox rightTextBox = (TextBox)paragraphDialog.FindName("RightIndentBox");
            TextBox firstLineTextBox = (TextBox)paragraphDialog.FindName("OneLineBox");
            ComboBox lineSpacingComboBox = (ComboBox)paragraphDialog.FindName("LineSpacingCombo");
            ComboBox align = (ComboBox)paragraphDialog.FindName("AlignCombo");

            if (Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Left)
            {
                align.SelectedItem = "Left";
            }
            if (Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Right)
            {
                align.SelectedItem = "Right";
            }
            if (Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Justify)
            {
                align.SelectedItem = "Justified";
            }
            if (Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Center)
            {
                align.SelectedItem = "Center";
            }
            if (Editor.Document.Selection.ParagraphFormat.LineSpacingRule == LineSpacingRule.Multiple &&
                Editor.Document.Selection.ParagraphFormat.LineSpacing == 1)
            {
                lineSpacingComboBox.SelectedItem = "1,00";
            }
            if (Editor.Document.Selection.ParagraphFormat.LineSpacingRule == LineSpacingRule.Multiple &&
                Editor.Document.Selection.ParagraphFormat.LineSpacing == (float)1.15)
            {
                lineSpacingComboBox.SelectedItem = "1,15";
            }
            if (Editor.Document.Selection.ParagraphFormat.LineSpacingRule == LineSpacingRule.Multiple &&
                Editor.Document.Selection.ParagraphFormat.LineSpacing == (float)1.50)
            {
                lineSpacingComboBox.SelectedItem = "1,50";
            }
            if (Editor.Document.Selection.ParagraphFormat.LineSpacingRule == LineSpacingRule.Multiple &&
            Editor.Document.Selection.ParagraphFormat.LineSpacing == 2)
            {
                lineSpacingComboBox.SelectedItem = "2,00";
            }
            leftTextBox.Text = Editor.Document.Selection.ParagraphFormat.LeftIndent.ToString();

            rightTextBox.Text = Editor.Document.Selection.ParagraphFormat.RightIndent.ToString();

            firstLineTextBox.Text = Editor.Document.Selection.ParagraphFormat.FirstLineIndent.ToString();


            // Show the dialog and wait for the user's input
            ContentDialogResult result = await paragraphDialog.ShowAsync();

            // If the user clicked the OK button, adjust the properties of the RichEditBox
            if (result == ContentDialogResult.Primary)
            {
                TabsDialog tabsDialog = new TabsDialog();
                ContentDialogResult result2 = await tabsDialog.ShowAsync();
            }

            // If the user clicked the OK button, adjust the properties of the RichEditBox
            if (result == ContentDialogResult.Secondary)
            {
                // Set properties of the RichEditBox based on the values from controls
                if (align.SelectedItem as string == "Left")
                {
                    Editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                }
                else if (align.SelectedItem as string == "Right")
                {
                    Editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                }
                else if (align.SelectedItem as string == "Justified")
                {
                    Editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
                }
                else if (align.SelectedItem as string == "Center")
                {
                    Editor.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                }

                if (lineSpacingComboBox.SelectedItem.ToString() == "1,00")
                {
                    Editor.Document.Selection.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, 1);
                }
                else if(lineSpacingComboBox.SelectedItem.ToString() == "1,15")
                {

                    Editor.Document.Selection.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, (float)1.15);
                }
                else if(lineSpacingComboBox.SelectedItem.ToString() == "1,50")
                {

                    Editor.Document.Selection.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, (float)1.50);
                }
                else if(lineSpacingComboBox.SelectedItem.ToString() == "2,00")
                {
                    Editor.Document.Selection.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, 2);
                }


                float.TryParse(leftTextBox.Text, out float leftIndent);
                float.TryParse(rightTextBox.Text, out float rightIndent);
                float.TryParse(firstLineTextBox.Text, out float firstLineIndent);
                SetParagraphIndents(leftIndent, rightIndent, firstLineIndent, false);
            }
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

        private void BulletButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs e)
        {
            if (Editor.Document.Selection.ParagraphFormat.ListType != MarkerType.None)
            {
                Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.None;
                TextBulletingButton.IsChecked = false;
            }
            else
            {
                Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.Bullet;
                TextBulletingButton.IsChecked = true;
            }
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
            AlignJustifyButton.IsChecked = Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Justify;
            TextBulletingButton.IsChecked = Editor.Document.Selection.ParagraphFormat.ListType != MarkerType.None;
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
            ShowParagraphDialog();
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
            Editor.Document.Selection.ParagraphFormat.SetIndents(Editor.Document.Selection.ParagraphFormat.FirstLineIndent,
                                                                 Editor.Document.Selection.ParagraphFormat.LeftIndent + 10,
                                                                 Editor.Document.Selection.ParagraphFormat.RightIndent - 10);
        }

        private void IndentationIncreaseLeft_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.SetIndents(Editor.Document.Selection.ParagraphFormat.FirstLineIndent,
                                                                 Editor.Document.Selection.ParagraphFormat.LeftIndent - 10,
                                                                 Editor.Document.Selection.ParagraphFormat.RightIndent + 10);
        }

        private void RadioMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, 1);
        }

        private void RadioMenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {

            Editor.Document.Selection.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, (float)1.15);
        }

        private void RadioMenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, (float)1.5);
        }

        private void RadioMenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, 2);
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