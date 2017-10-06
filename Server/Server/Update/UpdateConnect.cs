using System;
using System.Linq;
using System.Threading.Tasks;
using Server.DataClasses;

namespace Server.Update
{
    public static partial class UpdateDataObj
    {
        public static bool GetData(int currentIndex, out ushort addrGet, out ushort wordCount)
        {
			addrGet = ClassGetObjects[currentIndex].AddrObj;
			wordCount = (ushort)(ClassGetObjects[currentIndex].ByteObj >> 1);
			return true;
        }

        public static void UpdateDataGet(int currentIndex, ushort[] paramRtu)
        {
	        if (ClassGetObjects[currentIndex].TypeObj)  //Если дискретные 
	        {
		        UpdateD(currentIndex, paramRtu);
	        }
	        else
	        {
				UpdateA(currentIndex, paramRtu);
			}
		}

	    private static async void UpdateD(int index, ushort[] paramRtu)
	    {
		    ClassGetObjects[index].BitArray.GetBitArrayObj(paramRtu[0]);
			
			//Обновление
		    foreach (var itemDataClass in ClassGetObjects[index].DataClass)
		    {
				if (itemDataClass.DataObj.GetType() == typeof(SpsClass))
				{
					await UpdateSPS(index, itemDataClass);
				}
				else if (itemDataClass.DataObj.GetType() == typeof(ActClass))
				{
					await UpdateACT(index, itemDataClass);
				}
				else if (itemDataClass.DataObj.GetType() == typeof(SpcClass))
				{
					await UpdateSPC(index, itemDataClass);
				}
			}
	    }

	    private static async Task UpdateSPS(int index, DataObject itemDataClass)
	    {
		    var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);
		    ((SpsClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);

			Server.Server.UpdateDataGet(itemDataClass);
		}

	    private static async Task UpdateACT(int index, DataObject itemDataClass)
	    {
			var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);
		    ((ActClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);

			Server.Server.UpdateDataGet(itemDataClass);
	    }

	    private static async Task UpdateSPC(int index, DataObject itemDataClass)
	    {
		    var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);
		    ((SpcClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);

			Server.Server.UpdateDataGet(itemDataClass);
	    }

		private static async void UpdateA(int index, ushort[] paramRtu)
	    {
		    var itemDataClass = ClassGetObjects[index].DataClass.First();

		    if (itemDataClass.DataObj.GetType() == typeof(MvClass))
		    {
			    await UpdateMV(paramRtu, itemDataClass);
		    }
		    else if (itemDataClass.DataObj.GetType() == typeof(InsClass))
		    {
			    await UpdateINS(paramRtu, itemDataClass);
			}
		    else if (itemDataClass.DataObj.GetType() == typeof(BcrClass))
		    {
			    await UpdateBCR(paramRtu, itemDataClass);
			}
		    else if (itemDataClass.DataObj.GetType() == typeof(IncClass))
		    {
			    await UpdateINC(paramRtu, itemDataClass);
			}
		}
		
		private static async Task UpdateMV(ushort[] paramRtu, DataObject itemDataClass)
		{
			Int64 val = 0;

			for (int i = paramRtu.Length - 1; i >= 0; i--)
			{
				val += (long)paramRtu[i] << i * 16;
			}

			((MvClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, (ulong)val);

			Server.Server.UpdateDataGet(itemDataClass);
		}

	    private static async Task UpdateINS(ushort[] paramRtu, DataObject itemDataClass)
	    {
			Int32 val = 0;

		    for (int i = paramRtu.Length - 1; i >= 0; i--)
		    {
			    val += paramRtu[i] << i * 16;
		    }

		    ((InsClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);

			Server.Server.UpdateDataGet(itemDataClass);
	    }

	    private static async Task UpdateBCR(ushort[] paramRtu, DataObject itemDataClass)
	    {
			Int32 val = 0;

		    for (int i = paramRtu.Length - 1; i >= 0; i--)
		    {
			    val += paramRtu[i] << i * 16;
		    }

		    ((BcrClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);

			Server.Server.UpdateDataGet(itemDataClass);
	    }

	    private static async Task UpdateINC(ushort[] paramRtu, DataObject itemDataClass)
	    {
			Int32 val = 0;

		    for (int i = paramRtu.Length - 1; i >= 0; i--)
		    {
			    val += paramRtu[i] << i * 16;
		    }

		    ((IncClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);

			Server.Server.UpdateDataGet(itemDataClass);
	    }
	}
}