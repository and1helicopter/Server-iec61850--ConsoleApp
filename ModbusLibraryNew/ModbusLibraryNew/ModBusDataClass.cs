using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusLibraryNew
{
    public class ModBusDataClass
    {
        public ModbusSerialPort.OnDataRecieved OnDataReceived { get; set; }
        public byte[] TxBuffer { get; set; }
        public byte TxCount { get; set; }
        public byte ThreadOld { get; set; }
    }
}
