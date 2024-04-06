using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordPad.Classes
{
    public class CUnit
    {
        public int TPU { get; set; }
        public int SmallDiv { get; set; }
        public int MediumDiv { get; set; }
        public int LargeDiv { get; set; }
        public int MinMove { get; set; }
        public uint AbbrevID { get; set; }
        public bool SpaceAbbrev { get; set; }
        public string Abbrev { get; set; }

        // Copy constructor
        public CUnit(CUnit unit)
        {
            TPU = unit.TPU;
            SmallDiv = unit.SmallDiv;
            MediumDiv = unit.MediumDiv;
            LargeDiv = unit.LargeDiv;
            MinMove = unit.MinMove;
            AbbrevID = unit.AbbrevID;
            SpaceAbbrev = unit.SpaceAbbrev;
            Abbrev = unit.Abbrev;
        }

        // Constructor
        public CUnit(int tpu, int smallDiv, int mediumDiv, int largeDiv,
                     int minMove, string abbrev, bool spaceAbbrev)
        {
            TPU = tpu;
            SmallDiv = smallDiv;
            MediumDiv = mediumDiv;
            LargeDiv = largeDiv;
            MinMove = minMove;
            //AbbrevID = abbrevID;
            SpaceAbbrev = spaceAbbrev;
            Abbrev = abbrev;
        }
    }

}
