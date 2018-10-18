using System;
using System.Net;
using System.Net.Sockets;

namespace UniSerialPort
{
    public class ModbusTcpMaster
    {
        //*********************************КОДЫ СТАНДАРТНЫХ ФУНКЦИЙ *************************************************// 
        //***********************************************************************************************************//
	    // ReSharper disable once UnusedMember.Local
        private const byte FctReadCoil = 1;
	    // ReSharper disable once UnusedMember.Local
        private const byte FctReadDiscreteInputs = 2;
        private const byte FctReadHoldingRegister = 3;
	    // ReSharper disable once UnusedMember.Local
        private const byte FctReadInputRegister = 4;
        private const byte FctWriteSingleCoil = 5;
        private const byte FctWriteSingleRegister = 6;
        private const byte FctWriteMultipleCoils = 15;
        private const byte FctWriteMultipleRegister = 16;
        private const byte FctReadWriteMultipleRegister = 23;


        //***********************************************************************************************************//
        //***********************************************************************************************************//
        public Socket TcpAsyCl;

	    public bool Connected { get; set; }
	    public ushort Timeout { get; set; } = 500;
	    private byte[] tcpAsyClBuffer = new byte[2048];

        /// <summary>
        /// 
        /// </summary>
        public delegate void ResponseData(ushort id, byte unit, byte function, byte[] data);

        /// <summary>
        /// Событие вызывается, когда поступили новые данные в порт
        /// </summary>
        public event ResponseData OnResponseData;

        public delegate void ExceptionData(ushort id, byte unit, byte function, ModbusTcpExceptions exception);

        /// <summary>
        /// Событие вызывается, когда возникает ошибка обмена
        /// </summary>
        public event ExceptionData OnException;


        //************************ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ *************************************************************//
        //*************************************************************************************************************//
        private byte[] CreateReadHeader(ushort id, byte slaveAddr, ushort startAddress, ushort wordCount, byte function)
        {
            byte[] data = new byte[12];

            byte[] idTemp = BitConverter.GetBytes((short)id);
            data[0] = idTemp[1];			    // Slave id high byte
            data[1] = idTemp[0];				// Slave id low byte
            data[5] = 6;					// Message size
            data[6] = slaveAddr;			// Slave address
            data[7] = function;				// Function code
            byte[] adrTemp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)startAddress));
            data[8] = adrTemp[0];				// Start address
            data[9] = adrTemp[1];				// Start address
            byte[] lengthTemp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)wordCount));
            data[10] = lengthTemp[0];			// Number of data to read
            data[11] = lengthTemp[1];			// Number of data to read
            return data;
        }
        private byte[] CreateWriteHeader(ushort id, byte slaveAddr, ushort startAddress, ushort numData, ushort numBytes, byte function)
        {
            byte[] data = new byte[numBytes + 11];

            byte[] idTemp = BitConverter.GetBytes((short)id);
            data[0] = idTemp[1];				// Slave id high byte
            data[1] = idTemp[0];				// Slave id low byte
            byte[] sizeTemp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(5 + numBytes)));
            data[4] = sizeTemp[0];				// Complete message size in bytes
            data[5] = sizeTemp[1];				// Complete message size in bytes
            data[6] = slaveAddr;					// Slave address
            data[7] = function;				// Function code
            byte[] adrTemp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)startAddress));
            data[8] = adrTemp[0];				// Start address
            data[9] = adrTemp[1];				// Start address
            if (function >= FctWriteMultipleCoils)
            {
                byte[] cntTemp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)numData));
                data[10] = cntTemp[0];			// Number of bytes
                data[11] = cntTemp[1];			// Number of bytes
                data[12] = (byte)(numBytes - 2);
            }
            return data;
        }


        //**************************** УСТАНОВКА И РАЗРЫВ СОЕДИНЕНИЯ **************************************************//
        //*************************************************************************************************************//
        public ModbusTcpMaster(string ip, ushort port)
        {
            Connect(ip, port);
        }
        ~ModbusTcpMaster()
        {
            Dispose();
        }

        public void ChangeSocket(Socket newSocket)
        {
            TcpAsyCl = newSocket;
            TcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, Timeout);
            TcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout);
            TcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);

        }

        public void Connect(string ip, ushort port)
        {
           
            try
            {
	            if (IPAddress.TryParse(ip, out _) == false)
                {
                    IPHostEntry hst = Dns.GetHostEntry(ip);
                    ip = hst.AddressList[0].ToString();
                }
                // ----------------------------------------------------------------
                // Connect asynchronous client
                TcpAsyCl = new Socket(IPAddress.Parse(ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                TcpAsyCl.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                TcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, Timeout);
                TcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout);
                TcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);

                Connected = true;
            }
            catch (System.IO.IOException)
            {
                Connected = false;
                throw;
            }
            
        }
        public void Disconnnect()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (TcpAsyCl != null)
            {
                if (TcpAsyCl.Connected)
                {
	                try
	                {
		                TcpAsyCl.Shutdown(SocketShutdown.Both);
	                }
	                catch
	                {
						//ignore
	                }
                    TcpAsyCl.Close();
                }
                TcpAsyCl = null;
            }
            Connected = false;
        }


        //************************************** ЗАПИСЬ ДАННЫХ В ПОРТ ************************************************//
        //************************************************************************************************************//
        internal void CallException(ushort id, byte unit, byte function, ModbusTcpExceptions exception)
        {
            if (TcpAsyCl == null) return;
            if (exception == ModbusTcpExceptions.ExcExceptionConnectionLost)
            {
                TcpAsyCl = null;
            }

	        OnException?.Invoke(id, unit, function, exception);
        }
        private void WriteAsyncData(byte[] writeData, ushort id)
        {
            if ((TcpAsyCl != null) && (TcpAsyCl.Connected))
            {
                try
                {
                    TcpAsyCl.BeginSend(writeData, 0, writeData.Length, SocketFlags.None, OnSend, null);
                    TcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length, SocketFlags.None, OnReceive, TcpAsyCl);
                }
                catch (SystemException)
                {
                    CallException(id, writeData[6], writeData[7], ModbusTcpExceptions.ExcExceptionConnectionLost);
                }
            }
            else CallException(id, writeData[6], writeData[7], ModbusTcpExceptions.ExcExceptionConnectionLost);
        }
        private void OnSend(IAsyncResult result)
        {
            if (result.IsCompleted == false) CallException(0xFFFF, 0xFF, 0xFF, ModbusTcpExceptions.ExcSendFailt);
        }
        internal static UInt16 SwapUInt16(UInt16 inValue)
        {
            return (UInt16)(((inValue & 0xff00) >> 8) |
                     ((inValue & 0x00ff) << 8));
        }
        private void OnReceive(IAsyncResult result)
        {
            if (result.IsCompleted == false) CallException(0xFF, 0xFF, 0xFF, ModbusTcpExceptions.ExcExceptionConnectionLost);

            ushort id = SwapUInt16(BitConverter.ToUInt16(tcpAsyClBuffer, 0));
            byte unit = tcpAsyClBuffer[6];
            byte function = tcpAsyClBuffer[7];
            byte[] data;

            // ------------------------------------------------------------
            // Write response data
            if ((function >= FctWriteSingleCoil) && (function != FctReadWriteMultipleRegister))
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
            if (function > (byte)ModbusTcpExceptions.ExcExceptionOffset)
            {
                function -= (byte)ModbusTcpExceptions.ExcExceptionOffset;
                CallException(id, unit, function, ModbusTcpExceptions.ExcRecieveSlaveException);
            }
            // ------------------------------------------------------------
            // Response data is regular data
            else
            {
	            OnResponseData?.Invoke(id, unit, function, data);
            }
        }

        //******************************** СТАНДАРТНЫЕ ФУНКЦИИ *******************************************************//
        //************************************************************************************************************//
        public void ReadHoldingRegister(ushort id, byte slaveAddr, ushort startAddress, ushort wordCount)
        {
            WriteAsyncData(CreateReadHeader(id, slaveAddr, startAddress, wordCount, FctReadHoldingRegister), id);
        }
        public void WriteSingleRegister(ushort id, byte slaveAddr, ushort startAddr, ushort val)
        {
            byte[] data;
            data = CreateWriteHeader(id, slaveAddr, startAddr, 1, 1, FctWriteSingleRegister);
            data[10] = (byte)((val >> 8) & 0xFF);
            data[11] = (byte)(val & 0xFF);
            WriteAsyncData(data, id);
        }
        public void WriteMultipleRegister(ushort id, byte slaveAddr, ushort startAddr, params ushort[] val)
        {
            byte[] values = new byte[val.Length * 2];
            for (int i = 0; i < val.Length; i++)
            {
                values[2 * i] = (byte)((val[i] >> 8) & 0xFF);
                values[2 * i + 1] = (byte)(val[i] & 0xFF);
            }

            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes % 2 > 0) numBytes++;

	        var data = CreateWriteHeader(id, slaveAddr, startAddr, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes + 2), FctWriteMultipleRegister);
            Array.Copy(values, 0, data, 13, values.Length);
            WriteAsyncData(data, id);
        }
    }
}
