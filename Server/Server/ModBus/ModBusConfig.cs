using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UniSerialPort;

namespace Server.ModBus
{
    public static partial class ModBus
    {
        public static void ConfigModBusPort()
        {
            if (SerialPort.IsOpen)
            {
                Log.Log.Write("ModBus port is open! Close ModBus SerialPort and repeat.", "Error");
                return;
            }

            SerialPort.SerialPortMode = SerialPortModes.RSMode;
            SerialPort.BaudRate = ConfigModBus.BaudRate;
            SerialPort.Parity = ConfigModBus.SerialPortParity;
            SerialPort.StopBits = ConfigModBus.SerialPortStopBits;
            SerialPort.PortName = ConfigModBus.ComPortName;
            
            Log.Log.Write("ModBus! SerialPort configured", "Success");
        }

        private static void OpenModBusPort()
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
                        _modbusThread = new Thread(RunModBusPort)
                        {
                            Name = @"ModBus"
                        };
                        _modbusThread.Start();
                    }
                }
            }
            catch
            {
                Log.Log.Write("ModBus port not open!", "Error");
            }
        }

        private static void SerialPort_SerialPortError(object sender, System.EventArgs e)
        {
            ErrorPort = true;

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

    /* Настройки ModBus port  */

    public static class ConfigModBus
    {
        public static int BaudRate { get; private set; }
        public static Parity SerialPortParity { get; private set; }
        public static StopBits SerialPortStopBits { get; private set; }
        public static string ComPortName { get; private set; }
        public static int TimeUpdate { get; private set; }

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

        private static void ChangeTimeUpdate(int timeUpdate)
        {
            TimeUpdate = timeUpdate;
        }

        public static void InitConfigModBus(int serialPortSpeedIndex, string serialPortParity, string serialPortStopBits, string comPort, int timeUpdate)
        {
            ChangeBaudRate(serialPortSpeedIndex);
            ChangeSerialPortParity(serialPortParity);
            ChangeSerialPortStopBits(serialPortStopBits);
            ChangeComPortName(comPort);
            ChangeTimeUpdate(timeUpdate);
        }
    }


}