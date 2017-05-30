using System;
using System.Threading;
using IEC61850.Server;
using Server.Update;

namespace Server.Server
{
    public static class Server
    {
        private static readonly object Locker = new object();
        private static IedServer _iedServer;
        private static IedModel _iedModel;
        static bool _running = true;

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

        public static void StartServer(int portNum)
        {
            /* run until Ctrl-C is pressed */
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                _running = false;

                StopServer();
            };

            if (_iedModel != null)
            {
                _iedServer.Start(portNum);

                Thread myThread = new Thread(RuningServer)
                {
                    Name = @"Thread Server"
                };
                myThread.Start();

                Console.WriteLine(@"Server started");
                Log.Log.Write(@"Server: Server started", @"Start   ");
            }
            else
            {
                Console.WriteLine(@"No valid data model found!");
            }
        }

        public static void StopServer()
        {
            if (_iedServer.IsRunning())
            {
                _iedServer.Stop();
                _iedServer.Destroy();

                Console.WriteLine(@"Server stoped");
                Log.Log.Write(@"Server: Server stoped", @"Stop    ");
            }
            else
            {
                Console.WriteLine(@"No valid Server found!");
            }

            Console.ReadKey();
        }

    }
}
