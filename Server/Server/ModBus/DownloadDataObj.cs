using System;
using System.Linq;
using ServerLib.Update;
using UniSerialPort;

namespace ServerLib.ModBus
{
 public static partial class UpdateModBus
 {
		private static void DataGetRequest(ushort addrGet, ushort wordCount, UpdateDataObj.SourceClass item)
		{
			lock (Locker)
			{
				SerialPort.GetDataRTU(addrGet, wordCount, DataGetResponse, item);
			}
			
			if (item == UpdateDataObj.SourceList.First())
				_readAll = false;
		}

		private static void DataSetRequest(ushort addrSet, ushort[] value)
		{
			lock (Locker)
			{
				SerialPort.SetDataRTU(addrSet, null, RequestPriority.Normal, null, value);
			}
		}

		private static void DataGetResponse(bool dataOk, ushort[] paramRtu, object param)
		{
			if (dataOk)
			{
				((UpdateDataObj.SourceClass) param).GetValueResponse(paramRtu);
			}

			if (param == UpdateDataObj.SourceList.Last())
				_readAll = true;
		}
	}
}