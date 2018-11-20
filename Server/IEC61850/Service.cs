using System;
using System.Diagnostics;
using System.ServiceProcess;
using ServerLib.Log;

namespace ServiceIec61850
{
	public partial class Service : ServiceBase
	{
		public Service()
		{
			InitializeComponent();
			if (!EventLog.SourceExists("IEC61850 ServerIEC61850"))
			{
				EventLog.CreateEventSource("IEC61850 ServerIEC61850", "Log IEC61850 ServerIEC61850");
			}

			eventLog = new EventLog
			{
				Source = "IEC61850 ServerIEC61850",
				Log = "Log IEC61850 ServerIEC61850"
			};
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				//File directory
				string pathRootLocation;

				try
				{
					pathRootLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
					eventLog.WriteEntry($"Getting current directory finished with status succses:\n {pathRootLocation}", EventLogEntryType.SuccessAudit);
				}
				catch
				{
					pathRootLocation = "C:\\Server iec61850\\";
					eventLog.WriteEntry("Getting work directory finished with status error", EventLogEntryType.Error);
				}

				var rootName = pathRootLocation;

				//Установка пути для лог файла
				Log.SetRootPath(rootName);

				//Settings file
				var pathSetting = rootName + "Settings.xml";
				if (!ServerLib.Server.ServerIEC61850.ReadConfig(pathSetting))
				{
					eventLog.WriteEntry("Read Settings Error", EventLogEntryType.Error);
				}
				
				//dependences model true
				if (!ServerLib.Server.ServerIEC61850.ParseFile())
				{
					eventLog.WriteEntry("Parse File Error", EventLogEntryType.Error);
					return;
				}

				if (!ServerLib.Server.ServerIEC61850.ConfigServer())
				{
					eventLog.WriteEntry("Config ServerIEC61850 Error", EventLogEntryType.Error);
					return;
				}

				if (!ServerLib.Server.ServerIEC61850.StartServer())
				{
					eventLog.WriteEntry("Start ServerIEC61850 Error", EventLogEntryType.Error);
					return;
				}

				eventLog.WriteEntry("Start Service IEC61850", EventLogEntryType.SuccessAudit);
			}
			catch
			{
				eventLog.WriteEntry("Start Service IEC61850", EventLogEntryType.Error);
			}
		}

		protected override void OnStop()
		{
			try
			{
				if (!ServerLib.Server.ServerIEC61850.StopServer())
				{
					eventLog.WriteEntry("Stop ServerIEC61850 Error", EventLogEntryType.Error);
					return;
				}

				eventLog.WriteEntry("Stop Service IEC61850", EventLogEntryType.SuccessAudit);
			}
			catch
			{
				eventLog.WriteEntry("Stop Service IEC61850", EventLogEntryType.Error);
			}
		}
	}
}
