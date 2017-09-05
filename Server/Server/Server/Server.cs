using System;
using System.Threading;
using IEC61850.Common;
using IEC61850.Server;
using Server.DataClasses;
using Server.Update;


namespace Server.Server
{
	public static partial class Server
	{
		private static readonly object Locker = new object();
		private static IedServer _iedServer;
		private static IedModel _iedModel;
		private static bool _running = true;
		private static Thread _serverThread;

		private static void RuningServer()
		{
			while (_running)
			{
				if (ModBus.ModBus.StartPort)
				{
					_iedServer.LockDataModel();

					lock (Locker)
					{
						UpdateDataObj.UpdateDataGet(_iedServer, _iedModel);
					}

					_iedServer.UnlockDataModel();
				}

				Thread.Sleep(ServerConfig.TimeUpdate);
			}
		}

		public static bool ConfigServer()
		{
			try
			{
				_iedModel = IedModel.CreateFromFile(ServerConfig.NameModelFile);

				_iedServer = new IedServer(_iedModel);
			}
			catch 
			{
				Log.Log.Write("Server: Model file incorrect","Error");
				return false;
			}

			GC.Collect();   //Габредж коллектор
			UpdateDataObj.StaticUpdateData(_iedServer, _iedModel);      //Заполнение данными
			UpdateDataObj.InitControlClass(_iedServer, _iedModel);			//Установка оброботчиков событий

			return true;
		}

		public static bool StartServer()
		{
			if (_iedModel != null)
			{

				_iedServer.Start(ServerConfig.PortServer);

				_serverThread = new Thread(RuningServer)
				{
					Name = @"Server IEC61850"
				};
				_serverThread.Start();
				_running = true;

				Log.Log.Write(@"Server.StartServer: Server started", @"Start");
			}
			else
			{
				Log.Log.Write(@"Server.StartServer: No valid data model found!", @"Error");
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
						_running = false;

						_iedServer.Stop();
						_iedServer.Destroy();
						_iedServer = null;
						_serverThread.Abort();
					}
				}
				
				ServerModel.Model?.Clear();
				UpdateDataObj.BitArray?.Clear();
				UpdateDataObj.ClassGetObjects?.Clear();
				DataObj.StructDataObj?.Clear();

				Log.Log.Write(@"Server.StopServer: Server stoped", @"Stop");
			}
			catch 
			{
				Log.Log.Write(@"Server.StopServer: No valid Server found!", @"Error");
				Log.Log.Write(@"Server.StopServer: Server stoped", @"Stop");
				return false;
			}
			return true;
		}
	}
}
