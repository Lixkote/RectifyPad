using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace WordPad.Helpers
{
    public class ZoomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double zoom)
            {
                return $"{zoom * 100}%";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string text && text.EndsWith("%"))
            {
                if (double.TryParse(text.TrimEnd('%'), out double zoom))
                {
                    return zoom / 100;
                }
            }
            return value;
        }
    }
    public class HalfValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double number)
            {
                return number / 2;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double number)
            {
                return number * 2;
            }
            return value;
        }
    }
}
