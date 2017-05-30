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

        private static ConfigDownloadScope _downloadScope;

        private static readonly int[] NowStatus = new int[32];
        private static readonly int[] OldStatus = new int[32];

        private static int _indexDownloadScope;
        private static uint _oscilStartTemp;

        private static ConfigModBus _modbus;
        private static readonly AsynchSerialPort SerialPort = new AsynchSerialPort();
        private static readonly object Locker = new object();

        private static int _currentIndexGet;
        private static int _currentIndexSet;

        public static void ConfigDownloadScope(string enabele, string remove, string type, string comtradeType, string configurationAddr, string oscilCmndAddr, string pathScope, string oscilNominalFrequency)
        {
            try
            {
                _downloadScope = new ConfigDownloadScope(enabele, remove, type, comtradeType, configurationAddr, oscilCmndAddr, pathScope, oscilNominalFrequency);
            }
            catch
            {
                Logging.Log.Write("ModBus: ConfigDownloadScope finish with error", "Warning ");
            }

        }

        public static void ConfigModBus(string baudRate, string serialPortParity, string serialPortStopBits, string comPortName)
        {
            try
            {
                _modbus = new ConfigModBus(Convert.ToInt32(baudRate), serialPortParity, serialPortStopBits, comPortName);
            }
            catch 
            {
                Logging.Log.Write("ModBus: ConfigModBus finish with error", "Warning ");
                return;
            }

            ConfigModBusPort();
        }

        public static void StartModBus()
        {
            OpenModBusPort();
            Logging.Log.Write("ModBus: StartModBus!!!", "Success ");
        }

        public static void CloseModBus()
        {
            CloseModBusPort();
            Logging.Log.Write("ModBus: CloseModBus", "Warning ");
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
                    Logging.Log.Write("ModBus: OpenModBusPort", "Warning ");
                    OpenModBusPort();
                }
            }

            DataRequest();

            if (_downloadScope.Enable)
            {
                ScopoeRequest();
            }

            DownloadTimer.Enabled = true;
        }
    }
}
