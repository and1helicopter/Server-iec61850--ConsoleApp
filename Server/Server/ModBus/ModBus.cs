using System;
using System.Threading;
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

        private static Thread _modbusThread;
        private static bool _running;

        public static void InitConfigDownloadScope(string enabele, string remove, string type, string comtradeType, string configurationAddr, string oscilCmndAddr, string pathScope, string oscilNominalFrequency)
        {
            try
            {
                ConfigDownloadScope.InitConfigDownloadScope(enabele, remove, type, comtradeType, configurationAddr, oscilCmndAddr, pathScope, oscilNominalFrequency);
            }
            catch
            {
                Log.Log.Write(@"ModBus: InitConfigDownloadScope finish with error", @"Warning");
            }
        }

        public static void InitConfigModBus(string baudRate, string serialPortParity, string serialPortStopBits, string comPortName, int timeUpdate)
        {
            try
            {
                ConfigModBus.InitConfigModBus(Convert.ToInt32(baudRate), serialPortParity,serialPortStopBits, comPortName, timeUpdate);
            }
            catch 
            {
                Log.Log.Write(@"ModBus: InitConfigModBus finish with error", @"Warning");
                return;
            }

            ConfigModBusPort();
        }

        public static void StartModBus()
        {
            OpenModBusPort();
            Log.Log.Write(@"ModBus: StartModBus!!!", @"Success");
        }

        public static void CloseModBus()
        {
            StartPort = false;
            ErrorPort = false;
            _waitingAnswer = false;
            _running = false;

            _modbusThread?.Abort();
            _modbusThread = null;

            CloseModBusPort();

            Log.Log.Write(@"ModBus: CloseModBus", @"Warning");
        }


        private static bool _startDownloadScope;
        private static bool _configScopeDownload;

        private static void RunModBusPort()
        {
            while (_running)
            {
                if (!SerialPort.IsOpen)     //Если порт закрыт пытаемся открыть его
                {
                    if (ErrorPort)
                    {
                        Log.Log.Write(@"ModBus: OpenModBusPort", @"Warning ");
                        OpenModBusPort();
                    }
                }

                if (SerialPort.requests.Count == 0) //Ждем пока обработается запрос 
                {
                    DataRequest();

                    ScopoeRequest();
                }
            }
        }
    }
}
