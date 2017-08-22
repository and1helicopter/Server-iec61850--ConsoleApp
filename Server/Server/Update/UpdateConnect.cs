using System;
using Server.DataClasses;

namespace Server.Update
{
    public static partial class UpdateDataObj
    {
        public static bool GetData(int currentIndex, out ushort addrGet, out ushort b)
        {
            addrGet = DataClassGet[currentIndex].AddrDataObj;
            b = (ushort)(DataClassGet[currentIndex].ByteDataObj >> 1);
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
                Int64 val = 0;

                for (int i = paramRtu.Length - 1; i >= 0; i--)
                {
                    val += (long)paramRtu[i] << i * 16;
                }

                ((MvClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, (ulong)val);
            }
            else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(SpsClass))
            {
                Int64 temp = 0;

                for (int i = paramRtu.Length - 1; i >= 0; i--)
                {
                    temp += (long)paramRtu[i] << i * 16;
                }

                var val = (temp & (1 << Convert.ToInt32(DataClassGet[currentIndex].MaskDataObj))) > 0;

                ((SpsClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, val);
            }
            else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(InsClass))
            {
                Int32 val = 0;

                for (int i = paramRtu.Length - 1; i >= 0; i--)
                {
                    val += paramRtu[i] << i * 16;
                }

                ((InsClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, val);
            }
            else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(ActClass))
            {
	            Int64 temp = 0;

	            for (int i = paramRtu.Length - 1; i >= 0; i--)
	            {
		            temp += (long)paramRtu[i] << i * 16;
	            }

	            var val = (temp & (1 << Convert.ToInt32(DataClassGet[currentIndex].MaskDataObj))) > 0;

	            ((ActClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, val);
            }
		}
    }
}