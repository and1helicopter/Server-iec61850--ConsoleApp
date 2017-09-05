using System;
using Server.Update;
using UniSerialPort;

namespace Server.ModBus
{
    public static partial class ModBus
    {
        private static void DataRequest()
        {
            #region Get
            //if (UpdateDataObj.DataClassGet.Count != 0)
            //{
            //    for (var i = 0; i < UpdateDataObj.DataClassGet.Count; i++)
            //    {
            //        if (UpdateDataObj.GetData(i, out ushort addrGet, out ushort wordCount))
            //        {
            //            lock (Locker)
            //            {
            //                SerialPort.GetDataRTU(addrGet, wordCount, UpdateDataGet, i);
            //            }
            //        }
            //    }
            //}
			#endregion

			#region Set
	   //     if (UpdateDataObj.DataClassSet.Count != 0)
	   //     {
		  //      for (var i = 0; i < UpdateDataObj.DataClassSet.Count; i++)
			 //   {
				//	if (UpdateDataObj.SetData(i, out ushort addrSet, out ushort wordCount))
				//	{
				//		lock (Locker)
				//		{
				//			SerialPort.GetDataRTU(addrSet, wordCount, UpdateDataSet, i);
				//		}
				//	}
				//}
	   //     }
	        #endregion
		}

	    private class ParamSet
	    {
		    public ushort[] Value { get; }
			public int Index { get; }

		    public ParamSet(ushort[] value, int index)
		    {
			    Value = value;
			    Index = index;
		    }
		}

		public static void DataSetRequest(int index, ushort[] value)
		{
			var param = new ParamSet(value, index);
			if (UpdateDataObj.SetData(index, out ushort addrSet, out ushort wordCount))
			{
				lock (Locker)
				{
					SerialPort.GetDataRTU(addrSet, wordCount, DataSetResponse, param);
				}
			}
		}

	    private static void DataSetResponse(bool dataOk, ushort[] paramRtu, object param)
	    {
		    if (dataOk)
		    {
				var index = ((ParamSet)param).Index;
			    var value = ((ParamSet)param).Value;

			   // switch (UpdateDataObj.DataClassSet[index].ClassDataObj)
			   // {
				  //  case @"SPC":
					 //   SetSPC(index, value, paramRtu);
						//break;
				  //  case @"INC":
					 //   SetINC(index, value);
					 //   break;
				  //  case @"APC":
					 //   break;
			   // }
			}
		}

	    private static void SetSPC(int index, ushort[] value, ushort[] paramRtu)
	    {

		    //ushort invPosindex = (ushort)~(1 << UpdateDataObj.DataClassSet[index].MaskDataObj);		//Маска сбрасываемого бита 
		    //ushort tempVal = (ushort)(value[0] > 0 ? (1 << UpdateDataObj.DataClassSet[index].MaskDataObj) : 0);
		    //ushort answer = (ushort )((paramRtu[0] & invPosindex) + tempVal);


			//if (UpdateDataObj.SetData(index, out ushort addrSet, out ushort _))
		 //   {
			//    lock (Locker)
			//    {
			//	    SerialPort.SetDataRTU(addrSet, null, RequestPriority.High, null, answer);
			//    }
		 //   }
		}

	    private static void SetINC(int index, ushort[] value)
	    {
		    ushort[] answer = value;
			
			if (UpdateDataObj.SetData(index, out ushort addrSet, out ushort _))
		    {
			    lock (Locker)
			    {
				    SerialPort.SetDataRTU(addrSet, null, RequestPriority.High, null, answer);
			    }
		    }
	    }

		private static void UpdateDataGet(bool dataOk, ushort[] paramRtu, object param)
        {
            var index = Convert.ToInt32(param);

            if (dataOk)
            {
                UpdateDataObj.UpdateDataGet(index, paramRtu);
            }
        }

	    private static void UpdateDataSet(bool dataOk, ushort[] paramRtu, object param)
	    {
		    var index = Convert.ToInt32(param);

		    if (dataOk)
		    {
			    UpdateDataObj.UpdateDataSet(index, paramRtu);
		    }
	    }
	}
}