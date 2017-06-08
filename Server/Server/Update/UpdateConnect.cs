using System;
using Server.DataClasses;

namespace Server.Update
{
    public static partial class UpdateDataObj
    {
        public static bool GetData(int currentIndex, out ushort addrGet)
        {
            addrGet = DataClassGet[currentIndex].AddrDataObj;
            return true;
        }

        public static bool SetData(int currentIndex, out ushort addrSet, out ushort send)
        {
            addrSet = DataClassSet[currentIndex].AddrDataObj;

            //В зависимости от класса и типа атрибута должно конвертироватся в 
            send = Convert.ToUInt16(DataClassSet[currentIndex].ValueDataObj);
            return true;
        }

        public static void UpdateData(int currentIndex, ushort[] paramRtu)
        {
            if (DataClassGet[currentIndex].DataObj.GetType() == typeof(MvClass))
            {
                ((MvClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, Convert.ToInt64(paramRtu[0]));
            }
            else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(SpsClass))
            {
                bool val = (Convert.ToInt32(paramRtu[0]) & 1 << Convert.ToInt32(DataClassGet[currentIndex].MaskDataObj)) > 0;
                ((SpsClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, val);
            }
            else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(InsClass))
            {
                ((InsClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, paramRtu[0]);
            }
        }
    }
}