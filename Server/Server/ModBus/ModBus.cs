using System;
using System.Timers;
using UniSerialPort;

namespace Server.ModBus
{
   public static partial class ModBus
    {
        public static bool StartPort { get; private set; }
        public static bool ErrorPort { get; private set; }

        private static int _loadConfigStep;
        private static int _indexChannel;
        private static bool _waitingAnswer;

        private static readonly int[] NowStatus = new int[32];
        private static readonly int[] OldStatus = new int[32];

        private static int _indexDownloadScope;
        private static uint _oscilStartTemp;

        private static readonly AsynchSerialPort SerialPort = new AsynchSerialPort();
        private static readonly object Locker = new object();

        private static int _currentIndexGet;
        private static int _currentIndexSet;

        public static void InitConfigDownloadScope(string enabele, string remove, string type, string comtradeType, string configurationAddr, string oscilCmndAddr, string pathScope, string oscilNominalFrequency)
        {
            try
            {
                ConfigDownloadScope.InitConfigDownloadScope(enabele, remove, type, comtradeType, configurationAddr, oscilCmndAddr, pathScope, oscilNominalFrequency);
            }
            catch
            {
                Log.Log.Write("ModBus: InitConfigDownloadScope finish with error", "Warning ");
            }
        }

        public static void InitConfigModBus(string baudRate, string serialPortParity, string serialPortStopBits, string comPortName)
        {
            try
            {
                ConfigModBus.InitConfigModBus(Convert.ToInt32(baudRate), serialPortParity,serialPortStopBits, comPortName);
            }
            catch 
            {
                Log.Log.Write("ModBus: InitConfigModBus finish with error", "Warning ");
                return;
            }

            ConfigModBusPort();
        }

        public static void StartModBus()
        {
            OpenModBusPort();
            Log.Log.Write("ModBus: StartModBus!!!", "Success ");
        }

        public static void CloseModBus()
        {
            CloseModBusPort();
            Log.Log.Write("ModBus: CloseModBus", "Warning ");
        }

        private static readonly Timer DownloadTimer = new Timer
        {
            Interval = 20,
            Enabled = false
        };

        private static bool _startDownloadScope;
        private static bool _configScopeDownload;

        private static void StartModBusPort()
        {
            //Обновление параметров
            DownloadTimer.Elapsed += downloadTimer_Elapsed;
            downloadTimer_Elapsed(null, null);
        }
        
        private static void downloadTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DownloadTimer.Enabled = false;

            if (!SerialPort.IsOpen)     //Если порт закрыт пытаемся открыть его
            {
                if (ErrorPort)
                {
                    Log.Log.Write("ModBus: OpenModBusPort", "Warning ");
                    OpenModBusPort();
                }
            }

            DataRequest();

            if (ConfigDownloadScope.Enable)
            {
                ScopoeRequest();
            }

            DownloadTimer.Enabled = true;
        }
    }
}
