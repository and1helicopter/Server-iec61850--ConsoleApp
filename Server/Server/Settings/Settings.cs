using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Linq;

namespace Server.Settings
{
    public static class Settings
    {     
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
                    ModBus.ConfigDownloadScope.InitConfigDownloadScope(
                        elSg.Attribute("DownloadScope") != null ? Convert.ToString(elSg.Attribute("DownloadScope").Value) : "false",
                        elSg.Attribute("Remove") != null ? Convert.ToString(elSg.Attribute("Remove").Value) : "false",
                        elSg.Attribute("TypeScope") != null ? elSg.Attribute("TypeScope").Value : "comtrade",
                        elSg.Attribute("ComtradeType") != null ? elSg.Attribute("ComtradeType").Value : "1999",
                        elSg.Attribute("ConfigurationAddr") != null ? Convert.ToString(elSg.Attribute("ConfigurationAddr").Value) : "512",
                        elSg.Attribute("OscilCmndAddr") != null ? Convert.ToString(elSg.Attribute("OscilCmndAddr").Value) : "4092",
                        elSg.Attribute("PathScope") != null ? elSg.Attribute("PathScope").Value : @"Scope\",
                        elSg.Attribute("OscilNominalFrequency") != null ? elSg.Attribute("OscilNominalFrequency").Value : "50");
                }
                var elSm = xElement.Element("Settings_ModBus");
                if (elSm != null)
                {
                    ModBus.ConfigModBus.InitConfigModBus(
                        elSm.Attribute("BaudRate") != null ? Convert.ToInt32(elSm.Attribute("BaudRate").Value) : 115200,
                        elSm.Attribute("SerialPortParity") != null ? Convert.ToString(elSm.Attribute("SerialPortParity").Value) : "Odd",
                        elSm.Attribute("SerialPortStopBits") != null ? Convert.ToString(elSm.Attribute("SerialPortStopBits").Value) : "One",
                        elSm.Attribute("ComPortName") != null ? Convert.ToString(elSm.Attribute("ComPortName").Value) : "COM1");
                }
                var elSs = xElement.Element("Settings_Server");
                if (elSs != null)
                {
                    Server.Server.ServerConfig.PortServer = elSs.Attribute("PortServer") != null ? Convert.ToInt32(elSs.Attribute("PortServer").Value): 102;
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
                            new XAttribute("DownloadScope", Convert.ToString(ModBus.ConfigDownloadScope.Enable)),
                            new XAttribute("Remove", Convert.ToString(ModBus.ConfigDownloadScope.Remove)),
                            new XAttribute("TypeScope", ModBus.ConfigDownloadScope.Type),
                            new XAttribute("ComtradeType", ModBus.ConfigDownloadScope.ComtradeType),
                            new XAttribute("ConfigurationAddr", Convert.ToString(ModBus.ConfigDownloadScope.ConfigurationAddr)),
                            new XAttribute("OscilCmndAddr", Convert.ToString(ModBus.ConfigDownloadScope.OscilCmndAddr)),
                            new XAttribute("PathScope", ModBus.ConfigDownloadScope.PathScope),
                            new XAttribute("OscilNominalFrequency", ModBus.ConfigDownloadScope.OscilNominalFrequency)),
                        new XElement("Settings_ModBus",
                            new XAttribute("BaudRate", Convert.ToString(ModBus.ConfigModBus.BaudRate)),
                            new XAttribute("SerialPortParity", Convert.ToString(ModBus.ConfigModBus.SerialPortParity)),
                            new XAttribute("SerialPortStopBits", Convert.ToString(ModBus.ConfigModBus.SerialPortStopBits)),
                            new XAttribute("ComPortName", Convert.ToString(ModBus.ConfigModBus.ComPortName))),
                        new XElement("Settings_Server",
                            new XAttribute("PortServer", Convert.ToString(Server.Server.ServerConfig.PortServer)))));

            xDocument.Save(fs);
            fs.Close();
        }
    }
}