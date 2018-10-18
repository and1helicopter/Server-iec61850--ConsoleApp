namespace UniSerialPort
{
    public class RequestUnit
    {
        public readonly AsynchSerialPort.DataRecieved DataRecieved;
        public readonly AsynchSerialPort.DataRecievedRtu DataRecievedRtu;

        public readonly byte[] TxBuffer;
        public readonly int ReceivedBytesThreshold;
        public readonly PortAnswerType PortAnswerType;
        public int RtuReadCount { get; set; }
        public object Param { get; set; }

        public RequestUnit(byte[] txBuffer, int receivedBytesThreshold, AsynchSerialPort.DataRecieved dataRecieved)
        {
            DataRecieved = dataRecieved;
            TxBuffer = txBuffer;
            ReceivedBytesThreshold = receivedBytesThreshold;
            PortAnswerType = PortAnswerType.Byte;
            RtuReadCount = 0;
        }
        public RequestUnit(byte[] txBuffer, int receivedBytesThreshold, AsynchSerialPort.DataRecievedRtu dataRecievedRtu, int rtuReadCount, object param)
        {
            DataRecievedRtu = dataRecievedRtu;
            TxBuffer = txBuffer;
            ReceivedBytesThreshold = receivedBytesThreshold;
            PortAnswerType = PortAnswerType.Rtu;
            RtuReadCount = rtuReadCount;
            Param = param;
        }

        public byte GetSlaveAddr()
        {
            return TxBuffer[0];
        }
        public TcpFunctions GetTcpFunction()
        {
            if (TxBuffer[1] == 0x10)
            {
                return TcpFunctions.TcpWrite;
            }
            else
            {
                return TcpFunctions.TcpRead;
            }
        }
        public ushort GetTcpReadCount()
        {
            return TxBuffer[5];
        }
        public ushort GetTcpStartAddr()
        {
            ushort u = (ushort)((TxBuffer[2]<<8) | TxBuffer[3]);
            return u;
        }
        public ushort[] GetTcpWriteData()
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
