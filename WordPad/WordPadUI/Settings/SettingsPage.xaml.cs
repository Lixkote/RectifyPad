using RectifyPad;
using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WordPad.WordPadUI.Settings
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Initialize theme radio buttons
            InitializeThemeRadioButtons();

            // Initialize unit radio buttons
            InitializeUnitRadioButtons();

            // Initialize text wrapping radio buttons
            InitializeWrapRadioButtons();
        }

        private void InitializeThemeRadioButtons()
        {
            string selectedTheme = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["theme"];
            if (!string.IsNullOrEmpty(selectedTheme))
            {
                // Find the RadioButton with a matching Tag
                foreach (var item in radiocontainer.Items)
                {
                    if (item is RadioButton radioButton && radioButton.Tag is string tag && tag == selectedTheme)
                    {
                        radioButton.IsChecked = true;
                        ApplyTheme(selectedTheme);
                        break; // Exit the loop once a match is found
                    }
                }
            }
        }

        private void InitializeWrapRadioButtons()
        {
            string selectedWrap = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["textwrapping"];
            if (!string.IsNullOrEmpty(selectedWrap))
            {
                // Find the RadioButton with a matching Tag
                foreach (var item in wrapradiocontainer.Items)
                {
                    if (item is RadioButton radioButton && radioButton.Tag is string tag && tag == selectedWrap)
                    {
                        radioButton.IsChecked = true;
                        break; // Exit the loop once a match is found
                    }
                }
            }
        }

        private void InitializeUnitRadioButtons()
        {
            string selectedUnit = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["unit"];
            if (!string.IsNullOrEmpty(selectedUnit))
            {
                // Find the RadioButton with a matching Tag
                foreach (var item in unitradiocontainer.Items)
                {
                    if (item is RadioButton radioButton && radioButton.Tag is string tag && tag == selectedUnit)
                    {
                        radioButton.IsChecked = true;
                        break; // Exit the loop once a match is found
                    }
                }
            }
        }



        private RadioButton FindRadioButtonByTag(string tag)
        {
            foreach (var item in radiocontainer.Items)
            {
                if (item is RadioButton rb && rb.Tag.ToString() == tag)
                {
                    return rb;
                }
            }
            return null;
        }

        private void ApplyTheme(string selectedTheme)
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            if (selectedTheme == "Dark")
            {
                titleBar.ButtonForegroundColor = Colors.White;
                App.RootTheme = ElementTheme.Dark;
            }
            else if (selectedTheme == "Light")
            {
                titleBar.ButtonForegroundColor = Colors.Black;
                App.RootTheme = ElementTheme.Light;
            }
            else
            {
                App.RootTheme = ElementTheme.Default;
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    titleBar.ButtonForegroundColor = Colors.White;
                }
                else
                {
                    titleBar.ButtonForegroundColor = Colors.Black;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            ApplyTheme(selectedTheme);

            // Save the selected theme in app data
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["theme"] = selectedTheme;
        }

        private void UnitRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string selectedUnit = ((RadioButton)sender)?.Tag?.ToString();

            // Save the selected unit in app data
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["unit"] = selectedUnit;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void feedback_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discord.gg/gsgu9GCtsk"));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void WrapRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string selectedWrap = ((RadioButton)sender)?.Tag?.ToString();

            // Save the selected unit in app data
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["textwrapping"] = selectedWrap;
        }

        private void SpellCheckToggle_Toggled(object sender, RoutedEventArgs e)
        {

        }
    }
}
