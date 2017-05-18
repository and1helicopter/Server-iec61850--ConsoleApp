using ADSPLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using Server.Parser;
using UniSerialPort;
using Timer = System.Timers.Timer;

namespace Server.ModBus
{
    public static class ModBus
    {
        private static AsynchSerialPort _serialPort = new AsynchSerialPort();

        public static object Locker = new object();

        private static readonly Timer DownloadDataTimer = new Timer()
        {
            Interval = 100,
            Enabled = false
        };

        private static readonly Timer DownloadScopeTimer = new Timer()
        {
            Interval = 2000,
            Enabled = false
        };

        private static bool _startDownloadScope;
        private static bool _configScopeDownload;
        
        public static void ConfigModBusPort()
        {
            if (_serialPort.IsOpen)
            {
                Console.WriteLine(Settings.Settings.ConfigGlobal.Language == "rus"
                    ? "ModBus порт открыт! Закройте ModBus порт и повторите."
                    : "ModBus port is open! Close ModBus SerialPort and repeat.");
                return;
            }

            _serialPort.SerialPortMode = SerialPortModes.RSMode;
            _serialPort.BaudRate = Settings.Settings.ConfigModBus.BaudRate;
            _serialPort.Parity = Settings.Settings.ConfigModBus.SerialPortParity;
            _serialPort.StopBits = Settings.Settings.ConfigModBus.SerialPortStopBits;
            _serialPort.PortName = Settings.Settings.ConfigModBus.ComPortName;
        }

        public static void OpenModBusPort()
        {
            try
            {
                _serialPort.Open();
                if (_serialPort.IsOpen)
                {
                    //Обновление параметров
                    DownloadDataTimer.Elapsed += downloadDataTimer_Elapsed;
                    downloadDataTimer_Elapsed(null, null);
                    DownloadDataTimer.Enabled = true;
                    
                    //Обновление осциллограммы
                    if (Settings.Settings.ConfigGlobal.DownloadScope)
                    {
                        DownloadScopeTimer.Elapsed += downloadScopeTimer_Elapsed;
                        downloadScopeTimer_Elapsed(null, null);
                        DownloadScopeTimer.Enabled = true;
                    }
                }
            }
            catch
            {
                Console.WriteLine(Settings.Settings.ConfigGlobal.Language == "rus"
                   ? "ModBus порт не может быть открыт!"
                   : "ModBus port not open!");
            }
        }

        private static void downloadDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DataRequest();
        }

        private static void downloadScopeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ScopoeRequest();
        }

        private static int _currentIndex;

        private static void DataRequest()
        {
            if (!_serialPort.IsOpen)
            {
                return;
            }

            if (StructUpdateDataObj.DataClassGet.Count != 0)
            {
                if (_currentIndex == StructUpdateDataObj.DataClassGet.Count)
                {
                    _currentIndex = 0;
                }

                if (!StructUpdateDataObj.DataClassGet[_currentIndex].GetDataObj)
                {
                    lock (Locker)
                    {
                        _serialPort.GetDataRTU(StructUpdateDataObj.DataClassGet[_currentIndex].AddrDataObj, 1, UpdateData);
                        StructUpdateDataObj.DataClassGet[_currentIndex].GetDataObj_Set(true);
                    }
                }
            }
        }

        private static void UpdateData(bool dataOk, ushort[] paramRtu)
        {
            if (dataOk)
            {
                if(StructUpdateDataObj.DataClassGet[_currentIndex].DataObj.GetType() == typeof(MvClass))
                {
                    ((MvClass)StructUpdateDataObj.DataClassGet[_currentIndex].DataObj).UpdateClass(DateTime.Now, Convert.ToInt64(paramRtu[0]));
                    StructUpdateDataObj.DataClassGet[_currentIndex].GetDataObj_Set(false);
                }
                else if (StructUpdateDataObj.DataClassGet[_currentIndex].DataObj.GetType() == typeof(SpsClass))
                {
                    bool val = (Convert.ToInt32(paramRtu[0]) & 1 << Convert.ToInt32(StructUpdateDataObj.DataClassGet[_currentIndex].MaskDataObj)) > 0;
                   ((SpsClass)StructUpdateDataObj.DataClassGet[_currentIndex].DataObj).UpdateClass(DateTime.Now, val);
                    StructUpdateDataObj.DataClassGet[_currentIndex].GetDataObj_Set(false);
                }

                _currentIndex++;
            }
        }
        
        private static void ScopoeRequest()
        {

            if (!_startDownloadScope)
            {
                if (!_configScopeDownload)
                {
                    ScopeConfigRequest();
                }
                else
                {
                    ScopeStatusRequest();
                }
            }
            else
            {
               DownloadScopeTimer.Enabled = false;
               ScopeDownloadRequestSet();
            }
        }

        private static int _loadConfigStep;
        private static int _indexChannel;

        private static void ScopeConfigRequest()
        {
            if (_loadConfigStep == 0)                //Количество каналов 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 67), 1, UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 1)                //Количество осциллограмм 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 66), 1,UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 2)                //Предыстория 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 68), 1,UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 3)                //Делитель
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 69), 1,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 4)                //Режим работы
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 70), 1,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 5)                //Размер осциллограммы 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 64), 2,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 6)                //Частота выборки
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.OscilCmndAddr + 2), 1,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 7)                //Весь размер под осциллограммы 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.OscilCmndAddr + 376), 2,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 8)                //Размер одной выборки
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.OscilCmndAddr + 3), 1,
                        UpdateScopeConfig);
                }
                return;
            }
            if (_loadConfigStep == 9)                //Количество выборок на предысторию 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU(Settings.Settings.ConfigGlobal.OscilCmndAddr, 2, UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 10)                //Статус осциллогрофа
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.OscilCmndAddr + 378), 2,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 11)                //Адреса каналов 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 32), 32,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 12)                //Формат каналов 
            {
                _serialPort.GetDataRTU(Settings.Settings.ConfigGlobal.ConfigurationAddr, 32, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 13)                //Название каналов 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU(
                        (ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 71 + 16 * _indexChannel), 16,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 14)                //Фаза каналов 
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU(
                        (ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 583 + _indexChannel), 1,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 15)                //CCBM каналов
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU(
                        (ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 615 + 8 * _indexChannel), 8,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 16)                //Измеерение каналов
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU(
                        (ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 871 + 4 * _indexChannel), 4,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 17)                //Type каналов
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU(
                        (ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 999 + _indexChannel), 1,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 18)                //
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 1031), 16,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 19)                //
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 1047), 8,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 20)                //
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 1055), 4,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 21)                //
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 1059), 4,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 22)                //
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 1063), 4,
                        UpdateScopeConfig);
                }
                return;
            }

            if (_loadConfigStep == 23)                //
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.ConfigurationAddr + 1067), 4,
                        UpdateScopeConfig);
                }
            }
        }

        private static void UpdateScopeConfig(bool dataOk, ushort[] paramRtu)
        {
            if (dataOk)
            {
                switch (_loadConfigStep)
                {
                    case 0:                     //Количество каналов 
                        {
                            ScopeConfig.ChannelCount = paramRtu[0];
                            _loadConfigStep = 1;
                            ScopeConfigRequest();
                        }
                        break;

                    case 1:                     //Количество осциллограмм
                        {
                            ScopeConfig.ScopeCount = paramRtu[0];
                            _loadConfigStep = 2;
                            ScopeConfigRequest();
                        }
                        break;

                    case 2:                     //Предыстория 
                        {
                            ScopeConfig.HistoryCount = paramRtu[0];
                            _loadConfigStep = 3;
                            ScopeConfigRequest();
                        }
                        break;

                    case 3:                     //Делитель
                        {
                            ScopeConfig.FreqCount = paramRtu[0];
                            _loadConfigStep = 4;
                            ScopeConfigRequest();
                        }
                        break;

                    case 4:                     //Режим работы
                        {
                            ScopeConfig.OscilEnable = paramRtu[0];
                            _loadConfigStep = 5;
                            ScopeConfigRequest();
                        }
                        break;

                    case 5:                     //Размер осциллограммы 
                        {
                            ScopeConfig.OscilSize = (uint)(paramRtu[1] << 16);
                            ScopeConfig.OscilSize += paramRtu[0];
                            _loadConfigStep = 6;
                            ScopeConfigRequest();
                        }
                        break;

                    case 6:                     //Частота выборки
                        {
                            ScopeConfig.SampleRate = paramRtu[0];
                            ScopeConfig.ScopeEnabled = true;
                            _loadConfigStep = 7;
                            ScopeConfigRequest();
                        }
                        break;

                    case 7:                     //Размер осциллограммы 
                        {
                            ScopeConfig.OscilAllSize = (uint)(paramRtu[1] << 16);
                            ScopeConfig.OscilAllSize += (paramRtu[0]);
                            _loadConfigStep = 8;
                            ScopeConfigRequest();
                        }
                        break;

                    case 8:                     //Размер одной выборки
                        {
                            ScopeConfig.SampleSize = paramRtu[0];
                            _loadConfigStep = 9;
                            ScopeConfigRequest();
                        }
                        break;
                    case 9:                     //Размер всей памяти 
                        {
                            ScopeConfig.OscilHistCount = (uint)(paramRtu[1] << 16);
                            ScopeConfig.OscilHistCount += paramRtu[0];
                            _loadConfigStep = 10;
                            ScopeConfigRequest();

                        }
                        break;
                    case 10:                     //Статус осциллогрофа
                        {
                            ScopeConfig.StatusOscil = paramRtu[0];
                            _loadConfigStep = 11;
                            ScopeConfigRequest();
                        }
                        break;
                    case 11:                     //Адреса каналов 
                        {
                            ScopeConfig.InitOscilAddr(paramRtu);
                            _loadConfigStep = 12;
                            ScopeConfigRequest();
                        }
                        break;
                    case 12:                     //Формат каналов 
                        {
                            ScopeConfig.InitOscilFormat(paramRtu);
                            _loadConfigStep = 13;
                            ScopeConfigRequest();
                        }
                        break;
                    case 13:                     //Названия каналов 
                        {
                            if (ScopeConfig.ChannelCount == 0)   //Если в системе нет конфигурации
                            {
                                _loadConfigStep = 0;
                                _configScopeDownload = true;
                                break;
                            }
                            if (_indexChannel == 0) ScopeConfig.ChannelName.Clear();
                            ScopeConfig.InitChannelName(paramRtu);
                            if (_indexChannel == ScopeConfig.ChannelCount - 1)
                            {
                                _indexChannel = 0;
                                _loadConfigStep = 14;
                            }
                            else
                            {
                                _indexChannel++;
                                ScopeConfigRequest();
                            }
                        }
                        break;
                    case 14:                     //Названия каналов 
                        {
                            if (_indexChannel == 0) ScopeConfig.ChannelPhase.Clear();
                            ScopeConfig.InitChannelPhase(paramRtu);
                            if (_indexChannel == ScopeConfig.ChannelCount - 1)
                            {
                                _indexChannel = 0;
                                _loadConfigStep = 15;
                            }
                            else
                            {
                                _indexChannel++;
                                ScopeConfigRequest();
                            }
                        }
                        break;

                    case 15:                     //Названия каналов 
                        {
                            if (_indexChannel == 0) ScopeConfig.ChannelCcbm.Clear();
                            ScopeConfig.InitChannelCcbm(paramRtu);
                            if (_indexChannel == ScopeConfig.ChannelCount - 1)
                            {
                                _indexChannel = 0;
                                _loadConfigStep = 16;
                            }
                            else
                            {
                                _indexChannel++;
                                ScopeConfigRequest();
                            }
                        }
                        break;

                    case 16:                     //Названия каналов 
                        {
                            if (_indexChannel == 0) ScopeConfig.ChannelDemension.Clear();
                            ScopeConfig.InitChannelDemension(paramRtu);
                            if (_indexChannel == ScopeConfig.ChannelCount - 1)
                            {
                                _indexChannel = 0;
                                _loadConfigStep = 17;
                            }
                            else
                            {
                                _indexChannel++;
                                ScopeConfigRequest();
                            }
                        }
                        break;

                    case 17:                     //Названия каналов 
                        {
                            if (_indexChannel == 0) ScopeConfig.ChannelType.Clear();
                            ScopeConfig.InitChannelType(paramRtu);
                            if (_indexChannel == ScopeConfig.ChannelCount - 1)
                            {
                                _indexChannel = 0;
                                _loadConfigStep = 18;
                            }
                            else
                            {
                                _indexChannel++;
                                ScopeConfigRequest();
                            }
                        }
                        break;

                    case 18:                     //
                        {
                            ScopeConfig.InitStationName(paramRtu);
                            _loadConfigStep = 19;
                            ScopeConfigRequest();
                        }
                        break;

                    case 19:                     //Названия каналов 
                        {
                            ScopeConfig.InitRecordingId(paramRtu);
                            _loadConfigStep = 20;
                            ScopeConfigRequest();

                        }
                        break;

                    case 20:                     //Названия каналов 
                        {
                            ScopeConfig.InitTimeCode(paramRtu);
                            _loadConfigStep = 21;
                            ScopeConfigRequest();
                        }
                        break;

                    case 21:                     //Названия каналов 
                        {
                            ScopeConfig.InitLocalCode(paramRtu);

                            _loadConfigStep = 22;
                            ScopeConfigRequest();
                        }
                        break;

                    case 22:                     //Названия каналов 
                        {
                            ScopeConfig.InitTmqCode(paramRtu);

                            _loadConfigStep = 23;
                            ScopeConfigRequest();
                        }
                        break;

                    case 23:                     //Названия каналов 
                        {
                            ScopeConfig.InitLeapsec(paramRtu);

                            _loadConfigStep = 0;
                            DownloadScopeTimer.Enabled = true;
                            _configScopeDownload = true;
                        }
                        break;
                }
            }
        }

        private static readonly int[] NowStatus = new int[32];
        private static readonly int[] OldStatus = new int[32];

        private static int _indexDownloadScope;
        private static uint _oscilStartTemp;
        private static uint _oscilEndTemp;

        private static void ScopeStatusRequest()
        {
            lock (Locker)
            {
                _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.OscilCmndAddr + 8), 32,
                    UpdateScopeStatus);
            }
        }

        private static void UpdateScopeStatus(bool dataOk, ushort[] paramRtu)
        {
            if (dataOk)
            {
                for (int i = 0; i < ScopeConfig.ScopeCount; i++)
                {
                    NowStatus[i] = paramRtu[i];
                }
                for (int i = 0; i < ScopeConfig.ScopeCount; i++)
                {
                    if (NowStatus[i] == 4 && OldStatus[i] != 4)
                    {
                        _indexDownloadScope = i;
                        OldStatus[i] = NowStatus[i];
                        _oscilStartTemp = ((uint)_indexDownloadScope * (ScopeConfig.OscilSize >> 1)); //Начало осциллограммы 
                        _oscilEndTemp = (((uint)_indexDownloadScope + 1) * (ScopeConfig.OscilSize >> 1)); //Конец осциллограммы 
                        _startDownloadScope = true;
                        break;
                    }
                }
            }
        }

        private static uint _loadOscilTemp;
        private static uint _countTemp;
        private static int _loadOscDataStep;
        private static int _loadOscDataSubStep;
        private static uint _startLoadSample;
        private static DateTime StampDate;

        private static List<ushort[]> _downloadedData = new List<ushort[]>();
        private static ushort[] _loadParamPart = new ushort[32];
        private static ushort[] _writeArr = new ushort[3];


        private static uint CalcOscilLoadTemp()
        {
            if (_countTemp < (ScopeConfig.OscilSize >> 1))                               //Проход по осциллограмме 
            {
                _loadOscilTemp += 32;                                                    //Какую часть осциллограммы грузим 
                _countTemp += 32;
            }
            return (_loadOscilTemp - 32 + _oscilStartTemp);                               //+Положение относительно начала массива
        }

        private static void ScopeDownloadRequestSet()
        {
            switch (_loadOscDataStep)
            {
                //Загрузка номера выборки на котором заканчивается осциллограмма 
                case 0:
                    {
                    lock (Locker)
                        {
                            _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.OscilCmndAddr + 72 + _indexDownloadScope * 2), 2,UpdateScopoe);
                        }
                    }
                    break;

                //Загрузка данных
                case 1:
                    {
                        uint oscilLoadTemp = (CalcOscilLoadTemp());

                        _writeArr[0] = 0x0001;
                        _writeArr[1] = Convert.ToUInt16((oscilLoadTemp << 16) >> 16);
                        _writeArr[2] = Convert.ToUInt16(oscilLoadTemp >> 16);
                        lock (Locker)
                        {
                            _serialPort.SetDataRTU((ushort) (Settings.Settings.ConfigGlobal.OscilCmndAddr + 5),ScopeDownloadRequestGet, RequestPriority.Normal, _writeArr);
                        }
                    }
                    break;
            }
        }

        private static void ScopeDownloadRequestGet(bool dataOk, ushort[] paramRtu)
        {
            if (dataOk)
            {
                lock (Locker)
                {
                    _serialPort.GetDataRTU((ushort) (Settings.Settings.ConfigGlobal.OscilCmndAddr + 40), 32,
                        UpdateScopoe);
                }
            }
        }

        private static void UpdateScopoe(bool dataOk, ushort[] paramRtu)
        {
            if (dataOk)
            {
                switch (_loadOscDataStep)
                {
                    //Загрузка стартового адреса
                    case 0:
                        {
                            _startLoadSample = (uint)(paramRtu[1] << 16);
                            _startLoadSample += paramRtu[0];
                            _loadOscDataStep = 1;
                        }
                        break;

                    case 1:
                        {
                            _downloadedData.Add(new ushort[32]);
                            for (int i = 0; i < 32; i++)
                            {
                                _downloadedData[_downloadedData.Count - 1][i] = paramRtu[i];
                            }

                            _loadOscDataSubStep = 0;

                            if (_countTemp >= (ScopeConfig.OscilSize >> 1))
                            {

                                _loadOscDataStep = 0;
                                _loadOscilTemp = 0;
                                _countTemp = 0;
                                SaveToFileRequest();
                                return;
                            }
                        }
                        break;
                }
            }
            ScopeDownloadRequestSet();
        }

        private static void SaveToFileRequest()
        {
            lock (Locker)
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.OscilCmndAddr + 136 + _indexDownloadScope * 6), 6, SaveToFile);
            }
        }

        private static void SaveToFile(bool dataOk, ushort[] paramRtu)
        {
            if (dataOk)
            {
                string str1 = (paramRtu[0] & 0x3F).ToString("X2") + "/" + ((paramRtu[0] >> 8) & 0x1F).ToString("X2") + @"/20" + (paramRtu[1] & 0xFF).ToString("X2");
                string str2 = (paramRtu[3] & 0x3F).ToString("X2") + ":" + ((paramRtu[2] >> 8) & 0x7F).ToString("X2") + @":" + (paramRtu[2] & 0x7F).ToString("X2");
                string str3 = ((paramRtu[4] * 1000) >> 8).ToString("D3") + @"000";
                string str = str1 + "," + str2 + @"." + str3;
                try
                {
                    StampDate = DateTime.Parse(str);
                }
                catch
                {
                    // ignored
                }
                CreateFile();


                DownloadScopeTimer.Enabled = true;
                _startDownloadScope = false;
            }
        }

        #region
        private static string FileHeaderLine()
        {
            string str = " " + "\t";
            for (int i = 0; i < ScopeConfig.ChannelCount; i++)
            {
                str = str + ScopeConfig.ChannelName[i] + "\t";
            }
            return str;
        }

        private static int _count64, _count32, _count16;

        private static string FileParamLine(ushort[] paramLine, int lineNum)
        {
            int i;
            // ChFormats();
            string str = lineNum.ToString() + "\t";
            for (i = 0, _count64 = 0, _count32 = 0, _count16 = 0; i < ScopeConfig.ChannelCount; i++)
            {
                var ulTemp = ParseArr(i, paramLine);
                str = str + AdvanceConvert.HexToFormat(ulTemp, (byte)ScopeConfig.OscilFormat[i]) + "\t";
            }
            return str;
        }
        #endregion

        private static ulong ParseArr(int i, ushort[] paramLine)
        {
            ulong ulTemp = 0;
            if (ScopeConfig.OscilFormat[i] >> 8 == 3)
            {
                ulTemp = 0;
                ulTemp += (ulong)(paramLine[_count64 + 0]) << 8 * 2;
                ulTemp += (ulong)(paramLine[_count64 + 1]) << 8 * 3;
                ulTemp += (ulong)(paramLine[_count64 + 2]) << 8 * 0;
                ulTemp += (ulong)(paramLine[_count64 + 3]) << 8 * 1;
                _count64 += 4;
            }
            if (ScopeConfig.OscilFormat[i] >> 8 == 2)
            {
                ulTemp = 0;
                ulTemp += (ulong)(paramLine[_count64 + _count32 + 0]) << 8 * 0;
                ulTemp += (ulong)(paramLine[_count64 + _count32 + 1]) << 8 * 1;
                _count32 += 2;
            }
            if (ScopeConfig.OscilFormat[i] >> 8 == 1)
            {
                ulTemp = paramLine[_count64 + _count32 + _count16];
                _count16 += 1;
            }
            return ulTemp;
        }

        //Формирование строк всех загруженных данных
        private static List<ushort[]> InitParamsLines()
        {
            List<ushort[]> paramsLines = new List<ushort[]>();
            List<ushort[]> paramsSortLines = new List<ushort[]>();
            int k = 0;
            int j = 0;
            int l = 0;
            foreach (ushort[] t in _downloadedData)
            {
                while (j < 32)
                {
                    if (k == 0) paramsLines.Add(new ushort[ScopeConfig.SampleSize >> 1]);
                    while (k < (ScopeConfig.SampleSize >> 1) && j < 32)
                    {
                        paramsLines[paramsLines.Count - 1][k] = t[j];
                        k++;
                        j++;
                    }
                    if (k == (ScopeConfig.SampleSize >> 1)) k = 0;
                }
                j = 0;
            }
            paramsLines.RemoveAt(paramsLines.Count - 1);
            //Формирую список начиная с предыстории 
            for (int i = 0; i < paramsLines.Count; i++)
            {
                if ((i + (int)_startLoadSample + 1) >= paramsLines.Count)
                {
                    paramsSortLines.Add(new ushort[ScopeConfig.SampleSize >> 1]);
                    paramsSortLines[i] = paramsLines[l];
                    l++;
                }
                else
                {
                    paramsSortLines.Add(new ushort[ScopeConfig.SampleSize >> 1]);
                    paramsSortLines[i] = paramsLines[i + (int)_startLoadSample + 1];
                }
            }
            return paramsSortLines;
        }

        //Save to cometrade
        #region

        private static string FileParamLineData(ushort[] paramLine, int lineNum)
        {
            string str1;
            int i;
            ulong ulTemp;
            _count64 = 0;
            _count32 = 0;
            _count16 = 0;
            string str = (lineNum + 1) + ",";
            for (i = 0; i < ScopeConfig.ChannelCount; i++)
            {
                if (ScopeConfig.ChannelType[i] == 0)
                {
                    ulTemp = ParseArr(i, paramLine);
                    str1 = AdvanceConvert.HexToFormat(ulTemp, (byte)ScopeConfig.OscilFormat[i]);
                    str1 = str1.Replace(",", ".");
                    str = str + "," + str1;
                }
            }
            for (i = 0; i < ScopeConfig.ChannelCount; i++)
            {

                if (ScopeConfig.ChannelType[i] == 1)
                {
                    ulTemp = ParseArr(i, paramLine);
                    str1 = AdvanceConvert.HexToFormat(ulTemp, (byte)ScopeConfig.OscilFormat[i]);
                    str1 = str1.Replace(",", ".");
                    str = str + "," + str1;
                }
            }
            return str;
        }

        private static string Line1(int filterIndex)
        {
            string stationName = ScopeConfig.StationName;
            string recDevId = ScopeConfig.RecordingId;
            string revYear = "";
            if (filterIndex == 2) revYear = "1999";
            if (filterIndex == 3) revYear = "2013";
            string str = stationName + "," + recDevId + "," + revYear;
            return str;
        }

        private static string Line2()
        {
            int nA = 0, nD = 0;
            for (int i = 0; i < ScopeConfig.ChannelCount; i++)
            {
                //Если параметр в списке известных, то пишем его в файл
                if (ScopeConfig.ChannelType[i] == 0) nA += 1;
                if (ScopeConfig.ChannelType[i] == 1) nD += 1;
            }
            int tt = nA + nD;
            string str = tt + "," + nA + "A," + nD + "D";
            return str;
        }

        private static string Line3(int num, int nA)
        {
            string chId = ScopeConfig.ChannelName[num];
            string ph = ScopeConfig.ChannelPhase[num];
            string ccbm = ScopeConfig.ChannelCcbm[num];
            string uu = ScopeConfig.ChannelDemension[num];
            string a = "1";
            string b = "0";
            int skew = 0;
            int min = 0;
            int max = 0;
            int primary = 1;
            int secondary = 1;
            string ps = "P";

            string str = nA + "," + chId + "," + ph + "," + ccbm + "," + uu + "," + a + "," + b + "," + skew + "," +
                         min + "," + max + "," + primary + "," + secondary + "," + ps;

            return str;
        }

        private static string Line4(int num, int nD)
        {
            string chId = ScopeConfig.ChannelName[num];
            string ph = ScopeConfig.ChannelPhase[num];
            string ccbm = ScopeConfig.ChannelCcbm[num];
            int y = 0;

            string str = nD + "," + chId + "," + ph + "," + ccbm + "," + y;

            return str;
        }

        private static string Line5()
        {
            return Settings.Settings.ConfigGlobal.OscilNominalFrequency;
        }

        private static string Line6()
        {
            string nrates = "1";
            return nrates;
        }

        private static string Line7()
        {
            string samp = Convert.ToString(ScopeConfig.SampleRate / ScopeConfig.FreqCount);
            samp = samp.Replace(",", ".");
            string endsamp = InitParamsLines().Count.ToString();
            string str = samp + "," + endsamp;
            return str;
        }

        private static string Line8(int numOsc)
        {
            //Время начала осциллограммы 
            double milsec = 1000 * (double)ScopeConfig.OscilHistCount / ((double)ScopeConfig.SampleRate / ScopeConfig.FreqCount);

            DateTime dateTemp = StampDate.AddMilliseconds(-milsec);
            return dateTemp.ToString("dd'/'MM'/'yyyy,HH:mm:ss.fff000");
        }

        private static string Line9(int numOsc)
        {
            //Время срабатывания триггера
            DateTime dateTemp = StampDate;
            return dateTemp.ToString("dd'/'MM'/'yyyy,HH:mm:ss.fff000");
        }

        private static string Line10()
        {
            string ft = "ASCII";
            return ft;
        }

        private static string Line11()
        {
            string timemult = "1";
            return timemult;
        }

        private static string Line12()
        {
            string timecode = ScopeConfig.TimeCode;
            string localcode = ScopeConfig.LocalCode;
            return timecode + "," + localcode;
        }

        private static string Line13()
        {
            string tmqCode = ScopeConfig.TmqCode;
            string leapsec = ScopeConfig.Leapsec;
            return tmqCode + "," + leapsec;
        }
        #endregion//Save to cometrade

        private static uint _numName = 1;

        private static void CreateFile()
        {
            // Save to .txt
            #region 

            if (Settings.Settings.ConfigGlobal.TypeScope == "txt")
            {
                string writePath = Settings.Settings.ConfigGlobal.PathScope + "test" + _numName++  + ".txt";

                StreamWriter sw = new StreamWriter(writePath, false, Encoding.Default);

                try
                {
                    DateTime dateTemp = StampDate;
                    sw.WriteLine(dateTemp.ToString("dd'/'MM'/'yyyy HH:mm:ss.fff000"));                  //Штамп времени
                    sw.WriteLine(Convert.ToString(ScopeConfig.SampleRate / ScopeConfig.FreqCount));     //Частота выборки (частота запуска осциллогрофа/ делитель)
                    sw.WriteLine(ScopeConfig.OscilHistCount);                                           //Предыстория 
                    sw.WriteLine(FileHeaderLine());                                                     //Формирование заголовка (подписи названия каналов)

                    List<ushort[]> lu = InitParamsLines();                                              //Формирование строк всех загруженных данных (отсортированых с предысторией)
                    for (int i = 0; i < lu.Count; i++)
                    {
                        sw.WriteLine(FileParamLine(lu[i], i));
                    }
                }
                catch
                {
                    return;
                }

                sw.Close();
            }
            #endregion

            // Save to COMETRADE
            #region
            if (Settings.Settings.ConfigGlobal.TypeScope != "txt")
            {
                string writePathCfg = Settings.Settings.ConfigGlobal.PathScope + "test" + _numName + ".cfg";

                StreamWriter sw_cfg = new StreamWriter(writePathCfg, false, Encoding.GetEncoding("Windows-1251"));

                try
                {
                    sw_cfg.WriteLine(Line1(Settings.Settings.ConfigGlobal.ComtradeType));
                    sw_cfg.WriteLine(Line2());

                    for (int i = 0, j = 0; i < ScopeConfig.ChannelCount; i++)
                    {
                        if (ScopeConfig.ChannelType[i] == 0) { sw_cfg.WriteLine(Line3(i, j + 1)); j++; }
                    }
                    for (int i = 0, j = 0; i < ScopeConfig.ChannelCount; i++)
                    {
                        if (ScopeConfig.ChannelType[i] == 1) { sw_cfg.WriteLine(Line4(i, j + 1)); j++; }
                    }

                    sw_cfg.WriteLine(Line5());
                    sw_cfg.WriteLine(Line6());
                    sw_cfg.WriteLine(Line7());
                    sw_cfg.WriteLine(Line8(_indexDownloadScope));
                    sw_cfg.WriteLine(Line9(_indexDownloadScope));
                    sw_cfg.WriteLine(Line10());
                    sw_cfg.WriteLine(Line11());
                    if (Settings.Settings.ConfigGlobal.ComtradeType == 3)
                    {
                        sw_cfg.WriteLine(Line12());
                        sw_cfg.WriteLine(Line13());
                    }
                }
                catch
                {
                    Console.WriteLine(@"Ошибка при записи в файл!");
                    return;
                }

                sw_cfg.Close();

                string writePathDat = Settings.Settings.ConfigGlobal.PathScope + "test" + _numName++ + ".dat";

                StreamWriter sw_dat = new StreamWriter(writePathDat, false, Encoding.GetEncoding("Windows-1251"));

                try
                {
                    List<ushort[]> lud = InitParamsLines();
                    for (int i = 0; i < lud.Count; i++)
                    {
                        sw_dat.WriteLine(FileParamLineData(lud[i], i));
                    }
                }
                catch
                {
                    Console.WriteLine(@"Ошибка при записи в файл!");
                    return;
                }
                sw_dat.Close();

                _downloadedData.Clear();
                    //= new List<ushort[]>();
            }
            #endregion
        }

        public static void CloseModBusPort()
        {
            DownloadScopeTimer.Enabled = false;
            DownloadDataTimer.Enabled = false;
            _serialPort.Close();
        }
    }
}
