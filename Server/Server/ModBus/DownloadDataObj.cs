using System;
using System.Linq;
using ServerLib.Update;
using UniSerialPort;

namespace ServerLib.ModBus
{
 public static partial class UpdateModBus
 {
		private static void DataGetRequest()
  {
			//if (UpdateDataObj.ClassGetObjects.Count != 0)
			//{
			//	for (var i = 0; i < UpdateDataObj.ClassGetObjects.Count; i++)
			//	{
			//		if (UpdateDataObj.GetData(i, out ushort addrGet, out ushort wordCount))
			//		{
			//			lock (Locker)
			//			{
			//				SerialPort.GetDataRTU(addrGet, wordCount, UpdateDataGet, i);
			//			}
			//		}
			//	}

			//	//foreach (var item in UpdateDataObj.ClassGetObjects)
			//	//{
			//	//	if (UpdateDataObj.GetData(item, out ushort addrGet, out ushort wordCount))
			//	//	{
			//	//		lock (Locker)
			//	//		{
			//	//			SerialPort.GetDataRTU(addrGet, wordCount, UpdateDataGetItem, item);
			//	//		}
			//	//	}
			//	//}
			//}
		}

		//   private static void UpdateDataGetItem(bool dataOk, ushort[] paramrtu, object param)
		//   {
		// if (dataOk)
		// {

		// }
		//	}

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

		//private static void UpdateDataGet(bool dataOk, ushort[] paramRtu, object param)
		//{
		//	if (dataOk)
		//	{
		//		UpdateDataObj.UpdateDataGet(Convert.ToInt32(param), paramRtu);
		//	}
		//}

		//public static void DataSetRequest(int index, ushort[] value)
		//{
		//	if (UpdateDataObj.GetData(index, out ushort addrSet, out ushort _))
		//	{
		//		lock (Locker)
		//		{
		//			SerialPort.SetDataRTU(addrSet, null, RequestPriority.Normal, null, value);
		//		}
		//	}
		//}
	}
}