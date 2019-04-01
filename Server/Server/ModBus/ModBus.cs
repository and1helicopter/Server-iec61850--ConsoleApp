using System;
using UniSerialPort;

namespace ServerLib.ModBus
{
	public static partial class ModBus
	{
		internal static readonly object Locker = new object();
             
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
			if(ModBusPort.SerialPort.IsOpen && !ModBusPort.SerialPort.PortError)
            {
                DataGetRequest(addrGet, wordCount, param);

            }
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
			if (ModBusPort.SerialPort.IsOpen && !ModBusPort.SerialPort.PortError)
				DataGetRequest04(addrGet, wordCount, param);
			else
				DataGetResponse(false, new ushort[] { 0 }, param);
		}

		/// <summary>Set data</summary>
		/// <param name="addrGet">Address set</param>
		/// <param name="value">Value set</param>
		public static void SetRequest(ushort addrGet, ushort[] value)
		{
			if (ModBusPort.SerialPort.IsOpen && !ModBusPort.SerialPort.PortError)
				DataSetRequest(addrGet, value);
		}

		/// <summary>Open Com Port</summary>
		/// <returns>Success status</returns>
		public static bool StartModBus()
		{
            if (!ConfigModBusPort()) return false;
            OpenModBusPort();
            var isOpen = ModBusPort.SerialPort.IsOpen;
            Log.Log.Write($"UpdateModBus: { (isOpen ? "StartModBus!!!" : "port not open")}",
                          $"{(isOpen ? "Success" : "Error")}");
            return isOpen;
        }

		private static bool ConfigModBusPort()
		{
            if (ModBusPort.SerialPort.IsOpen)
            {
                Log.Log.Write("UpdateModBus port is open! Close UpdateModBus SerialPort and repeat.", "Error");
                return false;
            }

            ModBusPort.SerialPort.SerialPortMode = SerialPortModes.RsMode;
            ModBusPort.SerialPort.BaudRate = ConfigModBus.BaudRate;
            ModBusPort.SerialPort.Parity = ConfigModBus.SerialPortParity;
            ModBusPort.SerialPort.StopBits = ConfigModBus.SerialPortStopBits;
            ModBusPort.SerialPort.PortName = ConfigModBus.ComPortName;
            ModBusPort.SerialPort.SlaveAddr = ConfigModBus.AddrPort;

            Log.Log.Write("UpdateModBus! SerialPort configured", "Success");
            return true;
        }

        private static void OpenModBusPort()
		{
			try
			{
                lock (Locker)
                {
                    ModBusPort.Open();
                }

                Log.Log.Write(@"UpdateModBus: OpenModBus", @"Success");
            }
            catch
			{
                Log.Log.Write(@"UpdateModBus: OpenModBus", @"Error");
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
                    ModBusPort.Close();
                }
                //ModBusPort.Abort();

                Log.Log.Write(@"UpdateModBus: CloseModBus", @"Success");
				return true;
			}
			catch
			{
				Log.Log.Write(@"UpdateModBus: CloseModBus", @"Error");
				return false;
			}
		}

		internal class ModBusPort
		{
            private static AsynchSerialPort _serialPort = new AsynchSerialPort();

            public static AsynchSerialPort SerialPort => _serialPort;

            public static void Open()
            {
                _serialPort.Open();
                _serialPort.SerialPortError += SerialPort_SerialPortError;
                _serialPort.FatalSerialPortError += _serialPort_FatalSerialPortError;
            }
                       
            public static void Close()
			{
                if (_serialPort != null)
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Requests.Clear();
                        _serialPort.UnsetPortBusy();
                        _serialPort.Close();
                        //IsStart = false;
                    }
            }

            internal static DateTime SuccessRead { get; set; }
            //internal static bool IsError { get; set; }
            //internal static bool IsStart { get; set; }
            //internal static bool IsEmpty { get {
            //	lock (Locker)
            //	{
            //		try
            //		{
            //			return _serialPort?.Requests.Count == 0;
            //		}
            //		catch 
            //		{
            //			return false;
            //		}
            //	}
            //}}
        }

        private static void SerialPort_SerialPortError(object sender, EventArgs e)
        {

        }

        private static void _serialPort_FatalSerialPortError(object sender, EventArgs e)
        {

        }
    }
}
