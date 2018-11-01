using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeApp;
using ServerLib.Format;
using ServerLib.Server;
using ServerLib.Update;


namespace ServerLib.DownloadScope
{
	public static  partial class DownloadScope
	{
		/// <summary>
		/// Initialize Download Scope
		/// </summary>
		/// <param name="enabele">Enable download scope</param>
		/// <param name="remove">Remove scope after download</param>
		/// <param name="type">Type saveing scope</param>
		/// <param name="comtradeType">Type cometrade format if select</param>
		/// <param name="configurationAddr">Address configuration fild</param>
		/// <param name="oscilCmndAddr">Address command fild</param>
		/// <param name="pathScope">Name of directory where downloads scope</param>
		/// <param name="oscilNominalFrequency">Nominal frequency scoping</param>
		public static void InitConfigDownloadScope(string enabele, string remove, string type, string comtradeType, string configurationAddr, string oscilCmndAddr, string pathScope, string oscilNominalFrequency)
		{
			try
			{
				ConfigDownloadScope.InitConfigDownloadScope(enabele, remove, type, comtradeType, configurationAddr, oscilCmndAddr, pathScope, oscilNominalFrequency);
			}
			catch
			{
				Log.Log.Write(@"UpdateModBus: InitConfigDownloadScope finish with error", @"Warning");
			}
		}

		/// <summary>
		/// Initialize method update data
		/// </summary>
//		public static void InitMethodWork()
//		{
//			if (ConfigDownloadScope.Enable)
//				UpdateClass.CycleClass.AddMethodWork(UpdateReadDataObjMethodWork);
//		}
//
//		//public static 
//
//		private static readonly ReadDataObjMethodWork UpdateReadDataObjMethodWork = new ReadDataObjMethodWork();
//
//		private class ReadDataObjMethodWork : UpdateClass.CycleClass.MethodWork
//		{
//			internal override void Request(dynamic status)
//			{
//				var ready = (bool)status;
//				if (_configStart)
//				{
//					_configStart = false;
//					_configLoading = true;
//					ConfigLoadObj.GetValueRequest(Response);
//				}
//				else if (_statusStart)
//				{
//					_statusStart = false;
//					_statusLoading = true;
//					StatusLoadObj.GetValueRequest(Response);
//				}
//				else if (_dataStart)
//				{
//					_dataStart = false;
//					_dataLoading = true;
//					DataLoadObj.StartLoad(Response);
//				}
//			}
//
//			internal override void Response(dynamic value, dynamic source, bool status)
//			{
//				source.GetValueResponse(value, source, status);
//			}
//		}

		/// <summary>
		/// Start cheack existing scopes
		/// </summary>
		public static void StartThreadCheack()
		{
			for(int i = 0; i < 32; i++) Scopes.Add(new CurrentScope());
			Cycle(Token);
		}

		/// <summary>
		/// Stop cheack existing scopes
		/// </summary>
		public static void StopThreadCheack()
		{
			Cts.Cancel();
			Scopes.Clear();
		}

		//private static ThreadPool _taskThread;

		private static volatile bool _changeConfig;

		private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
		private static readonly CancellationToken Token = Cts.Token;

		private static void Cycle(CancellationToken token)
		{
			ReadConfiguration(token);
			ReadStatus(token);
			ReadData(token);
		}

		//Read config from mcu
//		private static volatile bool _configStart;
		private static volatile bool _configLoading;
		private static volatile bool _configLoaded;

		private static async void ReadConfiguration(CancellationToken token)
		{
			await Task.Run((Action) ReadConfig, token);
		}

		
		private static void ReadConfig()
		{
			do
			{
				lock (Lock)
				{
					if (!_configLoading && !_statusLoading && !_dataLoading)
					{
						_configLoading = true;
						ConfigLoadObj.GetValueRequest();
					}
				}

				if (ConfigDownloadScope.TimeWaitEnable) Thread.Sleep(ConfigDownloadScope.TimeWait);
			} while (ConfigDownloadScope.TimeWaitEnable);
		}

		//private static void Response(dynamic value, dynamic source, bool status)
		//{
		//	source.GetValueResponse(value, source, status);
		//}

		private static readonly ConfigLoad ConfigLoadObj = new ConfigLoad();

		public delegate void ClassResponseHandler(dynamic value, dynamic param, bool status);

		public class ConfigLoad
		{
			public ConfigLoad()
			{
				Response = GetValueResponse;
			}

			private int Step { get; set; }
			private int IndexChannel { get; set; }
			private ClassResponseHandler Response { get; set; }


			public void GetValueRequest()
			{
				if (NewScopeConfig == null)
				{
					NewScopeConfig = new ScopeConfig();
				}
				switch (Step)
				{
					case 0:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilChNum), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response});
						break;
					case 1:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilQuantity), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 2:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilHistoryPercent), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 3:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilFreqDiv), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 4:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilEnable), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 5:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilSize), 2,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 6:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilSampleRate), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 7:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilMemorySize), 2,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 8:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilSampleSize), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 9:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilHistoryCount), 2,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 10:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilStatusLoad), 2,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 11:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilAddr), 32,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 12:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilTypeData), 32,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 13:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilChNumName + 16 * IndexChannel), 16,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 14:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Phase + 16 * IndexChannel), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 15:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Ccbm + 8 * IndexChannel), 8,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 16:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Demension + 4 * IndexChannel), 4,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 17:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Type + IndexChannel), 1,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 18:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.StationName), 16,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 19:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.RecordingId), 8,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 20:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.TimeCode), 4,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 21:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.LocalCode), 4,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 22:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.TmqCode), 4,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 23:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Leapsec), 4,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
				}

			}

			public void GetValueResponse(dynamic value, dynamic param, bool status)
			{
				if (!status)
				{
					Thread.Sleep(100);
					GetValueRequest();
					return;
				}
				var step = param.Step;
				switch (step)
				{
					case 0: //Количество каналов 
						NewScopeConfig.ChannelCount = value[0];
						Step = 1;
						GetValueRequest();
						break;
					case 1: //Количество осциллограмм
						NewScopeConfig.ScopeCount = value[0];
						Step = 2;
						GetValueRequest();
						break;
					case 2: //Предыстория 
						NewScopeConfig.HistoryCount = value[0];
						Step = 3;
						GetValueRequest();
						break;
					case 3: //Делитель
						NewScopeConfig.FreqCount = value[0];
						Step = 4;
						GetValueRequest();
						break;
					case 4: //Режим работы
						NewScopeConfig.OscilEnable = value[0];
						Step = 5;
						GetValueRequest();
						break;
					case 5: //Размер осциллограммы 
						NewScopeConfig.OscilSize = (uint)(value[1] << 16);
						NewScopeConfig.OscilSize += value[0];
						Step = 6;
						GetValueRequest();
						break;
					case 6: //Частота выборки
						NewScopeConfig.SampleRate = value[0];
						if (NewScopeConfig.SampleRate == 0) return;
						Step = 7;
						GetValueRequest();
						break;
					case 7: //Размер осциллограммы 
						NewScopeConfig.OscilAllSize = (uint)(value[1] << 16);
						NewScopeConfig.OscilAllSize += (value[0]);
						if (NewScopeConfig.SampleRate == 0) return;
						Step = 8;
						GetValueRequest();
						break;
					case 8: //Размер одной выборки
						NewScopeConfig.SampleSize = value[0];
						Step = 9;
						GetValueRequest();
						break;
					case 9: //Размер всей памяти 
						NewScopeConfig.OscilHistCount = (uint)(value[1] << 16);
						NewScopeConfig.OscilHistCount += value[0];
						Step = 10;
						GetValueRequest();
						break;
					case 10: //Статус осциллогрофа
						NewScopeConfig.StatusOscil = value[0];
						Step = 11;
						GetValueRequest();
						break;
					case 11: //Адреса каналов 
						NewScopeConfig.InitOscilAddr(value);
						Step = 12;
						GetValueRequest();
						break;
					case 12: //Формат каналов 
						NewScopeConfig.InitOscilFormat(value);
						Step = 13;
						GetValueRequest();
						break;
					case 13: //Названия каналов 
						if (NewScopeConfig.ChannelCount == 0) break;//Если в системе нет конфигурации
						if (IndexChannel == 0) NewScopeConfig.ChannelName.Clear();
						NewScopeConfig.InitChannelName(value);
						if (IndexChannel == NewScopeConfig.ChannelCount - 1)
						{
							IndexChannel = 0;
							Step = 14;
						}
						else
						{
							IndexChannel++;
						}
						GetValueRequest();
						break;
					case 14: //Названия каналов 
						if (IndexChannel == 0) NewScopeConfig.ChannelPhase.Clear();
						NewScopeConfig.InitChannelPhase(value);
						if (IndexChannel == NewScopeConfig.ChannelCount - 1)
						{
							IndexChannel = 0;
							Step = 15;
						}
						else
						{
							IndexChannel++;
						}
						GetValueRequest();
						break;
					case 15: //Названия каналов 
						if (IndexChannel == 0) NewScopeConfig.ChannelCcbm.Clear();
						NewScopeConfig.InitChannelCcbm(value);
						if (IndexChannel == NewScopeConfig.ChannelCount - 1)
						{
							IndexChannel = 0;
							Step = 16;
						}
						else
						{
							IndexChannel++;
						}
						GetValueRequest();
						break;
					case 16: //Названия каналов 
						if (IndexChannel == 0) NewScopeConfig.ChannelDemension.Clear();
						NewScopeConfig.InitChannelDemension(value);
						if (IndexChannel == NewScopeConfig.ChannelCount - 1)
						{
							IndexChannel = 0;
							Step = 17;
						}
						else
						{
							IndexChannel++;
						}
						GetValueRequest();
						break;
					case 17: //Названия каналов 
						if (IndexChannel == 0) NewScopeConfig.ChannelType.Clear();
						NewScopeConfig.InitChannelType(value);
						if (IndexChannel == NewScopeConfig.ChannelCount - 1)
						{
							IndexChannel = 0;
							Step = 18;
						}
						else
						{
							IndexChannel++;
						}
						GetValueRequest();
						break;
					case 18: //
						NewScopeConfig.InitStationName(value);
						Step = 19;
						GetValueRequest();
						break;
					case 19: //Названия каналов 
						NewScopeConfig.InitRecordingId(value);
						Step = 20;
						GetValueRequest();
						break;
					case 20: //Названия каналов 
						NewScopeConfig.InitTimeCode(value);
						Step = 21;
						GetValueRequest();
						break;
					case 21: //Названия каналов 
						NewScopeConfig.InitLocalCode(value);
						Step = 22;
						GetValueRequest();
						break;
					case 22: //Названия каналов 
						NewScopeConfig.InitTmqCode(value);
						Step = 23;
						GetValueRequest();
						break;
					case 23: //Названия каналов 
						NewScopeConfig.InitLeapsec(value);
						CheackNewConfig();
						Step = 0;
						break;
					default:
						Step = 0;
						return;
				}
			}

			private void CheackNewConfig()
			{
				if (CurrentScopeConfig == null || !Equals(CurrentScopeConfig, NewScopeConfig))
				{
					CurrentScopeConfig = NewScopeConfig;
					_changeConfig = true;
					_configLoading = false;
					_configLoaded = true;
				}
				else
				{
					_changeConfig = false;
					_configLoading = false;
				}
			}
		}

		//Read status scope 
		private static volatile bool _statusLoading;
		private static volatile bool _statusLoaded;
//		private static volatile bool _statusStart;

		private static List<CurrentScope> Scopes = new List<CurrentScope>();//.Initialize(x => x = new CurrentScope());


		public class CurrentScope
		{
			public int StatusCurrent { get; set; } = 0;
			public DateTime TimeCurrent { get; set; } = new DateTime();
			public bool LoadingCurrent { get; set; } = false;
		}

		private static async void ReadStatus(CancellationToken token)
		{
			await Task.Run((Action)ReadStatus, token);
		}

		private static readonly object Lock = new object();

		private static void ReadStatus()
		{
			do
			{
				lock (Lock)
				{
					if (!_configLoading && !_statusLoading && !_dataLoading && _configLoaded)
					{
						_statusLoading = true;
						StatusLoadObj.GetValueRequest();
					}
				}

				Thread.Sleep(5500);
			} while (true);
		}

		private static readonly StatusLoad StatusLoadObj = new StatusLoad();

		public class StatusLoad
		{
			public StatusLoad()
			{
				Response = GetValueResponse;
			}

			private int Step { get; set; }
			private int Index { get; set; }
			private int Count { get; set; }

			private ClassResponseHandler Response { get; set; }

			public void GetValueRequest()
			{
				switch (Step)
				{
					case 0:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilStatus), 32,
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 1:
						for (;Index < CurrentScopeConfig.ScopeCount; Index++)
						{
							if (Scopes[Index].StatusCurrent >= 4)
							{
								ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilDateTime + Index * 4), 4,
									new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
								return;
							}
						}
						break;
				}
			}

			public void GetValueResponse(dynamic value, dynamic param, bool status)
			{
				if (!status)
				{
					GetValueRequest();
					return;
				}
				var step = param.Step;
				switch (step)
				{
					case 0:
						for (int i = 0; i < CurrentScopeConfig.ScopeCount; i++)
						{
							Scopes[i].StatusCurrent = value[i];
						}
						Count = Scopes.Count(x => x.StatusCurrent >= 4);
						Step = 1;
						GetValueRequest();
						break;
					case 1:
						var index = param.Index;
						ushort[] val = value;
						string str1 = (val[0] & 0x3F).ToString("X2") + "/" + ((val[0] >> 8) & 0x1F).ToString("X2") + @"/20" + (val[1] & 0xFF).ToString("X2");
						string str2 = (val[3] & 0x3F).ToString("X2") + ":" + ((val[2] >> 8) & 0x7F).ToString("X2") + @":" + (val[2] & 0x7F).ToString("X2");
						string str3 = ((val[3] >> 6) & 0x3E7).ToString("D3");
						string str = $"{str1},{str2}.{str3}";
						Scopes[index].TimeCurrent = DateTime.Parse(str);
						Count--;
						Index++;
						if (Count == 0) CheackNewStatus();
						else GetValueRequest();
						break;
				}
			}

			private void CheackNewStatus()
			{
				_statusLoading = false;
				_statusLoaded = true;
				Index = 0;
				Step = 0;
			}
		}

		//Read data scope
		private static volatile bool _dataLoading;
		private static volatile bool _dataLoaded = false;

		private static async void ReadData(CancellationToken token)
		{
			await Task.Run((Action)ReadData, token);
		}

		private static void ReadData()
		{
			do
			{
				lock (Lock)
				{
					if (!_configLoading && !_statusLoading && !_dataLoading && _configLoaded && _statusLoaded)
					{
						_dataLoading = true;
						DataLoadObj.StartLoad();
					}
				}

				Thread.Sleep(1000);
			} while (true);
		}

		private static readonly DataLoad DataLoadObj = new DataLoad();
		private static FileStream fs;
		private static StreamWriter sw;

		public class DataLoad
		{
			public DataLoad()
			{
				Response = GetValueResponse;
			}

			private int Step { get; set; }
			private ClassResponseHandler Response { get; set; }

			private int LoadOscNum { get; set; }
			private uint _countTemp { get; set; }
			private uint _loadOscilTemp { get; set; }
			private uint _oscilStartTemp { get; set; }
			private uint _startLoadSample { get; set; }
			private uint _loadOscilIndex { get; set; }
			private List<ushort[]> _downloadedData = new List<ushort[]>();
			private ushort[] _loadParamPart = new ushort[32];

			private bool free { get; set; }


			public void StartLoad()
			{
				CheackFree();
				if (free) GetValueRequest();
			}

			public void GetValueRequest()
			{
				switch (Step)
				{
					//Загрузка номера выборки на котором заканчивается осциллограмма 
					case 0:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilEnd + LoadOscNum * 2), 2, 
							new UpdateClass.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					//Загрузка данных
					case 1:
						uint oscilLoadTemp = (CalcOscilLoadTemp()) >> 5;
						ModBus.ModBus.GetRequest04((ushort) (oscilLoadTemp), 32,
							new UpdateClass.CycleClass.ResponseObj {Item = this, Response = Response });
						break;
				}

				uint CalcOscilLoadTemp()
				{
					if (_countTemp < (CurrentScopeConfig.OscilSize >> 1))                               //Проход по осциллограмме 
					{
						_loadOscilTemp += 32;                                                    //Какую часть осциллограммы грузим 
						_countTemp += 32;
					}
					return (_loadOscilTemp - 32 + _oscilStartTemp);                               //+Положение относительно начала массива
				}
			}

			public void GetValueResponse(dynamic value, dynamic param, bool status)
			{
				if (!status)
				{
					Thread.Sleep(100);
					GetValueRequest();
					return;
				}

				ushort[] val;
				var step = param.Step;
				switch (step)
				{
					//Загрузка стартового адреса
					case 0:
						val = value;
						_startLoadSample = (uint)(val[1] << 16);
						_startLoadSample += val[0];
						_loadOscilIndex = 0;
						Step = 1;
						break;
					case 1:
						val = value;
						for (int i = 0; i < 32; i++)
						{
							_loadParamPart[i] = val[i];
						}
						_downloadedData.Add(new ushort[32]);
						for (int i = 0; i < 32; i++)
						{
							_downloadedData[_downloadedData.Count - 1][i] = _loadParamPart[i];
						}
						_loadOscilIndex = (2 * _countTemp * 1000) / CurrentScopeConfig.OscilSize;

						if (_countTemp >= (CurrentScopeConfig.OscilSize >> 1))
						{
							Step = 0;
							//_loadOscilIndex = 0;
							_loadOscilTemp = 0;
							_countTemp = 0;

							//Начать сохранение в файл
							try
							{
								var postfix = ConfigDownloadScope.Type == "txt" ? "txt" : "cfg";
								var namefile =
									$"Scope №{LoadOscNum + 1} Time {Scopes[LoadOscNum].TimeCurrent:dd.MM.yyyy hh.mm.ss.fff}";
								var path =
									$"{ServerIEC61850.ServerConfig.NamePathDirectory}\\{ServerIEC61850.ServerConfig.NameDirectoryServer}\\{ConfigDownloadScope.PathScope}";

								CreateFile(postfix, namefile, path);

								Scopes[LoadOscNum].LoadingCurrent = true;
							}
							catch
							{
								Scopes[LoadOscNum].LoadingCurrent = false;
							}
							finally
							{

								_downloadedData.Clear();
								_dataLoading = false;
								free = false;
							}

							return;
						}
						break;
				}
				GetValueRequest();
			}

			private void CheackFree()
			{
				if (Scopes.Any(x => x.StatusCurrent == 4 && x.LoadingCurrent == false))
				{
					var currentScope = Scopes.First(x => x.StatusCurrent == 4 && x.LoadingCurrent == false);
					var indexCurrentScope = Scopes.IndexOf(currentScope);
					LoadOscNum = indexCurrentScope;
					free = true;
				}
			}

			//СОЗДАНИЕ ФАЙЛА
			// Save to .txt
			#region

			private string FileHeaderLine()
			{
				string str = " " + "\t";
				for (int i = 0; i < CurrentScopeConfig.ChannelCount; i++)
				{
					str = str + CurrentScopeConfig.ChannelName[i].Substring(0, 29).Replace("\0", String.Empty) + "\t";
				}
				return str;
			}

			private string FileHeaderLineColor()
			{
				string str = " " + "\t";
				for (int i = 0; i < CurrentScopeConfig.ChannelCount; i++)
				{
					var name = Encoding.Default.GetBytes(CurrentScopeConfig.ChannelName[i].Substring(29, 3));
					str = str + BitConverter.ToString(name).Replace("-", string.Empty).ToLower() + "\t";
				}
				return str;
			}

			private int _count64, _count32, _count16;

			/*
			 ushort Format 
			 Contain:
			 (byte)							|(byte)
			 Bit depth index		|Format index   			 
			*/

			private string FileParamLine(ushort[] paramLine, int lineNum)
			{
				int i;
				// ChFormats();
				string str = lineNum + "\t";
				for (i = 0, _count64 = 0, _count32 = 0, _count16 = 0; i < CurrentScopeConfig.ChannelCount; i++)
				{
					var ulTemp = ParseArr(i, paramLine);
					str = str + FormatConverter.GetValue(ulTemp, (byte)CurrentScopeConfig.OscilFormat[i]) + "\t";
				}
				return str;
			}
			#endregion

			private ulong ParseArr(int i, ushort[] paramLine)
			{
				ulong ulTemp = 0;
				if (CurrentScopeConfig.OscilFormat[i] >> 8 == 3)
				{
					ulTemp = 0;
					ulTemp += (ulong)(paramLine[_count64 + 0]) << 8 * 2;
					ulTemp += (ulong)(paramLine[_count64 + 1]) << 8 * 3;
					ulTemp += (ulong)(paramLine[_count64 + 2]) << 8 * 0;
					ulTemp += (ulong)(paramLine[_count64 + 3]) << 8 * 1;
					_count64 += 4;
				}
				if (CurrentScopeConfig.OscilFormat[i] >> 8 == 2)
				{
					ulTemp = 0;
					ulTemp += (ulong)(paramLine[_count64 + _count32 + 0]) << 8 * 0;
					ulTemp += (ulong)(paramLine[_count64 + _count32 + 1]) << 8 * 1;
					_count32 += 2;
				}
				if (CurrentScopeConfig.OscilFormat[i] >> 8 == 1)
				{
					ulTemp = paramLine[_count64 + _count32 + _count16];
					_count16 += 1;
				}
				return ulTemp;
			}

			//Формирование строк всех загруженных данных
			private List<ushort[]> InitParamsLines()
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
						if (k == 0) paramsLines.Add(new ushort[CurrentScopeConfig.SampleSize >> 1]);
						while (k < (CurrentScopeConfig.SampleSize >> 1) && j < 32)
						{
							paramsLines[paramsLines.Count - 1][k] = t[j];
							k++;
							j++;
						}
						if (k == (CurrentScopeConfig.SampleSize >> 1)) k = 0;
					}
					j = 0;
				}
				// paramsLines.RemoveAt(paramsLines.Count-1);
				//Формирую список начиная с предыстории 
				for (int i = 0; i < paramsLines.Count; i++)
				{
					if ((i + (int)_startLoadSample + 1) >= paramsLines.Count)
					{
						paramsSortLines.Add(new ushort[CurrentScopeConfig.SampleSize >> 1]);
						paramsSortLines[i] = paramsLines[l];
						l++;
					}
					else
					{
						paramsSortLines.Add(new ushort[CurrentScopeConfig.SampleSize >> 1]);
						paramsSortLines[i] = paramsLines[i + (int)_startLoadSample + 1];
					}
				}
				return paramsSortLines;
			}

			//Save to cometrade
			#region

			private string FileParamLineData(ushort[] paramLine, int lineNum)
			{
				string str1;
				int i;
				ulong ulTemp;
				_count64 = 0;
				_count32 = 0;
				_count16 = 0;
				string str = (lineNum + 1) + ",";
				for (i = 0; i < CurrentScopeConfig.ChannelCount; i++)
				{
					if (CurrentScopeConfig.ChannelType[i] == 0)
					{
						ulTemp = ParseArr(i, paramLine);
						str1 = FormatConverter.GetValue(ulTemp, (byte)CurrentScopeConfig.OscilFormat[i]);
						str1 = str1.Replace(",", ".");
						str = str + "," + str1;
					}
				}
				for (i = 0; i < CurrentScopeConfig.ChannelCount; i++)
				{

					if (CurrentScopeConfig.ChannelType[i] == 1)
					{
						ulTemp = ParseArr(i, paramLine);
						str1 = FormatConverter.GetValue(ulTemp, (byte)CurrentScopeConfig.OscilFormat[i]);
						str1 = str1.Replace(",", ".");
						str = str + "," + str1;
					}
				}
				return str;
			}

			private string Line1(string revYear)
			{
				string stationName = CurrentScopeConfig.StationName;
				string recDevId = CurrentScopeConfig.RecordingId;
				string str = stationName + "," + recDevId + "," + revYear;
				return str;
			}

			private string Line2()
			{
				int nA = 0, nD = 0;
				for (int i = 0; i < CurrentScopeConfig.ChannelCount; i++)
				{
					//Если параметр в списке известных, то пишем его в файл
					if (CurrentScopeConfig.ChannelType[i] == 0) nA += 1;
					if (CurrentScopeConfig.ChannelType[i] == 1) nD += 1;
				}
				int tt = nA + nD;
				string str = tt + "," + nA + "A," + nD + "D";
				return str;
			}

			private string Line3(int num, int nA)
			{
				string chId = CurrentScopeConfig.ChannelName[num].Substring(0, 29).Replace("\0", String.Empty);
				string ph = CurrentScopeConfig.ChannelPhase[num];
				string ccbm = CurrentScopeConfig.ChannelCcbm[num];
				string uu = CurrentScopeConfig.ChannelDemension[num];
				string a = "1";
				string b = "0";
				int skew = 0;
				double min = 0;
				double max = 0;

				int primary = 1;
				int secondary = 1;
				string ps = "P";

				string str = nA + "," + chId + "," + ph + "," + ccbm + "," + uu + "," + a + "," + b + "," + skew + "," +
							 min + "," + max + "," + primary + "," + secondary + "," + ps;

				return str;
			}

			private string Line4(int num, int nD)
			{
				string chId = CurrentScopeConfig.ChannelName[num];
				string ph = CurrentScopeConfig.ChannelPhase[num];
				string ccbm = CurrentScopeConfig.ChannelCcbm[num];
				int y = 0;

				string str = nD + "," + chId + "," + ph + "," + ccbm + "," + y;

				return str;
			}

			private string Line5()
			{
				string str;
				try
				{
					str = ConfigDownloadScope.OscilNominalFrequency;
				}
				catch
				{
					str = "50";
				}

				return str;
			}

			private string Line6()
			{
				string nrates = "1";
				return nrates;
			}

			private string Line7()
			{
				string samp = Convert.ToString(CurrentScopeConfig.SampleRate / CurrentScopeConfig.FreqCount);
				samp = samp.Replace(",", ".");
				string endsamp = InitParamsLines().Count.ToString();
				string str = samp + "," + endsamp;
				return str;
			}

			private string Line8(int numOsc)
			{
				//Время начала осциллограммы 
				double milsec = 1000 * (double)CurrentScopeConfig.OscilHistCount / ((double)CurrentScopeConfig.SampleRate / CurrentScopeConfig.FreqCount);

				DateTime dateTemp = Scopes[numOsc].TimeCurrent.AddMilliseconds(-milsec);
				return dateTemp.ToString("dd'/'MM'/'yyyy,HH:mm:ss.fff000");
			}

			private string Line9(int numOsc)
			{
				//Время срабатывания триггера
				DateTime dateTemp = Scopes[numOsc].TimeCurrent;
				return dateTemp.ToString("dd'/'MM'/'yyyy,HH:mm:ss.fff000");
			}

			private string Line10()
			{
				string ft = "ASCII";
				return ft;
			}

			private string Line11()
			{
				string timemult = "1";
				return timemult;
			}

			private string Line12()
			{
				string timecode = CurrentScopeConfig.TimeCode;
				string localcode = CurrentScopeConfig.LocalCode;
				return timecode + "," + localcode;
			}

			private string Line13()
			{
				string tmqCode = CurrentScopeConfig.TmqCode;
				string leapsec = CurrentScopeConfig.Leapsec;
				return tmqCode + "," + leapsec;
			}
			#endregion//Save to cometrade

			private void CreateFile(string postfix, string nameFile, string namePath)
			{
				// Save to .txt
				#region 
				if (postfix == "txt")
				{
					var path = $"{namePath}\\{nameFile}.{postfix}";
					using (StreamWriter sw = new StreamWriter(path, false))
					{
						try
						{
							DateTime dateTemp = Scopes[LoadOscNum].TimeCurrent;
							sw.WriteLine(dateTemp.ToString("dd'/'MM'/'yyyy HH:mm:ss.fff000"));                  //Штамп времени
							sw.WriteLine(Convert.ToString(CurrentScopeConfig.SampleRate / CurrentScopeConfig.FreqCount));     //Частота выборки (частота запуска осциллогрофа/ делитель)
							sw.WriteLine(CurrentScopeConfig.OscilHistCount);                                           //Предыстория 
							sw.WriteLine(FileHeaderLine());                                                     //Формирование заголовка (подписи названия каналов)
							sw.WriteLine(FileHeaderLineColor());

							List<ushort[]> lu = InitParamsLines();                                              //Формирование строк всех загруженных данных (отсортированых с предысторией)
							for (int i = 0; i < lu.Count; i++)
							{
								sw.WriteLine(FileParamLine(lu[i], i));
							}
						}
						catch
						{
							// ignored
						}
						finally
						{
							sw.Close();
						}
					}
				}
				#endregion

				// Save to COMETRADE
				#region

				if (postfix == "cfg")
				{
					var path = $"{namePath}\\{nameFile}.{postfix}";
					using (StreamWriter sw = new StreamWriter(path, false))
					{
						try
						{
							sw.WriteLine(Line1(ConfigDownloadScope.ComtradeType));
							sw.WriteLine(Line2());

							for (int i = 0, j = 0; i < CurrentScopeConfig.ChannelCount; i++)
							{
								if (CurrentScopeConfig.ChannelType[i] == 0)
								{
									sw.WriteLine(Line3(i, j + 1));
									j++;
								}
							}

							for (int i = 0, j = 0; i < CurrentScopeConfig.ChannelCount; i++)
							{
								if (CurrentScopeConfig.ChannelType[i] == 1)
								{
									sw.WriteLine(Line4(i, j + 1));
									j++;
								}
							}

							sw.WriteLine(Line5());
							sw.WriteLine(Line6());
							sw.WriteLine(Line7());
							sw.WriteLine(Line8(LoadOscNum));
							sw.WriteLine(Line9(LoadOscNum));
							sw.WriteLine(Line10());
							sw.WriteLine(Line11());

							if (ConfigDownloadScope.ComtradeType == "2013")
							{
								sw.WriteLine(Line12());
								sw.WriteLine(Line13());
							}
						}
						catch
						{
							//
						}
						finally
						{
							sw.Close();
						}
					}

					try
					{
						string pathDateFile = $"{namePath}\\{nameFile}.dat";

						using (StreamWriter sw = new StreamWriter(pathDateFile, false))
						{
							List<ushort[]> lud = InitParamsLines();
							for (int i = 0; i < lud.Count; i++)
							{
								sw.WriteLine(FileParamLineData(lud[i], i));
							}
						}
					}
					catch
					{
						//
					}
					finally
					{
						sw.Close();
					}
				}
				#endregion
			}
		}
	}
}