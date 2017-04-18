using System;
using System.Collections.Generic;
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
        private static SclParser _sclParser = new SclParser();

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

            _sclParser.ParseFile();

            /*
            Parser.StructDataObj.AddStructDataObj("%", 0x0209, "%");
            Parser.StructDataObj.AddStructDataObj("%", 0x020a, "%");
            */
            StructDataObj.AddStructDataObj("LD0/MMXU1.TotVAr.mag.f", 0x020b, "A", "NONE", "float", "");
            StructDataObj.AddStructDataObj("LD0/MMXU1.TotW.mag.f", 0x020c, "A", "NONE", "float", "");
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
                Console.WriteLine($@"{item.AddrDataObj}  {item.NameDataObj} {item.ConvertDataObj}");
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

            ConfigServer("test.cfg");
            //ConfigServer("myModel.cfg");

            StartServer(Settings.Settings.ConfigServer.PortServer);



           // RuningServer();

           
        }

        private static void RuningServer()
        {         
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

                StaticUpdateData();

                Thread myThread = new Thread(RuningServer)
                {
                    Name = "Thread Server"
                };
                myThread.Start();

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

        private static void StaticUpdateData()
        {
            InitDefultParam();

            string format;
            string value;
            string path;

            foreach (var itemDefultDataObj in StructDefultDataObj.structDefultDataObj)
            {
                _sclParser.UpdateStaticDataObj(itemDefultDataObj, out format, out value, out path);

                InitStaticUpdateData(format, value, path);
            }
        }

        private static void InitDefultParam()
        {
            string format;
            string value;
            string path;

            foreach (var itemLd in StructModelObj.Model.ListLD)
            {
                foreach (var itemLn in itemLd.ListLN)
                {
                    foreach (var itemDo in itemLn.ListDO)
                    {
                        foreach (var itemDa in itemDo.ListDA)
                        {
                            _sclParser.CoonvertStaticDataObj(_sclParser.MapLibiecType(itemDa.BTypeDA), out format);

                            value = itemDa.Value;
                            path = itemLd.NameLD + "/" + itemLn.NameLN + "." + itemDo.NameDO + "." + itemDa.NameDA;


                            if (itemDa.ListDA.Count == 0)
                            {
                                if (value != null)
                                {
                                    InitStaticUpdateData(format, value, path);
                                }
                            }
                            else
                            {
                                InitDefultParamBda(itemDa.ListDA, format, value, path);
                            }

                        }
                    }
                }
            }
        }

        private static void InitDefultParamBda(List <StructModelObj.NodeDA> Da, string format, string value, string path)
        {
            foreach (var itemDa in Da)
            {
                _sclParser.CoonvertStaticDataObj(_sclParser.MapLibiecType(itemDa.BTypeDA), out format);

                value = itemDa.Value;
                path += "." + itemDa;

                if (value != null)
                {
                    if (itemDa.ListDA.Count == 0)
                    {
                        InitStaticUpdateData(format, value, path);
                    }
                    else
                    {
                        InitDefultParamBda(itemDa.ListDA, format, value, path);
                    }
                }
            }
        }

        private static void InitStaticUpdateData(string format, string value, string path)
        {
            if (format == "bool")
            {
                UpdateBool(path, value);
                return;
            }
            if (format == "int")
            {
                UpdateInt(path, value);
                return;
            }
            if (format == "float")
            {
                UpdateFloat(path, value);
                return;
            }
            if (format == "string")
            {
                UpdateString(path, value);
                return;
            }
            if (format == "datetime")
            {
                UpdateDateTime(path, value);
                return;
            }
            if (format == "ushort")
            {
                UpdateUshort(path, value);
            }
        }

        private static void UpdateBool(string path, string value)
        {
            _iedServer.LockDataModel();
            bool str = Convert.ToBoolean(value);

            _iedServer.UpdateBooleanAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference(path), str);

            _iedServer.UnlockDataModel();
        }

        private static void UpdateInt(string path, string value)
        {
            _iedServer.LockDataModel();
            int str = Convert.ToInt32(value);

            _iedServer.UpdateInt32AttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference(path), str);

            _iedServer.UnlockDataModel();
        }

        private static void UpdateFloat(string path, string value)
        {
            _iedServer.LockDataModel();
            float str = Convert.ToSingle(value);

            _iedServer.UpdateFloatAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference(path), str);

            _iedServer.UnlockDataModel();
        }

        private static void UpdateString(string path, string value)
        {
            _iedServer.LockDataModel();
            string str = Convert.ToString(value);

            _iedServer.UpdateVisibleStringAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference(path), str);

            _iedServer.UnlockDataModel();
        }

        private static void UpdateDateTime(string path, string value)
        {
            _iedServer.LockDataModel();
            DateTime str = Convert.ToDateTime(value);

            _iedServer.UpdateUTCTimeAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference(path), str);

            _iedServer.UnlockDataModel();
        }

        private static void UpdateUshort(string path, string value)
        {
            _iedServer.LockDataModel();
            ushort str = Convert.ToUInt16(value);

            _iedServer.UpdateQuality((DataAttribute)_iedModel.GetModelNodeByShortObjectReference(path), str);

            _iedServer.UnlockDataModel();
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
                if(item.ConvertDataObj == "float")
                _iedServer.UpdateFloatAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference(item.NameDataObj), Convert.ToSingle(item.ValueDataObj));

                _iedServer.UpdateUTCTimeAttributeValue((DataAttribute)_iedModel.GetModelNodeByShortObjectReference("LD0/MMXU1.TotW.t"), DateTime.Now);
            }
        }
    }
}
