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
        private static Thread _myThread;

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

                Thread.Sleep(ServerConfig.TimeUpdate);
            }
        }

        public static void ConfigServer()
        {
            _iedModel = IedModel.CreateFromFile(ServerConfig.NameModelFile);
            _iedServer = new IedServer(_iedModel);


            UpdateDataObj.StaticUpdateData(_iedServer, _iedModel);
        }

        public static void StartServer()
        {
            if (_iedModel != null)
            {
                _iedServer.Start(ServerConfig.LocalIPAddr, ServerConfig.PortServer);

                _myThread = new Thread(RuningServer)
                {
                    Name = @"Thread Server"
                };
                _myThread.Start();

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
                _running = false;
                ModBus.ModBus.CloseModBus();
                _iedServer.Stop();
                _iedServer.Destroy();
                _myThread.Abort();

                Log.Log.Write(@"Server.StopServer: Server stoped", @"Stop");

            }
            else
            {
                Log.Log.Write(@"Server.StopServer: No valid Server found!", @"Error");
                Log.Log.Write(@"Server.StopServer: Server stoped", @"Stop");
            }
        }
    }
}
