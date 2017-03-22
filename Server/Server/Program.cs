using System;
using System.Runtime.CompilerServices;
using IEC61850.Server;
using IEC61850.Common;
using System.Threading;

namespace Server
{
    static class Program
    {
        static IedServer _iedServer;
        static IedModel _iedModel;

        static bool running = true;

        public static void Main(string[] args)
        {
           /* run until Ctrl-C is pressed */
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                running = false;

                StopServer();
            };
            Settings.Settings.InitStartSettings();

            Settings.Settings.ConfigModBus.ShowPortList();
            Settings.Settings.ConfigModBus.InitPort(4,0,0);
            Console.ReadKey();

            Settings.Settings.ConfigModBus.ShowPortSettings();

            Settings.Settings.ConfigGlobal.ChangeAddrScope(512, 4092);
            Settings.Settings.ConfigGlobal.ChangeScope(true, "txt");

            Parser.StructDataObj.AddStructDataObj("%", 0x0209, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x020a, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x020b, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x020c, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x020d, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x020e, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x0210, "%");
            Parser.StructDataObj.AddStructDataObj("-", 0x0222, "-");
            Parser.StructDataObj.AddStructDataObj("-", 0x0223, "-");
            Parser.StructDataObj.AddStructDataObj("-", 0x0221, "-");
            Parser.StructDataObj.AddStructDataObj("-", 0x0217, "-");




            foreach (var item in Parser.StructDataObj.structDataObj)
            {
                Console.WriteLine($"{item.addrDataObj}  {item.nameDataObj} {item.formatDataObj}");
            }

            ModBus.ModBus.ConfigModBusPort();
            ModBus.ModBus.OpenModBusPort();
            /*
            Thread myThread = new Thread(ModBus.ModBus.OpenModBusPort)
            {
                Name = "Thread ModBus"
            };
            myThread.Start();
            */





            //ModBus.ModBus ModBusPort = new ModBus.ModBus();


            Console.ReadKey();
            

            ConfigFile();

            ConfigServer("myModel.cfg");

            StartServer(102);

            Thread myThread = new Thread(RuningServer)
            {
                Name = "Thread Server"
            };
            myThread.Start();

           // RuningServer();

           
        }

        private static void RuningServer()
        {
            while (running)
            {
                //Console.WriteLine($"Thread: {Thread.CurrentThread.Name}");


                _iedServer.LockDataModel();

                _iedServer.UnlockDataModel();

                //Thread.Sleep(100);
            }
        }

        private static void ConfigFile()
        {
            
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
                Console.WriteLine("No valid Server found!");
            }

            Console.ReadKey();
        }
        /*
        private static void ConnectModBus(ushort addr, string port, string speed, byte parity)
        {
            ModBus.ConnectModBus(addr, port, speed, parity);
        }

        private static void CloseModBus()
        {
            ModBus.CloseModBus();
        }
        */

        private static void UpdateData()
        {
            
        }

        private static void DownloadScope()
        {
            
        }
    }
}
