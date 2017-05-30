using System;
using System.Collections.Generic;

namespace Server.Update
{
    public static partial class UpdateDataObj
    {
        public static readonly List<DataObject> DataClassGet = new List<DataObject>();           //Список классов в которых данные получаем с платы 
        public static readonly List<DataObject> DataClassSet = new List<DataObject>();           //Список классов в которых данные загружаем на плату 

        public class DataObject
        {
            public string NameDataObj { get; }         //Путь до класса  

            public string FormatDataObj { get; }
            public ushort MaskDataObj { get; }
            public ushort AddrDataObj { get; }         //Адрес куда писать или откуда брать данные
            public string ClassDataObj { get; }

            public DateTime DateUpdateDataObj { get; }
            public long ValueDataObj { get; }

            public object DataObj { get; }

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
