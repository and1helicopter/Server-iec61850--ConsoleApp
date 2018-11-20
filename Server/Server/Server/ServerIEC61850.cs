using System.IO;
using System.Threading;
using IEC61850.Server;
using ServerLib.DataClasses;

namespace ServerLib.Server
{
	public static partial class ServerIEC61850
	{
		private static IedServer _iedServer;
		private static IedModel _iedModel;
		private static Thread _cycleThread;

		/// <summary>
		/// Configuration Server IEC61850
		/// </summary>
		/// <returns>Return result configuration</returns>
		public static bool ConfigServer()
		{
			try
			{
				//Создавать в дириктории с настройками
				var pathServer = $"{ServerConfig.NamePathDirectory}\\{ServerConfig.NameDirectoryServer}";
				Directory.CreateDirectory(pathServer);

				_iedModel = ConfigFileParser.CreateModelFromConfigFile(ServerConfig.NameModelFile);
				
				IedServerConfig config = new IedServerConfig
				{
					ReportBufferSize = 10000,
					FileServiceBasePath = pathServer
				};
				_iedServer = new IedServer(_iedModel, config);
				_iedServer.SetLocalIpAddress(ServerConfig.LocalIpAddr);
			}
			catch 
			{
				Log.Log.Write("ServerIEC61850: Model file incorrect", "Error");
				return false;
			}

			UpdateServer.SetParams(_iedServer, _iedModel);          //Устаовка 
			UpdateServer.InitUpdate(_iedServer, _iedModel);         //Заполнение данными
			UpdateServer.InitHandlers(_iedServer, _iedModel);       //Установка оброботчиков событий
			UpdateServer.InitQualityAndTime();
			UpdateServer.InitMethodWork();
			DownloadScope.DownloadScope.InitMethodWork();
			return true;
		}

		/// <summary>
		/// Start setver IEC 61850
		/// </summary>
		/// <returns>Return result start</returns>
		public static bool StartServer(){
			if (_iedModel != null)
			{
				ModBus.ModBus.StartModBus();
				_iedServer.Start(ServerConfig.ServerPort);

				if (_iedServer.IsRunning())
				{
					_cycleThread = new Thread(ModBusTaskController.ModBusTaskController.CycleClass.Cycle)
					{
						Name = "Cycle",
						IsBackground = true
					};
					_cycleThread.Start();

					Log.Log.Write(@"ServerIEC61850.StartServer: ServerIEC61850 started", @"Start");
				}
				else
				{
					return false;
				}
			}
			else
			{
				Log.Log.Write(@"ServerIEC61850.StartServer: No valid data model found!", @"Error");

				return false;
			}

			return true;
		}

		/// <summary>
		/// Stop server IEC61850
		/// </summary>
		/// <returns>Return result stop</returns>
		public static bool StopServer()
		{
			try
			{
				ModBusTaskController.ModBusTaskController.CycleClass.RemoveAllMethodWork();
				ModBus.ModBus.CloseModBus();

				if (_iedServer != null)
				{
					if (_iedServer.IsRunning())
					{
						_cycleThread.Abort();
						_cycleThread = null;

						_iedServer.Stop();
						_iedServer.Destroy();
						_iedServer = null;
						_iedModel.Destroy();
						_iedModel = null;
					}
				}

				ServerModel.Model?.Clear();
				DownloadScope.DownloadScope.StopDownloadScope();
				UpdateServer.Clear();

				Log.Log.Write(@"ServerIEC61850.StopServer: ServerIEC61850 stoped", @"Stop");
			}
			catch 
			{
				Log.Log.Write(@"ServerIEC61850.StopServer: No valid ServerIEC61850 found!", @"Error");
				Log.Log.Write(@"ServerIEC61850.StopServer: ServerIEC61850 stoped", @"Stop");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Parse file model to sever model
		/// </summary>
		/// <param name="dependencesModel">Dependences Model</param>
		/// <returns>Return result parse</returns>
		public static bool ParseFile()
		{
			return Parser.Parser.ParseFile($"{ServerConfig.NamePathDirectory}\\{ServerConfig.NameConfigFile}");
		}

		/// <summary>
		/// Read Config file
		/// </summary>
		/// <param name="pathName">Path configuration file</param>
		/// <returns>Return result read</returns>
		public static bool ReadConfig(string pathName)
		{
			return Settings.Settings.ReadSettings(pathName);
		}
	}
}
