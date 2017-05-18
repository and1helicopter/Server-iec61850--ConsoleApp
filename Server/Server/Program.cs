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
        private static readonly FileParser FileParser = new FileParser();

        static bool _running = true;

        public static void Main(string[] args)
        {
           /* run until Ctrl-C is pressed */
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                _running = false;

                StopServer();
            };
            
            //Открываем настройки сервера
            Settings.Settings.ReadSettings();
            Settings.Settings.SaveSettings();

            //Открываем настройки MODBUS порта
            ModBus.ModBus.ConfigModBusPort();
            ModBus.ModBus.OpenModBusPort();

            //Парсим файл конфигурации
            FileParser.ParseFile();
            
            //Создаем модель сервера
            ConfigServer("test.cfg");

            //Запуск сервера 
            StartServer(Settings.Settings.ConfigServer.PortServer);
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


        
        private static void ConfigServer(string nameConfigFile)
        {
            _iedModel = IedModel.CreateFromFile(nameConfigFile);
            _iedServer = new IedServer(_iedModel);

            //
            StaticUpdateData();


        }

        private static void StartServer(int portNum)
        {
            if (_iedModel != null)
            {
                _iedServer.Start(portNum);

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
            foreach (var itemDefultDataObj in StructDefultDataObj.structDefultDataObj)
            {
                InitStaticUpdateData(itemDefultDataObj.Type, itemDefultDataObj.Value, itemDefultDataObj.Path);
            }

            foreach (var itemDefultDataObj in StructDefultDataObj.structDefultDataObj)
            {
                InitStaticUpdateData(itemDefultDataObj.Type, itemDefultDataObj.Value, itemDefultDataObj.Path);
            }

            //InitDefultParam();


        }

        private static void InitDefultParam()
        {
            
            //InitDefultParamClass();


        }

        private static void InitDefultParamClass()
        {
            foreach (var itemDataObject in StructUpdateDataObj.DataClassGet)
            {
                if (itemDataObject.ClassDataObj == "MV")
                {
                    MV_ClassSet(itemDataObject);
                    continue;
                }
                
                if (itemDataObject.ClassDataObj == "SPS")
                {
                    SPS_ClassSet(itemDataObject);
                    continue;
                }
            }

            //List<StructModelObj.NodeDA> da, string format, string value, string path
            //foreach (var itemDa in da)
            //{
            //    _fileParser.CoonvertStaticDataObj(_fileParser.MapLibiecType(itemDa.BTypeDA), out format);

            //    value = itemDa.Value;
            //    path += "." + itemDa;

            //    if (value != null)
            //    {
            //        if (itemDa.ListDA.Count == 0)
            //        {
            //            InitStaticUpdateData(format, value, path);
            //        }
            //        else
            //        {
            //            InitDefultParamBda(itemDa.ListDA, format, value, path);
            //        }
            //    }
            //}
        }

        private static void MV_ClassSet(StructUpdateDataObj.DataObject itemDataObject)
        {
            //units (multiplier, SIUnit)
            InitStaticUpdateData("int", ((MvClass)itemDataObject.DataObj).Unit.Multiplier.ToString(), itemDataObject.NameDataObj + ".units.multiplier");
            InitStaticUpdateData("int", ((MvClass)itemDataObject.DataObj).Unit.SIUnit.ToString(), itemDataObject.NameDataObj + ".units.SIUnit");
            //sVC (scaleFactor, offset)
            InitStaticUpdateData("float", ((MvClass)itemDataObject.DataObj).sVC.ScaleFactor.ToString(), itemDataObject.NameDataObj + ".sVC.scaleFactor");
            InitStaticUpdateData("float", ((MvClass)itemDataObject.DataObj).sVC.Offset.ToString(), itemDataObject.NameDataObj + ".sVC.offset");
            //d
            InitStaticUpdateData("string", ((MvClass)itemDataObject.DataObj).d, itemDataObject.NameDataObj + ".d");
        }

        private static void SPS_ClassSet(StructUpdateDataObj.DataObject itemDataObject)
        {
            //d
            InitStaticUpdateData("string", ((SpsClass)itemDataObject.DataObj).d, itemDataObject.NameDataObj + ".d");
        }

        private static void InitStaticUpdateData(string format, string value, string path)
        {
            if (format.ToUpper() == "bool".ToUpper())
            {
                UpdateBool(path, value);
                return;
            }
            if (format.ToUpper() == "int".ToUpper())
            {
                UpdateInt(path, value);
                return;
            }
            if (format.ToUpper() == "float".ToUpper())
            {
                UpdateFloat(path, value);
                return;
            }
            if (format.ToUpper() == "string".ToUpper())
            {
                UpdateString(path, value);
                return;
            }
            if (format.ToUpper() == "datetime".ToUpper())
            {
                UpdateDateTime(path, value);
                return;
            }
            if (format.ToUpper() == "ushort".ToUpper())
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

        private static void UpdateData()
        {
            foreach (var itemDataObject in StructUpdateDataObj.DataClassGet)
            {
                if (itemDataObject.ClassDataObj == "MV")
                {
                    MV_ClassUpdate(itemDataObject);
                    continue;
                }

                if (itemDataObject.ClassDataObj == "SPS")
                {
                    SPS_ClassUpdate(itemDataObject);
                    continue;
                }
            }
        }

        private static void MV_ClassUpdate(StructUpdateDataObj.DataObject itemDataObject)
        {
            ((MvClass)itemDataObject.DataObj).QualityCheckClass();

            var magPath = (DataAttribute) _iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + ".mag.f");
            var magVal = Convert.ToSingle(((MvClass)itemDataObject.DataObj).Mag.AnalogueValue.f);
            _iedServer.UpdateFloatAttributeValue(magPath, magVal);

            var tPath = (DataAttribute)_iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + ".t");
            var tVal = Convert.ToDateTime(((MvClass) itemDataObject.DataObj).t);
            _iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

            var qPath = (DataAttribute)_iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + ".q");
            var qVal = Convert.ToUInt16(((MvClass)itemDataObject.DataObj).q.Validity);
            _iedServer.UpdateQuality(qPath, qVal);
        }
        
        private static void SPS_ClassUpdate(StructUpdateDataObj.DataObject itemDataObject)
        {
            ((SpsClass)itemDataObject.DataObj).QualityCheckClass();

            var stValPath = (DataAttribute)_iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + ".stVal");
            var stValVal = Convert.ToBoolean(((SpsClass)itemDataObject.DataObj).stVal);
            _iedServer.UpdateBooleanAttributeValue(stValPath, stValVal);

            var tPath = (DataAttribute)_iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + ".t");
            var tVal = Convert.ToDateTime(((SpsClass)itemDataObject.DataObj).t);
            _iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

            var qPath = (DataAttribute)_iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + ".q");
            var qVal = Convert.ToUInt16(((SpsClass)itemDataObject.DataObj).q.Validity);
            _iedServer.UpdateQuality(qPath, qVal);
        }
    }
}
