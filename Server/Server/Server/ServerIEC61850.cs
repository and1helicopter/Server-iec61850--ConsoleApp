using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using IEC61850.Server;
using IEC61850.TLS;
using ServerLib.DataClasses;
using ServerLib.Update;

namespace ServerLib.Server
{
	public static partial class ServerIEC61850
	{
		private static IedServer _iedServer;
		private static IedModel _iedModel;
		private static Thread _cycleThread;

		public static bool ConfigServer()
		{
			try
			{
				//Создавать в дириктории с настройками
				var pathServer = $"{ServerConfig.NamePathDirectory}\\{ServerConfig.NameDirectoryServer}";
				Directory.CreateDirectory(pathServer);

				_iedModel = ConfigFileParser.CreateModelFromConfigFile(ServerConfig.NameModelFile);
					// IedModel.CreateFromFile(ServerConfig.NameModelFile);
				IedServerConfig config = new IedServerConfig
				{
					ReportBufferSize = 100000,
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

			UpdateServer.SetParams(_iedServer, _iedModel);			//Устаовка 
			UpdateServer.InitUpdate(_iedServer, _iedModel);			//Заполнение данными
			UpdateServer.InitHandlers(_iedServer, _iedModel);       //Установка оброботчиков событий
			UpdateServer.InitQualityAndTime();
			UpdateServer.InitMethodWork();
			return true;
		}

		public static bool StartServer(){
			if (_iedModel != null)
			{
				ModBus.ModBus.StartModBus();
				_iedServer.Start(ServerConfig.ServerPort);

				_cycleThread = new Thread(UpdateClass.CycleClass.Cycle)
				{
					Name = "Cycle",
					IsBackground = true
				};
				_cycleThread.Start();

				Thread.Sleep(1000);
				DownloadScope.DownloadScope.StartThreadCheack();

				Log.Log.Write(@"ServerIEC61850.StartServer: ServerIEC61850 started", @"Start");
				

				GC.Collect();
			}
			else
			{
				Log.Log.Write(@"ServerIEC61850.StartServer: No valid data model found!", @"Error");

				return false;
			}

			return true;
		}

		public static bool StopServer()
		{
			try
			{
				ModBus.ModBus.CloseModBus();

				if (_iedServer != null)
				{
					if (_iedServer.IsRunning())
					{
						_iedServer.Stop();
						_iedServer.Destroy();
						_iedServer = null;
					}
				}
				
				ServerModel.Model?.Clear();
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

		public static bool ParseFile(bool dependencesModel)
		{
			return Parser.Parser.ParseFile($"{ServerConfig.NamePathDirectory}\\{ServerConfig.NameConfigFile}", dependencesModel);
		}

		public static bool ReadConfig(string pathName)
		{
			return Settings.Settings.ReadSettings(pathName);
		}
	}
}
