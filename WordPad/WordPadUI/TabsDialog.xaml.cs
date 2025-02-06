using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WordPad.WordPadUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabsDialog : ContentDialog
    {
        private ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public TabsDialog()
        {
            this.InitializeComponent();
        }

        private void IndicateTextBoxImproperValue()
        {
            // Set the error indication on the border
            EnteringBox.BorderBrush = (SolidColorBrush)Application.Current.Resources["TextBoxErrorIndication"];
            TabsDialog1.IsSecondaryButtonEnabled = false;
            SetButton.IsEnabled = false;
            return;
        }

        private void EnteringBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetButton.IsEnabled = true;
            ClearButton.IsEnabled = true;
            string enteredvalue = EnteringBox.Text;

            // Check if the input is empty
            if (string.IsNullOrEmpty(enteredvalue))
            {
                IndicateTextBoxImproperValue();
            }

            // Check if the input consists only of numbers
            if (!Regex.IsMatch(enteredvalue, @"^\d+$"))
            {
                IndicateTextBoxImproperValue();
            }

            // Convert the input to an integer and validate the range
            if (int.TryParse(enteredvalue, out int number))
            {
                // Check if the number is within the valid range
                if (number > 0 && number < 132)
                {
                    // Input is valid, so enable the secondary button and reset the border color
                    EnteringBox.BorderBrush = (LinearGradientBrush)Application.Current.Resources["TextControlElevationBorderFocusedBrush"];
                    TabsDialog1.IsSecondaryButtonEnabled = true;
                    SetButton.IsEnabled=true;
                }
                else
                {
                    IndicateTextBoxImproperValue();
                }
            }
            else
            {
                IndicateTextBoxImproperValue();
            }
        }


        private void ClearButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var selectedItem = TabsPostitionsListView.SelectedItem;

            if (selectedItem != null)
            {
                TabsPostitionsListView.Items.Remove(selectedItem);
            }
            IndicateTextBoxImproperValue();
            EnteringBox.Text = string.Empty;
        }

        private void SetButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string enteredvalue = EnteringBox.Text;
            string unitprefix = (string)localSettings.Values["unit"];
            TabsPostitionsListView.Items.Add(enteredvalue + " " + unitprefix);
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            TabsPostitionsListView.Items.Clear();
            IndicateTextBoxImproperValue();
            EnteringBox.Text = string.Empty;
        }
    }
}
