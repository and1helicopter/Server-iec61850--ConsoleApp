using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace ModbusLibraryNew
{
    public static class ModBusRequestQueue
    {
        private static Thread worker;
        private static AutoResetEvent wh = new AutoResetEvent(false);
        private static object locker = new object();

        private static Queue<ModBusDataClass> requestQueue = new Queue<ModBusDataClass>();
        public static void InitModBusRequest()
        {
            worker = new Thread(Work);
            worker.Start();
        }

        public static void AddRequest(ModBusDataClass modBusData)
        {
            lock (locker)
            {
                requestQueue.Enqueue(modBusData);
            }
            wh.Set();
        }

        static ModbusSerialPort.OnDataRecieved CurrDataRecieved;

        static bool nowRequest = false;
        public static void SendRequest(ModBusDataClass modBusData)
        {
            //Отправляем запрос, запоминаем указатель на метод обработчик
            ModbusSerialPort.SendData(modBusData.TxBuffer, modBusData.TxCount, modBusData.ThreadOld,OnDataReceived);
            CurrDataRecieved = modBusData.OnDataReceived;
            nowRequest = true;
        }

        //Поток, узнает что запрос выполнен => сообщает об этом владельцу запроса (CurrDataReceived) (ModBusUnit у)
        private static void OnDataReceived()
        {
            if (CurrDataRecieved != null) { CurrDataRecieved(); }
            nowRequest = false;
            wh.Set(); //Можно выполнять следующий запрос
        }

        public static void Dispose()
        {

        }
    
        static void Work()
        {
            while (true)
            {
                wh.WaitOne(); //Ждем поступления сигнала о необходимости проверки очереди

                lock (locker)
                {
                    if ((requestQueue.Count > 0)&&(!nowRequest))
                    {
                        SendRequest(requestQueue.Dequeue());
                    }
                }
            }
        }


    }
}
