using System;
using Server.Update;
using UniSerialPort;

namespace Server.ModBus
{
    public static partial class ModBus
    {
        private static void DataGetRequest()
        {
			if (UpdateDataObj.ClassGetObjects.Count != 0)
			{
				for (var i = 0; i < UpdateDataObj.ClassGetObjects.Count; i++)
				{
					if (UpdateDataObj.GetData(i, out ushort addrGet, out ushort wordCount))
					{
						lock (Locker)
						{
							SerialPort.GetDataRTU(addrGet, wordCount, UpdateDataGet, i);
						}
					}
				}
			}
		}

	    private static void UpdateDataGet(bool dataOk, ushort[] paramRtu, object param)
	    {
		    if (dataOk)
		    {
			    UpdateDataObj.UpdateDataGet(Convert.ToInt32(param), paramRtu);
		    }
	    }

		public static void DataSetRequest(int index, ushort[] value)
		{
			if (UpdateDataObj.GetData(index, out ushort addrSet, out ushort _))
			{
				lock (Locker)
				{
					SerialPort.SetDataRTU(addrSet, null, RequestPriority.Normal, null, value);
				}
			}
	    }
    }
}