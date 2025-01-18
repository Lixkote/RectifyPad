using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
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
    public sealed partial class FontToolbar : UserControl
    {
        private bool textboxwasalreadyloaded = false;
        private bool updateFontFormat = true;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public RichEditBox Editor { get; set; }

        public List<double> FontSizes { get; } = new List<double>()
            {
                8,
                9,
                10,
                11,
                12,
                14,
                16,
                18,
                20,
                24,
                28,
                36,
                48,
                72
            };

        private void UpdateFontSize()
        {
            fontSizeComboBox.SelectedItem = Editor.FontSize;
        }

        private void CancelColor_Click(object sender, RoutedEventArgs e)
        {
            // Cancel flyout
            colorPickerButton.Flyout.Hide();
        }

        private void ConfirmColor_Click(object sender, RoutedEventArgs e)
        {
            // Confirm color picker choice and apply color to text
            Windows.UI.Color color = myColorPicker.Color;
            Editor.Document.Selection.CharacterFormat.ForegroundColor = color;
            FontColorMarker.SetValue(ForegroundProperty, new SolidColorBrush(color));

            // Hide flyout
            colorPickerButton.Flyout.Hide();
            FontTextColorDropDownButton.Flyout.Hide();
            Editor.Focus(FocusState.Programmatic);
        }

        public FontToolbar()
        {
            this.InitializeComponent();
            BackTextColorMarkerMain.FontFamily = (FontFamily)Application.Current.Resources["CustomIconFont"];
            BackTextColorMarker.FontFamily = (FontFamily)Application.Current.Resources["CustomIconFont"]; 
            FontColorMarkerMain.FontFamily = (FontFamily)Application.Current.Resources["CustomIconFont"];
            FontColorMarker.FontFamily = (FontFamily)Application.Current.Resources["CustomIconFont"];
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Editor.SelectionChanged += Editor_SelectionChanged;
        }

        private void FontSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is double selectedValue)
                {
                    Editor.Document.Selection.CharacterFormat.Size = (float)selectedValue;
                    Editor.Focus(FocusState.Programmatic);
                }
            }
        }


        private void FontSizeCombo_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            bool isDouble = double.TryParse(sender.Text, out double newValue);

            // Check if the user selected a predefined font size from the ComboBox.
            if (sender.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is double predefinedSize)
            {
                newValue = predefinedSize;
            }

            // Set the selected item if:
            // - The value successfully parsed to double AND
            // - The value is in the list of sizes OR is a custom value between 8 and 100
            if (isDouble && (FontSizes.Contains(newValue) || (newValue < 300 && newValue > 0)))
            {
                // Update the SelectedItem to the new value. 
                sender.SelectedItem = newValue;
                Editor.Document.Selection.CharacterFormat.Size = (float)newValue;
            }
            else
            {
                // If the item is invalid, reject it and revert the text. 
                sender.Text = sender.SelectedValue?.ToString();

                var dialog = new ContentDialog
                {
                    Content = "The font size must be a number between 0 and 300.",
                    CloseButtonText = "Close",
                    DefaultButton = ContentDialogButton.Close
                };
                var task = dialog.ShowAsync();
            }

            // Mark the event as handled so the framework doesn’t update the selected item automatically. 
            args.Handled = true;
        }

        private void fontSizeComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            {
                fontSizeComboBox.SelectedIndex = 3;

                if ((ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7)))
                {
                    fontSizeComboBox.TextSubmitted += FontSizeCombo_TextSubmitted;
                }
            }
        }

        private void FontsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Editor.Document.Selection == null || !updateFontFormat)
                return;
            try
            {
                // Check if SelectedValue is null before attempting to access it
                var selectedValue = FontsComboBox.SelectedValue;
                if (selectedValue != null)
                {
                    string choosenvalue = selectedValue.ToString();
                    Editor.Document.Selection.CharacterFormat.Name = choosenvalue;
                }
                else
                {
                    // Handle the case where SelectedValue is null (fallback to a default font)
                    Editor.Document.Selection.CharacterFormat.Name = "Calibri";
                }

                // Ensure the editor gets focus
                Editor.Focus(FocusState.Programmatic);
            }
            catch
            {
                // In case of any other unexpected errors, fallback to the default font
                Editor.Document.Selection.CharacterFormat.Name = "Calibri";
                Editor.Focus(FocusState.Programmatic);
            }
        }


        private void FontTextColorDropDownButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            var color = ((SolidColorBrush)FontColorMarker.Foreground).Color;
            Editor.Document.Selection.CharacterFormat.ForegroundColor = color;
            Editor.Focus(FocusState.Keyboard);
        }

        private void FontBackgroundColorDropDownButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            var color = ((SolidColorBrush)BackTextColorMarker.Foreground).Color;
            Editor.Document.Selection.CharacterFormat.BackgroundColor = color;
            Editor.Focus(FocusState.Keyboard);
        }

        private void FontSizesComboBox_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            bool isDouble = double.TryParse(sender.Text, out double newValue);

            // Check if the user selected a predefined font size from the ComboBox.
            if (sender.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is double predefinedSize)
            {
                newValue = predefinedSize;
            }

            // Set the selected item if:
            // - The value successfully parsed to double AND
            // - The value is in the list of sizes OR is a custom value between 8 and 100
            if (isDouble && (FontSizes.Contains(newValue) || (newValue < 200 && newValue > 8)))
            {
                // Update the SelectedItem to the new value. 
                sender.SelectedItem = newValue;
                Editor.Document.Selection.CharacterFormat.Size = (float)newValue;
            }
            else
            {
                // If the item is invalid, reject it and revert the text. 
                sender.Text = sender.SelectedValue?.ToString();

                var dialog = new ContentDialog
                {
                    Content = "The font size must be a number between 8 and 200.",
                    CloseButtonText = "Close",
                    DefaultButton = ContentDialogButton.Close
                };
                var task = dialog.ShowAsync();
            }

            // Mark the event as handled so the framework doesn’t update the selected item automatically. 
            args.Handled = true;
            Editor.Focus(FocusState.Programmatic);
        }

        private void TextBoldButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Bold);
            Editor.Focus(FocusState.Programmatic);
        }

        private void TextItalicButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Italic);
            Editor.Focus(FocusState.Programmatic);
        }

        private void TextUnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Underline);
            Editor.Focus(FocusState.Programmatic);
        }

        private void TextStrikethroughButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Strikethrough);
            Editor.Focus(FocusState.Programmatic);
        }

        private void TextSubscriptButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Subscript);
            Editor.Focus(FocusState.Programmatic);
        }

        private void TextSuperscriptButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Superscript);
            Editor.Focus(FocusState.Programmatic);
        }

        private void FontEnlargeButton_Click(object sender, RoutedEventArgs e)
        {
            // Get a reference to the RichEditBox control
            RichEditBox richEditBox = Editor;

            // Increase the font size of the currently selected text by 2 points
            richEditBox.Document.Selection.CharacterFormat.Size += 2;
            // Get the index of the currently selected item
            fontSizeComboBox.SelectedItem = richEditBox.Document.Selection.CharacterFormat.Size;
        }

        private void FontDislargeButton_Click(object sender, RoutedEventArgs e)
        {
            // Get a reference to the RichEditBox control
            RichEditBox richEditBox = Editor;

            // Decrease the font size of the currently selected text by 2 points
            richEditBox.Document.Selection.CharacterFormat.Size -= 2;
            fontSizeComboBox.SelectedItem = richEditBox.Document.Selection.CharacterFormat.Size;
        }

        private void TextColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var borderone = (Windows.UI.Xaml.Controls.Border)clickedColor.Content;
            var bordertwo = (Windows.UI.Xaml.Controls.Border)borderone.Child;
            var rectangle = (Windows.UI.Xaml.Shapes.Rectangle)bordertwo.Child;
            var color = (rectangle.Fill as SolidColorBrush).Color;
            Editor.Document.Selection.CharacterFormat.ForegroundColor = color;
            FontColorMarker.SetValue(ForegroundProperty, new SolidColorBrush(color));

            FontTextColorDropDownButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void BackColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var borderone = (Windows.UI.Xaml.Controls.Border)clickedColor.Content;
            var bordertwo = (Windows.UI.Xaml.Controls.Border)borderone.Child;
            var rectangle = (Windows.UI.Xaml.Shapes.Rectangle)bordertwo.Child;
            var color = (rectangle.Fill as SolidColorBrush).Color;
            Editor.Document.Selection.CharacterFormat.BackgroundColor = color;
            BackTextColorMarker.SetValue(ForegroundProperty, new SolidColorBrush(color));

            FontBackgroundColorDropDownButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void FontsComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (textboxwasalreadyloaded == false)
            {
                if (localSettings.Values["FontFamily"] is string fontSetting)
                {
                    FontsComboBox.SelectedItem = fontSetting;
                    Editor.FontFamily = new FontFamily(fontSetting);
                }
                else
                {
                    FontsComboBox.SelectedItem = "Calibri";
                    Editor.FontFamily = new FontFamily("Calibri");
                }
                textboxwasalreadyloaded = true;
            }
        }

        private void Editor_SelectionChanged(object sender, RoutedEventArgs args)
        {
            TextBoldButton.IsChecked = Editor.Document.Selection.CharacterFormat.Bold == FormatEffect.On;

            TextItalicButton.IsChecked = Editor.Document.Selection.CharacterFormat.Italic == FormatEffect.On;

            TextUnderlineButton.IsChecked = Editor.Document.Selection.CharacterFormat.Underline != UnderlineType.None &&
                                        Editor.Document.Selection.CharacterFormat.Underline != UnderlineType.Undefined;

            TextStrikethroughButton.IsChecked = Editor.Document.Selection.CharacterFormat.Strikethrough == FormatEffect.On;

            TextSubscriptButton.IsChecked = Editor.Document.Selection.CharacterFormat.Subscript == FormatEffect.On;

            TextSuperscriptButton.IsChecked = Editor.Document.Selection.CharacterFormat.Superscript == FormatEffect.On;

            if (Editor.Document.Selection.CharacterFormat.Size > 0)
                // Font size is negative when selection contains multiple font sizes
                fontSizeComboBox.Text = Editor.Document.Selection.CharacterFormat.Size.ToString();

            // Prevent accidental font changes when selection contains multiple styles
            updateFontFormat = false;
            FontsComboBox.SelectedItem = Editor.Document.Selection.CharacterFormat.Name;
            updateFontFormat = true;
        }

        private void Autobutton_Checked(object sender, RoutedEventArgs e)
        {
            // Black is the default
            Windows.UI.Color color = ActualTheme == ElementTheme.Light ? Windows.UI.Colors.Black : Windows.UI.Colors.White;
            Editor.Document.Selection.CharacterFormat.ForegroundColor = color;
            FontTextColorDropDownButton.Flyout.Hide();
            Editor.Focus(FocusState.Programmatic);
        }

        private void Autobutton_Unchecked(object sender, RoutedEventArgs e)
        {
            SolidColorBrush fontColorMarkerBrush = FontColorMarker.Foreground as SolidColorBrush;
            if (fontColorMarkerBrush != null)
            {
                Windows.UI.Color markerColor = fontColorMarkerBrush.Color;

                // Set the selection color in the RichEditBox
                var selectedText = Editor.Document.Selection;
                if (selectedText != null)
                {
                    selectedText.CharacterFormat.ForegroundColor = markerColor;
                    FontTextColorDropDownButton.Flyout.Hide();
                    Editor.Focus(FocusState.Programmatic);
                }
            }
        }

        private void NoColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Transparent is the default
            Windows.UI.Color color = Windows.UI.Colors.Transparent;
            Editor.Document.Selection.CharacterFormat.BackgroundColor = color;
            FontTextColorDropDownButton.Flyout.Hide();
            Editor.Focus(FocusState.Programmatic);
        }
    }
}
