using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.ModBus
{
    public static class ScopeConfig
    {
        //Было сделано изменение конфигурации
        public static bool ChangeScopeConfig = false;

        //Скаченные параметры
        public static ushort[] LoadParams { get; set; }

        public static void SetLoadParamsBlock(ushort[] newPartLoadParams, int startIndex, int paramCount)
        {
            try
            {
                for (int i = 0; i < paramCount; i++)
                {
                    LoadParams[startIndex + i] = newPartLoadParams[i];
                }
            }
            catch
            {
                // ignored
            }
        }

        public static bool ConnectMcu { get; set; }

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

        //Предыстория 
        public static ushort HistoryCount { get; set; }

        //Делитель 
        public static ushort FreqCount { get; set; }

        //Режим работы 
        public static ushort OscilEnable { get; set; }

        //Размер осциллограммы 
        public static uint OscilSize { get; set; }

        //Статус осциллогрофа
        public static ushort StatusOscil { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public static bool Coincides { get; set; }
        
        //Адреса каналов 
        public static List<ushort> OscilAddr { get; set; }

        public static void InitOscilAddr(ushort[] loadParams)
        {
            OscilAddr.Clear();
            for (int i = 0; i < ChannelCount; i++)
            {
                OscilAddr.Add(loadParams[i]);
            }
        }

        //Формат каналов 
        public static List<ushort> OscilFormat { get; set; }

        public static void InitOscilFormat(ushort[] loadParams)
        {
            OscilFormat.Clear();
            for (int i = 0; i < ChannelCount; i++)
            {
                OscilFormat.Add(loadParams[i]);
            }
        }

        //Осциллографирумые параметры (получаем список параметров которые будем осциллогофировать)
        public static List<int> OscilParams { get; set; }

        //Проверка по адресу и формату 


        //Осциллограф включен
        public static bool ScopeEnabled = true;

        static ScopeConfig()
        {
            ChannelType = new List<ushort>();
            ChannelDemension = new List<string>();
            ChannelCcbm = new List<string>();
            ChannelPhase = new List<string>();
            ChannelName = new List<string>();
            OscilFormat = new List<ushort>();
            OscilParams = new List<int>();
            OscilAddr = new List<ushort>();
            LoadParams = new ushort[32];
        }

        //Дополнительные данные о каналах
        //Название канала
        public static List<string> ChannelName { get; set; }

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
        public static List<string> ChannelPhase { get; set; }

        public static void InitChannelPhase(ushort[] loadParams)
        {
            string сhannelPhase = "";

            char[] chars = Encoding.Default.GetChars(BitConverter.GetBytes(loadParams[0]));
            сhannelPhase += chars[0].ToString();
            сhannelPhase += chars[1].ToString();

            ChannelPhase.Add(сhannelPhase);
        }

        //CCBM
        public static List<string> ChannelCcbm { get; set; }

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
        public static List<string> ChannelDemension { get; set; }

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
        public static List<ushort> ChannelType { get; set; }

        public static void InitChannelType(ushort[] loadParams)
        {
            ChannelType.Add(loadParams[0]);
        }

        //Название станции
        public static string StationName { get; set; }

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
        public static string RecordingId { get; set; }

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
        public static string TimeCode { get; set; }

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
        public static string LocalCode { get; set; }

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
        public static string TmqCode { get; set; }

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
        public static string Leapsec { get; set; }

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
