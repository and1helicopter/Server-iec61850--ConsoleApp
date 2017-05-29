using System.Collections.Generic;
using System.IO.Ports;
using UniSerialPort;

namespace Server.ModBus
{
    public static partial class ModBus
    {
        private static void ConfigModBusPort()
        {
            if (SerialPort.IsOpen)
            {
                Logging.Log.Write("ModBus port is open! Close ModBus SerialPort and repeat.", "Error ");
                return;
            }

            SerialPort.SerialPortMode = SerialPortModes.RSMode;
            SerialPort.BaudRate = _modbus.BaudRate;
            SerialPort.Parity = _modbus.SerialPortParity;
            SerialPort.StopBits = _modbus.SerialPortStopBits;
            SerialPort.PortName = _modbus.ComPortName;
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
                    StartModBusPort();
                }
            }
            catch
            {
                Logging.Log.Write("ModBus port not open!", "Error ");
            }
        }

        private static void SerialPort_SerialPortError(object sender, System.EventArgs e)
        {
            CloseModBusPort();
            OpenModBusPort();
        }

        private static void CloseModBusPort()
        {
            //DownloadScopeTimer.Enabled = false;
            DownloadTimer.Enabled = false;
            StartPort = false;
            ErrorPort = true;
            lock (Locker)
            {
                SerialPort.Close();
            }
        }
    }

    /* Настройки ModBus port  */

    public class ConfigModBus
    {
        public int BaudRate { get; private set; }
        public Parity SerialPortParity { get; private set; }
        public StopBits SerialPortStopBits { get; private set; }
        public string ComPortName { get; private set; }


        private void ChangeBaudRate(int baudRate)
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
        
        private void ChangeSerialPortParity(string serialPortParity)
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


        private void ChangeSerialPortStopBits(string serialPortStopBits)
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


        private void ChangeComPortName(string comPort)
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

        public ConfigModBus(int serialPortSpeedIndex, string serialPortParity, string serialPortStopBits, string comPort)
        {
            ChangeBaudRate(serialPortSpeedIndex);
            ChangeSerialPortParity(serialPortParity);
            ChangeSerialPortStopBits(serialPortStopBits);
            ChangeComPortName(comPort);
        }
    }


}