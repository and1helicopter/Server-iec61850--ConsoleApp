using System;
using System.Linq;
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

	    private static void UpdateD(int index, ushort[] paramRtu)
	    {
		    ClassGetObjects[index].BitArray.GetBitArrayObj(paramRtu[0]);
			
			//Обновление
		    foreach (var itemDataClass in ClassGetObjects[index].DataClass)
		    {
				if (itemDataClass.DataObj.GetType() == typeof(SpsClass))
				{
					var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);
					((SpsClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);
			    }
				else if (itemDataClass.DataObj.GetType() == typeof(ActClass))
				{
					var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);
					((ActClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);			
				}
				else if (itemDataClass.DataObj.GetType() == typeof(SpcClass))
				{
					var val = ClassGetObjects[index].BitArray.BitArray.Get(itemDataClass.IndexDataOBj);
					((SpcClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);
				}

				Server.Server.UpdateDataGet(itemDataClass);
			}
	    }

	    private static void UpdateA(int index, ushort[] paramRtu)
	    {
		    var itemDataClass = ClassGetObjects[index].DataClass.First();

		    if (itemDataClass.DataObj.GetType() == typeof(MvClass))
		    {
				Int64 val = 0;

			    for (int i = paramRtu.Length - 1; i >= 0; i--)
			    {
				    val += (long)paramRtu[i] << i * 16;
			    }
				
				((MvClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, (ulong)val);
		    }
		    else if (itemDataClass.DataObj.GetType() == typeof(InsClass))
		    {
			    Int32 val = 0;

			    for (int i = paramRtu.Length - 1; i >= 0; i--)
			    {
				    val += paramRtu[i] << i * 16;
			    }

				((InsClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);
		    }
		    else if (itemDataClass.DataObj.GetType() == typeof(BcrClass))
		    {
			    Int32 val = 0;

			    for (int i = paramRtu.Length - 1; i >= 0; i--)
			    {
				    val += paramRtu[i] << i * 16;
			    }

			    ((BcrClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);
		    }
		    else if (itemDataClass.DataObj.GetType() == typeof(IncClass))
		    {
				Int32 val = 0;

			    for (int i = paramRtu.Length - 1; i >= 0; i--)
			    {
				    val += paramRtu[i] << i * 16;
			    }

			    ((IncClass)itemDataClass.DataObj).UpdateClass(DateTime.Now, val);
		    }

			Server.Server.UpdateDataGet(itemDataClass);
		}
	}
}