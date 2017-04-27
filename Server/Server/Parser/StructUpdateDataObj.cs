using System;
using System.Collections.Generic;
using System.Data;

namespace Server.Parser
{
    public static class StructUpdateDataObj
    {
        public static List<DataObject> DataClassGet = new List<DataObject>();           //Список классов в которых данные получаем с платы 
        public static List<DataObject> DataClassSet = new List<DataObject>();           //Список классов в которых данные загружаем на плату 

        public class DataObject
        {
            public string NameDataObj { get; private set; }         //Путь до класса  
            public ushort AddrDataObj { get; private set; }         //Адрес куда писать или откуда брать данные
            public string TypeDataObj { get; private set; }
            public string MaskDataObj { get; private set; }
            public string ConvertDataObj { get; private set; }
            public string ClassDataObj { get; private set; }

            public DateTime DateUpdateDataObj { get; private set; }
            public long ValueDataObj { get; private set; }

            public object DataObj;

            public bool GetDataObj { get; private set; }
            public bool SetDataObj { get; private set; }

            public void UpdateDataObj(string value, DateTime time)
            {
                DateUpdateDataObj = time;
                ValueDataObj = Convert.ToInt64(value);
            }

            
        }
    }
}
