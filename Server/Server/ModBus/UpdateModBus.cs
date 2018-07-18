using System;
using System.Threading;
using ServerLib.Update;
using UniSerialPort;

namespace ServerLib.ModBus
{
   public static partial class UpdateModBus
	{
		public static bool StartPort { get; private set; }
		internal static bool ErrorPort { get; private set; }
		internal static bool StopUpdate { get; set; }

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

		internal static void InitConfigDownloadScope(string enabele, string remove, string type, string comtradeType, string configurationAddr, string oscilCmndAddr, string pathScope, string oscilNominalFrequency)
		{
			try
			{
				ConfigDownloadScope.InitConfigDownloadScope(enabele, remove, type, comtradeType, configurationAddr, oscilCmndAddr, pathScope, oscilNominalFrequency);
			}
			catch
			{
				Log.Log.Write(@"UpdateModBus: InitConfigDownloadScope finish with error", @"Warning");
			}
		}

		internal static void InitConfigModBus(string baudRate, string serialPortParity, string serialPortStopBits, string comPortName, byte addrPort)
		{
			try
			{
				ConfigModBus.InitConfigModBus(Convert.ToInt32(baudRate), serialPortParity,serialPortStopBits, comPortName, addrPort);
			}
			catch 
			{
				Log.Log.Write(@"UpdateModBus: InitConfigModBus finish with error", @"Warning");
				return;
			}

			ConfigModBusPort();
		}

		internal static bool StartModBus()
		{
			if (OpenModBusPort())
			{
				Log.Log.Write(@"UpdateModBus: StartModBus!!!", @"Success");
				return true;
			}
			else
			{
				Log.Log.Write(@"UpdateModBus port not open!", @"Error");
				return false;
			}
		}

		internal static void CloseModBus()
		{
			StartPort = false;
			ErrorPort = false;
			_waitingAnswer = false;
			_running = false;

			_modbusThread?.Abort();
			_modbusThread = null;

			CloseModBusPort();

			Log.Log.Write(@"UpdateModBus: CloseModBus", @"Warning");
		}

		internal static void GetRequest(ushort addrGet, ushort wordCount, UpdateDataObj.SourceClass item)
		{
			DataGetRequest(addrGet, wordCount, item);
		}

		internal static void SetRequest(ushort addrGet, ushort[] value)
		{
			DataSetRequest(addrGet, value);
		}

		private static bool _startDownloadScope;
		private static bool _configScopeDownload;

		private static bool _readAll = true;

		private static void ModBusRead()
		{
			while (_running)
			{
				//Если все получено
				if (_readAll)
				{
					foreach (var source in UpdateDataObj.SourceList)
					{
						source.GetValueRequest();
					}
				}

				Thread.Sleep(10);
			}
		}

		//private static void RunModBusPort()
		//{
		//	while (_running)
		//	{
		//		if (!StopUpdate)
		//		{
		//			if (!SerialPort.IsOpen)  //Если порт закрыт пытаемся открыть его
		//			{
		//				if (ErrorPort)
		//				{
		//					Log.Log.Write(@"UpdateModBus: OpenModBusPort", @"Warning");
		//					OpenModBusPort();

		//					if (ConfigDownloadScope.Enable && SerialPort.IsOpen && _startDownloadScope)
		//					{
		//						ScopeDownloadRequestSet();
		//					}
		//				}
		//			}

		//			Update.UpdateDataObj.ChackChangeStatus.Chack(!ErrorPort);

		//			if (SerialPort.requests.Count == 0) //Ждем пока обработается запрос 
		//			{
		//				DataGetRequest();

		//				ScopoeRequest();
		//			}
		//		}

		//		//Thread.Sleep(ConfigModBus.TimeUpdate);
		//	}
		//}
	}
}
