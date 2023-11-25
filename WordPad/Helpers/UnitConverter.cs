using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Data;

namespace WordPad.Helpers
{
    public class UnitConverter
    {
        #region UnitConverters
        public double ConvertToPixels(double inches, string selectedUnit)
        {
            const double inchesToPixels = 96.0; // Assuming standard DPI

            if (selectedUnit == "Inches")
            {
                return inches * inchesToPixels;
            }
            else if (selectedUnit == "Centimeters")
            {
                // Convert centimeters to inches and then to pixels
                const double cmToInches = 0.393701;
                return inches * cmToInches * inchesToPixels;
            }
            else
            {
                // Handle other units as needed
                return inches;
            }
        }
        public double ConvertToUnitAndFormat(string value, string unit)
        {
            if (double.TryParse(value, out double margin))
            {
                // Convert margin values to inches
                margin = ConvertToUnit(margin, unit);

                // Limit the margin value
                double maxMargin = 100.0; // Set a maximum margin value
                margin = Math.Min(maxMargin, margin);

                // Format the margin value as needed
                string formattedMargin = margin.ToString("0.##"); // Display with up to 2 decimal places
                return double.Parse(formattedMargin);
            }
            else
            {
                // Handle the case where the input value is not a valid number
                Debug.WriteLine($"Invalid numeric value: {value}");
                return 0.0; // or some default value
            }
        }

        public double ConvertToUnit(double value, string unit)
        {
            switch (unit)
            {
                case "Inches":
                    return value;
                case "Centimeters":
                    return value * 2.54; // Convert inches to centimeters
                case "Points":
                    return value * 72; // Convert inches to points
                case "Picas":
                    return value * 6; // Convert inches to picas
                default:
                    return value; // Default to inches
            }
        }

        public double ConvertDipsToPixels(double dips)
        {
            var dpiFactor = DisplayInformation.GetForCurrentView().LogicalDpi / 33; // Convert from DIPs to physical pixels
            return dips * dpiFactor;
        }

        #endregion

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
}
