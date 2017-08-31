using System;
using Server.DataClasses;

namespace Server.Update
{
    public static partial class UpdateDataObj
    {
        public static bool GetData(int currentIndex, out ushort addrGet, out ushort wordCount)
        {
            addrGet = DataClassGet[currentIndex].AddrDataObj;
	        wordCount = (ushort)(DataClassGet[currentIndex].ByteDataObj >> 1);
            return true;
        }

        public static bool SetData(int currentIndex, out ushort addrSet, out ushort wordCount)
        {
            addrSet = DataClassSet[currentIndex].AddrDataObj;
	        wordCount = (ushort)(DataClassSet[currentIndex].ByteDataObj >> 1);
            return true;
        }

        public static void UpdateDataGet(int currentIndex, ushort[] paramRtu)
        {
			#region Классы общих данных для информации о состоянии
	        if (DataClassGet[currentIndex].DataObj.GetType() == typeof(SpsClass))
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
	        else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(BcrClass))
	        {
		        Int32 val = 0;

		        for (int i = paramRtu.Length - 1; i >= 0; i--)
		        {
			        val += paramRtu[i] << i * 16;
		        }

		        ((BcrClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, val);
	        }
			#endregion
			
			#region  Классы общих данных для информации об измеряемой величине
			else if (DataClassGet[currentIndex].DataObj.GetType() == typeof(MvClass))
            {
	            Int64 val = 0;

	            for (int i = paramRtu.Length - 1; i >= 0; i--)
	            {
		            val += (long)paramRtu[i] << i * 16;
	            }

	            ((MvClass)DataClassGet[currentIndex].DataObj).UpdateClass(DateTime.Now, (ulong)val);
            }
			#endregion
		}

	    public static void UpdateDataSet(int currentIndex, ushort[] paramRtu)
	    {
			#region Классы общих данных для информации о состоянии
			if (DataClassSet[currentIndex].DataObj.GetType() == typeof(SpcClass))
	        {
		        Int64 temp = 0;

		        for (int i = paramRtu.Length - 1; i >= 0; i--)
		        {
			        temp += (long)paramRtu[i] << i * 16;
		        }

		        var val = (temp & (1 << Convert.ToInt32(DataClassSet[currentIndex].MaskDataObj))) > 0;

		        ((SpcClass)DataClassSet[currentIndex].DataObj).UpdateClass(DateTime.Now, val);
	        }
			#endregion
		}
	}
}