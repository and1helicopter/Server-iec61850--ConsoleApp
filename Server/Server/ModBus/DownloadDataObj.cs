
using Server.Parser;
using UniSerialPort;

namespace Server.ModBus
{
    public static partial class ModBus
    {
        private static void DataRequest()
        {
            if (!SerialPort.IsOpen)
            {
                return;
            }

            if (UpdateDataObj.DataClassGet.Count != 0)
            {
                if (_currentIndexGet == UpdateDataObj.DataClassGet.Count)
                {
                    lock (Locker)
                    {
                        _currentIndexGet = 0;
                    }
                }

                if (UpdateDataObj.GetData(_currentIndexGet, out ushort addrGet))
                {
                    lock (Locker)
                    {
                        SerialPort.GetDataRTU(addrGet, 1, UpdateData);
                    }
                }
            }

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
        }

        private static void UpdateData(bool dataOk, ushort[] paramRtu)
        {
            if (dataOk)
            {
                UpdateDataObj.UpdateData(_currentIndexGet, paramRtu);
                _currentIndexGet++;
            }
        }
    }
}