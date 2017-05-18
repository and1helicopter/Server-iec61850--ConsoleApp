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

            public string FormatDataObj { get; private set; }
            public ushort MaskDataObj { get; private set; }
            public ushort AddrDataObj { get; private set; }         //Адрес куда писать или откуда брать данные
            public string ClassDataObj { get; private set; }

            public DateTime DateUpdateDataObj { get; private set; }
            public long ValueDataObj { get; private set; }

            public object DataObj { get; private set; }

            public bool GetDataObj { get; private set; }
            public bool SetDataObj { get; private set; }

            public DataObject(string name, string format, ushort mask, ushort addr,  string classType, object dataObj)
            {
                NameDataObj = name;
                AddrDataObj = addr;
                FormatDataObj = format;
                MaskDataObj = mask;
                ClassDataObj = classType;

                DateUpdateDataObj = DateTime.Now;
                ValueDataObj = 0;

                DataObj = dataObj;

                GetDataObj = false;
                SetDataObj = false;
            }

            public void GetDataObj_Set(bool getDataObj)
            {
                GetDataObj = getDataObj;
            }

            public void SetDataObj_Set(bool setDataObj)
            {
                SetDataObj = setDataObj;
            }
        }
    }
}
