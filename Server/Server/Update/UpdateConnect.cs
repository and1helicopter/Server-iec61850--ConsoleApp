using System;
using Server.DataClasses;

namespace Server.Update
{
    public static partial class UpdateDataObj
    {
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
                ((MvClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, Convert.ToInt64(paramRtu[0]));
                DataClassGet[currentIndex].GetDataObj_Set(false);
            }
            else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(SpsClass))
            {
                bool val = (Convert.ToInt32(paramRtu[0]) & 1 << Convert.ToInt32(DataClassGet[currentIndex].MaskDataObj)) > 0;
                ((SpsClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, val);
                DataClassGet[currentIndex].GetDataObj_Set(false);
            }
            else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(InsClass))
            {
                ((InsClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, paramRtu[0]);
                DataClassGet[currentIndex].GetDataObj_Set(false);
            }
        }
    }
}