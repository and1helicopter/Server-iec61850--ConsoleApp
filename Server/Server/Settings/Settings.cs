using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Xml;

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

            public static Parity SerialPortParity { get; private set; }

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

            public static StopBits SerialPortStopBits { get; private set; }

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

            public static string ComPortName { get; private set; }

            private static void ChangeComPortName(string comPort)
            {
                ReadPortList();

                if (_portList.Count != 0)
                {
                    ComPortName = _portList.Contains(comPort) ? comPort : _portList[0];
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

            public static void InitPort(int serialPortSpeedIndex, string serialPortParity, string serialPortStopBits, string comPort)
            {
                ChangeBaudRate(serialPortSpeedIndex);
                ChangeSerialPortParity(serialPortParity);
                ChangeSerialPortStopBits(serialPortStopBits);
                ChangeComPortName(comPort);
            }
        }

        /* Настройки Server  */

        public static class ConfigServer
        {
            public static int PortServer { get; private set; }

            public static void ChangePortServer(int portServer)
            {
                PortServer = portServer;
            }

        }


        /* Настройки начальных параметров */
        public static void InitStartSettings()
        {
            ConfigGlobal.ChangeLanguge("eng");
            ConfigModBus.InitPort(0, "Odd", "One", "COM1");
        }



        public static void ReadSettings()
        {
            
        }

        public static void SaveSettings()
        {
            string savePath = "Settings.xml";

            FileStream fs = new FileStream(savePath, FileMode.Create);
            XmlTextWriter xmlOut = new XmlTextWriter(fs, Encoding.Unicode)
            {
                Formatting = Formatting.Indented
            };

            /*Global Settings*/
            xmlOut.WriteStartDocument();
            xmlOut.WriteStartElement("Settings");
            /////////////////////////////////////////////////////////////

            xmlOut.WriteStartElement("Settings Global");

            xmlOut.WriteStartElement("Language", ConfigGlobal.Language);
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("DownloadScope", Convert.ToString(ConfigGlobal.DownloadScope));
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("TypeScope", ConfigGlobal.TypeScope);
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("ComtradeType", ConfigGlobal.ComtradeType == 3 ? "2013" : "1999");
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("ConfigurationAddr", Convert.ToString(ConfigGlobal.ConfigurationAddr));
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("OscilCmndAddr", Convert.ToString(ConfigGlobal.OscilCmndAddr));
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("PathScope", ConfigGlobal.PathScope);
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("OscilNominalFrequency", ConfigGlobal.OscilNominalFrequency);
            xmlOut.WriteEndElement();

            xmlOut.WriteEndElement();
            /*ModBus Settings*/
            xmlOut.WriteStartElement("Settings ModBus");

            xmlOut.WriteStartElement("BaudRate", Convert.ToString(ConfigModBus.BaudRate));
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("SerialPortParity", Convert.ToString(ConfigModBus.SerialPortParity));
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("SerialPortStopBits", Convert.ToString(ConfigModBus.SerialPortStopBits));
            xmlOut.WriteEndElement();
            
            xmlOut.WriteStartElement("ComPortName", Convert.ToString(ConfigModBus.ComPortName));
            xmlOut.WriteEndElement();
            
            xmlOut.WriteEndElement();
            /*Server Settings*/
            xmlOut.WriteStartElement("Settings Server");

            xmlOut.WriteStartElement("PortServer", Convert.ToString(ConfigServer.PortServer));
            xmlOut.WriteEndElement();

            xmlOut.WriteEndElement();

            xmlOut.WriteEndElement();
            xmlOut.WriteEndDocument();
            xmlOut.Close();
            fs.Close();
        }
    }
}