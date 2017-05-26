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
          
            public static string OscilNominalFrequency { get; private set; }


            public static void ChangeOscilNominalFrequency(string nominFreq)
            {
                OscilNominalFrequency = nominFreq;
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
           // ConfigGlobal.ChangeLanguge("eng");
        //    ConfigModBus.InitPort(0, "Odd", "One", "COM1");
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
                 //   ConfigGlobal.ChangeLanguge(elSg.Attribute("Language") != null ? elSg.Attribute("Language").Value : "eng");
                 //   ConfigGlobal.ChangeScope(elSg.Attribute("DownloadScope") != null ? Convert.ToBoolean(elSg.Attribute("DownloadScope").Value) : false,
                        //elSg.Attribute("TypeScope") != null ? elSg.Attribute("TypeScope").Value : "comtrade",
                        //elSg.Attribute("ComtradeType") != null ? elSg.Attribute("ComtradeType").Value:"1999");
                 //   ConfigGlobal.ChangeAddrScope(elSg.Attribute("ConfigurationAddr") != null ? Convert.ToUInt16(elSg.Attribute("ConfigurationAddr").Value) : (ushort)512,
                        //elSg.Attribute("OscilCmndAddr") != null ? Convert.ToUInt16(elSg.Attribute("OscilCmndAddr").Value):(ushort)4092);
                  //  ConfigGlobal.ChangePathScope(elSg.Attribute("PathScope") != null ? elSg.Attribute("PathScope").Value : @"vmd - filestore\");
                    ConfigGlobal.ChangeOscilNominalFrequency(elSg.Attribute("OscilNominalFrequency") != null ? elSg.Attribute("OscilNominalFrequency").Value : "50");
                }
                var elSm = xElement.Element("Settings_ModBus");
                if (elSm != null)
                {
                //    ConfigModBus.InitPort(elSm.Attribute("BaudRate") != null ? Convert.ToInt32(elSm.Attribute("BaudRate").Value): 115200,
                //        elSm.Attribute("SerialPortParity") != null ? Convert.ToString(elSm.Attribute("SerialPortParity").Value):"Odd",
                //        elSm.Attribute("SerialPortStopBits") != null ? Convert.ToString(elSm.Attribute("SerialPortStopBits").Value):"One",
                //        elSm.Attribute("ComPortName") != null ? Convert.ToString(elSm.Attribute("ComPortName").Value):"COM1");
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
                 //           new XAttribute("Language", ConfigGlobal.Language),
                       //     new XAttribute("DownloadScope", Convert.ToString(ConfigGlobal.DownloadScope)),
                        //    new XAttribute("TypeScope", ConfigGlobal.TypeScope),
                        //    new XAttribute("ComtradeType", ConfigGlobal.ComtradeType == 3 ? "2013" : "1999"),
                         //   new XAttribute("ConfigurationAddr", Convert.ToString(ConfigGlobal.ConfigurationAddr)),
                         //   new XAttribute("OscilCmndAddr", Convert.ToString(ConfigGlobal.OscilCmndAddr)),
                        //    new XAttribute("PathScope", ConfigGlobal.PathScope),
                            new XAttribute("OscilNominalFrequency", ConfigGlobal.OscilNominalFrequency)),
                        //new XElement("Settings_ModBus",
                            //new XAttribute("BaudRate", Convert.ToString(ConfigModBus.BaudRate)),
                            //new XAttribute("SerialPortParity", Convert.ToString(ConfigModBus.SerialPortParity)),
                            //new XAttribute("SerialPortStopBits", Convert.ToString(ConfigModBus.SerialPortStopBits)),
                            //new XAttribute("ComPortName", Convert.ToString(ConfigModBus.ComPortName))),
                        new XElement("Settings_Server",
                            new XAttribute("PortServer", Convert.ToString(ConfigServer.PortServer)))));

            xDocument.Save(fs);
            fs.Close();
        }
    }
}