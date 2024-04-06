using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Data;
using WordPad.Classes;

namespace WordPad.Helpers
{
    public class UnitManager
    {
        /// <summary>
        /// Based on original WordPad source code: https://github.com/microsoft/VCSamples/tree/9e1d4475555b76a17a3568369867f1d7b6cc6126/VC2010Samples/MFC/ole/wordpad
        /// </summary>
        int primaryunits = 4;
        int allunits = 6;
        int m_nUnits;
        int m_nNumUnits;
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
        public static CUnit[] Units = new CUnit[6]
        {
            //  TPU,    SmallDiv,   MedDiv, LargeDiv,   MinMove,    Abbrev,           SpaceAbbrev
            new CUnit(1440, 180,      720,    1440,       90,         "inches",           false), // inches MAINUNIT:YES
            new CUnit(568,  142,      284,    568,        142,        "cm",               true),  // centimeters MAINUNIT:YES
            new CUnit(20,   120,      720,    720,        100,        "points",           true),  // points MAINUNIT:YES
            new CUnit(240,  240,      1440,   1440,       120,        "picas",            true),  // picas MAINUNIT:YES
            new CUnit(1440, 180,      720,    1440,       90,         "in",               false), // in
            new CUnit(1440, 180,      720,    1440,       90,         "inch",             false), // inch
        };

        public int GetUnits() { return m_nUnits; }

        public int GetCurrentlySetUnit()
        {
            SettingsManager settingsManager = new SettingsManager();
            string currentunitabbrev = settingsManager.GetSettingString("unit");
            return FindUnitIndex("currentunitabbrev");
        }
        public static int FindUnitIndex(string abbreviation)
        {
            for (int i = 0; i < Units.Length; i++)
            {
                if (Units[i].Abbrev == abbreviation)
                {
                    return i;
                }
            }
            return -1; // Not found
        }

        public string GetAbbrev() { return Units[m_nUnits].Abbrev; }
        public string GetAbbrev(int n) { return Units[n].Abbrev; }
        int GetTPU() { return GetTPU(m_nUnits); }
        int GetTPU(int n) { return Units[n].TPU; }

        public void ConvertTwips(ref string buf, int nSize, int nValue, int nDec)
        {
            if (nDec != 2)
            {
                throw new ArgumentException("nDec must be 2.");
            }

            int div = GetTPU();
            int lval = nValue;
            bool bNeg = false;

            int[] pVal = new int[nDec + 1];

            if (lval < 0)
            {
                bNeg = true;
                lval = -lval;
            }

            for (int h = 0; h <= nDec; h++)
            {
                pVal[h] = lval / div; // Integer number
                lval -= pVal[h] * div;
                lval *= 10;
            }

            int i = nDec;
            if (lval >= div / 2)
            {
                pVal[i]++;
            }

            while (nDec > 0 && pVal[nDec] == 0)
            {
                nDec--;
            }

            buf = $"{(float)nValue / div:FnDec}";

            if (Units[m_nUnits].SpaceAbbrev)
            {
                buf += " ";
            }
            buf += Units[m_nUnits].Abbrev;

            // Free memory
            Array.Clear(pVal, 0, pVal.Length);
        }

        public bool ParseMeasurement(string buf, out int lVal)
        {
            lVal = 0;

            if (string.IsNullOrEmpty(buf))
                return false;

            float f;
            if (!float.TryParse(buf, out f))
                return false;

            // Trim leading whitespace, if any
            string trimmedBuf = buf.TrimStart();
            if (string.IsNullOrEmpty(trimmedBuf)) // Default case
            {
                lVal = (int)Math.Round(f * GetTPU());
                return true;
            }

            for (int i = 0; i < m_nNumUnits; i++)
            {
                if (string.Equals(trimmedBuf, GetAbbrev(i), StringComparison.OrdinalIgnoreCase))
                {
                    lVal = (int)Math.Round(f * GetTPU(i));
                    return true;
                }
            }
            return false;
        }

    }
}
