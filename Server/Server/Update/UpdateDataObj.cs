using System;
using System.Collections.Generic;

namespace Server.Parser
{
    public static class UpdateDataObj
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

        public static bool GetData(int currentIndex, out ushort addrGet)
        {
            if (!DataClassGet[currentIndex].GetDataObj)
            {
                addrGet = DataClassGet[currentIndex].AddrDataObj;
                DataClassGet[currentIndex].GetDataObj_Set(true);
                return true;
            }

            addrGet = 0;
            return false;
        }

        public static bool SetData(int currentIndex, out ushort addrSet, out ushort send)
        {
            if (!DataClassGet[currentIndex].SetDataObj)
            {
                addrSet = DataClassSet[currentIndex].AddrDataObj;

                //В зависимости от класса и типа атрибута должно конвертироватся в 
                send = Convert.ToUInt16(DataClassSet[currentIndex].ValueDataObj);
                return true;
            }
            addrSet = 0;
            send = 0;
            return false;
        }

        public static void UpdateData(int currentIndex, ushort[] paramRtu)
        {
            if (DataClassGet[currentIndex].DataObj.GetType() == typeof(MvClass))
            {
                ((MvClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now,Convert.ToInt64(paramRtu[0]));
                DataClassGet[currentIndex].GetDataObj_Set(false);
            }
            else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(SpsClass))
            {
                bool val = (Convert.ToInt32(paramRtu[0]) & 1 << Convert.ToInt32(DataClassGet[currentIndex].MaskDataObj)) > 0;
                ((SpsClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, val);
                DataClassGet[currentIndex].GetDataObj_Set(false);
            }
        }
    }
}
