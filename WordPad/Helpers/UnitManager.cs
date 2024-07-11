using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WordPad.Helpers.SettingsManager;

////////////////////////////////////////////////
// Lixkote RectifyPad Project Unit Calculator //
////////////////////////////////////////////////

namespace WordPad.Helpers
{
    internal class UnitManager
    {
        // In settings we keep all measurement values in twips (twentieth of a point), for consistency and precision purposes.
        // Possible human readable units that RectifyPad supports are: Inches | Centimeters | Points | Picas.

        // Convert from HumanUnit to Twip
        public double ConvertToTwip(string HumanUnit, double SourceValue)
        {
            if (HumanUnit == "Inches")
            {
                // 1 Inch = 1440 twips
                return SourceValue * 1440;
            }
            if (HumanUnit == "Centimeters")
            {
                // 1 cm = 566.9291338583 twips
                return SourceValue * 566.9291338583;
            }
            if (HumanUnit == "Points")
            {
                // 1 point = 20 twips
                return SourceValue * 20;
            }
            if (HumanUnit == "Picas")
            {
                // 1 Pica = 240.000000001693 twips
                return SourceValue * 240.000000001693;
            }
            else { return 0; }
        }

        // Convert from Twip to HumanUnit
        public double ConvertFromTwip(string HumanUnit, double SourceValue)
        {
            if (HumanUnit == "Inches")
            {
                // 1 Inch = 1440 twips
                return SourceValue / 1440;
            }
            if (HumanUnit == "Centimeters")
            {
                // 1 cm = 566.9291338583 twips
                return SourceValue / 566.9291338583;
            }
            if (HumanUnit == "Points")
            {
                // 1 point = 20 twips
                return SourceValue / 20;
            }
            if (HumanUnit == "Picas")
            {
                // 1 Pica = 240.000000001693 twips
                return SourceValue / 240.000000001693;
            }
            else { return 0; }
        }
    }
}
