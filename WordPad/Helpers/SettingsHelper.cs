using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WordPad.Helpers
{
    public static class SettingsHelper
    {
        public static object GetSetting(string key)
        {
            ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
            return LocalSettings.Values[key];
        }

        public static string GetSettingString(string key)
        {
            ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
            return (string)LocalSettings.Values[key];
        }

        public static void SetSetting(string key, object value)
        {
            ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
            LocalSettings.Values[key] = value;
        }
    }
}
