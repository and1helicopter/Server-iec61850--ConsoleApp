using System;
using System.Timers;

namespace Server.ModBus
{
   public static partial class ModBus
    {
        public static bool StartPort { get; private set; }
        public static bool ErrorPort { get; private set; }

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

        private static readonly Timer DownloadDataTimer = new Timer
        {
            Interval = 100,
            Enabled = false
        };

        //private static readonly Timer DownloadScopeTimer = new Timer
        //{
        //    Interval = 2000,
        //    Enabled = false
        //};

        private static bool _startDownloadScope;
        private static bool _configScopeDownload;

        private static void StartModBusPort()
        {
            //Обновление параметров
            DownloadDataTimer.Elapsed += downloadDataTimer_Elapsed;
            downloadDataTimer_Elapsed(null, null);
            DownloadDataTimer.Enabled = true;

            ////Обновление осциллограммы
            //if (Settings.Settings.ConfigGlobal.DownloadScope)
            //{
            //    DownloadScopeTimer.Elapsed += downloadScopeTimer_Elapsed;
            //    downloadScopeTimer_Elapsed(null, null);
            //    DownloadScopeTimer.Enabled = true;
            //}
        }
        
        private static void downloadDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            DataRequest();

            //if ()
            //{
                
            //}

        }

        private static void downloadScopeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {




            ScopoeRequest();

        }
    }
}
