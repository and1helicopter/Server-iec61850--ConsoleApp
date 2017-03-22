using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ModbusLibraryNew
{
    public static class ModBusRequest
    {
        /*
        static bool NowRequest { get; set; }
        static ModbusSerialPort.OnDataRecieved CurrDataRecieved;
        private static void OnDataReceived()
        {
            if (CurrDataRecieved != null) { CurrDataRecieved(); }

            lock (queueLock)
            {
                if (closePortFlag)
                {
                    ModbusSerialPort.ClosePort();
                    requestQueue.Clear();
                    closePortFlag = false;
                    NowRequest = false;
                    return;
                }

                if (requestQueue.Count == 0)
                {
                    NowRequest = false;
                    return;
                }
                SendRequest(requestQueue.Dequeue());
            }
        }

        static object queueLock = new object();
        private static Queue<ModBusDataClass> requestQueue;

        public static void AddRequest(byte[] txBuffer, byte txCount, byte ThreadOld, ModbusSerialPort.OnDataRecieved CurrOnDataReceived)
        {
            ModBusDataClass newModBusData = new ModBusDataClass();
            newModBusData.TxBuffer = txBuffer;
            newModBusData.TxCount = txCount;
            newModBusData.ThreadOld = ThreadOld;
            newModBusData.OnDataReceived = CurrOnDataReceived;

            lock (queueLock)
            {
                if (!closePortFlag)
                {
                    requestQueue.Enqueue(newModBusData);
                }
                if (!NowRequest)
                {
                    SendRequest(requestQueue.Dequeue());
                }
            }
        }
        private static void SendRequest(ModBusDataClass modBusData)
        {
            NowRequest = true;
            ModbusSerialPort.SendData(modBusData.TxBuffer, modBusData.TxCount, modBusData.ThreadOld, new ModbusSerialPort.OnDataRecieved(OnDataReceived));
            CurrDataRecieved = modBusData.OnDataReceived;
        }

        public static void InitModBusRequest()
        {
            NowRequest = false;
            requestQueue = new Queue<ModBusDataClass>();
        }

        private static bool closePortFlag = false;
        public static void ClosePort()
        {
            lock (queueLock)
            {
                //Отправлен какой-то запрос, порт пока не закрываем
                if (NowRequest) { closePortFlag = true; }
                else
                {
                    ModbusSerialPort.ClosePort();
                }
            }
        }
         * */
    }
}
