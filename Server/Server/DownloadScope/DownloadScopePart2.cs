using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using ServerLib.Update;

namespace ServerLib.DownloadScope
{
	public static partial class DownloadScope
	{
		public static ScopeConfig CurrentScopeConfig = null;

		public static ScopeConfig NewScopeConfig = null;

		public class ScopeConfig
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
			public uint OscilSize { get; set; }

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
