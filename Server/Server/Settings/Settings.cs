using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Ports;
using System.Xml.Linq;

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

            public static void ShowPortList()
            {
                ReadPortList();

                foreach (var item in PortList)
                {
                    Console.WriteLine(ConfigGlobal.Language == "rus"
                        ? $"COM порт: {item} - Индекс: {PortList.IndexOf(item)}"
                        : $"COM port: {item} - Index: {PortList.IndexOf(item)}");
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



        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void ReadSettings()
        {
            //задаем путь к нашему рабочему файлу XML
            string filePath = @"Settings.xml";
            //читаем данные из файла
            XDocument doc = XDocument.Load(filePath);

            var xElement = doc.Element("Settings");
            if (xElement != null)
            {
                var elSg = xElement.Element("Settings_Global");
                if (elSg != null)
                {
                    ConfigGlobal.ChangeLanguge(elSg.Attribute("Language") != null ? elSg.Attribute("Language").Value : "eng");
                    ConfigGlobal.ChangeScope(elSg.Attribute("DownloadScope") != null ? Convert.ToBoolean(elSg.Attribute("DownloadScope").Value) : false,
                        elSg.Attribute("TypeScope") != null ? elSg.Attribute("TypeScope").Value : "comtrade",
                        elSg.Attribute("ComtradeType") != null ? elSg.Attribute("ComtradeType").Value:"1999");
                    ConfigGlobal.ChangeAddrScope(elSg.Attribute("ConfigurationAddr") != null ? Convert.ToUInt16(elSg.Attribute("ConfigurationAddr").Value) : (ushort)512,
                        elSg.Attribute("OscilCmndAddr") != null ? Convert.ToUInt16(elSg.Attribute("OscilCmndAddr").Value):(ushort)4092);
                    ConfigGlobal.ChangePathScope(elSg.Attribute("PathScope") != null ? elSg.Attribute("PathScope").Value : @"vmd - filestore\");
                    ConfigGlobal.ChangeOscilNominalFrequency(elSg.Attribute("OscilNominalFrequency") != null ? elSg.Attribute("OscilNominalFrequency").Value : "50");
                }
                var elSm = xElement.Element("Settings_ModBus");
                if (elSm != null)
                {
                    ConfigModBus.InitPort(elSm.Attribute("BaudRate") != null ? Convert.ToInt32(elSm.Attribute("BaudRate").Value): 115200,
                        elSm.Attribute("SerialPortParity") != null ? Convert.ToString(elSm.Attribute("SerialPortParity").Value):"Odd",
                        elSm.Attribute("SerialPortStopBits") != null ? Convert.ToString(elSm.Attribute("SerialPortStopBits").Value):"One",
                        elSm.Attribute("ComPortName") != null ? Convert.ToString(elSm.Attribute("ComPortName").Value):"COM1");
                }
                var elSs = xElement.Element("Settings_Server");
                if (elSs != null)
                {
                    ConfigServer.ChangePortServer(elSs.Attribute("PortServer") != null ? Convert.ToInt32(elSs.Attribute("PortServer").Value): 102);
                }
            }
        }

        public static void SaveSettings()
        {
            string savePath = "Settings.xml";

            FileStream fs = new FileStream(savePath, FileMode.Create);
            
            XDocument xDocument =
                new XDocument(
                    new XElement("Settings",
                        new XElement("Settings_Global",
                            new XAttribute("Language", ConfigGlobal.Language),
                            new XAttribute("DownloadScope", Convert.ToString(ConfigGlobal.DownloadScope)),
                            new XAttribute("TypeScope", ConfigGlobal.TypeScope),
                            new XAttribute("ComtradeType", ConfigGlobal.ComtradeType == 3 ? "2013" : "1999"),
                            new XAttribute("ConfigurationAddr", Convert.ToString(ConfigGlobal.ConfigurationAddr)),
                            new XAttribute("OscilCmndAddr", Convert.ToString(ConfigGlobal.OscilCmndAddr)),
                            new XAttribute("PathScope", ConfigGlobal.PathScope),
                            new XAttribute("OscilNominalFrequency", ConfigGlobal.OscilNominalFrequency)),
                        new XElement("Settings_ModBus",
                            new XAttribute("BaudRate", Convert.ToString(ConfigModBus.BaudRate)),
                            new XAttribute("SerialPortParity", Convert.ToString(ConfigModBus.SerialPortParity)),
                            new XAttribute("SerialPortStopBits", Convert.ToString(ConfigModBus.SerialPortStopBits)),
                            new XAttribute("ComPortName", Convert.ToString(ConfigModBus.ComPortName))),
                        new XElement("Settings_Server",
                            new XAttribute("PortServer", Convert.ToString(ConfigServer.PortServer)))));

            xDocument.Save(fs);
            fs.Close();
        }
    }
}