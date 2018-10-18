using System;
using System.Threading;
using System.Threading.Tasks;
using UniSerialPort;

namespace ServerLib.ModBus
{
	public static partial class ModBus
	{
		private static readonly AsynchSerialPort SerialPort = new AsynchSerialPort();

		private static readonly object Locker = new object();

		/// <summary>Open Com Port</summary>
		/// <returns>Success status</returns>
		public static bool StartModBus()
		{
			if (Status.IsStart) return false;
			if (!ConfigModBusPort()) return false;

			if (OpenModBusPort())
			{
				Log.Log.Write(@"UpdateModBus: StartModBus!!!", @"Success");
				Status.IsStart = true;
				return true;
			}

			Log.Log.Write(@"UpdateModBus port not open!", @"Error");
			Status.IsStart = false;
			return false;
		}

		private static bool ConfigModBusPort()
		{
			lock (Locker)
			{
				if (SerialPort.IsOpen)
				{
					Log.Log.Write("UpdateModBus port is open! Close UpdateModBus SerialPort and repeat.", "Error");
					return false;
				}

				SerialPort.SerialPortMode = SerialPortModes.RsMode;
				SerialPort.BaudRate = ConfigModBus.BaudRate;
				SerialPort.Parity = ConfigModBus.SerialPortParity;
				SerialPort.StopBits = ConfigModBus.SerialPortStopBits;
				SerialPort.PortName = ConfigModBus.ComPortName;
				SerialPort.SlaveAddr = ConfigModBus.AddrPort;

				Log.Log.Write("UpdateModBus! SerialPort configured", "Success");
				return true;
			}
		}

		private static Thread _checkPortThread;

		private static bool OpenModBusPort()
		{
			try
			{
				lock (Locker)
				{
					SerialPort.Open();

					_checkPortThread = new Thread(CheckPort) {Name = "CheckPort", IsBackground = true};
					_checkPortThread.Start();
				}

				return true;
			}
			catch
			{
				Status.IsStart = false;
				return false;
			}
		}

		private static async void RestartPortInit()
		{
			await RestartPort();
		}

		private static async Task RestartPort()
		{
			Thread.Sleep(100);
			await TryClose();
			await TryOpen();
		}

		private static async Task TryOpen() => OpenModBusPort();
		private static async Task TryClose() => CloseModBusPort();

		private delegate void ChackHandler(dynamic val, dynamic obj, bool status);

		private static readonly ChackHandler СhackHandlerDelegate = Response;

		private static void Response(dynamic val, dynamic obj, bool status)
		{
			_count--;
			Status.IsStart = true;
			Status.IsError = false;
		}

		private static int _count;

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
					if (!SerialPort.PortError && !SerialPort.PortBusy && !SerialPort.IsOpen)
					{
						Status.IsStart = false;
						Status.IsError = false;
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (!SerialPort.PortError && !SerialPort.PortBusy && SerialPort.IsOpen)
					{
						Status.IsStart = true;
						if (_count > 5)
						{
							Status.IsError = true;
							Log.Log.Write("UpdateModBus port: SerialPortError!", "Error");
							Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
							restartPortThread.Start();
							work = false;
						}
						else
						{
							Status.IsError = false;
							GetRequest(255, 1, new { item = new object(), response = СhackHandlerDelegate });
							_count++;
						}
					}
					else if (!SerialPort.PortError && SerialPort.PortBusy && !SerialPort.IsOpen)
					{
						Status.IsStart = false;
						Status.IsError = false;
						SerialPort.UnsetPortBusy();
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (!SerialPort.PortError && SerialPort.PortBusy && SerialPort.IsOpen)
					{
						Status.IsStart = true;
						if (_count > 5)
						{
							Status.IsError = true;
							Log.Log.Write("UpdateModBus port: SerialPortError!", "Error");
						}
						else
						{
							Status.IsError = false;
							GetRequest(255, 1, new { item = new object(), response = СhackHandlerDelegate });
							_count++;
						}
					}
					else if (SerialPort.PortError && !SerialPort.PortBusy && !SerialPort.IsOpen)
					{
						Status.IsStart = false;
						Status.IsError = true;
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (SerialPort.PortError && !SerialPort.PortBusy && SerialPort.IsOpen)
					{
						Status.IsStart = true;
						Status.IsError = true;
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (SerialPort.PortError && SerialPort.PortBusy && !SerialPort.IsOpen)
					{
						Status.IsStart = false;
						Status.IsError = true;
						SerialPort.UnsetPortBusy();
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
					else if (SerialPort.PortError && SerialPort.PortBusy && SerialPort.IsOpen)
					{
						Status.IsStart = true;
						Status.IsError = true;
						SerialPort.UnsetPortBusy();
						Thread restartPortThread = new Thread(RestartPortInit) { Name = "RestartPort", IsBackground = true };
						restartPortThread.Start();
						work = false;
					}
				}
			}
		}



		/// <summary>Close ModBus Port</summary>
		/// <returns>IsClose = !IsOpen</returns>
		public static bool CloseModBus()
		{
			try
			{
				CloseModBusPort();
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
				if(SerialPort.IsOpen) SerialPort.Close();
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
		public static void GetRequest(ushort addrGet, ushort wordCount, object param)
		{
			if(Status.IsStart && !Status.IsError)
				DataGetRequest(addrGet, wordCount, param);
			else 
				DataGetResponse(false, new ushort[] {0}, param);
		}

		/// <summary>Set data</summary>
		/// <param name="addrGet">Address set</param>
		/// <param name="value">Value set</param>
		public static void SetRequest(ushort addrGet, ushort[] value)
		{
			if (Status.IsStart && !Status.IsError)
				DataSetRequest(addrGet, value);
		}

		/// <summary>
		/// Status
		/// </summary>
		public static class Status
		{
			public static bool IsError { get; set; }
			public static bool IsStart { get; set; }
			public  static bool IsEmpty { get {
				lock (Locker)
				{
					return SerialPort.Requests.Count == 0;
				}
			} }

		}
	}
}
