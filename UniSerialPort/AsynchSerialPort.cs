using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Xml.Linq;
using System.Xml;
using System.Threading;


namespace UniSerialPort
{
	public class AsynchSerialPort
	{

		public delegate void DataRecieved(bool dataOk, byte[] rxBuffer);
		public delegate void DataRecievedRtu(bool dataOk, ushort[] paramRtu, object param);

		public bool PortError { get; private set; }//= false;
		public event EventHandler SerialPortError;
		public event EventHandler FatalSerialPortError;

		readonly SerialPort _serialPort;
		//System.Windows.Forms.Timer 

		readonly System.Timers.Timer _requestTimer;

		public byte SlaveAddr { get; set; } = 1;

		public int BaudRate
		{
			get => _serialPort.BaudRate;
			set => _serialPort.BaudRate = value;
		}

		public Parity Parity
		{
			get => _serialPort.Parity;
			set => _serialPort.Parity = value;
		}

		public StopBits StopBits
		{
			get => _serialPort.StopBits;
			set => _serialPort.StopBits = value;
		}

		public string PortName
		{
			get => _serialPort.PortName;
			set => _serialPort.PortName = value;
		}

		public bool IsOpen
		{

			get 
			{
				if (SerialPortMode == SerialPortModes.RsMode)
				{
					return _serialPort.IsOpen;
				}
				else
				{
					if (_tcpMaster == null) { return false; }
					return _tcpMaster.Connected;
				}
			
			}
		}

		public bool PortBusy { get; private set; }

		public void UnsetPortBusy()
		{
			PortBusy = false;
		}

		public string IpAddress { get; set; } = "213.21.27.140";
		public ushort PortNum { get; set; } = 502;
		public SerialPortModes SerialPortMode { get; set; } = SerialPortModes.RsMode;

		ModbusTcpMaster _tcpMaster;

		byte[] _tcpReadData;
		bool _tcpWriteOk;

		public void SetNewPortMode(SerialPortModes serialPortMode)
		{
			if (IsOpen)
			{
				throw new Exception("Serial port is open!");
			}

			if (serialPortMode == SerialPortModes.RsMode)
			{
				SerialPortMode = SerialPortModes.RsMode;
				_tcpMaster = null;
			}
			else
			{
				SerialPortMode = SerialPortModes.TcpMode;
			}
		}

		public AsynchSerialPort()
		{
			_serialPort = new SerialPort();
			_serialPort.DataReceived +=serialPort_DataReceived;
			_requestTimer = new System.Timers.Timer
			{
				Interval = 10
			};
			_requestTimer.Elapsed += requestTimer_Tick;
		}

		public void Open()
		{
			PortError = false;
			PortBusy = false;
			if (SerialPortMode == SerialPortModes.RsMode)
			{
				if (_serialPort.IsOpen)
				{
					return;
				}
				_serialPort.Open();
			}
			else if (SerialPortMode == SerialPortModes.TcpMode)
			{
				
				_tcpMaster = new ModbusTcpMaster(IpAddress, PortNum);
				_tcpMaster.OnResponseData +=tcpMaster_OnResponseData;
			}
			_requestTimer.Enabled = true; 
		}

		private bool _flagToClose;
		public void Close()
		{
			if (SerialPortMode == SerialPortModes.RsMode)
			{
				#region RS232Mode
				if (!_serialPort.IsOpen) { return; }
				if (PortError)
				{
					try
					{
						_serialPort.Close();
						_flagToClose = false;
						Requests.Clear();
						PortClosed?.Invoke(null, null);
					}
					catch
					{
						FatalSerialPortError?.Invoke(_serialPort, null);
					}
					return;
				}
				_flagToClose = true;
				#endregion
			}

			if (SerialPortMode == SerialPortModes.TcpMode)
			{
				#region TCPMode
				Requests.Clear();
				if (_tcpMaster == null) {
					PortClosed?.Invoke(null, null);
					return; }
				if (!_tcpMaster.Connected) {
					PortClosed?.Invoke(null, null);
					return; }
				_flagToClose = true;
				PortClosed?.Invoke(null, null);
				#endregion
			}
		}

	   
		void CloseBody()
		{
			_requestTimer.Enabled = false;
			if (SerialPortMode == SerialPortModes.RsMode)
			{
				_serialPort.Close();
			}
			if (SerialPortMode == SerialPortModes.TcpMode)
			{
				
				try
				{
					_tcpMaster.Disconnnect();
				}
				catch
				{
					//ignore
				}
			}
			_flagToClose = false;
			Requests.Clear();
			PortClosed?.Invoke(null, null);
		}
		public event EventHandler PortClosed;


		private readonly EventWaitHandle _waitSerialData = new AutoResetEvent(false);

		void serialPort_DataReceived(object sender, EventArgs e)
		{
			_waitSerialData.Set();
		}


		void tcpMaster_OnResponseData(ushort id, byte unit, byte function, byte[] data)
		{
			if (function == 0x03 || function == 0x04)
			{
				_tcpReadData = data;
				_waitSerialData.Set();
			}
			else if (function == 0x10)
			{
				_tcpWriteOk = true;
				_waitSerialData.Set();
			}
		}


		void SendDataBody(RequestUnit requestUnit)
		{
			if (PortError) { return; }
			if (SerialPortMode == SerialPortModes.TcpMode)
			{
				if (requestUnit.PortAnswerType != PortAnswerType.Rtu)
				{
					throw new Exception("Invalid request  type");
				}
			}

			if (SerialPortMode == SerialPortModes.RsMode)
			{
				try
				{
					if (_serialPort.BytesToRead !=0)
					{
						byte[] buff = new byte[_serialPort.BytesToRead+1];
						_serialPort.Read(buff, 0, _serialPort.BytesToRead);
					}
					Thread.Sleep(3);
					_serialPort.Write(requestUnit.TxBuffer, 0, requestUnit.TxBuffer.Length);
				}
				catch
				{
					PortError = true;
					SerialPortError?.Invoke(_serialPort, null);
					return;

				}
			}

			_tcpReadData = null;
			_tcpWriteOk = false;
			if (SerialPortMode == SerialPortModes.TcpMode)
			{
				if (requestUnit.GetTcpFunction() == TcpFunctions.TcpRead)
				{
					_tcpMaster.ReadHoldingRegister(1, requestUnit.GetSlaveAddr(), requestUnit.GetTcpStartAddr(), requestUnit.GetTcpReadCount());
				}
				else
				{
					_tcpMaster.WriteMultipleRegister(1, requestUnit.GetSlaveAddr(), requestUnit.GetTcpStartAddr(), requestUnit.GetTcpWriteData());
				}
			}

			if (SerialPortMode == SerialPortModes.RsMode)
			{
				_waitSerialData.WaitOne(TimeSpan.FromMilliseconds(100));

				for (int i = 0; i < 100; i++)
				{
					if (_serialPort.BytesToRead < requestUnit.ReceivedBytesThreshold)
					{
						_waitSerialData.WaitOne(TimeSpan.FromMilliseconds(10));
					}
					else break;
				}
			}

			if (SerialPortMode == SerialPortModes.TcpMode)
			{
				_waitSerialData.WaitOne(TimeSpan.FromMilliseconds(3000));
			}

			bool dataOk = false;
			#region RecieveRS232
			if (SerialPortMode == SerialPortModes.RsMode)
			{
				byte[] rxBuffer = new byte[0];

				if (_serialPort.BytesToRead >= requestUnit.ReceivedBytesThreshold)
				{
					int count = _serialPort.BytesToRead;
					rxBuffer = new byte[count];

					try
					{
						_serialPort.Read(rxBuffer, 0, count);
					}
					catch
					{
						PortError = true;
						SerialPortError?.Invoke(_serialPort, null);
						return;
					}

					dataOk = true;
				}
				else
				{
					// System.Windows.Forms.MessageBox.Show(RequestUnit.ReceivedBytesThreshold.ToString());
				}

				switch (requestUnit.PortAnswerType)
				{
					case PortAnswerType.Byte:
						{
							if (requestUnit.DataRecieved != null)
							{
								requestUnit.DataRecieved(dataOk, rxBuffer);
							} break;
						}
					case PortAnswerType.Rtu:
						{
							ushort[] ubuff = new ushort[0];

							if (!ModBusCrc.CheckCrc(rxBuffer, rxBuffer.Length))
							{
								// System.Windows.Forms.MessageBox.Show("Ошибка CRC  " + rxBuffer.Length.ToString());
								dataOk = false;
							}
							else
							{
								if (rxBuffer[1] == 0x03 || rxBuffer[1] == 0x04)
								{
									ModBusCrc.RemoveData(rxBuffer, 3, requestUnit.RtuReadCount, out ubuff);
								}
								else
								{
									ubuff = new ushort[0];
								}
								dataOk = true;
							}

							if (requestUnit.DataRecievedRtu != null)
							{
								requestUnit.DataRecievedRtu(dataOk, ubuff, requestUnit.Param);
							}

						} break;
				}
			}
			#endregion
			#region RecieveTCP
			if (SerialPortMode == SerialPortModes.TcpMode)
			{
				if (requestUnit.GetTcpFunction() == TcpFunctions.TcpRead)
				{
					dataOk = (_tcpReadData != null);
					ushort[] us = new ushort[0];
					if (dataOk) ModBusCrc.RemoveData(_tcpReadData, 0, requestUnit.RtuReadCount, out us);
					requestUnit.DataRecievedRtu?.Invoke(dataOk, us, requestUnit.Param);
				}
				else
				{
					ushort[] us = new ushort[0];
					requestUnit.DataRecievedRtu?.Invoke(_tcpWriteOk, us, requestUnit.Param);
				}
			}
			#endregion

			lock (_locker)
			{
				bool b = CheckQueue(false);
				PortBusy = b;
			}

			
		}

		delegate void SendDataHandler(RequestUnit requestUnit);

		private void SendData(RequestUnit requestUnit)
		{
			
			if (!IsOpen) { return; }
			lock (_locker)
			{
				PortBusy = true;
			}
			SendDataHandler senddelegate = SendDataBody;
			senddelegate.BeginInvoke(requestUnit, null, null);
		}

		public readonly Queue<RequestUnit> Requests = new Queue<RequestUnit>();
		public readonly Queue<RequestUnit> RequestsMain = new Queue<RequestUnit>();
		public void AddRequest(byte[] txBuffer, int receivedBytesThreshold, DataRecieved onDataRecieved)
		{
			if (!IsOpen) { return; }
			lock (_locker)
			{
				Requests.Enqueue(new RequestUnit(txBuffer, receivedBytesThreshold, onDataRecieved));
			}

		}
		private void AddRequest(byte[] txBuffer, int receivedBytesThreshold, DataRecievedRtu onDataRecievedRtu, int rtuReadCount, RequestPriority requestPriority, object param)
		{
			if (!IsOpen) { return; }

			if (requestPriority == RequestPriority.High)
			{
				lock (_locker)
				{
					RequestsMain.Enqueue(new RequestUnit(txBuffer, receivedBytesThreshold, onDataRecievedRtu, rtuReadCount, param));
				}
			}
			else
			{
				lock (_locker)
				{
					var request = new RequestUnit(txBuffer, receivedBytesThreshold, onDataRecievedRtu,rtuReadCount, param);
					Requests.Enqueue(request);
				}
			}
		}

		private void AddRequest(byte[] txBuffer, int receivedBytesThreshold, DataRecievedRtu onDataRecievedRtu, RequestPriority requestPriority, object param)
		{
			lock (_locker)
			{
				AddRequest(txBuffer, receivedBytesThreshold, onDataRecievedRtu, 0, requestPriority, param);
			}
		}


		bool CheckQueue(bool checkPortBusy)
		{
			lock (_locker)
			{
				if (PortBusy && checkPortBusy) { return false; }
			}

			if (_flagToClose) { CloseBody(); return false; }
			if (!IsOpen) { return false; }

			RequestUnit mu;
			
			lock (_locker)
			{
				if (RequestsMain.Count != 0)
				{
					mu = RequestsMain.Dequeue();
					SendData(mu);
					return true;
				}
			}

			lock (_locker)
			{
				if (Requests.Count == 0) { return false; }
				mu = Requests.Dequeue();
				SendData(mu);
				return true;
			}
		}

		readonly object _locker = new object();

		void requestTimer_Tick(object sender, EventArgs e)
		{
				CheckQueue(true);
		}

		private void GetDataRtu(byte slaveAddress, ushort startAddr, ushort wordCount, DataRecievedRtu dataRecievedRtu, RequestPriority requestPriority, object param)
		{
			byte[] buffer = new byte[6];
			buffer[0] = slaveAddress;
			buffer[1] = 0x03;
			buffer[2] = (byte)((startAddr >> 8) & 0xFF);
			buffer[3] = (byte)(startAddr & 0xFF);
			buffer[4] = (byte)((wordCount >> 8) & 0xFF);
			buffer[5] = (byte)(wordCount & 0xFF);
			ModBusCrc.CalcCrc(buffer, 6, out var buffer1);
			AddRequest(buffer1, wordCount * 2 + 5, dataRecievedRtu, wordCount, requestPriority, param);
		}
		public void GetDataRtu(ushort startAddr, ushort wordCount, DataRecievedRtu dataRecievedRtu, object param)
		{
			GetDataRtu(SlaveAddr, startAddr, wordCount, dataRecievedRtu, RequestPriority.Normal, param);
		}
		public void GetDataRtu(ushort startAddr, ushort wordCount, DataRecievedRtu dataRecievedRtu, RequestPriority requestPriority, object param)
		{
			GetDataRtu(SlaveAddr, startAddr, wordCount, dataRecievedRtu, requestPriority, param);
		}

		private void GetDataRtu04(byte slaveAddress, ushort startAddr, ushort wordCount, DataRecievedRtu dataRecievedRtu, RequestPriority requestPriority, object param)
		{
			byte[] buffer = new byte[6];
			buffer[0] = slaveAddress;
			buffer[1] = 0x04;
			buffer[2] = (byte)((startAddr >> 8) & 0xFF);
			buffer[3] = (byte)(startAddr & 0xFF);
			buffer[4] = (byte)((wordCount >> 8) & 0xFF);
			buffer[5] = (byte)(wordCount & 0xFF);
			ModBusCrc.CalcCrc(buffer, 6, out var buffer1);
			AddRequest(buffer1, wordCount * 2 + 5, dataRecievedRtu, wordCount, requestPriority, param);
		}
		public void GetDataRtu04(ushort startAddr, ushort wordCount, DataRecievedRtu dataRecievedRtu, object param)
		{
			GetDataRtu04(SlaveAddr, startAddr, wordCount, dataRecievedRtu, RequestPriority.Normal, param);
		}

		private void SetDataRtu(byte slaveAddress, ushort startAddr, DataRecievedRtu dataRecievedRtu, RequestPriority  requestPriority, object param, params ushort[] data)
		{
			if ((data.Length > 32) || (data.Length < 1))
			{
				dataRecievedRtu?.Invoke(false, new ushort[0], null);
				return;
			}
			byte[] buffer = new byte[9+data.Length*2];
			buffer[0] = slaveAddress;
			buffer[1] = 0x10;
			buffer[2] = (byte)((startAddr >> 8) & 0xFF);
			buffer[3] = (byte)(startAddr & 0xFF);
			buffer[4] = (byte)((data.Length >> 8) & 0xFF);
			buffer[5] = (byte)(data.Length & 0xFF);
			buffer[6] = (byte)(2 * data.Length);
			
			for (int i = 0; i < data.Length; i++)
			{
				buffer[7+i*2] = (byte)((data[i] >> 8) & 0xFF);
				buffer[8+i*2] = (byte)(data[i] & 0xFF);
			}

			ModBusCrc.CalcCrc(buffer, 7+data.Length*2, out var buffer1);
			AddRequest(buffer1, 8, dataRecievedRtu, requestPriority, param);
		}

		public void SetDataRtu(ushort startAddr, DataRecievedRtu dataRecievedRtu, RequestPriority requestPriority, object param, params ushort[] data)
		{
			SetDataRtu(SlaveAddr, startAddr, dataRecievedRtu, requestPriority, param, data);
		}


		// ReSharper disable once UnusedMember.Global
		public void SaveSettingsToFile(string fileName)
		{
			try
			{
				XmlTextWriter textWritter = new XmlTextWriter(fileName, Encoding.UTF8);
				textWritter.WriteStartDocument();
				textWritter.WriteStartElement("Setup");
				textWritter.WriteEndElement();
				textWritter.Close();
			}
			catch
			{
				throw new Exception("Error create file!");
			}

			XmlDocument document = new XmlDocument();
			try
			{
				document.Load(fileName);
			}
			catch
			{
				throw new Exception("Error create file!");
			}

			XmlNode element = document.CreateElement("ComSettings");
			document.DocumentElement?.AppendChild(element); // указываем родителя

			XmlAttribute attribute = document.CreateAttribute("PortName"); // создаём атрибут
			attribute.Value = PortName;
			if (element.Attributes != null)
			{
				element.Attributes.Append(attribute); // добавляем атрибут

				attribute = document.CreateAttribute("Parity"); // создаём атрибут
				attribute.Value = Parity.ToString();
				element.Attributes.Append(attribute); // добавляем атрибут

				attribute = document.CreateAttribute("BaudRate"); // создаём атрибут
				attribute.Value = BaudRate.ToString();
				element.Attributes.Append(attribute); // добавляем атрибут

				attribute = document.CreateAttribute("StopBits"); // создаём атрибут
				attribute.Value = StopBits.ToString();
				element.Attributes.Append(attribute); // добавляем атрибут

				attribute = document.CreateAttribute("SlaveAddr"); // создаём атрибут
				attribute.Value = SlaveAddr.ToString();
				element.Attributes.Append(attribute); // добавляем атрибут

				attribute = document.CreateAttribute("IPAddr"); // создаём атрибут
				attribute.Value = IpAddress;
				element.Attributes.Append(attribute); // добавляем атрибут

				attribute = document.CreateAttribute("IPPortNum"); // создаём атрибут
				attribute.Value = PortNum.ToString();
				element.Attributes.Append(attribute); // добавляем атрибут

				attribute = document.CreateAttribute("PortMode"); // создаём атрибут
				attribute.Value = SerialPortMode.ToString();
				element.Attributes.Append(attribute); // добавляем атрибут
			}

			try
			{
				document.Save(fileName);
			}
			catch
			{
				throw new Exception("Error create file!");
			}

		}

		// ReSharper disable once UnusedMember.Global
		public void LoadSettingsFromFile(string fileName)
		{
			XDocument document;
			try
			{
				document = XDocument.Load(fileName);

			}
			catch
			{
				throw new Exception("Error open xml file!");
			}

			try
			{
				XElement element = document.Root?.Element("ComSettings");
				if (element != null)
				{
					PortName = Convert.ToString(element.Attribute("PortName")?.Value);

					PortNum = Convert.ToUInt16(element.Attribute("IPPortNum")?.Value);
					IpAddress = element.Attribute("IPAddr")?.Value;

					SerialPortMode = element.Attribute("PortMode")?.Value.ToUpper() == "RSMODE"
						? SerialPortModes.RsMode
						: SerialPortModes.TcpMode;

					BaudRate = Convert.ToInt32(element.Attribute("BaudRate")?.Value);
					SlaveAddr = Convert.ToByte(element.Attribute("SlaveAddr")?.Value);

					var strPar = Convert.ToString(element.Attribute("Parity")?.Value);
					var strStops = Convert.ToString(element.Attribute("StopBits")?.Value);

					switch (strPar)
					{
						case "None":
							Parity = Parity.None;
							break;
						case "Even":
							Parity = Parity.Even;
							break;
						default:
							Parity = Parity.Odd;
							break;
					}

					StopBits = strStops == "Two" ? StopBits.Two : StopBits.One;
				}
			}

			catch
			{
				throw new Exception("Error load settings from file!");
			}


		}
	}
}
