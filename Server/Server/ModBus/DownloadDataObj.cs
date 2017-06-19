using System;
using Server.Update;
using UniSerialPort;

namespace Server.ModBus
{
    public static partial class ModBus
    {
        private static int _currentIndexSet;

        private static void DataRequest()
        {
            #region Get
            if (UpdateDataObj.DataClassGet.Count != 0)
            {
                for (var i = 0; i < UpdateDataObj.DataClassGet.Count; i++)
                {
                    if (UpdateDataObj.GetData(i, out ushort addrGet, out ushort b))
                    {
                        lock (Locker)
                        {
                            SerialPort.GetDataRTU(addrGet, b, UpdateDataGet, i);
                        }
                    }
                }
            }
            #endregion

            #region Set
            if (UpdateDataObj.DataClassSet.Count != 0)
            {
                if (_currentIndexSet == UpdateDataObj.DataClassSet.Count)
                {
                    _currentIndexSet = 0;
                }

                if (UpdateDataObj.SetData(_currentIndexSet, out ushort addrSet, out ushort send))
                {
                    lock (Locker)
                    {
                        SerialPort.SetDataRTU(addrSet, null, RequestPriority.Normal, send);
                    }
                }
            }
            #endregion
        }

        private static void UpdateDataGet(bool dataOk, ushort[] paramRtu, object param)
        {
            var index = Convert.ToInt32(param);

            if (dataOk)
            {
                UpdateDataObj.UpdateData(index, paramRtu);
            }

            if (index == UpdateDataObj.DataClassGet.Count - 1)
            {
                DataRequest();
            }
        }
    }
}