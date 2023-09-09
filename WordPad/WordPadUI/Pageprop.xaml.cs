using RectifyPad;
using System;
using System.Collections.Generic;
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
            var valueu = Windows.Storage.ApplicationData.Current.LocalSettings.Values["orienation"];
            if (valueu != null)
            {
                string unit = valueu.ToString();
                // Find and check the corresponding radio button
                var radioButton = FindRadioButtonByTag(unit);
                if (radioButton != null)
                {
                    radioButton.IsChecked = true;
                }
            }
        }

        private RadioButton FindRadioButtonByTag(string tag)
        {
            // Assuming your muxc:RadioButtons control is named radiocontainer
            foreach (var item in Orientcontainer.Children)
            {
                if (item is RadioButton rb && rb.Tag.ToString() == tag)
                {
                    return rb;
                }
            }
            return null;
        }

        private void exampletext_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the saved unit value
            string valueb = Windows.Storage.ApplicationData.Current.LocalSettings.Values["pagesetupBmargin"].ToString();
            if (valueb != null)
            {
                BottomMarginTextBox.Text = valueb;
            }
            string valueT = Windows.Storage.ApplicationData.Current.LocalSettings.Values["pagesetupTmargin"].ToString();
            if (valueT != null)
            {
                BottomMarginTextBox.Text = valueT;
            }
            string valueL = Windows.Storage.ApplicationData.Current.LocalSettings.Values["pagesetupLmargin"].ToString();
            if (valueL != null)
            {
                BottomMarginTextBox.Text = valueL;
            }
            string valueR = Windows.Storage.ApplicationData.Current.LocalSettings.Values["pagesetupLmargin"].ToString();
            if (valueR != null)
            {
                BottomMarginTextBox.Text = valueR;
            }

            // Get the values from the textboxes
            double left = double.Parse(LeftMarginTextBox.Text);
            double right = double.Parse(RightMarginTextBox.Text);
            double top = double.Parse(TopMarginTextBox.Text);
            double bottom = double.Parse(BottomMarginTextBox.Text);

            // Load the saved unit value
            var unit = Windows.Storage.ApplicationData.Current.LocalSettings.Values["unitSetting"];

            // Convert the values to pixels based on the unit
            double pixelsPerUnit = 0;
            switch (unit)
            {
                case "Inches":
                    pixelsPerUnit = 96; // 96 pixels per inch
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
            }

            // Create a Thickness object with the converted values
            Thickness margin = new Thickness(
                left * pixelsPerUnit,
                top * pixelsPerUnit,
                right * pixelsPerUnit,
                bottom * pixelsPerUnit);

            // Set the margin of the textblock
            exampletext.Margin = margin;

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
    }
}
