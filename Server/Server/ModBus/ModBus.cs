using System;
using System.Collections.Generic;
using System.Timers;
using UniSerialPort;
using Timer = System.Timers.Timer;

namespace Server.ModBus
{
    public static class ModBus
    {
        private static AsynchSerialPort _serialPort = new AsynchSerialPort();

        private static readonly Timer DownloadDataTimer = new Timer()
        {
            Interval = 1000,
            Enabled = false
        };

        private static readonly Timer DownloadScopeTimer = new Timer()
        {
            Interval = 2000,
            Enabled = false
        };

        private static bool _startDownloadScope = false;
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

            if (Parser.StructDataObj.structDataObj.Count != 0)
            {
                if (_currentIndex == Parser.StructDataObj.structDataObj.Count)
                {
                    _currentIndex = 0;
                }

                _serialPort.GetDataRTU(Parser.StructDataObj.structDataObj[_currentIndex].addrDataObj, 1, UpdateData);
            }
        }

        private static void UpdateData(bool dataOk, ushort[] paramRtu)
        {
            if (!dataOk)
            {
                DataRequest();
                return;
            }

            Parser.StructDataObj.structDataObj[_currentIndex].valueDataObj = Convert.ToInt64(paramRtu[0]);
            _currentIndex++;
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
                UpdateScopoe();
            }
        }

        private static int _loadConfigStep;
        private static int _indexChannel;

        private static void ScopeConfigRequest()
        {
            if (_loadConfigStep == 0)                //Количество каналов 
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 67), 1, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 1)                //Количество осциллограмм 
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 66), 1, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 2)                //Предыстория 
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 68), 1, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 3)                //Делитель
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 69), 1, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 4)                //Режим работы
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 70), 1, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 5)                //Размер осциллограммы 
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 64), 2, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 6)                //Частота выборки
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.OscilCmndAddr + 2), 1, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 7)                //Весь размер под осциллограммы 
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.OscilCmndAddr + 376), 2, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 8)                //Размер одной выборки
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.OscilCmndAddr + 3), 1, UpdateScopeConfig);
                return;
            }
            if (_loadConfigStep == 9)                //Количество выборок на предысторию 
            {
                _serialPort.GetDataRTU(Settings.Settings.ConfigGlobal.OscilCmndAddr, 2, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 10)                //Статус осциллогрофа
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.OscilCmndAddr + 378), 2, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 11)                //Адреса каналов 
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 32), 32, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 12)                //Формат каналов 
            {
                _serialPort.GetDataRTU(Settings.Settings.ConfigGlobal.ConfigurationAddr, 32, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 13)                //Название каналов 
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 71 + 16 * _indexChannel), 16, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 14)                //Фаза каналов 
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 583 + _indexChannel), 1, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 15)                //CCBM каналов
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 615 + 8 * _indexChannel), 8, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 16)                //Измеерение каналов
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 871 + 4 * _indexChannel), 4, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 17)                //Type каналов
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 999 + _indexChannel), 1, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 18)                //
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 1031), 16, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 19)                //
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 1047), 8, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 20)                //
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 1055), 4, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 21)                //
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 1059), 4, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 22)                //
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 1063), 4, UpdateScopeConfig);
                return;
            }

            if (_loadConfigStep == 23)                //
            {
                _serialPort.GetDataRTU((ushort)(Settings.Settings.ConfigGlobal.ConfigurationAddr + 1067), 4, UpdateScopeConfig);
            }
        }

        private static void UpdateScopeConfig(bool dataOk, ushort[] paramRtu)
        {
            if (!dataOk)
            {
                ScopeConfigRequest();
            }
            else
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

        public class ScopeStatus
        {
            List<NowStatus> nowStatus = new List<NowStatus>();
            public class NowStatus
            {
                public int Status { get; private set; }
                public DateTime StampDate { get; private set; }

                public NowStatus(int status, DateTime stampDate)
                {
                    Status = status;
                    StampDate = stampDate;
                }
            }

            public void AddNowStatus(int status, DateTime stampDate)
            {
                nowStatus.Add(new NowStatus(status, stampDate));
            }

            List<OldStatus> oldStatus = new List<OldStatus>();
            public class OldStatus
            {
                public int Status { get; private set; }
                public DateTime StampDate { get; private set; }

                public OldStatus(int status, DateTime stampDate)
                {
                    Status = status;
                    StampDate = stampDate;
                }
            }

            public void AddOldStatus(int status, DateTime stampDate)
            {
                oldStatus.Add(new OldStatus(status, stampDate));
            }

            public void AutoOldStaus(ushort countScope)
            {
                for (int i = 0; i < countScope; i++)
                {
                    AddOldStatus(0, DateTime.Now);
                }
            }
        }

        private static void ScopeStatusRequest()
        {
            //_serialPort.GetDataRTU((ushort)(), , UpdateScopeStatus); 
        }

        private static void UpdateScopeStatus(bool dataOk, ushort[] paramRtu)
        {
            
        }

        private static void UpdateScopoe()
        {

        }

        public static void CloseModBusPort()
        {
            DownloadScopeTimer.Enabled = false;
            DownloadDataTimer.Enabled = false;
            _serialPort.Close();
        }
    }
}
