using System;
using System.IO;
using System.Xml.Linq;

namespace ServerLib.Settings
{
	public static class Settings
	{
		public static bool ReadSettings(string str)
		{
			//задаем путь к нашему рабочему файлу XML
			var filePath = @"Settings.xml";
			if (str != null)
			{
				filePath = str;
			}

			XDocument doc;
			try
			{
				doc = XDocument.Load(filePath);
				Server.ServerIEC61850.ServerConfig.NamePathDirectory = Path.GetDirectoryName(filePath);
			}
			catch
			{
				Log.Log.Write("Settings: File Settings no found!!!", "Error");
				return false;
			}

			//читаем данные из файла
			var xElement = doc.Element("Settings");
			if (xElement != null)
			{
				var elSg = xElement.Element("Settings_Global");
				if (elSg != null)
				{
					DownloadScope.ConfigDownloadScope.InitConfigDownloadScope(
					 elSg.Attribute("DownloadScope") != null ? Convert.ToString(elSg.Attribute("DownloadScope")?.Value) : "false",
					 elSg.Attribute("Remove") != null ? Convert.ToString(elSg.Attribute("Remove")?.Value) : "false",
					 elSg.Attribute("TypeScope") != null ? elSg.Attribute("TypeScope")?.Value : "comtrade",
					 elSg.Attribute("ComtradeType") != null ? elSg.Attribute("ComtradeType")?.Value : "1999",
					 elSg.Attribute("ConfigurationAddr") != null ? Convert.ToString(elSg.Attribute("ConfigurationAddr")?.Value) : "512",
					 elSg.Attribute("OscilCmndAddr") != null ? Convert.ToString(elSg.Attribute("OscilCmndAddr")?.Value) : "4092",
					 elSg.Attribute("PathScope") != null ? elSg.Attribute("PathScope")?.Value : "Scope",
					 elSg.Attribute("OscilNominalFrequency") != null ? elSg.Attribute("OscilNominalFrequency")?.Value : "50");
				}
				var elSm = xElement.Element("Settings_ModBus");
				if (elSm != null)
				{
					ModBus.ConfigModBus.InitConfigModBus(
					 elSm.Attribute("BaudRate") != null ? Convert.ToInt32(elSm.Attribute("BaudRate")?.Value) : 115200,
					 elSm.Attribute("SerialPortParity") != null ? Convert.ToString(elSm.Attribute("SerialPortParity")?.Value) : "Odd",
					 elSm.Attribute("SerialPortStopBits") != null ? Convert.ToString(elSm.Attribute("SerialPortStopBits")?.Value) : "One",
					 elSm.Attribute("ComPortName") != null ? Convert.ToString(elSm.Attribute("ComPortName")?.Value) : "COM1",
					 elSm.Attribute("AddrPort") != null ? Convert.ToByte(elSm.Attribute("AddrPort")?.Value) : (byte)1);
				}
				var elSs = xElement.Element("Settings_Server");
				if (elSs != null)
				{
					Server.ServerIEC61850.ServerConfig.ServerPort = elSs.Attribute("ServerPort") != null ? Convert.ToInt32(elSs.Attribute("ServerPort")?.Value) : 102;
					Server.ServerIEC61850.ServerConfig.LocalIpAddr = elSs.Attribute("LocalIpAddr") != null ? Convert.ToString(elSs.Attribute("LocalIpAddr")?.Value) : "localhost";
					Server.ServerIEC61850.ServerConfig.NameConfigFile = elSs.Attribute("NameConfigFile") != null ? Convert.ToString(elSs.Attribute("NameConfigFile")?.Value) : "model.icd";
					Server.ServerIEC61850.ServerConfig.NameModelFile = elSs.Attribute("NameModelFile") != null ? Convert.ToString(elSs.Attribute("NameModelFile")?.Value) : "model.cfg";
					Server.ServerIEC61850.ServerConfig.NameDirectoryServer = elSs.Attribute("NameDirectoryServer") != null ? Convert.ToString(elSs.Attribute("NameDirectoryServer")?.Value) : "vmd-filestore";
					Server.ServerIEC61850.ServerConfig.Autostart = elSs.Attribute("Autostart") != null && Convert.ToBoolean(elSs.Attribute("Autostart").Value);
					Server.ServerIEC61850.ServerConfig.AdditionalParams = elSs.Attribute("AdditionalParams") != null && Convert.ToBoolean(elSs.Attribute("AdditionalParams").Value);
				}
			}
			Log.Log.Write("ServerIEC61850.Settings: Settings file read success", "Success");
			return true;
		}

		public static void SaveSettings()
		{
			string savePath = "Settings.xml";

			FileStream fs = new FileStream(savePath, FileMode.Create);

			XDocument xDocument =
			 new XDocument(
			  new XElement("Settings",
			   new XElement("Settings_Global",
				new XAttribute("DownloadScope", Convert.ToString(DownloadScope.ConfigDownloadScope.Enable)),
				new XAttribute("Remove", Convert.ToString(DownloadScope.ConfigDownloadScope.Remove)),
				new XAttribute("TypeScope", DownloadScope.ConfigDownloadScope.Type),
				new XAttribute("ComtradeType", DownloadScope.ConfigDownloadScope.ComtradeType),
				new XAttribute("ConfigurationAddr", Convert.ToString(DownloadScope.ConfigDownloadScope.ConfigurationAddr)),
				new XAttribute("OscilCmndAddr", Convert.ToString(DownloadScope.ConfigDownloadScope.OscilCmndAddr)),
				new XAttribute("PathScope", DownloadScope.ConfigDownloadScope.PathScope),
				new XAttribute("OscilNominalFrequency", DownloadScope.ConfigDownloadScope.OscilNominalFrequency)),
			   new XElement("Settings_ModBus",
				new XAttribute("BaudRate", Convert.ToString(ModBus.ConfigModBus.BaudRate)),
				new XAttribute("SerialPortParity", Convert.ToString(ModBus.ConfigModBus.SerialPortParity)),
				new XAttribute("SerialPortStopBits", Convert.ToString(ModBus.ConfigModBus.SerialPortStopBits)),
				new XAttribute("ComPortName", Convert.ToString(ModBus.ConfigModBus.ComPortName)),
				new XAttribute("AddrPort", Convert.ToString(ModBus.ConfigModBus.AddrPort))),
			   new XElement("Settings_Server",
				new XAttribute("ServerPort", Convert.ToString(Server.ServerIEC61850.ServerConfig.ServerPort)),
				new XAttribute("LocalIpAddr", Convert.ToString(Server.ServerIEC61850.ServerConfig.LocalIpAddr)),
				new XAttribute("NameConfigFile", Convert.ToString(Server.ServerIEC61850.ServerConfig.NameConfigFile)),
				new XAttribute("NameModelFile", Convert.ToString(Server.ServerIEC61850.ServerConfig.NameModelFile)),
				new XAttribute("NameDirectoryServer", Convert.ToString(Server.ServerIEC61850.ServerConfig.NameDirectoryServer)),
				new XAttribute("Autostart", Convert.ToString(Server.ServerIEC61850.ServerConfig.Autostart)),
				new XAttribute("AdditionalParams", Convert.ToString(Server.ServerIEC61850.ServerConfig.AdditionalParams)))));
			
			xDocument.Save(fs);
			fs.Close();
		}
	}
}