﻿using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
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
        public FontToolbar()
        {
            this.InitializeComponent();
        }

        private void UpdateFontSize()
        {
            FontSizesComboBox.SelectedItem = Editor.FontSize;
        }

        private void FontsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Editor.Document.Selection == null || !updateFontFormat)
                return;

            Editor.Document.Selection.CharacterFormat.Name = FontsComboBox.SelectedValue.ToString();
            Editor.Focus(FocusState.Programmatic);
        }

        private void FontTextColorDropDownButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {

        }

        private void FontBackgroundColorDropDownButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {

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
        }

        private void FontSizesComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if ((ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7)))
            {
                FontSizesComboBox.TextSubmitted += FontSizesComboBox_TextSubmitted;
            }
        }

        private void FontSizesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ComboBox comboBox)
                {
                    if (comboBox.SelectedItem is double selectedValue)
                    {
                        Editor.Document.Selection.CharacterFormat.Size = (float)selectedValue;
                    }
                }
            }
            catch { }
        }

        private void FontsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Editor.Document.Selection.CharacterFormat.Name = FontsComboBox.SelectedValue.ToString();
        }

        private void TextBoldButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Bold);
        }

        private void TextItalicButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Italic);
        }

        private void TextUnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Underline);
        }

        private void TextStrikethroughButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Strikethrough);
        }

        private void TextSubscriptButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Subscript);
        }

        private void TextSuperscriptButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Superscript);
        }

        private void FontEnlargeButton_Click(object sender, RoutedEventArgs e)
        {
            // Get a reference to the RichEditBox control
            RichEditBox richEditBox = Editor;

            // Increase the font size of the currently selected text by 2 points
            richEditBox.Document.Selection.CharacterFormat.Size += 2;
            // Get the index of the currently selected item
            FontSizesComboBox.SelectedItem = richEditBox.Document.Selection.CharacterFormat.Size;
        }

        private void FontDislargeButton_Click(object sender, RoutedEventArgs e)
        {
            // Get a reference to the RichEditBox control
            RichEditBox richEditBox = Editor;

            // Decrease the font size of the currently selected text by 2 points
            richEditBox.Document.Selection.CharacterFormat.Size -= 2;
            FontSizesComboBox.SelectedItem = richEditBox.Document.Selection.CharacterFormat.Size;
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

            // SplitButton.Flyout.Hide();
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

            // SplitButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void FontsComboBox_Loaded(object sender, RoutedEventArgs e)
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
        }

        private void FontsComboBox_Loaded_1(object sender, RoutedEventArgs e)
        {
            FontsComboBox.SelectedItem = Editor.FontFamily;
        }

        private void FontSizesComboBox_Loaded_1(object sender, RoutedEventArgs e)
        {
            Editor.SelectionChanging += Editor_SelectionChanging;
        }

        private void Editor_SelectionChanging(RichEditBox sender, RichEditBoxSelectionChangingEventArgs args)
        {
            FontSizesComboBox.SelectedItem = Editor.Document.Selection.CharacterFormat.Size;
        }
    }
}
