using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADSPLibrary
{
    public enum ConvertFormats :byte
    {
        Percent = 0,
        UInt = 1,
        Int = 2,
        Freq160 = 3,
        Format8_8 = 4,
        Format0_16 = 5,
        Slide = 6,
        Digits = 7,
        RegulMode = 8,
        AVRType = 9,
        Int10 = 10,
        Hex = 11,
        Uf0_135 = 12,
        Freq25000 = 13,
        TT = 14,
        TransAlarm = 15,
        Int8 = 16,
        Uint1000 = 17,
        Percent4 = 18,
        Freq90000 = 19,
        PercentUpp = 20,
        Freq16000 = 21,
        FCFreqRef = 22
    }

    public static class ConvertUtilites
    {
        public static string ToFormat(this ushort value, ConvertFormats Format)
        {
            return AdvanceConvert.HexToFormat(value, Format);
        }

        public static string ToFormat(this short value, ConvertFormats Format)
        {
            return AdvanceConvert.HexToFormat((ushort)value, Format);
        }

        public static string ToFormat(this int value, ConvertFormats Format)
        {
            return AdvanceConvert.HexToFormat((ushort)value, Format);
        }

        public static double ToFormatDouble(this ushort value, ConvertFormats Format)
        {
            string str = value.ToFormat(Format);
            double d = 0;
            double.TryParse(str, out d);
            return d;
        }
    
    }

}
