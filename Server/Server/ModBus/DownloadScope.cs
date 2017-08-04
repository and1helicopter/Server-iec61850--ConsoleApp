using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Server.Format;
using UniSerialPort;

namespace Server.ModBus
{
	public static  partial class ModBus
	{
		private static void ScopoeRequest()
		{
			if (!ConfigDownloadScope.Enable || _waitingAnswer || !SerialPort.IsOpen)
			{
				return;
			}

			if (_startDownloadScope)
			{
				ScopeDownloadRequestSet();
			}
			else
			{
				if (_configScopeDownload)
				{
					ScopeStatusRequest();
				}
				else
				{
					ScopeConfigRequest();
				}
			}
		}

		private static void ScopeStatusRequest()
		{
			lock (Locker)
			{
				SerialPort.GetDataRTU((ushort)(ConfigDownloadScope.OscilCmndAddr + 8), 32,UpdateScopeStatus, null);
				_waitingAnswer = true;
			}
		}

		private static void UpdateScopeStatus(bool dataOk, ushort[] paramRtu, object param)
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
						_startDownloadScope = true;
						break;
					}
				}
			}
			_waitingAnswer = false;
		}

		private static uint _loadOscilTemp;
		private static uint _countTemp;
		private static int _loadOscDataStep;
		private static uint _startLoadSample;
		private static DateTime _stampDate;

		private static readonly List<ushort[]> DownloadedData = new List<ushort[]>();

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
						SerialPort.GetDataRTU((ushort)(ConfigDownloadScope.OscilCmndAddr + 72 + _indexDownloadScope * 2), 2, UpdateScopoe, null);
						_waitingAnswer = true;
					}
				}
					break;
				//Загрузка данных
				case 1:
				{
					uint oscilLoadTemp = (CalcOscilLoadTemp()) >> 5;

					lock (Locker)
					{
						SerialPort.GetDataRTU04((ushort)(oscilLoadTemp), 32, UpdateScopoe);
						_waitingAnswer = true;
					}
				}
					break;
			}
		}

		private static void UpdateScopoe(bool dataOk, ushort[] paramRtu, object param)
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
						DownloadedData.Add(new ushort[32]);
						for (int i = 0; i < 32; i++)
						{
							DownloadedData[DownloadedData.Count - 1][i] = paramRtu[i];
						}

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
			_waitingAnswer = false;
			ScopeDownloadRequestSet();
		}

		public static bool StatusLoad(out double countP)
		{
			countP = (double) 100 / (ScopeConfig.OscilSize >> 1) * _countTemp;
			return _startDownloadScope;
		}

		private static void SaveToFileRequest()
		{
			lock (Locker)
			{
				SerialPort.GetDataRTU((ushort)(ConfigDownloadScope.OscilCmndAddr + 136 + _indexDownloadScope * 6), 6, SaveToFile, null);
				_waitingAnswer = true;
			}
		}

		private static void SaveToFile(bool dataOk, ushort[] paramRtu, object param)
		{
			if (dataOk)
			{
				string str1 = (paramRtu[0] & 0x3F).ToString("X2") + "/" + ((paramRtu[0] >> 8) & 0x1F).ToString("X2") + @"/20" + (paramRtu[1] & 0xFF).ToString("X2");
				string str2 = (paramRtu[3] & 0x3F).ToString("X2") + ":" + ((paramRtu[2] >> 8) & 0x7F).ToString("X2") + @":" + (paramRtu[2] & 0x7F).ToString("X2");
				string str3 = ((paramRtu[4] * 1000) >> 8).ToString("D3") + @"000";
				string str = str1 + "," + str2 + @"." + str3;
				try
				{
					_stampDate = DateTime.Parse(str);
				}
				catch
				{
					// ignored
				}
				CreateFile();

				_startDownloadScope = false;

				if (ConfigDownloadScope.Remove)
				{
					SerialPort.SetDataRTU((ushort)(ConfigDownloadScope.OscilCmndAddr + 8 + _indexDownloadScope), null, RequestPriority.Normal, 0);
					OldStatus[_indexDownloadScope] = 0;
				}
			}
			_waitingAnswer = false;
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
				str = str + FormatConverter.GetValue(ulTemp, (byte)ScopeConfig.OscilFormat[i]) + "\t";
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
			foreach (ushort[] t in DownloadedData)
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
					str1 = FormatConverter.GetValue(ulTemp, (byte) ScopeConfig.OscilFormat[i]);
					str1 = str1.Replace(",", ".");
					str = str + "," + str1;
				}
			}
			for (i = 0; i < ScopeConfig.ChannelCount; i++)
			{

				if (ScopeConfig.ChannelType[i] == 1)
				{
					ulTemp = ParseArr(i, paramLine);
					str1 = FormatConverter.GetValue(ulTemp, (byte) ScopeConfig.OscilFormat[i]);
					str1 = str1.Replace(",", ".");
					str = str + "," + str1;
				}
			}
			return str;
		}

		private static string Line1(string rev)
		{
			string stationName = ScopeConfig.StationName;
			string recDevId = ScopeConfig.RecordingId;
			string revYear = rev;
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
			return ConfigDownloadScope.OscilNominalFrequency;
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

		private static string Line8()
		{
			//Время начала осциллограммы 
			double milsec = 1000 * (double)ScopeConfig.OscilHistCount / ((double)ScopeConfig.SampleRate / ScopeConfig.FreqCount);

			DateTime dateTemp = _stampDate.AddMilliseconds(-milsec);
			return dateTemp.ToString("dd'/'MM'/'yyyy,HH:mm:ss.fff000");
		}

		private static string Line9()
		{
			//Время срабатывания триггера
			DateTime dateTemp = _stampDate;
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

			DateTime time = DateTime.Now;

			if (ConfigDownloadScope.Type == "txt")
			{
				string writePath = "vmd-filestore\\" + ConfigDownloadScope.PathScope + "Scope_No" + _numName++ + "(" + time.ToLocalTime().ToString(CultureInfo.CurrentCulture).Replace(':', '.').Replace(' ', '_') + ").txt";

				StreamWriter sw = new StreamWriter(writePath, false, Encoding.GetEncoding("Windows-1251"));

				try
				{
					DateTime dateTemp = _stampDate;
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
			if (ConfigDownloadScope.Type != "txt")
			{
				
				string writePathCfg = "vmd-filestore\\" + ConfigDownloadScope.PathScope + "Scope_No" + _numName + "(" + time.ToLocalTime().ToString(CultureInfo.CurrentCulture).Replace(':','.').Replace(' ', '_') + ").cfg";

				StreamWriter swCfg = new StreamWriter(writePathCfg, false, Encoding.GetEncoding("Windows-1251"));

				try
				{
					swCfg.WriteLine(Line1(ConfigDownloadScope.ComtradeType));
					swCfg.WriteLine(Line2());

					for (int i = 0, j = 0; i < ScopeConfig.ChannelCount; i++)
					{
						if (ScopeConfig.ChannelType[i] == 0) { swCfg.WriteLine(Line3(i, j + 1)); j++; }
					}
					for (int i = 0, j = 0; i < ScopeConfig.ChannelCount; i++)
					{
						if (ScopeConfig.ChannelType[i] == 1) { swCfg.WriteLine(Line4(i, j + 1)); j++; }
					}

					swCfg.WriteLine(Line5());
					swCfg.WriteLine(Line6());
					swCfg.WriteLine(Line7());
					swCfg.WriteLine(Line8());
					swCfg.WriteLine(Line9());
					swCfg.WriteLine(Line10());
					swCfg.WriteLine(Line11());
					if (ConfigDownloadScope.ComtradeType == "2013")
					{
						swCfg.WriteLine(Line12());
						swCfg.WriteLine(Line13());
					}
				}
				catch
				{
					Console.WriteLine(@"Ошибка при записи в файл!");
					return;
				}

				swCfg.Close();

				string writePathDat = "vmd-filestore\\" + ConfigDownloadScope.PathScope + "Scope_No" + _numName++ + "(" + time.ToLocalTime().ToString(CultureInfo.CurrentCulture).Replace(':', '.').Replace(' ', '_') + ").dat";

				StreamWriter swDat = new StreamWriter(writePathDat, false, Encoding.GetEncoding("Windows-1251"));

				try
				{
					List<ushort[]> lud = InitParamsLines();
					for (int i = 0; i < lud.Count; i++)
					{
						swDat.WriteLine(FileParamLineData(lud[i], i));
					}
				}
				catch
				{
					Console.WriteLine(@"Ошибка при записи в файл!");
					return;
				}
				swDat.Close();

				DownloadedData.Clear();
			}
			#endregion
		}
	}

	public static class ConfigDownloadScope
	{
		public static bool Enable { get; private set; }
		public static bool Remove { get; private set; }
		public static string Type { get; private set; }
		public static string ComtradeType { get; private set; }
		public static ushort ConfigurationAddr { get; private set; }
		public static ushort OscilCmndAddr { get; private set; }
		public static string PathScope { get; private set; }
		public static string OscilNominalFrequency { get; private set; }

		private static void ChangeEnabale(bool enable)
		{
			Enable = enable;
		}

		private static void ChangeRemove(bool remove)
		{
			Remove = remove;
		}

		private static void ChangeType(string type)
		{
			switch (type.ToLower())
			{
				case "comtrade":
				{
					Type = "comtrade";
				}
					break;
				default:
				{
					Type = "txt";
				}
					break;
			}
		}

		private static void ChangeComtradeType(string comtradeType)
		{
			switch (comtradeType.ToLower())
			{
				case "2013":
				{
					ComtradeType = "2013";
				}
					break;
				default:
				{
					ComtradeType = "1999";
				}
					break;
			}
		}

		private static void ChangeConfigurationAddr(ushort configurationAddr)
		{
			ConfigurationAddr = configurationAddr;
		}

		private static void ChangeOscilCmndAddr(ushort oscilCmndAddr)
		{
			OscilCmndAddr = oscilCmndAddr;
		}

		private static void ChangePathScope(string pathScope)
		{
			//"vmd-filestore" + путь до папки где лежат осциллограммы
			PathScope = pathScope;
		}

		private static void ChangeOscilNominalFrequency(string oscilNominalFrequency)
		{
			OscilNominalFrequency = oscilNominalFrequency;
		}


		public static void InitConfigDownloadScope(string enabele, string remove, string type, string comtradeType, string configurationAddr, string oscilCmndAddr, string pathScope, string oscilNominalFrequency)
		{

			ChangeType(type);
			ChangeComtradeType(comtradeType);
			ChangePathScope(pathScope);
			ChangeOscilNominalFrequency(oscilNominalFrequency);

			try
			{
				ChangeEnabale(Convert.ToBoolean(enabele));
				ChangeRemove(Convert.ToBoolean(remove));
				ChangeConfigurationAddr(Convert.ToUInt16(configurationAddr));
				ChangeOscilCmndAddr(Convert.ToUInt16(oscilCmndAddr));
			}
			catch 
			{
				ChangeEnabale(false);
				Log.Log.Write("ModBus: DownloadScope.InitConfigDownloadScope finish with error - Scope = disable ", "Warning ");
			}

		}
	}
}