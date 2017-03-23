using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADSPLibrary
{
    public static class SystemParamsFormat
    {
        public static ushort NomUAddr = 0x0020;
        public static ushort NomIAddr = 0x0021;
        public static ushort KttAddr = 0x0024;
        public static ushort NomIfAddr = 0x0026;
        public static ushort KIfAddr = 0x0025;
        public static ushort NomUfAddr = 0x022;
        public static ushort NomUTEAddr = 0x0023;

        public static ushort StandartSTSRegUAddr = 0x0060;
        public static ushort StandartSTSRegUKdAddr = 0x00A3;
        public static ushort StandartSTSRegIfAddr = 0x0063;
        public static ushort StandartSTSRegIreAddr = 0x0066;

        public static ushort StandartSTSRegSlowIreAddr = 0x00BE;
        public static ushort StandartSTSRegSlowPFAddr = 0x00BF;

        public static ushort StandartSTSStatismAddr = 0x005A;

        public static ushort SystemStabCoef1Addr = 0x00A7;
        public static ushort SystemStabCoef2Addr = 0x00AB;
        public static ushort SystemStabCoef3Addr = 0x00AF;
        public static ushort SystemStabEnaAddr = 0x003F;

        public static ushort OverheatTiAddr = 0x0053;
        public static ushort OverheatDBPosAddr = 0x0051;
        public static ushort OverheatDBNegAddr = 0x0052;
        public static ushort OverheatDropDownAddr = 0x0054;
        public static ushort OverheatForceIfAddr = 0x0056;


        public static ushort StartUELAddr = 0x00C0;


        public static ushort StandartSTSLimRefUAddr = 0x0048;
        public static ushort StandartSTSLimRefIfAddr = 0x004E;
        public static ushort StandartSTSLimRefFiAddr = 0x0079;
        public static ushort StandartSTSLimRefFi2Addr = 0x004A;
        public static ushort StandartSTSLimRefIreAddr = 0x004C;


        public static ushort StandartSTSRelayRefEnaAddr     = 0x00B6;
        public static ushort StandartSTSRelayRefDisaAddr    = 0x00B7;
        public static ushort StandartSTSRelayDelayAddr      = 0x00B5;
        public static ushort StandartSTSRelayEnabledAddr    = 0x003E;

        public static ushort StandartSTSVHZHiAddr = 0x00B1;
        public static ushort StandartSTSVHZLoAddr = 0x00B2;
        public static ushort StandartSTSVHZEnabledAddr = 0x003D;

        public static ushort StandartSTSInvCurrAddr = 0x003C;
        public static ushort StandartSTSISOAddr = 0x008F;

        public static ushort StandartSTSReleNETime = 0x009B;
        public static ushort StandartSTSRefMANDefault = 0x0075;

        public static ushort StandartSTSOverVAddr = 0x0080;
        public static ushort StandartSTSOverVTimeAddr = 0x0081;

        public static ushort StandartSTSOverCurrVDAddr = 0x0082;
        public static ushort StandartSTSOverCurrVDTimeAddr = 0x0096;

        public static ushort StandartSTSTransAlarmAddr = 0x00D8;
        public static ushort StandartSTSTransAlarmTimeAddr = 0x00D9;

        public static ushort StandartSTSTransAlarm2Addr = 0x010A;
        public static ushort StandartSTSTransAlarm2TimeAddr = 0x010B;

        public static ushort StandartSTSOverVTEAddr = 0x008B;

        public static ushort StandartSTSLEAddr = 0x0088;
        public static ushort StandartSTSOverCurr1Addr = 0x0085;

        public static ushort StandartSTSAsynchAddr      = 0x0071;
        public static ushort StandartSTSAsynchTimeAddr  = 0x0070;

        public static ushort StandartSTSLowFreqProtectAddr = 0x010E;
        public static ushort StandartSTSLowFreqProtectEnaAddr = 0x011F;

        public static ushort StandartSTSRotorProtectAddr = 0x0110;
        public static ushort StandartSTSRotorProtectTimeAddr = 0x0118;

        public static ushort StandartSTSPasswordAddr = 0x00D4;

    }
}
