using System;
using System.Threading;
using System.Threading.Tasks;
using UniSerialPort;

namespace ServerLib.ModBus
{
	public static partial class ModBus
	{
		private static readonly object Locker = new object();
		
		private static Thread _checkPortThread;

		private static async void RestartPortInit()
		{
			await RestartPort();
		}

		private static async Task RestartPort()
		{
			Thread.Sleep(100);
			await Task.Run((Action)CloseModBusPort);
			await Task.Run((Action)OpenModBusPort); 
		}

		private delegate void ChackHandler(dynamic val, dynamic obj, bool status);

		private static readonly ChackHandler СhackHandlerDelegate = Response;

		private static void Response(dynamic val, dynamic obj, bool status)
		{
			_count--;
			ModBusPort.IsStart = true;
			ModBusPort.IsError = false;
		}

		private static volatile int _count;

		/// <summary>
		/// Checked status Com Port
		/// </summary>
		private static void CheckPort()
		{
			_count = 0;
			bool work = true;
			while (work)
			{
				Thread.Sleep(250);

				lock (Locker)
				{
					if (!ModBusPort.GetInstance().PortError && 
					    !ModBusPort.GetInstance().PortBusy && 
					    !ModBusPort.GetInstance().IsOpen)
					{
						ModBusPort.IsStart = false;
						ModBusPort.IsError = false;
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (!ModBusPort.GetInstance().PortError &&
							 !ModBusPort.GetInstance().PortBusy &&
							 ModBusPort.GetInstance().IsOpen)
					{
						ModBusPort.IsStart = true;
						if (_count > 5)
						{
							ModBusPort.IsError = true;
							Log.Log.Write("UpdateModBus port: SerialPortError!", "Error");
							Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
							restartPortThread.Start();
							work = false;
						}
						else
						{
							ModBusPort.IsError = false;
							GetRequest(255, 1, new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = new object(), Response = СhackHandlerDelegate });
							_count++;
						}
					}
					else if (!ModBusPort.GetInstance().PortError && 
					         ModBusPort.GetInstance().PortBusy && 
					         !ModBusPort.GetInstance().IsOpen)
					{
						ModBusPort.IsStart = false;
						ModBusPort.IsError = false;
						ModBusPort.GetInstance().UnsetPortBusy();
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (!ModBusPort.GetInstance().PortError && 
					         ModBusPort.GetInstance().PortBusy && 
					         ModBusPort.GetInstance().IsOpen)
					{
						ModBusPort.IsStart = true;
						if (_count > 5)
						{
							ModBusPort.IsError = true;
							Log.Log.Write("UpdateModBus port: SerialPortError!", "Error");
							ModBusPort.GetInstance().UnsetPortBusy();
							//Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
							//restartPortThread.Start();
							//work = false;
						}
						else
						{
							ModBusPort.IsError = false;
							GetRequest(255, 1, new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = new object(), Response = СhackHandlerDelegate });
							_count++;
						}
					}
					else if (ModBusPort.GetInstance().PortError && 
					         !ModBusPort.GetInstance().PortBusy && 
					         !ModBusPort.GetInstance().IsOpen)
					{
						ModBusPort.IsStart = false;
						ModBusPort.IsError = true;
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (ModBusPort.GetInstance().PortError && 
					         !ModBusPort.GetInstance().PortBusy &&
					         ModBusPort.GetInstance().IsOpen)
					{
						ModBusPort.IsStart = true;
						ModBusPort.IsError = true;
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (ModBusPort.GetInstance().PortError && 
					         ModBusPort.GetInstance().PortBusy && 
					         !ModBusPort.GetInstance().IsOpen)
					{
						ModBusPort.IsStart = false;
						ModBusPort.IsError = true;
						ModBusPort.GetInstance().UnsetPortBusy();
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (ModBusPort.GetInstance().PortError &&
					         ModBusPort.GetInstance().PortBusy &&
					         ModBusPort.GetInstance().IsOpen)
					{
						ModBusPort.IsStart = true;
						ModBusPort.IsError = true;
						ModBusPort.GetInstance().UnsetPortBusy();
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
				}
			}
		}

		/// <summary>
		/// Initialize ModBus
		/// </summary>
		/// <param name="baudRate">Baud Rate</param>
		/// <param name="serialPortParity">Port Parity</param>
		/// <param name="serialPortStopBits">Port StopBits</param>
		/// <param name="comPortName">Name Com Port</param>
		/// <param name="addrPort">Addres Port</param>
		/// <returns></returns>
		public static bool InitConfigModBus(string baudRate, string serialPortParity, string serialPortStopBits, string comPortName, byte addrPort)
		{
			try
			{
				ConfigModBus.InitConfigModBus(Convert.ToInt32(baudRate), serialPortParity, serialPortStopBits, comPortName, addrPort);
				return true;
			}
			catch
			{
				Log.Log.Write(@"UpdateModBus: InitConfigModBus finish with error", @"Warning");
				return false;
			}
		}

		/// <summary>Get data</summary>
		/// <param name="addrGet">Address get</param>
		/// <param name="wordCount">Count world get</param>
		/// <param name="param">new {item = ... "item callback", response([value] , item, statusOk) = ..."what do when load data complete"}</param>
		/// <returns>Callback function response([value], item, statusOk)</returns>
		public static void GetRequest(ushort addrGet, ushort wordCount, ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj param)
		{
			if(ModBusPort.IsStart && !ModBusPort.IsError)
				DataGetRequest(addrGet, wordCount, param);
			else 
				DataGetResponse(false, new ushort[] {0}, param);
		}

		/// <summary>Get data</summary>
		/// <param name="addrGet">Address get</param>
		/// <param name="wordCount">Count world get</param>
		/// <param name="param">new {item = ... "item callback", response([value] , item, statusOk) = ..."what do when load data complete"}</param>
		/// <returns>Callback function response([value], item, statusOk)</returns>
		public static void GetRequest04(ushort addrGet, ushort wordCount, ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj param)
		{
			if (ModBusPort.IsStart && !ModBusPort.IsError)
				DataGetRequest04(addrGet, wordCount, param);
			else
				DataGetResponse(false, new ushort[] { 0 }, param);
		}

		/// <summary>Set data</summary>
		/// <param name="addrGet">Address set</param>
		/// <param name="value">Value set</param>
		public static void SetRequest(ushort addrGet, ushort[] value)
		{
			if (ModBusPort.IsStart && !ModBusPort.IsError)
				DataSetRequest(addrGet, value);
		}

		/// <summary>Open Com Port</summary>
		/// <returns>Success status</returns>
		public static bool StartModBus()
		{
			if (ModBusPort.IsStart) return false;
			if (!ConfigModBusPort()) return false;
			OpenModBusPort();
			if (ModBusPort.GetInstance().IsOpen)
			{
				Log.Log.Write(@"UpdateModBus: StartModBus!!!", @"Success");
				ModBusPort.IsStart = true;
				return true;
			}

			Log.Log.Write(@"UpdateModBus port not open!", @"Error");
			ModBusPort.IsStart = false;
			return false;
		}

		private static bool ConfigModBusPort()
		{
			lock (Locker)
			{
				if (ModBusPort.GetInstance().IsOpen)
				{
					Log.Log.Write("UpdateModBus port is open! Close UpdateModBus SerialPort and repeat.", "Error");
					return false;
				}

				ModBusPort.GetInstance().SerialPortMode = SerialPortModes.RsMode;
				ModBusPort.GetInstance().BaudRate = ConfigModBus.BaudRate;
				ModBusPort.GetInstance().Parity = ConfigModBus.SerialPortParity;
				ModBusPort.GetInstance().StopBits = ConfigModBus.SerialPortStopBits;
				ModBusPort.GetInstance().PortName = ConfigModBus.ComPortName;
				ModBusPort.GetInstance().SlaveAddr = ConfigModBus.AddrPort;

				Log.Log.Write("UpdateModBus! SerialPort configured", "Success");
				return true;
			}
		}

		private static void OpenModBusPort()
		{
			try
			{
				lock (Locker)
				{
					ModBusPort.GetInstance().Open();
					_checkPortThread = new Thread(CheckPort) { Name = "CheckPort", IsBackground = true };
					_checkPortThread.Start();
				}
			}
			catch
			{
				ModBusPort.IsStart = false;
			}
		}

		/// <summary>Close ModBus Port</summary>
		/// <returns>IsClose = !IsOpen</returns>
		public static bool CloseModBus()
		{
			try
			{
				lock (Locker)
				{
					_checkPortThread.Abort();
					_checkPortThread = null;
					_count = 0;
				}

				ModBusPort.Abort();

				Log.Log.Write(@"UpdateModBus: CloseModBus", @"Success");
				return true;
			}
			catch
			{
				Log.Log.Write(@"UpdateModBus: CloseModBus", @"Error");
				return false;
			}
		}

		private static void CloseModBusPort()
		{
			lock (Locker)
			{
				ModBusPort.Close();
			}
		}

		internal class ModBusPort
		{
			private static AsynchSerialPort _instance;

			public static AsynchSerialPort GetInstance()
			{
				return _instance ?? (_instance = new AsynchSerialPort());
			}

			public static void Close()
			{
				if(_instance != null)
					if (_instance.IsOpen)
					{
						_instance.UnsetPortBusy();
						_instance.Close();
					}
			}

			public static void QueueClear()
			{
				_instance?.Requests.Clear();
				_instance?.RequestsMain.Clear();
			}

			public static void Abort()
			{
				if (_instance != null)
					if (_instance.IsOpen)
					{
						_instance.UnsetPortBusy();
						_instance.Close();
					}

				IsError = false;
				IsStart = false;
			}

			internal static bool IsError { get; set; }
			internal static bool IsStart { get; set; }
			internal static bool IsEmpty { get {
				lock (Locker)
				{
					try
					{
						return _instance?.Requests.Count == 0;
					}
					catch 
					{
						return false;
					}
				}
			}}
		}
	}
}
