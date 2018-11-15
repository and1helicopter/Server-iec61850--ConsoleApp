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


namespace ServerLib.DownloadScope
{
	public static class DownloadScope
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
		/// <param name="timeWait">Time Wait for update scope config(msc)</param>
		/// <param name="timeWaitEnable">Enable update scope config</param>
		public static void InitConfigDownloadScope(string enabele, string remove, string type, string comtradeType, string configurationAddr, string oscilCmndAddr, string pathScope, string oscilNominalFrequency, string timeWait, string timeWaitEnable)
		{
			try
			{
				ConfigDownloadScope.InitConfigDownloadScope(enabele, remove, type, comtradeType, configurationAddr, oscilCmndAddr, pathScope, oscilNominalFrequency, timeWait, timeWaitEnable);
			}
			catch
			{
				Log.Log.Write(@"UpdateModBus: InitConfigDownloadScope finish with error", @"Warning");
			}
		}

		/// <summary>
		/// Initialize method load scope
		/// </summary>
		public static void InitMethodWork()
		{
			ModBusTaskController.ModBusTaskController.CycleClass.AddMethodWork(ReadScopeObjMethodWork.GetInstace());
		}



		private static readonly object LockRequest = new object();
		private static bool LockRead { get; set; }

		private class ReadScopeObjMethodWork : ModBusTaskController.ModBusTaskController.CycleClass.MethodWork
		{
			private static readonly ReadScopeObjMethodWork Instance = new ReadScopeObjMethodWork();

			public static ReadScopeObjMethodWork GetInstace()
			{
				return Instance;
			}

			internal override void Request(dynamic status)
			{
				var ready = (bool)status;
				lock (LockRequest)
					if (!LockRead)
						ready = true;
				if (ready) {
					ScopeDataLoad.GetInstance().Loading();
				}
			}

			internal override void Response(dynamic value, dynamic source, bool status)
			{

				source.GetValueResponse(value, source, status);
			}
		}

		public static void StopDownloadScope()
		{
			ScopeConfigLoad.GetInstance().Clear();
			ScopeStatusLoad.GetInstance().Clear();
			ScopeDataLoad.GetInstance().Clear();
			Scopes.GetInstance().Clear();
		}

		public delegate void ClassResponseHandler(dynamic value, dynamic param, bool status);

		public class ScopeConfigLoad
		{
			private static readonly ScopeConfigLoad Instance = new ScopeConfigLoad(ReadScopeObjMethodWork.GetInstace().Response);
			public static ScopeConfigLoad GetInstance()
			{
				return Instance;
			}

			private ScopeConfigLoad(ClassResponseHandler response)
			{
				Response = response;
			}

			private bool IsReady { get; set; }		//load full status
		///	private bool IsLoading { get; set; }	//process

			private int Step { get; set; }
			private int IndexChannel { get; set; }
			private ClassResponseHandler Response { get; set; }

			public bool Loading()
			{
				//Загрузка
				if (!IsReady)
				{
					GetValueRequest();
				}

				return IsReady;
			}

			public void Clear()
			{
				Step = IndexChannel = 0;
				IsReady = false;
			}

			private async void ReadTimeOutAsync()
			{
				await Task.Run(() =>
				{
					if (ConfigDownloadScope.TimeWaitEnable)
					{
						Thread.Sleep(ConfigDownloadScope.TimeWait);
						IsReady = false;
					}
				});
			}

			private void GetValueRequest()
			{
				lock (LockRequest) LockRead = true;

				switch (Step)
				{
					case 0:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilChNum), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response});
						break;
					case 1:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilQuantity), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 2:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilHistoryPercent), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 3:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilFreqDiv), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 4:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilEnable), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 5:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilSize), 2,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 6:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilSampleRate), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 7:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilMemorySize), 2,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 8:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilSampleSize), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 9:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilHistoryCount), 2,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 10:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilStatusLoad), 2,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 11:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilAddr), 32,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 12:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilTypeData), 32,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 13:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilChNumName + 16 * IndexChannel), 16,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 14:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Phase + 16 * IndexChannel), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 15:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Ccbm + 8 * IndexChannel), 8,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 16:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Demension + 4 * IndexChannel), 4,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 17:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Type + IndexChannel), 1,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 18:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.StationName), 16,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 19:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.RecordingId), 8,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 20:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.TimeCode), 4,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 21:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.LocalCode), 4,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 22:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.TmqCode), 4,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 23:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.ConfigurationAddr + StructAddr.OscilComtradeConfig + StructAddr.Leapsec), 4,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
				}
			}

			public void GetValueResponse(dynamic value, dynamic param, bool status)
			{
				if (!status)
				{
					lock (LockRequest) LockRead = false;
					return;
				}
				var step = param.Step;
				switch (step)
				{
					case 0: //Количество каналов 
						NewScopeConfig.ChannelCount = value[0];
						Step = 1;
						break;
					case 1: //Количество осциллограмм
						NewScopeConfig.ScopeCount = value[0];
						Step = 2;
						break;
					case 2: //Предыстория 
						NewScopeConfig.HistoryCount = value[0];
						Step = 3;
						break;
					case 3: //Делитель
						NewScopeConfig.FreqCount = value[0];
						Step = 4;
						break;
					case 4: //Режим работы
						NewScopeConfig.OscilEnable = value[0];
						Step = 5;
						break;
					case 5: //Размер осциллограммы 
						NewScopeConfig.OscilSize = value[1] << 16;
						NewScopeConfig.OscilSize += value[0];
						Step = 6;
						break;
					case 6: //Частота выборки
						NewScopeConfig.SampleRate = value[0];
						if (NewScopeConfig.SampleRate == 0) return;
						Step = 7;
						break;
					case 7: //Размер осциллограммы 
						NewScopeConfig.OscilAllSize = (uint)(value[1] << 16);
						NewScopeConfig.OscilAllSize += (value[0]);
						if (NewScopeConfig.SampleRate == 0) return;
						Step = 8;
						break;
					case 8: //Размер одной выборки
						NewScopeConfig.SampleSize = value[0];
						Step = 9;
						break;
					case 9: //Размер всей памяти 
						NewScopeConfig.OscilHistCount = (uint)(value[1] << 16);
						NewScopeConfig.OscilHistCount += value[0];
						Step = 10;
						break;
					case 10: //Статус осциллогрофа
						NewScopeConfig.StatusOscil = value[0];
						Step = 11;
						break;
					case 11: //Адреса каналов 
						NewScopeConfig.InitOscilAddr(value);
						Step = 12;
						break;
					case 12: //Формат каналов 
						NewScopeConfig.InitOscilFormat(value);
						Step = 13;
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
						break;
					case 18: //
						NewScopeConfig.InitStationName(value);
						Step = 19;
						break;
					case 19: //Названия каналов 
						NewScopeConfig.InitRecordingId(value);
						Step = 20;
						break;
					case 20: //Названия каналов 
						NewScopeConfig.InitTimeCode(value);
						Step = 21;
						break;
					case 21: //Названия каналов 
						NewScopeConfig.InitLocalCode(value);
						Step = 22;
						break;
					case 22: //Названия каналов 
						NewScopeConfig.InitTmqCode(value);
						Step = 23;
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

				lock (LockRequest) LockRead = false;

				void CheackNewConfig()
				{
					if (_currentScopeConfig == null || !Equals(_currentScopeConfig, NewScopeConfig))
					{
						_currentScopeConfig = NewScopeConfig;
					}

					IsReady = true;
					ReadTimeOutAsync();
				}
			} 
		}

		private class Scopes
		{
			private static Scopes _instance;

			public static Scopes GetInstance()
			{
				return _instance ?? (_instance = new Scopes());
			}

			private Scopes()
			{
				Scope = new List<CurrentScope>();
				var maxScopes = 32;
				for (int i = 0; i < maxScopes; i++)
				{
					Scope.Add(new CurrentScope());
				}
			}

			public List<CurrentScope> Scope;

			internal void Clear()
			{
				Scope?.Clear();
				Scope = null;
				_instance = null;
			}

			internal class CurrentScope
			{
				public int StatusCurrent { get; set; }
				public DateTime TimeCurrent { get; set; } = new DateTime();
				public bool LoadingCurrent { get; set; }
			}
		}

		public class ScopeStatusLoad
		{
			private static readonly ScopeStatusLoad Instance = new ScopeStatusLoad(ReadScopeObjMethodWork.GetInstace().Response);

			public static ScopeStatusLoad GetInstance()
			{
				return Instance;
			}

			public ScopeStatusLoad(ClassResponseHandler response)
			{
				Response = response;
			}

			private bool IsLoaded { get; set; }		//load config
			private bool IsReady { get; set; }		//load full status

			private int Step { get; set; }
			private int Index { get; set; }
			private int Count { get; set; }
			private ClassResponseHandler Response { get; set; }

			public bool Loading()
			{
				if (IsLoaded && !IsReady) 
				{
					GetValueRequest();
				}
				else 
				{
					IsLoaded = ScopeConfigLoad.GetInstance().Loading();
				}

				return IsReady;
			}

			public void Clear()
			{
				IsLoaded = IsReady = false;
				Step = Index = Count = 0;
			}

			private async void ReadTimeOutAsync()
			{
				await Task.Run(() =>
				{
					Thread.Sleep(10000);
					IsReady = false;
				});
			}

			private void GetValueRequest()
			{
				lock (LockRequest) LockRead = true;

				switch (Step)
				{
					case 0:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilStatus), 32,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					case 1:
						for (;Index < _currentScopeConfig.ScopeCount; Index++)
						{
							if (Scopes.GetInstance().Scope[Index].StatusCurrent >= 4)
							{
								ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilDateTime + Index * 4), 4,
									new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
								return;
							}
						}
						break;
				}
			}

			public void GetValueResponse(dynamic value, dynamic param, bool status)
			{
				if (status)
				{
					var step = param.Step;
					switch (step)
					{
						case 0:
							for (int i = 0; i < _currentScopeConfig.ScopeCount; i++)
							{
								Scopes.GetInstance().Scope[i].StatusCurrent = value[i];
							}
							Count = Scopes.GetInstance().Scope.Count(x => x.StatusCurrent >= 4);
							Step = 1;
							break;
						case 1:
							var index = param.Index;
							ushort[] val = value;
							string str1 = (val[0] & 0x3F).ToString("X2") + "/" + ((val[0] >> 8) & 0x1F).ToString("X2") + @"/20" + (val[1] & 0xFF).ToString("X2");
							string str2 = (val[3] & 0x3F).ToString("X2") + ":" + ((val[2] >> 8) & 0x7F).ToString("X2") + @":" + (val[2] & 0x7F).ToString("X2");
							string str3 = ((val[3] >> 6) & 0x3E7).ToString("D3");
							string str = $"{str1},{str2}.{str3}";
							Scopes.GetInstance().Scope[index].TimeCurrent = DateTime.Parse(str);
							Count--;
							Index++;
							if (Count == 0) CheackNewStatus();
							break;
					}
				}

				lock (LockRequest) LockRead = false;

				void CheackNewStatus()
				{
					IsReady = true;
					Index = Count = Step = 0;
					ReadTimeOutAsync();
				}
			}
		}

		public class ScopeDataLoad
		{
			private static readonly ScopeDataLoad Instance = new ScopeDataLoad(ReadScopeObjMethodWork.GetInstace().Response);
			public static ScopeDataLoad GetInstance()
			{
				return Instance;
			}

			private ScopeDataLoad(ClassResponseHandler response)
			{
				Response = response;
			}

			private int Step { get; set; }
			private int LoadOscNum { get; set; }
			private ClassResponseHandler Response { get; set; }

			private int CountTemp { get; set; }
			private int LoadOscilTemp { get; set; }
			private int OscilStartTemp { get; set; }
			private int StartLoadSample { get; set; }
			private int LoadOscilIndex { get; set; }
			private List<ushort[]> _downloadedData = new List<ushort[]>();
			private ushort[] _loadParamPart = new ushort[32];

			private bool Free { get; set; }

			private bool IsLoaded { get; set; }     //load status
			//private bool IsLoading { get; set; }    //process
			//private bool IsReady { get; set; }      //load full status

			public void Loading()
			{
				if(Free)
					GetValueRequest();
				else if (ScopeStatusLoad.GetInstance().Loading()) CheackFree();
			}

			public void Clear()
			{
				Step = LoadOscNum = 0;
				CountTemp = LoadOscilTemp = OscilStartTemp = StartLoadSample = LoadOscilIndex = 0;
				_downloadedData.Clear();
				Free = IsLoaded = false;
			}

			private void GetValueRequest()
			{
				lock (LockRequest) LockRead = true;

				switch (Step)
				{
					//Загрузка номера выборки на котором заканчивается осциллограмма 
					case 0:
						ModBus.ModBus.GetRequest((ushort)(ConfigDownloadScope.OscilCmndAddr + StructAddr.OscilEnd + LoadOscNum * 2), 2, 
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = Response });
						break;
					//Загрузка данных
					case 1:
						int oscilLoadTemp = CalcOscilLoadTemp() >> 5;
						ModBus.ModBus.GetRequest04((ushort) (oscilLoadTemp), 32,
							new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj {Item = this, Response = Response });
						break;
				}

				int CalcOscilLoadTemp()
				{
					if (CountTemp < (_currentScopeConfig.OscilSize >> 1))                               //Проход по осциллограмме 
					{
						LoadOscilTemp += 32;                                                    //Какую часть осциллограммы грузим 
						CountTemp += 32;
					}
					return (LoadOscilTemp - 32 + OscilStartTemp);                               //+Положение относительно начала массива
				}
			}

			public void GetValueResponse(dynamic value, dynamic param, bool status)
			{
				if (status)
				{
					ushort[] val;
					var step = param.Step;
					switch (step)
					{
						//Загрузка стартового адреса
						case 0:
							val = value;
							StartLoadSample = val[1] << 16;
							StartLoadSample += val[0];
							LoadOscilIndex = 0;
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
							LoadOscilIndex = (2 * CountTemp * 1000) / _currentScopeConfig.OscilSize;

							if (CountTemp >= (_currentScopeConfig.OscilSize >> 1))
							{
								Step = 0;
								//_loadOscilIndex = 0;
								LoadOscilTemp = 0;
								CountTemp = 0;

								//Начать сохранение в файл
								try
								{
									var postfix = ConfigDownloadScope.Type == "txt" ? "txt" : "cfg";
									var namefile =
										$"Scope №{LoadOscNum + 1} Time {Scopes.GetInstance().Scope[LoadOscNum].TimeCurrent:dd.MM.yyyy hh.mm.ss.fff}";
									var path =
										$"{ServerIEC61850.ServerConfig.NamePathDirectory}\\{ServerIEC61850.ServerConfig.NameDirectoryServer}\\{ConfigDownloadScope.PathScope}";

									CreateFile(postfix, namefile, path);

									Scopes.GetInstance().Scope[LoadOscNum].LoadingCurrent = true;
								}
								catch
								{
									Scopes.GetInstance().Scope[LoadOscNum].LoadingCurrent = false;
								}
								finally
								{
									_downloadedData.Clear();
									Free = false;
								}
							}
							break;
					}
				}
				
				lock (LockRequest) LockRead = false;
			}

			private void CheackFree()
			{
				if (Scopes.GetInstance().Scope.Any(x => x.StatusCurrent == 4 && x.LoadingCurrent == false))
				{
					var currentScope = Scopes.GetInstance().Scope.First(x => x.StatusCurrent == 4 && x.LoadingCurrent == false);
					var indexCurrentScope = Scopes.GetInstance().Scope.IndexOf(currentScope);
					LoadOscNum = indexCurrentScope;
					Free = true;
				}
			}

			//СОЗДАНИЕ ФАЙЛА
			// Save to .txt
			#region

			private string FileHeaderLine()
			{
				string str = " " + "\t";
				for (int i = 0; i < _currentScopeConfig.ChannelCount; i++)
				{
					str = str + _currentScopeConfig.ChannelName[i].Substring(0, 29).Replace("\0", String.Empty) + "\t";
				}
				return str;
			}

			private string FileHeaderLineColor()
			{
				string str = " " + "\t";
				for (int i = 0; i < _currentScopeConfig.ChannelCount; i++)
				{
					var name = Encoding.Default.GetBytes(_currentScopeConfig.ChannelName[i].Substring(29, 3));
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
				for (i = 0, _count64 = 0, _count32 = 0, _count16 = 0; i < _currentScopeConfig.ChannelCount; i++)
				{
					var ulTemp = ParseArr(i, paramLine);
					str = str + FormatConverter.GetValue(ulTemp, (byte)_currentScopeConfig.OscilFormat[i]) + "\t";
				}
				return str;
			}
			#endregion

			private ulong ParseArr(int i, ushort[] paramLine)
			{
				ulong ulTemp = 0;
				if (_currentScopeConfig.OscilFormat[i] >> 8 == 3)
				{
					ulTemp = 0;
					ulTemp += (ulong)(paramLine[_count64 + 0]) << 8 * 2;
					ulTemp += (ulong)(paramLine[_count64 + 1]) << 8 * 3;
					ulTemp += (ulong)(paramLine[_count64 + 2]) << 8 * 0;
					ulTemp += (ulong)(paramLine[_count64 + 3]) << 8 * 1;
					_count64 += 4;
				}
				if (_currentScopeConfig.OscilFormat[i] >> 8 == 2)
				{
					ulTemp = 0;
					ulTemp += (ulong)(paramLine[_count64 + _count32 + 0]) << 8 * 0;
					ulTemp += (ulong)(paramLine[_count64 + _count32 + 1]) << 8 * 1;
					_count32 += 2;
				}
				if (_currentScopeConfig.OscilFormat[i] >> 8 == 1)
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
						if (k == 0) paramsLines.Add(new ushort[_currentScopeConfig.SampleSize >> 1]);
						while (k < (_currentScopeConfig.SampleSize >> 1) && j < 32)
						{
							paramsLines[paramsLines.Count - 1][k] = t[j];
							k++;
							j++;
						}
						if (k == (_currentScopeConfig.SampleSize >> 1)) k = 0;
					}
					j = 0;
				}
				// paramsLines.RemoveAt(paramsLines.Count-1);
				//Формирую список начиная с предыстории 
				for (int i = 0; i < paramsLines.Count; i++)
				{
					if ((i + (int)StartLoadSample + 1) >= paramsLines.Count)
					{
						paramsSortLines.Add(new ushort[_currentScopeConfig.SampleSize >> 1]);
						paramsSortLines[i] = paramsLines[l];
						l++;
					}
					else
					{
						paramsSortLines.Add(new ushort[_currentScopeConfig.SampleSize >> 1]);
						paramsSortLines[i] = paramsLines[i + (int)StartLoadSample + 1];
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
				for (i = 0; i < _currentScopeConfig.ChannelCount; i++)
				{
					if (_currentScopeConfig.ChannelType[i] == 0)
					{
						ulTemp = ParseArr(i, paramLine);
						str1 = FormatConverter.GetValue(ulTemp, (byte)_currentScopeConfig.OscilFormat[i]);
						str1 = str1.Replace(",", ".");
						str = str + "," + str1;
					}
				}
				for (i = 0; i < _currentScopeConfig.ChannelCount; i++)
				{

					if (_currentScopeConfig.ChannelType[i] == 1)
					{
						ulTemp = ParseArr(i, paramLine);
						str1 = FormatConverter.GetValue(ulTemp, (byte)_currentScopeConfig.OscilFormat[i]);
						str1 = str1.Replace(",", ".");
						str = str + "," + str1;
					}
				}
				return str;
			}

			private string Line1(string revYear)
			{
				string stationName = _currentScopeConfig.StationName;
				string recDevId = _currentScopeConfig.RecordingId;
				string str = stationName + "," + recDevId + "," + revYear;
				return str;
			}

			private string Line2()
			{
				int nA = 0, nD = 0;
				for (int i = 0; i < _currentScopeConfig.ChannelCount; i++)
				{
					//Если параметр в списке известных, то пишем его в файл
					if (_currentScopeConfig.ChannelType[i] == 0) nA += 1;
					if (_currentScopeConfig.ChannelType[i] == 1) nD += 1;
				}
				int tt = nA + nD;
				string str = tt + "," + nA + "A," + nD + "D";
				return str;
			}

			private string Line3(int num, int nA)
			{
				string chId = _currentScopeConfig.ChannelName[num].Substring(0, 29).Replace("\0", String.Empty);
				string ph = _currentScopeConfig.ChannelPhase[num];
				string ccbm = _currentScopeConfig.ChannelCcbm[num];
				string uu = _currentScopeConfig.ChannelDemension[num];
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
				string chId = _currentScopeConfig.ChannelName[num];
				string ph = _currentScopeConfig.ChannelPhase[num];
				string ccbm = _currentScopeConfig.ChannelCcbm[num];
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
				string samp = Convert.ToString(_currentScopeConfig.SampleRate / _currentScopeConfig.FreqCount);
				samp = samp.Replace(",", ".");
				string endsamp = InitParamsLines().Count.ToString();
				string str = samp + "," + endsamp;
				return str;
			}

			private string Line8(int numOsc)
			{
				//Время начала осциллограммы 
				double milsec = 1000 * (double)_currentScopeConfig.OscilHistCount / ((double)_currentScopeConfig.SampleRate / _currentScopeConfig.FreqCount);

				DateTime dateTemp = Scopes.GetInstance().Scope[numOsc].TimeCurrent.AddMilliseconds(-milsec);
				return dateTemp.ToString("dd'/'MM'/'yyyy,HH:mm:ss.fff000");
			}

			private string Line9(int numOsc)
			{
				//Время срабатывания триггера
				DateTime dateTemp = Scopes.GetInstance().Scope[numOsc].TimeCurrent;
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
				string timecode = _currentScopeConfig.TimeCode;
				string localcode = _currentScopeConfig.LocalCode;
				return timecode + "," + localcode;
			}

			private string Line13()
			{
				string tmqCode = _currentScopeConfig.TmqCode;
				string leapsec = _currentScopeConfig.Leapsec;
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
							DateTime dateTemp = Scopes.GetInstance().Scope[LoadOscNum].TimeCurrent;
							sw.WriteLine(dateTemp.ToString("dd'/'MM'/'yyyy HH:mm:ss.fff000"));                  //Штамп времени
							sw.WriteLine(Convert.ToString(_currentScopeConfig.SampleRate / _currentScopeConfig.FreqCount));     //Частота выборки (частота запуска осциллогрофа/ делитель)
							sw.WriteLine(_currentScopeConfig.OscilHistCount);                                           //Предыстория 
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

							for (int i = 0, j = 0; i < _currentScopeConfig.ChannelCount; i++)
							{
								if (_currentScopeConfig.ChannelType[i] == 0)
								{
									sw.WriteLine(Line3(i, j + 1));
									j++;
								}
							}

							for (int i = 0, j = 0; i < _currentScopeConfig.ChannelCount; i++)
							{
								if (_currentScopeConfig.ChannelType[i] == 1)
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
							try
							{
								List<ushort[]> lud = InitParamsLines();
								for (int i = 0; i < lud.Count; i++)
								{
									sw.WriteLine(FileParamLineData(lud[i], i));
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
					}
					catch
					{
						//
					}

				}
				#endregion
			}
		}

		private static ScopeConfig _currentScopeConfig;
		private static readonly ScopeConfig NewScopeConfig = new ScopeConfig();

		private class ScopeConfig
		{
			//Частота выборки без делителя
			public ushort SampleRate { get; set; }

			//Размер всей памяти 
			public uint OscilAllSize { get; set; }

			//Размер выборки
			public ushort SampleSize { get; set; }

			//Количество выборок в предыстории 
			public uint HistoryCount { get; set; }

			//Количество осциллограмм 
			public ushort ScopeCount { get; set; }

			//Количество каналов
			public ushort ChannelCount { get; set; }

			//Делитель 
			public ushort FreqCount { get; set; }

			//Размер осциллограммы 
			public int OscilSize { get; set; }

			//Режим работы 
			public ushort OscilEnable { get; set; }

			//Количество выборок в предыстории 
			public uint OscilHistCount { get; set; }

			//Статус осциллогрофа
			public ushort StatusOscil { get; set; }

			//Адреса каналов 
			// ReSharper disable once MemberCanBePrivate.Global
			// ReSharper disable once CollectionNeverQueried.Global
			public List<ushort> OscilAddr { get; }

			public void InitOscilAddr(ushort[] loadParams)
			{
				OscilAddr.Clear();
				for (int i = 0; i < ChannelCount; i++)
				{
					OscilAddr.Add(loadParams[i]);
				}
			}

			//Формат каналов 
			public List<ushort> OscilFormat { get; }

			public void InitOscilFormat(ushort[] loadParams)
			{
				OscilFormat.Clear();
				for (int i = 0; i < ChannelCount; i++)
				{
					OscilFormat.Add(loadParams[i]);
				}
			}

			public ScopeConfig()
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
			public List<string> ChannelName { get; }

			public void InitChannelName(ushort[] loadParams)
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
			public List<string> ChannelPhase { get; }

			public void InitChannelPhase(ushort[] loadParams)
			{
				string сhannelPhase = "";

				char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[0]));
				сhannelPhase += chars[0].ToString();
				сhannelPhase += chars[1].ToString();

				ChannelPhase.Add(сhannelPhase);
			}

			//CCBM
			public List<string> ChannelCcbm { get; }

			public void InitChannelCcbm(ushort[] loadParams)
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
			public List<string> ChannelDemension { get; }

			public void InitChannelDemension(ushort[] loadParams)
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
			public List<ushort> ChannelType { get; }

			public void InitChannelType(ushort[] loadParams)
			{
				ChannelType.Add(loadParams[0]);
			}

			//Название станции
			public string StationName { get; private set; }

			public void InitStationName(ushort[] loadParams)
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
			public string RecordingId { get; private set; }

			public void InitRecordingId(ushort[] loadParams)
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
			public string TimeCode { get; private set; }

			public void InitTimeCode(ushort[] loadParams)
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
			public string LocalCode { get; private set; }

			public void InitLocalCode(ushort[] loadParams)
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
			public string TmqCode { get; private set; }

			public void InitTmqCode(ushort[] loadParams)
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
			public string Leapsec { get; private set; }

			public void InitLeapsec(ushort[] loadParams)
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
}