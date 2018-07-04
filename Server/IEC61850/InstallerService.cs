using System.ComponentModel;
using System.ServiceProcess;

namespace ServiceIec61850
{
	[RunInstaller(true)]
	public partial class InstallerService : System.Configuration.Install.Installer
	{
		ServiceInstaller serviceInstaller;
		ServiceProcessInstaller processInstaller;

		public InstallerService()
		{
			InitializeComponent();

			serviceInstaller = new ServiceInstaller
			{
				StartType = ServiceStartMode.Manual,
				ServiceName = "ServiceIEC61850",
				DisplayName = "Служба сервера IEC61850",
				Description = "Сервер IEC 61850"
			};

			processInstaller = new ServiceProcessInstaller
			{
				Account = ServiceAccount.LocalSystem
			};

			Installers.Add(processInstaller);
			Installers.Add(serviceInstaller);
		}
	}
}
