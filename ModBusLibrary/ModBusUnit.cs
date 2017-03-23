using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModBusLibrary
{
    public class ModBusUnit
    {
        public event EventHandler RequestFinished;
        public event EventHandler NewRequest;
   
        public bool lineBusy = false;
        
        bool portOpened = false;
        public bool PortOpened
        {
            get { return portOpened; }
            set { portOpened = value; }
        }
        
        int unitNum = 0;
        public int UnitNum
        {
            get { return unitNum; }
            set { unitNum = value; }
        }
        public ModBusDataClass modBusData;
        
        static int unitIndex = 0;

        public int UnitIndex
        {
            get { return unitIndex; }
            set { }
        }

        public void ClearUnitIndex()
        {
            unitIndex = 1;
        }

        public ModBusUnit()
        {
            unitNum = unitIndex;
            unitIndex++;
            ModBusClient.ModBusUnitList.Add(this);
            this.NewRequest += new EventHandler(ModBusClient.NewRequest);

            modBusData = new ModBusDataClass(unitNum);
        }

        public void EndRequest(bool ErrorLink, ushort[] newRxBuffer)
        {
            int i;
            if (!ErrorLink)
            {
                modBusData.RequestError = false;
                if (modBusData.RequestType == 0)
                {
                    for (i = 0; i < modBusData.ReadCount; i++)
                    {
                        modBusData.ReadData[i] = newRxBuffer[i];
                    }
                }
                else
                {

                }
            }
            else
            {
                modBusData.RequestError = true;
            }

            lineBusy = false;
            if (RequestFinished != null) RequestFinished(this, new EventArgs());
        }
        
        public void GetData(ushort addr, byte count)
        {

            if (!portOpened) { return; }
            if (lineBusy) { return; } else { lineBusy = true; }
            modBusData.RequestType = 0;
            modBusData.StartAddr = addr;
            modBusData.ReadCount = (ushort)count;

            while (ModBusClient.ModBusLocked) { }
            if (NewRequest != null) NewRequest(this, new EventArgs());
        }

        public void SetData(ushort addr, byte count, ushort[] newTxBuffer)
        {
            if (!portOpened) { return; }                                //Порт закрыт   
            if (lineBusy) { return; } else { lineBusy = true; }         //Линия занята
            modBusData.RequestType = 1;                                 //Тип запроса 
            modBusData.StartAddr = addr;                                //Начальный адресс
            modBusData.WriteCount = (ushort)count;                      //Колличество данных 
            for (int i = 0; i < (int)count; i++)
            {
                modBusData.WriteData[i] = newTxBuffer[i];
            }

            while (ModBusClient.ModBusLocked) { }
            if (NewRequest != null) NewRequest(this, new EventArgs());
        }
    }

}
