using System.IO;
using IEC61850.Common;
using IEC61850.Server;
using ServerLib.DataClasses;
using ServerLib.ModBus;
using ServerLib.Update;


namespace ServerLib.Server
{
	public static partial class ServerIEC61850
	{
		private static IedServer _iedServer;
		private static IedModel _iedModel;

		public static bool ConfigServer()
		{
			try
			{
				Directory.CreateDirectory(@"vmd-filestore" + "\\");

				_iedModel = IedModel.CreateFromFile(ServerConfig.NameModelFile);

				_iedServer = new IedServer(_iedModel);
			}
			catch 
			{
				Log.Log.Write("ServerIEC61850: Model file incorrect", "Error");
				return false;
			}

			UpdateServer.SetParams(_iedServer, _iedModel);			//Устаовка 
			UpdateServer.InitUpdate(_iedServer, _iedModel);			//Заполнение данными
			UpdateServer.InitHandlers(_iedServer, _iedModel);		//Установка оброботчиков событий

			UpdateModBus.ConfigModBusPort();

			return true;
		}

		public static bool StartServer(){
			if (_iedModel != null)
			{
				_iedServer.Start(ServerConfig.ServerPort);

				Log.Log.Write(@"ServerIEC61850.StartServer: ServerIEC61850 started", @"Start");
				
				UpdateModBus.StartModBus();
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
				UpdateModBus.CloseModBus();

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

		public static bool ParseFile(string pathName)
		{
			return Parser.Parser.ParseFile(pathName);
		}

		public static bool ReadConfig(string pathName)
		{
			return Settings.Settings.ReadSettings(pathName);
		}
	}
}
