using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WordPad.WordPadUI.Settings
{
    public static class SettingsPageManager
    {
        /// <summary>
        /// section for dark theme in editor
        /// </summary>
        public static bool IsDarkThemeEditor { get; private set; }

        // Event to notify theme changes
        public static event System.Action<bool> ThemeChanged;

        // Method to set theme and notify all listeners
        public static void SetTheme(bool isDarkThemeEditor)
        {
            IsDarkThemeEditor = isDarkThemeEditor;
            ThemeChanged?.Invoke(isDarkThemeEditor);
        }

        // Method to get the current theme
        public static ElementTheme GetRequestedTheme()
        {
            return IsDarkThemeEditor ? ElementTheme.Dark : ElementTheme.Light;
        }

        /// 
        /// section for spell check 
        /// 

        public static bool IsSpellCheckEnabled { get; private set; }

        // Event to notify theme changes
        public static event System.Action<bool> SpellChanged;

        // Method to set theme and notify all listeners
        public static void SetSpellCheck(bool isSpellCheckEnabled)
        {
            IsSpellCheckEnabled = isSpellCheckEnabled;
            SpellChanged?.Invoke(isSpellCheckEnabled);
        }

        // Method to get the current theme
        public static bool GetSpellCheckBool()
        {
            return IsSpellCheckEnabled ? true : false;
        }

        ///
        /// section for autocomplete
        ///


        public static bool IsTextPredictEnabled { get; private set; }

        // Event to notify theme changes
        public static event System.Action<bool> PredictChanged;

        // Method to set theme and notify all listeners
        public static void SetPredict(bool isTextPredictEnabled)
        {
            IsTextPredictEnabled = isTextPredictEnabled;
            PredictChanged?.Invoke(isTextPredictEnabled);
        }

        // Method to get the current theme
        public static bool GetPredictCheckBool()
        {
            return IsTextPredictEnabled ? true : false;
        }     



        
    }
}
