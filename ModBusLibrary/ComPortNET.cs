using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ModBusLibrary
{
    class ComPortNET
    {
        SerialPort serialPort;
        public event EventHandler ReceiveComPort;

        bool nowPortOpen = false;
        bool readProcess = false;
        bool closeProcess = false;
        bool initPortClose = false;
        bool enaPortRecieve = false;

        public bool CheckOnline()
        {
            return (nowPortOpen);
        }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)   //Получение
        {
            int i = 1;
            bool b = false;

            if (!enaPortRecieve) { return; }

            readProcess = true;
            while ((i!=0)&&(nowPortOpen))
            {
                if (!closeProcess)
                {
                    try
                    {
                        rxBuffer[rxByteIndex] = (byte)serialPort.ReadByte();
                    }
                    catch 
                    {
                        string st = "Код ошибки: 0x05 ";
                        if (ModBusClient.Tresh) { st = st + "00"; } else { st = st + "0x01"; }
                        if (ModBusClient.portBusy) { st = st + "00"; } else { st = st + "0x01"; }
                        MessageBox.Show("Ошибка при работе с COM-портом!\n" + st + "\nПриложение будет закрыто", "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
                else { break; }
                rxByteIndex++;

                if (!closeProcess)
                {
                    if (serialPort.IsOpen) { i = serialPort.BytesToRead; }
                    else { b = true; break; }
                }
                else { i = 0; }
                
            }
            readProcess = false;

            if (initPortClose)
            {
                rxByteIndex = 0;
                Close();
                return;
            }

            if ((b)||(!nowPortOpen))//Порт закрыли во время приема
            {
                ReceiveComPort(this, new EventArgs());
                rxByteIndex = 0;
                return;
            }

            if (rxByteIndex < receiveThreshold) { return; }
            if (ReceiveComPort != null) ReceiveComPort(this, new EventArgs());
            rxByteIndex = 0;
        }

        public ComPortNET()
        {
            #region SerialSetup
            serialPort = new SerialPort();
            serialPort.BaudRate = 9600;
            serialPort.PortName = "COM1";
            serialPort.Parity = System.IO.Ports.Parity.Odd;
            serialPort.DataBits = 8;
            serialPort.StopBits = System.IO.Ports.StopBits.One;
            serialPort.Handshake = System.IO.Ports.Handshake.None;
            serialPort.DtrEnable = false;
            serialPort.RtsEnable = false;
            serialPort.ParityReplace = 63;
            serialPort.ReadBufferSize = 4096;
            serialPort.ReadTimeout = 500;
            serialPort.WriteBufferSize = 2048;
            serialPort.WriteTimeout = 500;

            serialPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            #endregion

        }

        byte receiveThreshold = 40;
        public byte ReceiveThreshold
        {
            get { return receiveThreshold; }
            set { receiveThreshold = value; }

        }

        byte[] rxBuffer = new byte[120];
        public byte[] RxBuffer
        {
            set { rxBuffer = value; }
            get { return rxBuffer; }
        }

        byte rxByteIndex = 0;

        public void PurgeRxBufferComm()
        {
            rxByteIndex = 0;
        }

        public void Open()
        {
            initPortClose = false;
            PurgeRxBufferComm();
            serialPort.Open();
            byte b;
            while (serialPort.BytesToRead != 0)
            {
                b = (byte)serialPort.ReadByte();
            }

            enaPortRecieve = true;
            nowPortOpen = true;
        }

        public void Close()
        {
            if (serialPort.BytesToRead != 0) { initPortClose = true; return; }
            if (readProcess) { initPortClose = true; return; }
            closeProcess = true;
            serialPort.Close();
            nowPortOpen = false;
            closeProcess = false;
            enaPortRecieve = false;
        }

        public void ChangeComPortSettings(byte newSpeed, byte newParity, byte newPortIndex)
        {
            if (CheckOnline()) { return; }
            if (serialPort.IsOpen) { return; }

            switch (newSpeed)
            {
                case 0: { serialPort.BaudRate = 9600; } break;
                case 1: { serialPort.BaudRate = 19200; } break;
                case 2: { serialPort.BaudRate = 38400; } break;
                case 3: { serialPort.BaudRate = 57600; } break;
                case 4: { serialPort.BaudRate = 115200; } break;
                case 5: { serialPort.BaudRate = 230400; } break;
            }

            switch (newParity)
            {
                case 0: { serialPort.Parity = System.IO.Ports.Parity.Odd; serialPort.StopBits = System.IO.Ports.StopBits.One; } break;
                case 1: { serialPort.Parity = System.IO.Ports.Parity.Even; serialPort.StopBits = System.IO.Ports.StopBits.One; } break;
                case 2: { serialPort.Parity = System.IO.Ports.Parity.None; serialPort.StopBits = System.IO.Ports.StopBits.Two; } break;
            }

            serialPort.PortName = "COM" + newPortIndex.ToString();
        }

        byte ErrorMAssageBox = 0;

        public void Send(byte[] tosend, byte sendCount)
        {
            if (!nowPortOpen) { return; }
            if (!serialPort.IsOpen) { return; }
            try { serialPort.Write(tosend, 0, sendCount); }
            catch
            {
                if (ErrorMAssageBox == 0){
                    ErrorMAssageBox++;
                     MessageBox.Show("Ошибка при запись в COM-порт! Приложение будет закрыто!\n" +
                     "Код ошибки 0x03-" + ModBusClient.Tresh.ToString() + "  " + ModBusClient.portBusy.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     ErrorMAssageBox = 0;
                     Application.Exit();
                }
                //Application.Exit();
            }
        }
    }

}
