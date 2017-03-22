using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Xml;

namespace ModbusLibraryNew
{
    public static class AddModBusSerialPort
    {

        static void LoadSettingsFromXML(string comPortXMLName)
        {
            XmlNodeList xmls;
            XmlNode xmlNode;

            var doc = new XmlDocument();
            try { doc.Load(comPortXMLName); }
            catch
            {
                throw new Exception("Не найден файл comset.xml!");
            }

            xmls = doc.GetElementsByTagName("ComPort");

            if (xmls.Count != 1)
            {
                throw new Exception("Ошибки в файле comset.xml!");
            }

            xmlNode = xmls[0];

            string i1 = "COM1";
            int i2 = 9600;
            Parity i3 = Parity.Odd;
            int i3_1 = 0; ;
            // int i4 = 1;


            try
            {
                i1 = Convert.ToString(xmlNode.Attributes["Name"].Value);
                i2 = Convert.ToInt32(xmlNode.Attributes["Speed"].Value);
                i3_1 = Convert.ToInt32(xmlNode.Attributes["Parity"].Value);
                //i4 = Convert.ToInt32(xmlNode.Attributes["Addr"].Value);
            }
            catch
            {
                throw new Exception("Ошибки в файле comset.xml!");
            }

            switch (i3_1)
            {
                case 0: { i3 = System.IO.Ports.Parity.Odd; serialPortStopBits = System.IO.Ports.StopBits.One; } break;
                case 1: { i3 = System.IO.Ports.Parity.Even; serialPortStopBits = System.IO.Ports.StopBits.One; } break;
                case 2: { i3 = System.IO.Ports.Parity.None; serialPortStopBits = System.IO.Ports.StopBits.Two; } break;
            }

            serialPortParity = i3;
            serialPortName = i1;
            serialPortBaudRate = i2;
        }
        //Настройки Com-порта, которые будут подгужаться из файла
        #region PortSettings
        static Parity serialPortParity = Parity.Odd;
        static StopBits serialPortStopBits = StopBits.One;
        static int serialPortBaudRate = 9600;
        static string serialPortName = "COM2";



        #endregion

        static SerialPort serialPort;
        static public bool IsOpen
        {
            get
            {
                bool b = false;
                try
                {
                    b = serialPort.IsOpen;
                }
                catch { b = false; }
                return b;

            }
            set { }
        }

        //После прихода данного количества байтов, срабатывает прерывание "ReceiveComPort"
        private static int nowThreshold = 21;
        public static int NowThreshold
        {
            get { return nowThreshold; }
            set
            {
            }
        }

        //Устанавливается после отправки данных в COM-порт
        private static bool portBusy = false;
        public static bool PortBusy
        {
            get { return portBusy; }
            set { }
        }

        //Количество байт в приемном буфере, если таймаут вышел, но нужно количества байт для срабатывания прерывания не пиршло
        private static int bytesInBuffer = 0;
        public static int BytesInBuffer
        {
            get { return bytesInBuffer; }
            set { }
        }

        //Процедура открытия COM-порта
        public static void OpenPort(Parity newParity, StopBits newStopBits, int newBaudRate, string newComPortName)
        {
            serialPortParity = newParity;
            serialPortBaudRate = newBaudRate;
            serialPortStopBits = newStopBits;
            serialPortName = newComPortName;
            OpenPort();
        }
        public static void OpenPort(string FileName)
        {
            LoadSettingsFromXML(FileName);
            OpenPort();
        }
        private static void OpenPort()
        {
            if (IsOpen) { return; }
            //Таймер для формирования срабатывания прерывания "ReceiveComPort" по таймауту
            //инициализируется при первом открытии COM-порта
            if (timer == null)
            {
                timer = new System.Windows.Forms.Timer();
                timer.Enabled = true;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = 100;
            }

            serialPort = new SerialPort();
            serialPort.Parity = serialPortParity;
            serialPort.BaudRate = serialPortBaudRate;
            serialPort.PortName = serialPortName;
            serialPort.StopBits = serialPortStopBits;
            serialPort.DataBits = 8;
            serialPort.Handshake = System.IO.Ports.Handshake.None;
            serialPort.DtrEnable = false;
            serialPort.RtsEnable = false;
            serialPort.ParityReplace = 63;
            serialPort.ReadBufferSize = 4096;
            serialPort.ReadTimeout = 3000;
            serialPort.WriteBufferSize = 2048;
            serialPort.WriteTimeout = 3000;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            try
            {
                serialPort.Open();
            }
            catch
            {
                throw new Exception("Ошибка открытия COM-порта " + serialPortName + "!");
            }
        }

        public static void SendData(byte[] sendBuffer, int count, int newThreadOld)
        {
            if (portBusy) { return; }
            if (!serialPort.IsOpen) { return; }
            bool portErr = false;

            //Если что-то есть непрочитанное, то очищаем COM-порт
            while ((serialPort.BytesToRead != 0) && (!portErr))
            {
                byte b;
                try
                {
                    b = (byte)serialPort.ReadByte();
                }
                catch
                {
                    portErr = true;
                }
                if (portErr)
                {
                    if (PortError != null) { PortError(null, new EventArgs()); }
                }
            }

            nowThreshold = newThreadOld;
            sendCiclesCounter = 0;
            portBusy = true;
            rxBuffertmp = new byte[1000];
            rxIndex = 0;
            CurrentForm = null;

            bool b1 = false;
            try
            {
                serialPort.Write(sendBuffer, 0, count);
            }
            catch
            {
                b1 = true;
            }
            if (b1)
            {
                if (PortError != null) { PortError(null, new EventArgs()); }
            }
        }

        //Приемный буфер, прочитать можно только новый экземпляр объекта типа массива, 
        //что бы при доступе из разных потоков не обращаться к одному и тому же объекту, 
        //т.к. обработка RxBuffer может "затянуться", выполнится новый запрос и данные изменятся (на всякий случай)
        private static byte[] rxBuffer;
        public static byte[] RxBuffer
        {
            get
            {
                int i;
                byte[] rxb = new byte[rxBuffer.Length];
                for (i = 0; i < rxb.Length; i++) { rxb[i] = rxBuffer[i]; }
                return rxb;
            }
            set { }
        }

        //Флаг, что после срабатывания "ReceiveComPort", c данными все в порядке
        private static bool dataOk = false;
        public static bool DataOk
        {
            get { return dataOk; }
            set { }
        }
        static public event EventHandler ReceiveComPort;

        //Счетчик для формирования события по таймауту,
        //сбрасывает при каждой отправке данных в COM-порт
        static int sendCiclesCounter = 0;
        static System.Windows.Forms.Timer timer;
        static private void timer_Tick(object sender, EventArgs e)
        {
            if (portBusy) { sendCiclesCounter++; } else { sendCiclesCounter = 0; }

            //Если что-то начало приходить, то нужно дождаться пока посылка закончится (хоть она и плохая);
            if (sendCiclesCounter >= 10)
            {
                if (serialPort.BytesToRead != 0) { sendCiclesCounter = 8; }
                bool portErr = false;

                //Если что-то есть непрочитанное, то очищаем COM-порт
                while ((serialPort.BytesToRead != 0) && (!portErr))
                {
                    byte b;
                    try
                    {
                        b = (byte)serialPort.ReadByte();
                    }
                    catch
                    {
                        portErr = true;
                    }
                    if (portErr)
                    {
                        if (PortError != null) { PortError(null, new EventArgs()); }
                    }
                }
            }
            if (sendCiclesCounter > 10)
            {
                if (closePortFlag)
                {
                    closePortFlag = false;
                    serialPort.Close();
                    portBusy = false;
                    dataOk = false;
                    sendCiclesCounter = 0;
                    if (ComPortClosed != null) ComPortClosed(null, new EventArgs());
                    return;
                }

                sendCiclesCounter = 0;
                bytesInBuffer = serialPort.BytesToRead;
                //Формирование события конец запроса
                portBusy = false;
                dataOk = false;

                if (ReceiveComPort != null) ReceiveComPort(null, new EventArgs());
                if (CurrentForm != null) { CurrentForm(); }
            }

        }

        static byte[] rxBuffertmp = new byte[1000];

        public static byte[] RxBuffertmp
        {
            get { return rxBuffertmp; }
            set { rxBuffertmp = value; }
        }
        static int rxIndex = 0;
        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort.BytesToRead == 0) { return; }
            bool portErr = false;
            while ((serialPort.BytesToRead != 0) && (!portErr))
            {
                try
                {
                    rxBuffertmp[rxIndex++] = (byte)serialPort.ReadByte();
                }
                catch
                {
                    portErr = true;
                }
                if (portErr)
                {
                    if (PortError != null) { PortError(null, new EventArgs()); }
                }
            }

            if (rxIndex < nowThreshold)
            {
                return;
            }

            //Если есть ошибка CRC, то скорее всего ответный пакет длиннее чем предполагалось
            //В этом случае просто сбрасываем приемный буфер, и ждем срабатывания прерывания по таймауту
            if (!ModBus.CheckCRC(rxBuffertmp, rxIndex))
            {
                sendCiclesCounter = 6;

                return;
            }

            //Данные все пришли можно закрывать COM-порт
            if (closePortFlag)
            {
                closePortFlag = false;
                serialPort.Close();
                portBusy = false;
                dataOk = false;
                sendCiclesCounter = 0;
                if (ComPortClosed != null) ComPortClosed(null, new EventArgs());
                if (CurrentForm != null) { CurrentForm(); }
                return;
            }




            rxBuffer = new byte[rxIndex];
            int i1;
            for (i1 = 0; i1 < rxIndex; i1++)
            {
                rxBuffer[i1] = rxBuffertmp[i1];
            }
            portBusy = false;
            dataOk = true;

            //Формирование события "ответ"
            if (ReceiveComPort != null) ReceiveComPort(null, new EventArgs());
            if (CurrentForm != null) { CurrentForm(); }
        }

        private static bool closePortFlag = false;
        //Закрытие порта
        public static void ClosePort()
        {
            if (!serialPort.IsOpen) { return; }
            if (!portBusy)
            {
                closePortFlag = false;
                serialPort.Close();
                portBusy = false;
                dataOk = false;
                sendCiclesCounter = 0;
                if (ComPortClosed != null) ComPortClosed(null, new EventArgs());
                return;
            }

            closePortFlag = true;
        }
        static public event EventHandler ComPortClosed;

        //Отправка данных в порт с определенным обработчиком события
        public delegate void OnDataRecieved();
        private static OnDataRecieved CurrentForm { get; set; }
        public static void SendData(byte[] sendBuffer, int count, int newThreadOld, OnDataRecieved NewCurrentForm)
        {
            if (portBusy) { return; }
            if (!serialPort.IsOpen) { return; }

            //Если что-то есть непрочитанное, то очищаем COM-порт
            bool portErr = false;
            while ((serialPort.BytesToRead != 0) && (!portErr))
            {
                byte b;
                try
                {
                    b = (byte)serialPort.ReadByte();
                }
                catch
                {
                    portErr = true;
                }

                if (portErr)
                {
                    if (PortError != null) { PortError(null, new EventArgs()); }
                }

            }

            nowThreshold = newThreadOld;
            sendCiclesCounter = 0;
            portBusy = true;
            rxBuffertmp = new byte[1000];
            rxIndex = 0;
            CurrentForm = NewCurrentForm;

            bool b1 = false;
            try
            {
                serialPort.Write(sendBuffer, 0, count);
            }
            catch
            {
                b1 = true;
            }
            if (b1)
            {
                if (PortError != null) { PortError(null, new EventArgs()); }
            }


        }


        public static void ExtractData(out bool ExternalDataOk, out byte[] ExternalRxBuffer)
        {

            if (DataOk)
            {
                ExternalDataOk = DataOk;
                ExternalRxBuffer = new byte[RxBuffer.Length];
                for (int i = 0; i < RxBuffer.Length; i++)
                {
                    ExternalRxBuffer[i] = RxBuffer[i];
                }
            }
            else
            {
                ExternalDataOk = DataOk;
                ExternalRxBuffer = new byte[0];
            }
        }

        //Список всех COM-портов на компьютере
        static List<string> portList = new List<string>();
        public static List<string> PortList
        {
            get
            {
                string[] portStrList;
                portStrList = SerialPort.GetPortNames();
                portList.Clear();
                foreach (string port in portStrList)
                {
                    portList.Add(port);
                }
                portList.Sort();
                return portList;
            }
            set { }
        }


        //ОШИБКИ ПРИ РАБОТЕ С COM-ПОРТОМ
        static public event EventHandler PortError;
    }
}
