using System.ServiceProcess;

namespace ServiceIec61850
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
	        var servicesToRun = new ServiceBase[]
	        {
		        new Service()
	        };
	        ServiceBase.Run(servicesToRun);
        }
    }
}
