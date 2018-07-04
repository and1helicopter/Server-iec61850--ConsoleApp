using System.Diagnostics;
using System.ServiceProcess;
using Server.Parser;

namespace ServiceIec61850
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
            if (!EventLog.SourceExists("IEC61850 Server"))
            {
                EventLog.CreateEventSource("IEC61850 Server", "Log IEC61850 Server");
            }

            eventLog = new EventLog
            {
                Source = "IEC61850 Server",
                Log = "Log IEC61850 Server"
            };
        }

        protected override void OnStart(string[] args)
        {
            try
            {
				//Считает что находится в директории C:\WINDOWS\system32
				var str = "F:\\project\\Server-iec61850--ConsoleApp\\Server\\IEC61850\\bin\\x86\\Debug\\Settings.xml";
	            if (!Server.Settings.Settings.ReadSettings(str))
	            {
					eventLog.WriteEntry("Read Settings Error", EventLogEntryType.Error);
		            return;
				}

				str = "F:\\project\\Server-iec61850--ConsoleApp\\Server\\IEC61850\\bin\\x86\\Debug\\ESSrv.icd";
				if (!Parser.ParseFile(str))
                {
                    eventLog.WriteEntry("Parse File Error", EventLogEntryType.Error);
                    return;
                }

				if (!Server.Server.Server.ConfigServer())
	            {
		            eventLog.WriteEntry("Config Server Error", EventLogEntryType.Error);
		            return;
				}

	            if (!Server.Server.Server.StartServer())
	            {
		            eventLog.WriteEntry("Start Server Error", EventLogEntryType.Error);
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
	            if (!Server.Server.Server.StopServer())
	            {
		            eventLog.WriteEntry("Stop Server Error", EventLogEntryType.Error);
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
