using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace Server.Settings
{
    public static class Settings
    {
        /* Глобальные настройки проекта  */
        public static class ConfigGlobal
        {
            public static string Language { get;  private set; }
            public static bool DownloadScope { get; private set; }
            public static string TypeScope { get; private set; }
            public static int ComtradeType { get; private set; }
            public static string PathScope { get; private set; }
            public static ushort ConfigurationAddr { get; private set; }
            public static ushort OscilCmndAddr { get; private set; }
            public static string OscilNominalFrequency { get; private set; }

            public static void ChangeLanguge(string lang)
            {
                Language = lang == "rus" ? "rus" : "eng";
            }

            public static void ChangeScope(bool download, string type, string comtrade)
            {
                DownloadScope = download;
                TypeScope = type == "txt" ? "txt" : "comtrade";
                ComtradeType = comtrade == "2013" ? 3 : 2;
            }

            public static void ChangeAddrScope(ushort configAddr, ushort cmndAddr)
            {
                ConfigurationAddr = configAddr;
                OscilCmndAddr = cmndAddr;
            }

            public static void ChangePathScope(string path)
            {
                //"vmd-filestore" + путь до папки где лежат осциллограммы
                PathScope = path;
            }

            public static void ChangeOscilNominalFrequency(string nominFreq)
            {
                OscilNominalFrequency = nominFreq;
            }
        }

        /* Настройки ModBus port  */

        public static class ConfigModBus
        {
            public static int BaudRate { get; private set; }

            private static void ChangeBaudRate(byte serialPortSpeedIndex)
            {
                switch (serialPortSpeedIndex)
                {
                    case 0:
                    {
                        BaudRate = 9600;
                        return;
                    }
                    case 1:
                    {
                        BaudRate = 19200;
                        return;
                    }
                    case 2:
                    {
                        BaudRate = 38400;
                        return;
                    }
                    case 3:
                    {
                        BaudRate = 57600;
                        return;
                    }
                    case 4:
                    {
                        BaudRate = 115200;
                        return;
                    }
                    case 5:
                    {
                        BaudRate = 230400;
                        return;
                    }
                }
                BaudRate = 9600;
            }

            public static Parity SerialPortParity { get; private set; }

            private static void ChangeSerialPortParity(byte serialPortParityIndex)
            {
                switch (serialPortParityIndex)
                {
                    case 0:
                    {
                        SerialPortParity = Parity.Odd;
                        return;
                    }
                    case 1:
                    {
                        SerialPortParity = Parity.Even;
                        return;
                    }
                    case 2:
                    {
                        SerialPortParity = Parity.None;
                        return;
                    }
                }
                SerialPortParity = Parity.Odd;
            }

            public static StopBits SerialPortStopBits { get; private set; }

            private static void ChangeSerialPortStopBits(byte serialPortParityIndex)
            {
                switch (serialPortParityIndex)
                {
                    case 0:
                    {
                        SerialPortStopBits = StopBits.One;
                        return;
                    }
                    case 1:
                    {
                        SerialPortStopBits = StopBits.One;
                        return;
                    }
                    case 2:
                    {
                        SerialPortStopBits = StopBits.Two;
                        return;
                    }
                }
                SerialPortStopBits = StopBits.One;
            }

            public static string ComPortName { get; private set; }

            private static void ChangeComPortName(byte comPortIndex)
            {
                ReadPortList();

                if (_portList.Count != 0)
                {
                    if (comPortIndex >= _portList.Count)
                    {
                        ComPortName = _portList[0];
                    }
                    ComPortName = _portList[comPortIndex];
                }
                else
                {
                    ComPortName = "";
                }

            }

            //Список всех COM-портов на компьютере
            private static List<string> _portList = new List<string>();

            private static void ReadPortList()
            {
                string[] portStrList = SerialPort.GetPortNames();
                _portList.Clear();
                foreach (string port in portStrList)
                {
                    _portList.Add(port);
                }
                _portList.Sort();
            }

            public static void ShowPortList()
            {
                ReadPortList();

                foreach (var item in _portList)
                {
                    Console.WriteLine(ConfigGlobal.Language == "rus"
                        ? $"COM порт: {item} - Индекс: {_portList.IndexOf(item)}"
                        : $"COM port: {item} - Index: {_portList.IndexOf(item)}");
                }
            }

            public static void ShowPortSettings()
            {
                Console.WriteLine(ConfigGlobal.Language == "rus"
                        ? $"Скорость:{BaudRate} - Паритет:{SerialPortParity} - Стоп бит:{SerialPortStopBits} - Название COM порта:{ComPortName}"
                        : $"BaudRate:{BaudRate} - Parity:{SerialPortParity} - StopBits:{SerialPortStopBits} - ComPortName:{ComPortName}");
            }

            public static void InitPort(byte serialPortSpeedIndex, byte serialPortParityIndex, byte comPortIndex)
            {
                ChangeBaudRate(serialPortSpeedIndex);
                ChangeSerialPortParity(serialPortParityIndex);
                ChangeSerialPortStopBits(serialPortParityIndex);
                ChangeComPortName(comPortIndex);
            }
        }

        /* Настройки начальных параметров */
        public static void InitStartSettings()
        {
            ConfigGlobal.ChangeLanguge("eng");
            ConfigModBus.InitPort(0,0,0);
        }

    }
}