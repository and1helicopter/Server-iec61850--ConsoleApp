using System;
using System.Net;
using System.Net.Sockets;

namespace UniSerialPort
{
    public class ModbusTCPMaster
    {
        //*********************************КОДЫ СТАНДАРТНЫХ ФУНКЦИЙ *************************************************// 
        //***********************************************************************************************************//
        private const byte fctReadCoil = 1;
        private const byte fctReadDiscreteInputs = 2;
        private const byte fctReadHoldingRegister = 3;
        private const byte fctReadInputRegister = 4;
        private const byte fctWriteSingleCoil = 5;
        private const byte fctWriteSingleRegister = 6;
        private const byte fctWriteMultipleCoils = 15;
        private const byte fctWriteMultipleRegister = 16;
        private const byte fctReadWriteMultipleRegister = 23;


        //***********************************************************************************************************//
        //***********************************************************************************************************//
        public Socket tcpAsyCl;

        bool connected;
        public bool Connected
        {
            get { return connected; }
            set { connected = value; }
        }

        private ushort timeout = 500;
        public ushort Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        private byte[] tcpAsyClBuffer = new byte[2048];

        /// <summary>
        /// 
        /// </summary>
        public delegate void ResponseData(ushort ID, byte Unit, byte Function, byte[] Data);

        /// <summary>
        /// Событие вызывается, когда поступили новые данные в порт
        /// </summary>
        public event ResponseData OnResponseData;

        public delegate void ExceptionData(ushort ID, byte Unit, byte Function, ModbusTCPExceptions Exception);

        /// <summary>
        /// Событие вызывается, когда возникает ошибка обмена
        /// </summary>
        public event ExceptionData OnException;


        //************************ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ *************************************************************//
        //*************************************************************************************************************//
        private byte[] CreateReadHeader(ushort ID, byte SlaveAddr, ushort StartAddress, ushort WordCount, byte Function)
        {
            byte[] data = new byte[12];

            byte[] _id = BitConverter.GetBytes((short)ID);
            data[0] = _id[1];			    // Slave id high byte
            data[1] = _id[0];				// Slave id low byte
            data[5] = 6;					// Message size
            data[6] = SlaveAddr;			// Slave address
            data[7] = Function;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)StartAddress));
            data[8] = _adr[0];				// Start address
            data[9] = _adr[1];				// Start address
            byte[] _length = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)WordCount));
            data[10] = _length[0];			// Number of data to read
            data[11] = _length[1];			// Number of data to read
            return data;
        }
        private byte[] CreateWriteHeader(ushort ID, byte SlaveAddr, ushort StartAddress, ushort NumData, ushort NumBytes, byte Function)
        {
            byte[] data = new byte[NumBytes + 11];

            byte[] _id = BitConverter.GetBytes((short)ID);
            data[0] = _id[1];				// Slave id high byte
            data[1] = _id[0];				// Slave id low byte
            byte[] _size = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)(5 + NumBytes)));
            data[4] = _size[0];				// Complete message size in bytes
            data[5] = _size[1];				// Complete message size in bytes
            data[6] = SlaveAddr;					// Slave address
            data[7] = Function;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)StartAddress));
            data[8] = _adr[0];				// Start address
            data[9] = _adr[1];				// Start address
            if (Function >= fctWriteMultipleCoils)
            {
                byte[] _cnt = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)NumData));
                data[10] = _cnt[0];			// Number of bytes
                data[11] = _cnt[1];			// Number of bytes
                data[12] = (byte)(NumBytes - 2);
            }
            return data;
        }


        //**************************** УСТАНОВКА И РАЗРЫВ СОЕДИНЕНИЯ **************************************************//
        //*************************************************************************************************************//
        public ModbusTCPMaster(string IP, ushort Port)
        {
            Connect(IP, Port);
        }
        ~ModbusTCPMaster()
        {
            Dispose();
        }

        public void ChangeSocket(Socket NewSocket)
        {
            tcpAsyCl = NewSocket;
            tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
            tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
            tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);

        }

        public void Connect(string ip, ushort port)
        {
           
            try
            {
                IPAddress _ip;
                if (IPAddress.TryParse(ip, out _ip) == false)
                {
                    IPHostEntry hst = Dns.GetHostEntry(ip);
                    ip = hst.AddressList[0].ToString();
                }
                // ----------------------------------------------------------------
                // Connect asynchronous client
                tcpAsyCl = new Socket(IPAddress.Parse(ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                tcpAsyCl.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
                tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
                tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);

                connected = true;
            }
            catch (System.IO.IOException error)
            {
                connected = false;
                throw (error);
            }
            
        }
        public void Disconnnect()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (tcpAsyCl != null)
            {
                if (tcpAsyCl.Connected)
                {
                    try { tcpAsyCl.Shutdown(SocketShutdown.Both); }
                    catch { }
                    tcpAsyCl.Close();
                }
                tcpAsyCl = null;
            }
            connected = false;
        }


        //************************************** ЗАПИСЬ ДАННЫХ В ПОРТ ************************************************//
        //************************************************************************************************************//
        internal void CallException(ushort id, byte unit, byte function, ModbusTCPExceptions Exception)
        {
            if (tcpAsyCl == null) return;
            if (Exception == ModbusTCPExceptions.ExcExceptionConnectionLost)
            {
                tcpAsyCl = null;
            }
            if (OnException != null) OnException(id, unit, function, Exception);
        }
        private void WriteAsyncData(byte[] write_data, ushort id)
        {
            if ((tcpAsyCl != null) && (tcpAsyCl.Connected))
            {
                try
                {
                    tcpAsyCl.BeginSend(write_data, 0, write_data.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
                    tcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), tcpAsyCl);
                }
                catch (SystemException)
                {
                    CallException(id, write_data[6], write_data[7], ModbusTCPExceptions.ExcExceptionConnectionLost);
                }
            }
            else CallException(id, write_data[6], write_data[7], ModbusTCPExceptions.ExcExceptionConnectionLost);
        }
        private void OnSend(System.IAsyncResult result)
        {
            if (result.IsCompleted == false) CallException(0xFFFF, 0xFF, 0xFF, ModbusTCPExceptions.ExcSendFailt);
        }
        internal static UInt16 SwapUInt16(UInt16 inValue)
        {
            return (UInt16)(((inValue & 0xff00) >> 8) |
                     ((inValue & 0x00ff) << 8));
        }
        private void OnReceive(System.IAsyncResult result)
        {
            if (result.IsCompleted == false) CallException(0xFF, 0xFF, 0xFF, ModbusTCPExceptions.ExcExceptionConnectionLost);

            ushort id = SwapUInt16(BitConverter.ToUInt16(tcpAsyClBuffer, 0));
            byte unit = tcpAsyClBuffer[6];
            byte function = tcpAsyClBuffer[7];
            byte[] data;

            // ------------------------------------------------------------
            // Write response data
            if ((function >= fctWriteSingleCoil) && (function != fctReadWriteMultipleRegister))
            {
                data = new byte[2];
                Array.Copy(tcpAsyClBuffer, 10, data, 0, 2);
            }
            // ------------------------------------------------------------
            // Read response data
            else
            {
                data = new byte[tcpAsyClBuffer[8]];
                Array.Copy(tcpAsyClBuffer, 9, data, 0, tcpAsyClBuffer[8]);
            }
            // ------------------------------------------------------------
            // Response data is slave exception
            if (function > (byte)ModbusTCPExceptions.ExcExceptionOffset)
            {
                function -= (byte)ModbusTCPExceptions.ExcExceptionOffset;
                CallException(id, unit, function, ModbusTCPExceptions.ExcRecieveSlaveException);
            }
            // ------------------------------------------------------------
            // Response data is regular data
            else if (OnResponseData != null) OnResponseData(id, unit, function, data);
        }

        //******************************** СТАНДАРТНЫЕ ФУНКЦИИ *******************************************************//
        //************************************************************************************************************//
        public void ReadHoldingRegister(ushort ID, byte SlaveAddr, ushort StartAddress, ushort WordCount)
        {
            WriteAsyncData(CreateReadHeader(ID, SlaveAddr, StartAddress, WordCount, fctReadHoldingRegister), ID);
        }
        public void WriteSingleRegister(ushort ID, byte SlaveAddr, ushort StartAddr, ushort Value)
        {
            byte[] data;
            data = CreateWriteHeader(ID, SlaveAddr, StartAddr, 1, 1, fctWriteSingleRegister);
            data[10] = (byte)((Value >> 8) & 0xFF);
            data[11] = (byte)(Value & 0xFF);
            WriteAsyncData(data, ID);
        }
        public void WriteMultipleRegister(ushort ID, byte SlaveAddr, ushort StartAddr, params ushort[] Values)
        {
            byte[] values = new byte[Values.Length * 2];
            for (int i = 0; i < Values.Length; i++)
            {
                values[2 * i] = (byte)((Values[i] >> 8) & 0xFF);
                values[2 * i + 1] = (byte)(Values[i] & 0xFF);
            }

            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateWriteHeader(ID, SlaveAddr, StartAddr, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister);
            Array.Copy(values, 0, data, 13, values.Length);
            WriteAsyncData(data, ID);
        }
    }
}
