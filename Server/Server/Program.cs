using System;
using IEC61850.Server;
using IEC61850.Common;
using System.Threading;

namespace Server
{
    static class Program
    {
        static IedServer _iedServer;
        static IedModel _iedModel;

        public static void Main(string[] args)
        {
            bool running = true;

            /* run until Ctrl-C is pressed */
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                running = false;
            };

            ConfigServer("myModel.cfg");

            StartServer(102);

            while (running)
            {
                _iedServer.LockDataModel();

                _iedServer.UnlockDataModel();

                Thread.Sleep(500);
            }

            StopServer();
        }

        private static void ConfigServer(string nameConfigFile)
        {
            _iedModel = IedModel.CreateFromFile(nameConfigFile);
        }

        private static void StartServer(int portNum)
        {
            if (_iedModel != null)
            {
                _iedServer = new IedServer(_iedModel);
                _iedServer.Start(portNum);
                Console.WriteLine("Server started");
            }
            else
            {
                Console.WriteLine("No valid data model found!");
            }
        }

        private static void StopServer()
        {
            if (_iedServer.IsRunning())
            {
                _iedServer.Stop();
                _iedServer.Destroy();

                Console.WriteLine("Server stoped");
            }
            else
            {
                Console.WriteLine("No Server for stoped");
            }

            Console.ReadKey();
        }

        private static void UpdateData()
        {
            
        }

        private static void DownloadScope()
        {
            
        }
    }
}
