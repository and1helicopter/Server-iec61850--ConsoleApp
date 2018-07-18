using System;
using System.Linq;
using System.Threading.Tasks;
using ServerLib.DataClasses;

namespace ServerLib.Update
{
	//public partial class UpdateDataObj
	//{
	//	public static bool GetData(int currentIndex, out ushort addrGet, out ushort wordCount)
	//	{
	//		addrGet = ClassGetObjects[currentIndex].AddrObj;
	//		wordCount = (ushort)(ClassGetObjects[currentIndex].ByteObj >> 1);
	//		return true;
	//	}

	//	public static bool GetData(ItemObject item, out ushort addrGet, out ushort wordCount)
	//	{
	//		addrGet = item.AddrObj;
	//		wordCount = (ushort)(item.ByteObj >> 1);
	//		return true;
	//	}

	//	public static void UpdateDataGet(int currentIndex, ushort[] paramRtu)
	//	{
	//		try
	//		{
	//			if (ClassGetObjects[currentIndex].TypeObj)  //Если дискретные 
	//			{
	//				UpdateD(currentIndex, paramRtu);
	//			}
	//			else
	//			{
	//				UpdateA(currentIndex, paramRtu);
	//			}
	//		}
	//		catch
	//		{
	//			// ignored
	//		}
	//	}

	//	private static async void UpdateD(int index, ushort[] paramRtu)
	//	{
	//		ClassGetObjects[index].BitArray.GetBitArrayObj(paramRtu[0]);

	//		try
	//		{
	//			foreach (var itemDataClass in ClassGetObjects[index].DataClass)
	//			{
	//				if (itemDataClass.DataObj.GetType() == typeof(SpsClass))
	//				{
	//					await UpdateSPS(index, itemDataClass);
	//				}
	//				else if (itemDataClass.DataObj.GetType() == typeof(ActClass))
	//				{
	//					await UpdateACT(index, itemDataClass);
	//				}
	//				else if (itemDataClass.DataObj.GetType() == typeof(SpcClass))
	//				{
	//					await UpdateSPC(index, itemDataClass);
	//				}
	//			}
	//		}
	//		catch
	//		{
	//			// ignored
	//		}
	//	}

	//	private static async void UpdateA(int index, ushort[] paramRtu)
	//	{
	//		var itemDataClass = ClassGetObjects[index].DataClass.First();

	//		try
	//		{
	//			if (itemDataClass.DataObj.GetType() == typeof(MvClass))
	//			{
	//				await UpdateMV(paramRtu, itemDataClass);
	//			}
	//			else if (itemDataClass.DataObj.GetType() == typeof(InsClass))
	//			{
	//				await UpdateINS(paramRtu, itemDataClass);
	//			}
	//			else if (itemDataClass.DataObj.GetType() == typeof(BcrClass))
	//			{
	//				await UpdateBCR(paramRtu, itemDataClass);
	//			}
	//			else if (itemDataClass.DataObj.GetType() == typeof(IncClass))
	//			{
	//				await UpdateINC(paramRtu, itemDataClass);
	//			}
	//		}
	//		catch
	//		{
	//			// ignored
	//		}
	//	}

	//	private static async Task UpdateSPS(int index, DataObject itemDataClass)
	//	{
	//		var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);

	//		itemDataClass.DataObj.UpdateClass(val);

	//		ServerIEC61850.ServerIEC61850.UpdateDataServer(itemDataClass);
	//	}

	//	private static async Task UpdateACT(int index, DataObject itemDataClass)
	//	{
	//		var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);

	//		itemDataClass.DataObj.UpdateClass(val);

	//		ServerIEC61850.ServerIEC61850.UpdateDataServer(itemDataClass);
	//	}

	//	private static async Task UpdateSPC(int index, DataObject itemDataClass)
	//	{
	//		var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);

	//		itemDataClass.DataObj.UpdateClass(val);

	//		ServerIEC61850.ServerIEC61850.UpdateDataServer(itemDataClass);
	//	}



	//	private static async Task UpdateMV(ushort[] paramRtu, DataObject itemDataClass)
	//	{
	//		Int64 val = 0;

	//		for (int i = paramRtu.Length - 1; i >= 0; i--)
	//		{
	//			val += (Int64)paramRtu[i] << i * 16;
	//		}

	//		itemDataClass.DataObj.UpdateClass(val);

	//		ServerIEC61850.ServerIEC61850.UpdateDataServer(itemDataClass);
	//	}

	//	private static async Task UpdateINS(ushort[] paramRtu, DataObject itemDataClass)
	//	{
	//		Int32 val = 0;

	//		for (int i = paramRtu.Length - 1; i >= 0; i--)
	//		{
	//			val += paramRtu[i] << i * 16;
	//		}

	//		itemDataClass.DataObj.UpdateClass(val);

	//		ServerIEC61850.ServerIEC61850.UpdateDataServer(itemDataClass);
	//	}

	//	private static async Task UpdateBCR(ushort[] paramRtu, DataObject itemDataClass)
	//	{
	//		Int32 val = 0;

	//		for (int i = paramRtu.Length - 1; i >= 0; i--)
	//		{
	//			val += paramRtu[i] << i * 16;
	//		}

	//		itemDataClass.DataObj.UpdateClass(val);

	//		ServerIEC61850.ServerIEC61850.UpdateDataServer(itemDataClass);
	//	}

	//	private static async Task UpdateINC(ushort[] paramRtu, DataObject itemDataClass)
	//	{
	//		Int32 val = 0;

	//		for (int i = paramRtu.Length - 1; i >= 0; i--)
	//		{
	//			val += paramRtu[i] << i * 16;
	//		}

	//		itemDataClass.DataObj.UpdateClass(val);

	//		ServerIEC61850.ServerIEC61850.UpdateDataServer(itemDataClass);
	//	}
	//}
}