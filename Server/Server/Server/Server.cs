using System.IO;
using IEC61850.Common;
using IEC61850.Server;
using Server.DataClasses;
using Server.Update;


namespace Server.Server
{
	public static partial class Server
	{
		private static IedServer _iedServer;
		private static IedModel _iedModel;

		public static bool ConfigServer()
		{
			try
			{
				Directory.CreateDirectory(@"vmd-filestore" + "\\" + ModBus.ConfigDownloadScope.PathScope);

				_iedModel = IedModel.CreateFromFile(ServerConfig.NameModelFile);

				_iedServer = new IedServer(_iedModel);
			}
			catch 
			{
				Log.Log.Write("Server: Model file incorrect", "Error");
				return false;
			}

			StaticUpdateData(_iedServer, _iedModel);			//Заполнение данными
			InitControlClass(_iedServer, _iedModel);			//Установка оброботчиков событий

			return true;
		}

		public static bool StartServer()
		{
			if (_iedModel != null)
			{
				_iedServer.Start(ServerConfig.PortServer);

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
						_iedServer.Stop();
						_iedServer.Destroy();
						_iedServer = null;
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
