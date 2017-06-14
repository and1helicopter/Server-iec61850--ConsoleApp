namespace UniSerialPort
{
    public class RequestUnit
    {
        public AsynchSerialPort.DataRecieved DataRecieved;
        public AsynchSerialPort.DataRecievedRTU DataRecievedRTU;

        public byte[] TxBuffer;
        public int ReceivedBytesThreshold;
        public PortAnswerType PortAnswerType;
        public int RTUReadCount { get; set; }
        public object Param { get; set; }

        public RequestUnit(byte[] txBuffer, int receivedBytesThreshold, AsynchSerialPort.DataRecieved dataRecieved)
        {
            DataRecieved = dataRecieved;
            TxBuffer = txBuffer;
            ReceivedBytesThreshold = receivedBytesThreshold;
            PortAnswerType = PortAnswerType.Byte;
            RTUReadCount = 0;
        }
        public RequestUnit(byte[] txBuffer, int receivedBytesThreshold, AsynchSerialPort.DataRecievedRTU dataRecievedRTU, int rtuReadCount, object param)
        {
            DataRecievedRTU = dataRecievedRTU;
            TxBuffer = txBuffer;
            ReceivedBytesThreshold = receivedBytesThreshold;
            PortAnswerType = PortAnswerType.RTU;
            RTUReadCount = rtuReadCount;
            Param = param;
        }

        public byte GetSlaveAddr()
        {
            return TxBuffer[0];
        }
        public TCPFunctions GetTCPFunction()
        {
            if (TxBuffer[1] == 0x10)
            {
                return TCPFunctions.TCPWrite;
            }
            else
            {
                return TCPFunctions.TCPRead;
            }
        }
        public ushort GetTCPReadCount()
        {
            return (ushort)(TxBuffer[5]);
        }
        public ushort GetTCPStartAddr()
        {
            ushort u = (ushort)((TxBuffer[2]<<8) | TxBuffer[3]);
            return u;
        }
        public ushort[] GetTCPWriteData()
        {
            ushort count = (ushort)((TxBuffer[4] << 8) | TxBuffer[5]);
            ushort[] us = new ushort[count];
            for (int i = 0; i < us.Length; i++)
            {
                us[i] = (ushort)((TxBuffer[2 * i + 7] << 8) | TxBuffer[2 * i + 8]);
            }

            return us;

        }

    }
}
