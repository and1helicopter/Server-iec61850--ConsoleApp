using System;
using Server.Parser;

namespace Server.ModBus
{
    public static partial class ModBus
    {
        private static int _currentIndex;

        private static void DataRequest()
        {
            if (!SerialPort.IsOpen)
            {
                return;
            }


            if (UpdateDataObj.DataClassGet.Count != 0)
            {
                if (_currentIndex == UpdateDataObj.DataClassGet.Count)
                {
                    _currentIndex = 0;
                }

                if (!UpdateDataObj.DataClassGet[_currentIndex].GetDataObj)
                {
                    lock (Locker)
                    {
                        SerialPort.GetDataRTU(UpdateDataObj.DataClassGet[_currentIndex].AddrDataObj, 1, UpdateData);
                        UpdateDataObj.DataClassGet[_currentIndex].GetDataObj_Set(true);
                    }
                }
            }
        }

        private static void UpdateData(bool dataOk, ushort[] paramRtu)
        {
            if (dataOk)
            {
                if (UpdateDataObj.DataClassGet[_currentIndex].DataObj.GetType() == typeof(MvClass))
                {
                    ((MvClass)UpdateDataObj.DataClassGet[_currentIndex].DataObj).UpdateClass(DateTime.Now, Convert.ToInt64(paramRtu[0]));
                    UpdateDataObj.DataClassGet[_currentIndex].GetDataObj_Set(false);
                }
                else if (UpdateDataObj.DataClassGet[_currentIndex].DataObj.GetType() == typeof(SpsClass))
                {
                    bool val = (Convert.ToInt32(paramRtu[0]) & 1 << Convert.ToInt32(UpdateDataObj.DataClassGet[_currentIndex].MaskDataObj)) > 0;
                    ((SpsClass)UpdateDataObj.DataClassGet[_currentIndex].DataObj).UpdateClass(DateTime.Now, val);
                    UpdateDataObj.DataClassGet[_currentIndex].GetDataObj_Set(false);
                }

                _currentIndex++;
            }
        }
    }
}