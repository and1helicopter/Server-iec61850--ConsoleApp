﻿using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UniSerialPort;

namespace ServerLib.ModBus
{
	public static partial class UpdateModBus
	{
		public static void ConfigModBusPort()
		{
			if (SerialPort.IsOpen)
			{
				Log.Log.Write("UpdateModBus port is open! Close UpdateModBus SerialPort and repeat.", "Error");
				return;
			}

			SerialPort.SerialPortMode = SerialPortModes.RSMode;
			SerialPort.BaudRate = ConfigModBus.BaudRate;
			SerialPort.Parity = ConfigModBus.SerialPortParity;
			SerialPort.StopBits = ConfigModBus.SerialPortStopBits;
			SerialPort.PortName = ConfigModBus.ComPortName;
			SerialPort.SlaveAddr = ConfigModBus.AddrPort;


			Log.Log.Write("UpdateModBus! SerialPort configured", "Success");
		}

		private static bool OpenModBusPort()
		{
			try
			{
				lock (Locker)
				{
					SerialPort.Open();
					SerialPort.SerialPortError += SerialPort_SerialPortError;
					StartPort = true;
					ErrorPort = false;
					_waitingAnswer = false;
				}
				if (SerialPort.IsOpen)
				{
					_running = true;

					if (_modbusThread == null)
					{
						_modbusThread = new Thread(ModBusRead)
						{
							Name = @"UpdateModBus"
						};
						_modbusThread.Start();
					}

					if (_configScopeDownload)
					{

					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static void SerialPort_SerialPortError(object sender, System.EventArgs e)
		{
			Log.Log.Write("UpdateModBus port: SerialPortError!", "Error");

			ErrorPort = true;

			SerialPort.requests.Clear();

			CloseModBusPort();
			StartPort = false;
		}

		private static void CloseModBusPort()
		{
			lock (Locker)
			{
				SerialPort.Close();
			}
		}
	}

	/* Настройки UpdateModBus port  */

	public static class ConfigModBus
	{
		public static int BaudRate { get; private set; }
		public static Parity SerialPortParity { get; private set; }
		public static StopBits SerialPortStopBits { get; private set; }
		public static string ComPortName { get; private set; }
		public static byte AddrPort { get; private set; }

		private static void ChangeBaudRate(int baudRate)
		{
			switch (baudRate)
			{
				case 9600:
					BaudRate = 9600;
					return;
				case 19200:
					BaudRate = 19200;
					return;
				case 38400:
					BaudRate = 38400;
					return;
				case 57600:
					BaudRate = 57600;
					return;
				case 115200:
					BaudRate = 115200;
					return;
				case 230400:
					BaudRate = 230400;
					return;
				default:
					BaudRate = 9600;
					break;
			}
		}
		
		private static void ChangeSerialPortParity(string serialPortParity)
		{
			switch (serialPortParity)
			{
				case @"Odd":
					SerialPortParity = Parity.Odd;
					return;
				case @"Even":
					SerialPortParity = Parity.Even;
					return;
				case @"None":
					SerialPortParity = Parity.None;
					return;
				default:
					SerialPortParity = Parity.Odd;
					break;
			}
		}

		private static void ChangeSerialPortStopBits(string serialPortStopBits)
		{
			switch (serialPortStopBits)
			{
				case @"One":
				{
					SerialPortStopBits = StopBits.One;
					return;
				}
				case @"Two":
				{
					SerialPortStopBits = StopBits.Two;
					return;
				}
				default:
					SerialPortStopBits = StopBits.One;
					break;
			}

		}

		private static void ChangeComPortName(string comPort)
		{
			ReadPortList();

			if (PortList.Count != 0)
			{
				ComPortName = PortList.Contains(comPort) ? comPort : PortList[0];
			}
			else
			{
				ComPortName = "";
			}

		}

		private static void ChangeAddrPort(byte addrPort)
		{
			AddrPort = addrPort;
		}

		//Список всех COM-портов на компьютере
		private static readonly List<string> PortList = new List<string>();

		private static void ReadPortList()
		{
			string[] portStrList = SerialPort.GetPortNames();
			PortList.Clear();
			foreach (string port in portStrList)
			{
				PortList.Add(port);
			}
			PortList.Sort();
		}

		public static void InitConfigModBus(int serialPortSpeedIndex, string serialPortParity, string serialPortStopBits, string comPort, byte addrPort)
		{
			ChangeBaudRate(serialPortSpeedIndex);
			ChangeSerialPortParity(serialPortParity);
			ChangeSerialPortStopBits(serialPortStopBits);
			ChangeComPortName(comPort);
			ChangeAddrPort(addrPort);
		}
	}


}