using System;
using System.Threading;
using IEC61850.Server;
using Server.Update;

namespace Server.Server
{
    public static partial class Server
    {
        private static readonly object Locker = new object();
        private static IedServer _iedServer;
        private static IedModel _iedModel;
        private static bool _running = true;

        private static void RuningServer()
        {
            while (_running)
            {
                if (ModBus.ModBus.StartPort)
                {
                    _iedServer.LockDataModel();

                    lock (Locker)
                    {
                        UpdateDataObj.UpdateData(_iedServer, _iedModel);
                    }

                    _iedServer.UnlockDataModel();
                }

                Thread.Sleep(50);
            }
        }

        public static void ConfigServer(string nameConfigFile)
        {
            _iedModel = IedModel.CreateFromFile(nameConfigFile);
            _iedServer = new IedServer(_iedModel);

            UpdateDataObj.StaticUpdateData(_iedServer, _iedModel);
        }

        public static void StartServer()
        {
            if (_iedModel != null)
            {
                _iedServer.Start(ServerConfig.PortServer);

                Thread myThread = new Thread(RuningServer)
                {
                    Name = @"Thread Server"
                };
                myThread.Start();

                Log.Log.Write(@"Server.StartServer: Server started", @"Start   ");
            }
            else
            {
                Log.Log.Write(@"Server.StartServer: No valid data model found!", @"Error   ");
            }
        }

        public static void StopServer()
        {
            if (_iedServer.IsRunning())
            {
                ModBus.ModBus.CloseModBus();
                _iedServer.Stop();
                _iedServer.Destroy();

                Log.Log.Write(@"Server.StopServer: Server stoped", @"Stop   ");
            }
            else
            {
                Log.Log.Write(@"Server.StopServer: No valid Server found!", @"Error  ");
            }

            Console.ReadKey();
        }
    }
}
