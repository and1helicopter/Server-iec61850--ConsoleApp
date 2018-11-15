using UniSerialPort;

namespace ServerLib.ModBus
{
	public static partial class ModBus
	{
		private static void DataGetRequest(ushort addrGet, ushort wordCount, object param)
		{
			lock (Locker)
				ModBusPort.GetInstance().GetDataRtu(addrGet, wordCount, DataGetResponse, param);
		}

		private static void DataGetRequest04(ushort addrGet, ushort wordCount, object param)
		{
			lock (Locker)
				ModBusPort.GetInstance().GetDataRtu04(addrGet, wordCount, DataGetResponse, param);
		}


		private static void DataSetRequest(ushort addrSet, ushort[] value)
		{
			lock (Locker)
				ModBusPort.GetInstance().SetDataRtu(addrSet, null, RequestPriority.Normal, null, value);
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