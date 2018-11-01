using UniSerialPort;

namespace ServerLib.ModBus
{
	public static partial class ModBus
	{
		private static void DataGetRequest(ushort addrGet, ushort wordCount, object param)
		{
			lock (Locker)
				SerialPort.GetDataRtu(addrGet, wordCount, DataGetResponse, param);
		}

		private static void DataGetRequest04(ushort addrGet, ushort wordCount, object param)
		{
			lock (Locker)
				SerialPort.GetDataRtu04(addrGet, wordCount, DataGetResponse, param);
		}


		private static void DataSetRequest(ushort addrSet, ushort[] value)
		{
			lock (Locker)
				SerialPort.SetDataRtu(addrSet, null, RequestPriority.Normal, null, value);
		}

		private static void DataGetResponse(bool dataOk, ushort[] paramRtu, dynamic param)
		{
			lock (Locker)
			{
				var item = param.Item;
				var response = param.Response;

				response(paramRtu, item, dataOk);
			}
		}
	}
}