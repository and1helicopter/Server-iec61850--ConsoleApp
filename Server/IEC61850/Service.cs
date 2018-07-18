using System.Diagnostics;
using System.ServiceProcess;
using ServerLib.Parser;

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
				//Считает что находится в директории C:\WINDOWS\system32
				var str = "F:\\project\\ServerIEC61850-iec61850--ConsoleApp\\ServerIEC61850\\IEC61850\\bin\\x86\\Debug\\Settings.xml";
	            if (!ServerLib.Server.ServerIEC61850.ReadConfig(str))
	            {
					eventLog.WriteEntry("Read Settings Error", EventLogEntryType.Error);
		            return;
				}

				str = "F:\\project\\ServerIEC61850-iec61850--ConsoleApp\\ServerIEC61850\\IEC61850\\bin\\x86\\Debug\\ESSrv.icd";
				if (!ServerLib.Server.ServerIEC61850.ParseFile(str))
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
