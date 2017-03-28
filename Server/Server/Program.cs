using System;
using IEC61850.Server;
using IEC61850.Common;
using System.Threading;
using Server.Parser;

namespace Server
{
    static class Program
    {
        static IedServer _iedServer;
        static IedModel _iedModel;

        static bool _running = true;

        public static void Main(string[] args)
        {
           /* run until Ctrl-C is pressed */
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                _running = false;

                StopServer();
            };
          
            Settings.Settings.ReadSettings();

            SCLParser.ParseFile();

            /*
            Parser.StructDataObj.AddStructDataObj("%", 0x0209, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x020a, "%");
            */
            StructDataObj.AddStructDataObj("LD0/MMXU1.TotVAr.mag.f", 0x020b, "float");
            StructDataObj.AddStructDataObj("LD0/MMXU1.TotW.mag.f", 0x020c, "float");
            /*
            Parser.StructDataObj.AddStructDataObj("%", 0x020d, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x020e, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x0210, "%");
            Parser.StructDataObj.AddStructDataObj("-", 0x0222, "-");
            Parser.StructDataObj.AddStructDataObj("-", 0x0223, "-");
            Parser.StructDataObj.AddStructDataObj("-", 0x0221, "-");
            Parser.StructDataObj.AddStructDataObj("-", 0x0217, "-");
            */
            
            foreach (var item in StructDataObj.structDataObj)
            {
                Console.WriteLine($@"{item.addrDataObj}  {item.nameDataObj} {item.formatDataObj}");
            }

            Settings.Settings.SaveSettings();

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


            

            ConfigFile();

            ConfigServer("myModel.cfg");

            StartServer(Settings.Settings.ConfigServer.PortServer);

            Thread myThread = new Thread(RuningServer)
            {
                Name = "Thread Server"
            };
            myThread.Start();

           // RuningServer();

           
        }

        private static void RuningServer()
        {
            _iedServer.UpdateVisibleStringAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference("LD0/LPHD1.PhyNam.vendor"), @"Energocomplekt");
            
            while (_running)
            {
                _iedServer.LockDataModel();

                lock (ModBus.ModBus.Locker)
                {
                    UpdateData();
                }

                _iedServer.UnlockDataModel();

                Thread.Sleep(50);
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
                Console.WriteLine(@"Server started");
            }
            else
            {
                Console.WriteLine(@"No valid data model found!");
            }
        }

        private static void StopServer()
        {
            if (_iedServer.IsRunning())
            {
                _iedServer.Stop();
                _iedServer.Destroy();

                Console.WriteLine(@"Server stoped");
            }
            else
            {
                Console.WriteLine(@"No valid Server found!");
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
            foreach (var item in StructDataObj.structDataObj)
            {
                if(item.formatDataObj == "float")
                _iedServer.UpdateFloatAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference(item.nameDataObj), Convert.ToSingle(item.valueDataObj));

                _iedServer.UpdateUTCTimeAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference("LD0/MMXU1.TotW.t"), DateTime.Now);
            }
        }
    }
}
