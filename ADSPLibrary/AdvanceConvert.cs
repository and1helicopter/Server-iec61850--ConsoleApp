using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADSPLibrary;

namespace ADSPLibrary
{
    public static class AdvanceConvert
    {
        public static int iValue = 0;
        public static ushort uValue = 0;
        public static double fValue = 0;
        public static string strValue = "";

        public static List<ushort> LineToUshorts(string symFileLine)
        {
            string[] paramListstr;
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            List<ushort> paramsList = new List<ushort>();
            if (symFileLine != null)
            {
                paramListstr = symFileLine.Split(delimiterChars);

                foreach (string str in paramListstr)
                {
                    if (StrToInt(str)) { paramsList.Add(uValue); }
                }
            }
            return paramsList;
        }

        public static List<double> LineToFloats(string line)
        {
            string[] paramListstr;
            char[] delimiterChars = { ' ', '.', ':', '\t' };
            List<double> paramsList = new List<double>();
            double d;
            if (line != null)
            {
                paramListstr = line.Split(delimiterChars);

                foreach (string str in paramListstr)
                {
                    //if (StrToInt(str)) { paramsList.Add(uValue); }

                    if (double.TryParse(str, out d))
                    {
                        paramsList.Add(d);
                    }
                }
            }
            return paramsList;

        }

        public static short StrToShort(string Value)
        {
            short s = 0;
            if (((Value.Contains("0x")) || (Value.Contains("0X"))) && (!Value.Contains("-")))
            {
                try
                {
                    s = Convert.ToInt16(Value, 16);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }
            }
            else if ((Value.Contains("-0x")) || (Value.Contains("-0X")))
            {
                try
                {
                    string str = Value.Substring(1);
                    s = Convert.ToInt16(str, 16);
                    s = (short)(-s);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }
            }
            else
            {
                try
                {
                    s = Convert.ToInt16(Value, 10);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }

            }
            return s;
        }


        public static bool StrToInt(string newStrValue)
        {
            int i, i1;

            strValue = newStrValue;

            if (int.TryParse(strValue, out i))
            {
                i1 = ((ushort)i) >> 16;
                if (i1 != 0)
                {
                    iValue = 0;
                    uValue = 0;
                    fValue = 0;
                    return false;
                }
                else
                    iValue = (short)i;
                uValue = (ushort)i;
                fValue = 0;
                return true;
            }
            else
            {
                try
                {
                    i = Convert.ToInt32(strValue, 16);
                    if (i > 65535)
                    {
                        iValue = 0;
                        uValue = 0;
                        fValue = 0;
                        return false;
                    }
                }
                catch
                {
                    iValue = 0;
                    uValue = 0;
                    fValue = 0;
                    return false;
                }
                iValue = (short)i;
                uValue = (ushort)i;
                fValue = 0;
                return true;
            }
        }

        public static string HexToHex(ulong value)
        {
            return ("0x" + value.ToString("X4"));
        }

        public static string ToHex(this ushort value)
        {
            return HexToHex(value);
        }
        public static string ToHex(this int value)
        {
            ushort w = (ushort)value;
            return (w.ToHex());
        }

        public static string HexToPercent(ulong value)
        {
            double f = (short)value / 40.96;
            return (f.ToString("F2"));
        }
        public static string ToPercent(this ushort value)
        {
            return HexToPercent(value);
        }
        public static double ToPercentDouble(this ushort value)
        {
            double f = (short)value / 40.96;
            return f;
        }

        public static string ToPercent(this ushort value, ushort DigitCount)
        {
            return HexToPercent(value, DigitCount);
        }

        public static string HexToPercent(ulong value, ushort digitCount)
        {
            if (digitCount > 5) { digitCount = 5; }
            double f = (short)value / 40.96;
            return (f.ToString("F" + digitCount.ToString()));
        }

        public static string HexToPercent(ulong value, ushort digitCount, out double OutValue)
        {
            if (digitCount > 5) { digitCount = 5; }
            double f = (short)value / 40.96;
            OutValue = f;
            return (f.ToString("F" + digitCount.ToString()));
        }

        public static string HexToPercentUpp(ulong value, ushort digitCount)
        {
            if (digitCount > 5) { digitCount = 5; }
            double f = (short)value / 20.48;
            return (f.ToString("F" + digitCount.ToString()));
        }
        public static string ToPercentUpp(this ushort value, ushort DigitCount)
        {
            return HexToPercentUpp(value, DigitCount);
        }

        public static string HexToPercent4(ulong value)
        {
            double f = (short)value / 10.24;
            return (f.ToString("F2"));
        }
        public static string ToPercent4(this ushort value)
        {
            return HexToPercent4(value);
        }

        public static string HexToUint(ulong value)
        {
            return (value.ToString());
        }

        public static string HexToUint1000(ulong value)
        {
            double f = ((double)value) / 1000.0;
            return (f.ToString("F3"));
        }

        public static string ToUint1000(this ushort value)
        {
            return HexToUint1000(value);
        }

        public static double HexToUint1000Double(ushort value)
        {
            double f = ((double)(value)) / 1000.0;
            return f;
        }

        public static ushort Uint1000ToHex(double value)
        {
            if (value > 65) { return (65000); }
            if (value < 0) { return (0); }

            double f = value * 1000;
            return ((ushort)(f + 0.5));
        }


        public static string HexToUint4000(ushort value)
        {
            double f = ((double)value) / 4000.0;
            return (f.ToString("F3"));
        }

        public static string ToUint4000(this ushort value)
        {
            return HexToUint4000(value);
        }

        public static double HexToUint4000Double(ushort value)
        {
            double f = ((double)(value)) / 4000;
            return f;
        }

        public static ushort Uint4000ToHex(double value)
        {
            if (value > 16) { return (64000); }
            if (value < 0) { return (0); }

            double f = value * 4000;
            return ((ushort)(f + 0.5));
        }


        public static string HexToInt(ulong value)
        {
            return (((short)value).ToString());
        }
        public static string ToInt(this ulong value)
        {
            return HexToInt(value);
        }

        public static string HexToInt10(ulong value)
        {
            double f = (short)value / 10.0;
            return (f.ToString("F1"));
        }

        public static string HexToInt100(ushort value, int digitCount)
        {
            double f = (short)value / 100.0;
            return (f.ToString("F" + digitCount.ToString()));
        }


        public static double HexToInt10Double(ushort value)
        {
            double f = ((double)((short)value)) / 10.0;
            return (f);
        }

        public static ushort Int10ToHex(double value)
        {
            int i = (int)(Math.Round(value * 10));
            return ((ushort)i);
        }

        public static string ToInt10(this ulong value)
        {
            return HexToInt10(value);
        }

        public static string HexToInt10(ulong value, ushort digitCount)
        {
            double f = (short)value / 10.0;
            return (f.ToString("F" + digitCount.ToString()));
        }
        public static string ToInt10(this ushort value, ushort digitCount)
        {
            double f = (short)value / 10.0;
            return (f.ToString("F" + digitCount.ToString()));
        }



        public static string HexToInt8(ulong value)
        {
            double f = (short)value / 8.0;
            return (f.ToString("F0"));
        }
        public static string HexToInt8(ulong value, int digitCount)
        {
            double f = (short)value / 8.0;
            return (f.ToString("F" + digitCount.ToString()));
        }
        public static string ToInt8(this ushort value, ushort digitCount)
        {
            return (HexToInt8(value, digitCount));
        }
        public static string ToInt8(this ushort value)
        {
            return (HexToInt8(value, 0));
        }
        public static ushort Int8ToHex(double f)
        {
            f = f * 8;
            return ((ushort)f);
        }

        public static string HexToFreq(ulong value)
        {
            string str;
            if (value == 0) { str = "Н/Д"; }
            else if (value == 0x4000) { str = "Н/Д"; }
            else
            {
                double f = 8000.0 / value;
                str = f.ToString("F2");
            }
            return str;
        }

        public static ushort FreqToHex(double value)
        {
            if (value <= 0) { return 0; }
            else if
                (value >= 100) { return 80; }
            else { return ((ushort)(8000.0 / value)); }
        }


        public static string HexToFreqNew(ulong value)
        {
            double f = (ushort)value / 500.0;
            return (f.ToString("F1"));
        }

        public static string HexToFreqNew2(ulong value)
        {
            string str;
            if (value == 0) { str = "Нет данных"; }
            else if (value == 3600) { str = "Нет данных"; }
            else
            {
                double f = 90000.0 / value;
                str = f.ToString("F2");
            }
            return str;
        }
        public static string HexTo8_8(ulong value)
        {
            double f = (short)value / 256.0;
            return (f.ToString("F2"));
        }
        public static string HexTo8_8(ulong value, ushort digitCount)
        {
            if (digitCount > 5) { digitCount = 5; }
            double f = (short)value / 256.0;
            return (f.ToString("F" + digitCount.ToString()));
        }


        public static string HexTo0_16(ulong value)
        {
            double f = (ushort)(value) / 65536.0;
            return (f.ToString("F3"));

        }

        public static double HexTo0_16Double(ushort value)
        {
            double f = (ushort)(value) / 65536.0;
            return f;
        }

        public static string HexToSlide(ulong value)
        {
            double f = (ushort)value / 327.68;
            if (value == 320) { f = 0; }
            return (f.ToString("F2"));
        }

        public static string HexToCoefStatSTS(ushort value, int DigitCount)
        {
            double f = (short)value / 327.68;
            return (f.ToString("F" + DigitCount.ToString()));
        }

        public static ushort CoefStatSTSToHex(double value)
        {
            ushort u = (ushort)((short)(value * 327.68));
            return u;
        }


        public static string HexToSlide(ulong value, ushort digitCount)
        {
            if (digitCount > 5) { digitCount = 5; }
            double f = (ushort)value / 327.68;
            if (value == 320) { f = 0; }
            return (f.ToString("F" + digitCount.ToString()));
        }
        public static string HexToDigits(ulong value)
        {
            string str = "";
            ulong value2 = value;
            int i, i2;

            for (i = 0; i < 4; i++)
            {
                for (i2 = 0; i2 < 4; i2++)
                {
                    if ((value2 & 1) != 0) { str = "1" + str; }
                    else { str = "0" + str; }
                    value2 = (ushort)(value2 >> 1);
                }
                if (i != 3) { str = " " + str; }
            }
            return (str);
        }

        public static string HexRegulMode(ulong value)
        {
            string str;
            switch (value)
            {
                case 0: { str = "Авто"; } break;
                case 1: { str = "Ручн."; } break;
                default: { str = "Тест"; } break;
            }
            return (str);
        }

        public static string HexToAVRType(ulong value)
        {
            string str;
            switch (value)
            {
                case 1: { str = "cosФ"; } break;
                case 2: { str = "Ire"; } break;
                case 3: { str = "If"; } break;
                default: { str = "U"; } break;
            }
            return (str);
        }

        public static string HexToUf(ulong value)
        {
            double f = (short)value * 0.135;
            return (f.ToString("F1"));
        }

        public static string HexToTT(ulong value)
        {
            if (value < 0x0010) { return ("Ошибка"); }
            else
            {
                double f = 2560.0 / (short)(value);
                return (f.ToString("F2"));
            }
        }

        public static string HexToTT(ulong value, bool invert)
        {
            if (value < 0x0010) { return ("Ошибка"); }
            else
            {
                double f = 2560.0 / (short)(value);
                if (invert) { f = -f; }
                return (f.ToString("F2"));
            }
        }

        public static ushort TTToHex(double value)
        {
            if (value < 0) { return 0; }
            if (value > 5.0) { return 0x0200; }

            double f = 2560.0 / value;
            return ((ushort)f);

        }

        public static ushort DoubleToBCD(double Value)
        {
            ushort u = (ushort)Value;
            int i = (int)(u & 0xFF);
            i = (i / 10) * 16 + (i - (i / 10) * 10);
            return (ushort)i;
        }

        public static ushort TTToHex(double value, bool invert)
        {
            if (value < 2.0) { return 0x0200; }
            if (value > 5.0) { return 0x0200; }

            double f = 2560.0 / value;
            if (invert) { f = -f; }
            return ((ushort)f);

        }

        public static string HexToTransAlarm(ulong value)
        {
            double f = (short)value * 0.00172633491500621954199424893092;
            return (f.ToString("F1"));
        }

        public static string HexToTransAlarm(ulong value, bool atomFormat)
        {
            if (!atomFormat)
            {
                double f = (short)value * 0.00172633491500621954199424893092;
                return (f.ToString("F1"));
            }
            else
            {
                double f = (short)value * 0.00244498777506112469437652811736;
                return (f.ToString("F1"));
            }
        }

        public static ushort TransAlarmToHex(double value)
        {
            double f = value;
            if (f > 70) { f = 70; }
            if (f < 0) { f = 0; }
            f = f / 0.00172633491500621954199424893092;
            return ((ushort)f);
        }

        public static ushort TransAlarmToHex(double value, bool atomFormat)
        {
            if (!atomFormat)
            {
                double f = value;
                if (f > 70) { f = 70; }
                if (f < 0) { f = 0; }
                f = f / 0.00172633491500621954199424893092;
                return ((ushort)f);
            }
            else
            {
                double f = value;
                if (f > 70) { f = 70; }
                if (f < 0) { f = 0; }
                f = f / 0.00244498777506112469437652811736;
                return ((ushort)f);

            }

        }

        public static string CalcNomIf(ushort coefIf, ushort nomIf)
        {
            if (coefIf < 0x0010) { return ("Ошибка"); }
            else
            {
                double f = nomIf * 0x200 / (short)(coefIf);
                return (f.ToString("F0"));
            }
        }

        public static ushort CalcKIf(double GenIf, ushort nomIf)
        {
            double f;
            if ((ushort)GenIf > (5 * nomIf)) { return 0x0200; }
            if ((ushort)GenIf < 0) { return 0x0200; }

            f = (double)nomIf / GenIf;
            f = 512.0 * f + 0.5;
            return ((ushort)f);
        }

        public static string HexToTi(ushort value)
        {
            if (value == 0) { return ("0"); }
            double f = 65535.0 / (double)value;
            return (f.ToString("F0"));
        }

        public static string HexToTi(ushort value, bool wordDocMode)
        {
            if (!wordDocMode)
            {
                if (value == 0) { return ("0"); }
                double f = 65535.0 / (double)value;
                return (f.ToString("F0"));
            }
            else
            {
                if (value == 0) { return ("Откл."); }
                double f = 65535.0 / (double)value;
                return (f.ToString("F0") + " мс");

            }
        }

        public static ushort TiToHex(double value)
        {
            double f;
            if (value == 0) { return 0; }
            f = 65535.0 / value;
            return ((ushort)(f + 0.5));
        }

        public static ushort F8_8ToHex(double value)
        {
            if (value > 127.99) { return (0x7FFF); }
            if (value < -127.99) { return (0x8000); }

            double f = value * 256.0;
            return ((ushort)(f + 0.5));

        }

        public static string HexToSlowTi(ushort value)
        {
            if (value == 0) { return "0"; }
            double f = 24.4140625;
            f = f / (double)value;
            return (f.ToString("F1"));
        }

        public static ushort SlowTiToHex(double value)
        {
            if (value <= 0.005) { return 0; }
            if (value >= 5) { return 5; }
            double f = 24.4140625 / value;
            return ((ushort)(f + 0.5));
        }

        public static ushort SlideToHex(double value)
        {

            double f = value;
            if (value < 0) { value = 0; }
            if (value > 150) { value = 150; }

            value = value * 327.68;
            return ((ushort)(value + 0.5));

        }

        public static string HexToOverHeatTi(ushort value)
        {
            if (value == 0) { return "0"; }
            double f = 67109.0 / (double)value;
            return (f.ToString("F0"));
        }

        public static string HexToOverHeatTi(ushort value, out double tiValue)
        {
            if (value == 0) { tiValue = 0; return "0"; }
            double f = 67109.0 / (double)value;
            tiValue = f;
            return (f.ToString("F0"));
        }

        public static ushort OverHeatTiToUShort(double value)
        {
            if (value < 2) { value = 2; }
            double f = 67109.0 / value;
            return ((ushort)(f + 0.5));
        }

        public static ushort PercentToHex(double value)
        {
            double f = value;
            if (f < -790) { f = -790; }
            if (f > 790) { f = 790; }
            return ((ushort)((f * 40.96) + 0.5));
        }

        public static ushort CalcAngle(double value)
        {
            int i;
            i = (int)(System.Math.Tan(value * 3.141592 / 180.0) * 32768);
            return ((ushort)i);
        }

        public static string LongHexToVHz(ushort valueHi, ushort valueLo)
        {
            double temp = (double)(valueHi) + (double)((UInt16)(valueLo) / 65536.0);
            temp = ((UInt16)(80000.0 / temp + 0.5)) / 10.0;
            return (temp.ToString("F1"));
        }

        public static void VHzToLongHex(double value, out ushort valueLo, out ushort valueHi)
        {
            if (value < 40) { value = 40; }
            double temp = 8000.0 / value;

            valueHi = (ushort)(temp);
            valueLo = (ushort)(temp * 65536.0);
        }
        /// <summary>
        /// Расчет значения частоты в формате 50Гц = 0x01E84
        /// </summary>
        public static string HexToFreqFC(ushort value, ushort digitCount)
        {
            if (digitCount > 5) { digitCount = 5; }
            double f = (short)value / 156.24;
            return (f.ToString("F" + digitCount.ToString()));
        }

        public static string HexToFCVoltage(ushort value, ushort cellsCount, ushort digitCount)
        {
            if (digitCount > 5) { digitCount = 5; }
            double f;
            if (cellsCount == 8) { f = (short)value * 2.44140625; }
            else { f = (short)value * 1.46484375; }
            return (f.ToString("F" + digitCount.ToString()));
        }

        public static string HexToPowerKVT(ushort value, ushort nomU, ushort nomI)
        {
            double f = (short)value / 4096;
            f = 1.732 * nomU * nomI * f / 1000.0;
            return (f.ToString("F0"));


        }

        public static string HexToFreqUPTF(ulong value)
        {
            string str;
            if (value == 0) { str = "Нет данных"; }
            else if (value == 0x4000) { str = "Нет данных"; }
            else
            {
                double f = 16000.0 / value;
                str = f.ToString("F3");
            }
            return str;
        }

        public static string HexToFCRefFreq(ulong value, int digitCount)
        {
            double f = (short)value / 156.24;
            return (f.ToString("F" + digitCount.ToString()));
        }

        public static double HexToFCRefFreqDouble(ushort value)
        {
            double f = (short)value / 156.24;
            return f;
        }

        public static string HexToFCRefFreq(ulong value)
        {
            return (HexToFCRefFreq(value, 1));
        }

        public static string HexToFCVoltage(ushort value, double coef)
        {
            double f = (short)(value * coef);
            return (f.ToString("F0"));
        }

        public static string HVFCCalcCosFi(ushort P, ushort Q)
        {
            double Pf = (int)P;
            double Qf = (int)Q;

            double d;
            d = Math.Atan2(Qf, Pf);
            d = Math.Cos(d);
            string st = d.ToString("F2");
            if (Qf < 0) { st = st + " C"; } else { st = st + " L"; }

            return st;
        }

        public static string CalcPowerFactorCTC(ushort P, ushort Q, bool Absolute)
        {
            double Pf = (short)P;
            double Qf = (short)Q;

            double d;
            d = Math.Atan2(Qf, Pf);

            if (!Absolute) { return ((180.0 * d / Math.PI).ToString("F0")); }

            d = Math.Cos(d);
            string st = d.ToString("F2");
            if (Qf < 0) { st = st + " C"; } else { st = st + " L"; }

            return st;

        }

        public static string CalcPowerFactorEXSR(ushort P, ushort Q, bool Absolute)
        {
            double Pf = (short)P;
            double Qf = (short)Q;

            Pf = -Pf;

            double d;
            d = Math.Atan2(Qf, Pf);

            if (!Absolute) { return ((180.0 * d / Math.PI).ToString("F0")); }

            d = Math.Cos(d);
            string st = d.ToString("F2");
            if (Qf < 0) { st = st + " C"; } else { st = st + " L"; }

            return st;

        }

        public static string CalcAbsoluteParam(ushort Nom, ushort digits, ushort value)
        {
            double f = (short)value / 4096.0;
            f = Nom * f;
            return (f.ToString("F" + digits.ToString()));
        }


        public static string CalcAbsoluteParam(double Nom, ushort digits, ushort value)
        {
            double f = (short)value / 4096.0;
            f = Nom * f;
            return (f.ToString("F" + digits.ToString()));
        }

        public static string CalcPowerKWt(ushort NomU, ushort NomI, ushort P)
        {
            double S = Math.Sqrt(3) * NomU * NomI / 1000.0;
            double Pf = (int)P;
            Pf = ((short)P) / 4096.0;
            return ((S * Pf).ToString("F0"));
        }

        public static string CalcPowerMWt(ushort NomU, ushort NomI, ushort P)
        {
            double S = Math.Sqrt(3) * NomU * NomI / 1000000.0;
            double Pf = (int)P;
            Pf = ((short)P) / 4096.0;
            return ((S * Pf).ToString("F1"));
        }

        public static string CalcIf(ushort NomIf, ushort KIf, ushort value)
        {
            short Data = (short)(value);
            double NomData = (double)NomIf / (double)KIf;
            double f = NomData * (double)Data / 8.0;
            return (f.ToString("F0"));
        }

        public static string HexToFCVoltageZero(ushort value)
        {
            double f = (short)value / 39.06;
            return (f.ToString("F1"));
        }

        public static string CalcRefCosFiSTS(ushort value)
        {
            double f = Math.Atan2((short)value, 32768) * 180 / 3.141592;
            return (f.ToString("F1"));
        }

        /// <summary>
        /// Рассчитывает температуру от термодата(Пермь)
        /// Если Value == 32000, то возвращается значение FaultText
        /// </summary>
        public static string CalcTermoTermodat(ushort Value, string FaultText)
        {
            short v = (short)Value;
            if (v == 32000) { return FaultText; }
            else
            {
                double d = v / 10.0;
                return (d.ToString("F1"));
            }
        }
        //32 bit
        public static string HexTo16_16(ulong value)
        {
            double f = (int)value / 65535.0;
            return (f.ToString("F2"));
        }

        //64 bit
        public static string HexTo32_32(ulong value)
        {
            double f = (uint)value / 4294967296.0;
            return (f.ToString("F2"));
        }
        /// <summary>
        /// Рассчитывает температуру от термодата(Пермь)
        /// Если Value == 32000, то возвращается значение FaultText
        /// </summary>
        public static string ToTermoTermodat(this ushort Value, string FaultText)
        {
            return (CalcTermoTermodat(Value, FaultText));
        }

        public static ushort CalcTermoInt(ushort termoV, int TermoFormat)
        {
            ushort[] termoVolt = { 1747, 2061, 2421, 2599, 2781, 3110, 3390, 3613, 3702, 3781, 3846, 3901, 3949, 3988, 4022, 4052 };
            ushort[] temps = { 0, 10, 20, 25, 30, 40, 50, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
            double tlo, thi;
            double tvlo, tvhi;
            double tgrad;
            ushort termoIndex;
            ushort TermoV;

            TermoV = termoV;

            if (TermoFormat != 0) { TermoV = (ushort)(TermoV & 0xFF); TermoV = (ushort)(TermoV << 4); }

            if (TermoV <= 1747) { return (0); }
            if (TermoV >= 4052) { return (100); }

            termoIndex = 1;

            while (termoVolt[termoIndex] < TermoV)
            {
                termoIndex++;
            }

            thi = (double)(temps[termoIndex]); tlo = (double)(temps[termoIndex - 1]);
            tvhi = (double)(termoVolt[termoIndex]); tvlo = (double)(termoVolt[termoIndex - 1]);

            tgrad = TermoV * (thi - tlo) - thi * tvlo + tlo * tvhi;
            tgrad = tgrad / (tvhi - tvlo);
            return (((ushort)(tgrad + 0.5)));
        }
        public static string CalcTermo(ushort termoV, int TermoFormat)
        {
            ushort t = CalcTermoInt(termoV, TermoFormat);
            return (t.ToString());
        }

        public static ushort TermoToHex(double Temp, int TermoFormat)
        {
            ushort[] TermoVolt = { 1747, 2061, 2421, 2599, 2781, 3110, 3390, 3613, 3702, 3781, 3846, 3901, 3949, 3988, 4022, 4052 };


            ushort[] Temps = { 0, 10, 20, 25, 30, 40, 50, 60, 65, 70, 75, 80, 85, 90, 95, 100 };

            double tlo, thi;
            double tvlo, tvhi;

            double TermoV;


            ushort TermoIndex;
            ushort TermoV1;

            if (Temp <= 10) { if (TermoFormat != 0) { return (2061 >> 4); } else { return 2061; } }
            if (Temp >= 100) { if (TermoFormat != 0) { return (4052 >> 4); } else { return 4052; } }

            TermoIndex = 1;

            while (Temps[TermoIndex] < Temp)
            {
                TermoIndex++;
            }


            thi = (double)(Temps[TermoIndex]); tlo = (double)(Temps[TermoIndex - 1]);
            tvhi = (double)(TermoVolt[TermoIndex]); tvlo = (double)(TermoVolt[TermoIndex - 1]);



            TermoV = Temp * (tvhi - tvlo) - tvhi * tlo + tvlo * thi;
            TermoV = TermoV / (thi - tlo);

            TermoV1 = (ushort)(TermoV + 0.5);

            if (TermoFormat != 0) { TermoV1 = (ushort)((TermoV1 + 0x08) >> 4); }

            return (TermoV1);
        }

        #region ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ДЛЯ ВОЛГОДОНСКА
        static string HexToUStatVolgodonsk(ulong value)
        {
            double f = (short)value / 40.96;
            f = 63 * f;
            return (f.ToString("F0"));
        }
        static string HexToIStatVolgodonsk(ulong value)
        {
            double f = (short)value / 40.96;
            f = 7.24 * f;
            return (f.ToString("F0"));
        }
        static string HexToPowerVolgodonsk(ulong value)
        {
            double f = (short)value / 40.96;
            f = 0.07900230143483163111237388252877 * f;
            return (f.ToString("F2"));
        }
        static string HexToIfVolgodonsk(ulong value)
        {
            double f = (short)value / 40.96;
            f = 5.12 * f;
            return (f.ToString("F0"));
        }
        #endregion


        public static string HexToFormat(ulong value, ConvertFormats Format)
        {
            return HexToFormat(value, (byte)Format);
        }

        public static string HexToFormat(ulong value, byte format)
        {

            string str;
            switch (format)
            {
                case 0: { str = HexToPercent(value, 1); } break;
                case 1: { str = HexToUint(value); } break;
                case 2: { str = HexToInt(value); } break;
                case 3: { str = HexToFreq(value); } break;
                case 4: { str = HexTo8_8(value); } break;
                case 5: { str = HexTo0_16(value); } break;
                case 6: { str = HexToSlide(value); } break;
                case 7: { str = HexToDigits(value); } break;
                case 8: { str = HexRegulMode(value); } break;
                case 9: { str = HexToAVRType(value); } break;
                case 10: { str = HexToInt10(value); } break;
                case 11: { str = HexToHex(value); } break;
                case 12: { str = HexToUf(value); } break;
                case 13: { str = HexToFreqNew(value); } break;
                case 14: { str = HexToTT(value); } break;
                case 15: { str = HexToTransAlarm(value); } break;
                case 16: { str = HexToInt8(value); } break;
                case 17: { str = HexToUint1000(value); } break;
                case 18: { str = HexToPercent4(value); } break;
                case 19: { str = HexToFreqNew2(value); } break;
                case 20: { str = HexToPercentUpp(value, 1); } break;
                case 21: { str = HexToFreqUPTF(value); } break;
                case 22: { str = HexTo16_16(value); } break;
                case 23: { str = HexTo32_32(value); } break;
                //case 22: { str = HexToFCRefFreq(value, 1); } break;

                //Функции, сделанные под конкретного заказчика
                case 200: { str = HexToUStatVolgodonsk(value); } break;
                case 201: { str = HexToIStatVolgodonsk(value); } break;
                case 202: { str = HexToPowerVolgodonsk(value); } break;
                case 203: { str = HexToIfVolgodonsk(value); } break;



                default: { str = "Неизв. формат"; } break;

            }
            return (str);
        }

    }
}
