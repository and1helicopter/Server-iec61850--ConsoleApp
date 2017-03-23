using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADSPLibrary
{
    public class InputValueFormSettings
    {
        public double Min { get; private set; }
        public double Max { get; private set; }
        public int DigitCount { get; private set; }
        public bool SignPossible { get; private set; }
        public InputValueFormSettings(double MinValue, double MaxValue, int DigCount)
        {
            Min = MinValue;
            Max = MaxValue;
            DigitCount = DigCount;
            if ((Min < 0) || (Max < 0)) { SignPossible = true; } else { SignPossible = false; }
        }
    }
}
