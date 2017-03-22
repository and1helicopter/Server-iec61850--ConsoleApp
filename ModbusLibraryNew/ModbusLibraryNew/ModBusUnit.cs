using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModbusLibraryNew
{
    public class ModBusUnit
    {
        
        public string Name { get; set; }

        Timer timer;
        int waitCounter;

        public bool NoResponce { get; set; }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (waitCounter > 0) { waitCounter--; }
            if (waitCounter == 1)
            {
                NoResponce = true;
            }
        }


        public ModBusUnit()
        {
            waitCounter = 0;
            timer = new Timer();
            timer.Enabled = true;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 100;
            

            UnitBusy = false;
            NoResponce = false;
            RxBuffer = new byte[0];
            DataOk = false;
            ModbusSerialPort.ComPortClosed += new EventHandler(ModbusSerialPort_ComPortClosed);
        }

        public delegate void OnDataReceivedDelegate(bool DataOk, byte[] RxBuffer);
        public OnDataReceivedDelegate OnDataReceived;
        public bool UnitBusy { get; set; }
        public byte[] RxBuffer { get; set; }
        public bool DataOk { get; set; }

        private void DataReceived()
        {

            if (ModbusSerialPort.DataOk)
            {
                DataOk = ModbusSerialPort.DataOk;
                RxBuffer = new byte[ModbusSerialPort.RxBuffer.Length];
                for (int i = 0; i < ModbusSerialPort.RxBuffer.Length; i++)
                {
                    RxBuffer[i] = ModbusSerialPort.RxBuffer[i];
                }
            }
            else
            {
                DataOk = ModbusSerialPort.DataOk;
                RxBuffer = new byte[0];
            }

            UnitBusy = false;
            OnDataReceived(DataOk, RxBuffer);
        }
        public void SendRequest(byte[] TxBuffer, byte TxCount, byte ThreadOld)
        {
            if (!ModbusSerialPort.IsOpen) { return; }
            if (UnitBusy)
            {
                return;
            }

            UnitBusy = true;
            waitCounter = 300;


            ModBusDataClass newModBusData = new ModBusDataClass();
            newModBusData.TxBuffer = TxBuffer;
            newModBusData.TxCount = TxCount;
            newModBusData.ThreadOld = ThreadOld;
            newModBusData.OnDataReceived = DataReceived;

            ModBusRequestQueue.AddRequest(newModBusData);
        }

        private void ModbusSerialPort_ComPortClosed(object sender, EventArgs e)
        {
            UnitBusy = false;
            RxBuffer = new byte[0];
            DataOk = false;

            if (OnDataReceived != null)
            {
                UnitBusy = false;
                try
                {
                    OnDataReceived(DataOk, RxBuffer);
                }
                catch { }
            }
        }

    }
}
