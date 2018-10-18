using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLib.ModBus
{
	public static partial class ModBus
	{
		private static void ScopeConfigRequest()
		{
			if (_waitingAnswer)
			{
				return;
			}

			if (_loadConfigStep == 0)    //Количество каналов 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 67), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 1)    //Количество осциллограмм 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 66), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 2)    //Предыстория 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 68), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 3)    //Делитель
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 69), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 4)    //Режим работы
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 70), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 5)    //Размер осциллограммы 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 64), 2, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 6)    //Частота выборки
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.OscilCmndAddr + 2), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 7)    //Весь размер под осциллограммы 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.OscilCmndAddr + 376), 2, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 8)    //Размер одной выборки
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.OscilCmndAddr + 3), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}
			if (_loadConfigStep == 9)    //Количество выборок на предысторию 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu(ConfigDownloadScope.OscilCmndAddr, 2, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 10)    //Статус осциллогрофа
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.OscilCmndAddr + 378), 2, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 11)    //Адреса каналов 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 32), 32, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 12)    //Формат каналов 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu(ConfigDownloadScope.ConfigurationAddr, 32, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 13)    //Название каналов 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 71 + 16 * _indexChannel), 16, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 14)    //Фаза каналов 
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 583 + _indexChannel), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 15)    //CCBM каналов
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 615 + 8 * _indexChannel), 8, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 16)    //Измеерение каналов
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 871 + 4 * _indexChannel), 4, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 17)    //Type каналов
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 999 + _indexChannel), 1, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 18)    //
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 1031), 16, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 19)    //
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 1047), 8, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 20)    //
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 1055), 4, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 21)    //
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 1059), 4, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 22)    //
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 1063), 4, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
				return;
			}

			if (_loadConfigStep == 23)    //
			{
				lock (Locker)
				{
					SerialPort.GetDataRtu((ushort)(ConfigDownloadScope.ConfigurationAddr + 1067), 4, UpdateScopeConfig, null);
					_waitingAnswer = true;
				}
			}
		}

		private static void UpdateScopeConfig(bool dataOk, ushort[] paramRtu, object param)
		{
			if (dataOk)
			{
				switch (_loadConfigStep)
				{
					case 0:      //Количество каналов 
						{
							ScopeConfig.ChannelCount = paramRtu[0];
							_loadConfigStep = 1;
							ScopeConfigRequest();
						}
						break;

					case 1:      //Количество осциллограмм
						{
							ScopeConfig.ScopeCount = paramRtu[0];
							_loadConfigStep = 2;
							ScopeConfigRequest();
						}
						break;

					case 2:      //Предыстория 
						{
							_loadConfigStep = 3;
							ScopeConfigRequest();
						}
						break;

					case 3:      //Делитель
						{
							ScopeConfig.FreqCount = paramRtu[0];
							_loadConfigStep = 4;
							ScopeConfigRequest();
						}
						break;

					case 4:      //Режим работы
						{
							_loadConfigStep = 5;
							ScopeConfigRequest();
						}
						break;

					case 5:      //Размер осциллограммы 
						{
							ScopeConfig.OscilSize = (uint)(paramRtu[1] << 16);
							ScopeConfig.OscilSize += paramRtu[0];
							_loadConfigStep = 6;
							ScopeConfigRequest();
						}
						break;

					case 6:      //Частота выборки
						{
							ScopeConfig.SampleRate = paramRtu[0];
							_loadConfigStep = 7;
							ScopeConfigRequest();
						}
						break;

					case 7:      //Размер осциллограммы 
						{
							ScopeConfig.OscilAllSize = (uint)(paramRtu[1] << 16);
							ScopeConfig.OscilAllSize += (paramRtu[0]);
							_loadConfigStep = 8;
							ScopeConfigRequest();
						}
						break;

					case 8:      //Размер одной выборки
						{
							ScopeConfig.SampleSize = paramRtu[0];
							_loadConfigStep = 9;
							ScopeConfigRequest();
						}
						break;
					case 9:      //Размер всей памяти 
						{
							ScopeConfig.OscilHistCount = (uint)(paramRtu[1] << 16);
							ScopeConfig.OscilHistCount += paramRtu[0];
							_loadConfigStep = 10;
							ScopeConfigRequest();

						}
						break;
					case 10:      //Статус осциллогрофа
						{
							_loadConfigStep = 11;
							ScopeConfigRequest();
						}
						break;
					case 11:      //Адреса каналов 
						{
							ScopeConfig.InitOscilAddr(paramRtu);
							_loadConfigStep = 12;
							ScopeConfigRequest();
						}
						break;
					case 12:      //Формат каналов 
						{
							ScopeConfig.InitOscilFormat(paramRtu);
							_loadConfigStep = 13;
							ScopeConfigRequest();
						}
						break;
					case 13:      //Названия каналов 
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
					case 14:      //Названия каналов 
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

					case 15:      //Названия каналов 
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

					case 16:      //Названия каналов 
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

					case 17:      //Названия каналов 
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

					case 18:      //
						{
							ScopeConfig.InitStationName(paramRtu);
							_loadConfigStep = 19;
							ScopeConfigRequest();
						}
						break;

					case 19:      //Названия каналов 
						{
							ScopeConfig.InitRecordingId(paramRtu);
							_loadConfigStep = 20;
							ScopeConfigRequest();

						}
						break;

					case 20:      //Названия каналов 
						{
							ScopeConfig.InitTimeCode(paramRtu);
							_loadConfigStep = 21;
							ScopeConfigRequest();
						}
						break;

					case 21:      //Названия каналов 
						{
							ScopeConfig.InitLocalCode(paramRtu);

							_loadConfigStep = 22;
							ScopeConfigRequest();
						}
						break;

					case 22:      //Названия каналов 
						{
							ScopeConfig.InitTmqCode(paramRtu);

							_loadConfigStep = 23;
							ScopeConfigRequest();
						}
						break;

					case 23:      //Названия каналов 
						{
							ScopeConfig.InitLeapsec(paramRtu);

							_loadConfigStep = 0;
							//DownloadScopeTimer.Enabled = true;
							_configScopeDownload = true;
						}
						break;
				}
				_waitingAnswer = false;
			}
		}
	}

	public static class ScopeConfig
	{

		//Частота выборки без делителя
		public static ushort SampleRate { get; set; }

		//Размер всей памяти 
		public static uint OscilAllSize { get; set; }

		//Размер выборки
		public static ushort SampleSize { get; set; }

		//Количество выборок в предыстории 
		public static uint OscilHistCount { get; set; }

		//Количество осциллограмм 
		public static ushort ScopeCount { get; set; }

		//Количество каналов
		public static ushort ChannelCount { get; set; }

		//Делитель 
		public static ushort FreqCount { get; set; }

		//Размер осциллограммы 
		public static uint OscilSize { get; set; }

		//Адреса каналов 
		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once CollectionNeverQueried.Global
		public static List<ushort> OscilAddr { get; }

		public static void InitOscilAddr(ushort[] loadParams)
		{
			OscilAddr.Clear();
			for (int i = 0; i < ChannelCount; i++)
			{
				OscilAddr.Add(loadParams[i]);
			}
		}

		//Формат каналов 
		public static List<ushort> OscilFormat { get; }

		public static void InitOscilFormat(ushort[] loadParams)
		{
			OscilFormat.Clear();
			for (int i = 0; i < ChannelCount; i++)
			{
				OscilFormat.Add(loadParams[i]);
			}
		}

		static ScopeConfig()
		{
			ChannelType = new List<ushort>();
			ChannelDemension = new List<string>();
			ChannelCcbm = new List<string>();
			ChannelPhase = new List<string>();
			ChannelName = new List<string>();
			OscilFormat = new List<ushort>();
			OscilAddr = new List<ushort>();
		}

		//Дополнительные данные о каналах
		//Название канала
		public static List<string> ChannelName { get; }

		public static void InitChannelName(ushort[] loadParams)
		{
			string channelName = "";
			for (int i = 0; i < 16; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				channelName += chars[0].ToString();
				channelName += chars[1].ToString();
			}
			ChannelName.Add(channelName);
		}

		//Фаза канала
		public static List<string> ChannelPhase { get; }

		public static void InitChannelPhase(ushort[] loadParams)
		{
			string сhannelPhase = "";

			char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[0]));
			сhannelPhase += chars[0].ToString();
			сhannelPhase += chars[1].ToString();

			ChannelPhase.Add(сhannelPhase);
		}

		//CCBM
		public static List<string> ChannelCcbm { get; }

		public static void InitChannelCcbm(ushort[] loadParams)
		{
			string channelCcbm = "";
			for (int i = 0; i < 8; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				channelCcbm += chars[0].ToString();
				channelCcbm += chars[1].ToString();
			}
			ChannelCcbm.Add(channelCcbm);
		}
		//Измерение канала
		public static List<string> ChannelDemension { get; }

		public static void InitChannelDemension(ushort[] loadParams)
		{
			string channelDemension = "";
			for (int i = 0; i < 4; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				channelDemension += chars[0].ToString();
				channelDemension += chars[1].ToString();
			}
			ChannelDemension.Add(channelDemension);
		}

		//Тип канала
		public static List<ushort> ChannelType { get; }

		public static void InitChannelType(ushort[] loadParams)
		{
			ChannelType.Add(loadParams[0]);
		}

		//Название станции
		public static string StationName { get; private set; }

		public static void InitStationName(ushort[] loadParams)
		{
			StationName = null;
			for (int i = 0; i < 16; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				StationName += chars[0].ToString();
				StationName += chars[1].ToString();
			}
		}

		//RecordID
		public static string RecordingId { get; private set; }

		public static void InitRecordingId(ushort[] loadParams)
		{
			RecordingId = null;
			for (int i = 0; i < 8; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				RecordingId += chars[0].ToString();
				RecordingId += chars[1].ToString();
			}
		}

		//TimeCode
		public static string TimeCode { get; private set; }

		public static void InitTimeCode(ushort[] loadParams)
		{
			TimeCode = null;
			for (int i = 0; i < 4; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				TimeCode += chars[0].ToString();
				TimeCode += chars[1].ToString();
			}
		}

		//LocalCode
		public static string LocalCode { get; private set; }

		public static void InitLocalCode(ushort[] loadParams)
		{
			LocalCode = null;
			for (int i = 0; i < 4; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				LocalCode += chars[0].ToString();
				LocalCode += chars[1].ToString();
			}
		}

		//TmqCode
		public static string TmqCode { get; private set; }

		public static void InitTmqCode(ushort[] loadParams)
		{
			TmqCode = null;
			for (int i = 0; i < 4; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				TmqCode += chars[0].ToString();
				TmqCode += chars[1].ToString();
			}
		}

		//Leapsec 
		public static string Leapsec { get; private set; }

		public static void InitLeapsec(ushort[] loadParams)
		{
			Leapsec = null;
			for (int i = 0; i < 4; i++)
			{
				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[i]));
				Leapsec += chars[0].ToString();
				Leapsec += chars[1].ToString();
			}
		}
	}
}
