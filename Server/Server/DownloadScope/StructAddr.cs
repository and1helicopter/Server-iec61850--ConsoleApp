namespace ScopeApp
{
    public static class StructAddr
    {
        //OscilConfig_type
        public static int OscilTypeData = 0;
        public static int OscilAddr = 32;
        public static int OscilSize = 64;
        public static int OscilQuantity = 66;
        public static int OscilChNum = 67;
        public static int OscilHistoryPercent = 68;
        public static int OscilFreqDiv = 69;
        public static int OscilEnable = 70;
        public static int OscilChNumName = 72;
        public static int OscilComtradeConfig = 584;
        //for comtrade
        public static int Phase = 0;
        public static int Ccbm = 32;
        public static int Demension = 288;
        public static int Type = 416;
        public static int StationName = 448;
        public static int RecordingId = 464;
        public static int TimeCode = 472;
        public static int LocalCode = 476;
        public static int TmqCode = 480;
        public static int Leapsec = 484;
        //OscilCmnd_type
        public static int OscilMemorySize = 0;//Old 376
        public static int OscilSampleRate = 2;
        public static int OscilSampleSize = 3;
        public static int OscilHistoryCount = 6;//Old 0
        public static int OscilEnd = 16;//Old 72
        public static int OscilDateTime = 80;//Old 136
        public static int OscilStatus = 336;//Old 8
        public static int OscilNeed = 368;//Old 4
        public static int OscilStatusLoad = 369;//Old 378
        public static int OscilNewConfig = 370;//Old 328
        public static int Padding = 12;  //Случайный участок памяти для проверки
    }
}
