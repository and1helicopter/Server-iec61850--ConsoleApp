using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;


namespace ModBusLibrary
{
    public static class ModBusClient
    {
        private static int currentAddr=1;
        private static int currentPortParity = 0;
        private static int currentPortSpeed = 0;

        public static int CurrentAddr { get { return currentAddr; } set { } }
        public static int CurrentPortParity { get { return currentPortParity; } set { } }
        public static int CurrentPortSpeed { get { return currentPortSpeed; } set { } }


        
        private static System.Windows.Forms.Timer timer;
        private static bool linkOk = false;

        public static bool LinkOk { get { return linkOk; } set { } }

        private static ModBusUnit modBusUnit;

        private static void EndCheckLinkRequest(object sender, EventArgs e)
        {
            if (!modBusUnit.modBusData.RequestError) { linkOk = true; }
            else { linkOk = false; } 
        }

        private static void timer_Tick(object sender, EventArgs e)
        {
            if (!modBusOpened) { linkOk = false; return; }
            if (modBusUnit.lineBusy) { return; }
            modBusUnit.GetData(0x3FFF, 1);
        }

        public static bool Tresh = false;

        public static List<ModBusUnit> ModBusUnitList = new List<ModBusUnit>();

        static ModBus modBus = new ModBus();

        public static void InitModBusEvent()
        {
            modBus.RequestFinished += new EventHandler(EndRequest);
            #region TimerSetup
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = true;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 250;
            #endregion
            InitModBusUnit();
        }

        public static void InitModBusUnit()
        {
            modBusUnit = new ModBusUnit();
            modBusUnit.RequestFinished += new EventHandler(EndCheckLinkRequest);

        }
        static Queue<int> modBusIndexQueue = new Queue<int>();

        static public bool portBusy = false;
        static bool modBusOpened = false;
        static public bool ModBusOpened
        {
            get { return modBusOpened; }
            set { }
        }

        static bool initModBusClose = false;
        static bool closeClicked = false;

        public static bool ModBusLocked = false;

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

        static ModBusDataClass sendModBusData = new ModBusDataClass(0);

        static void SetLinkPossible()
        {
            int i;
            for (i = 0; i < ModBusUnitList.Count; i++) { ModBusUnitList[i].PortOpened = true; ModBusUnitList[i].lineBusy = false; }
        }

        public static void OpenModBusPort(ushort newAddr, byte newPortIndex, byte newSpeed, byte newParity)
        {
            if (modBusOpened) 
            {
                string st="Код ошибки: 0x01";
                if (ModBusClient.Tresh) { st = st + "00"; } else { st = st + "0x01"; }
                if (ModBusClient.portBusy) { st = st + "00"; } else { st = st + "0x01"; }
                MessageBox.Show("Ошибка при работе с COM-портом!\n"+st+"\nПриложение будет закрыто","Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
            modBusIndexQueue = new Queue<int>();
            sendModBusData = null;
            if (modBus.comPort.CheckOnline()) { return; }
            modBus.addr = (ushort)newAddr;
            modBus.comPort.ChangeComPortSettings(newSpeed, newParity, newPortIndex);
            
            ModBusLocked = true;
                modBus.OpenModBusPort();
                portBusy = false;
                if (modBus.comPort.CheckOnline())
                {
                    initModBusClose = false;
                    SetLinkPossible();
                    modBusOpened = true;
                }
            ModBusLocked = false;

            currentAddr = newAddr;
            currentPortParity = newParity;
            currentPortSpeed = newSpeed;
        }

        static void ResetLinkPossible()
        {
            for (int i = 0; i < ModBusUnitList.Count; i++)
            {
                ModBusUnitList[i].PortOpened = false;
            }
        }

        public static event EventHandler PortClosed;
        public static void CloseModBusPortProcess()
        {
            initModBusClose = true;
            ResetLinkPossible();
            modBusIndexQueue.Clear();

            portBusy = false;
            modBusOpened = false;
            modBus.CloseModBusPort();
            CancelModBusRequest();
            if (PortClosed != null) PortClosed(null, new EventArgs());       
        }

        public static void CloseModBusPort()
        {
            closeClicked = true;
            return;
            /*if (!portBusy) { CloseModBusPortProcess(); }
            else
            {
                closeClicked = true;
            }*/
        }

        public static void NewRequest(object sender, EventArgs e)
        {
            if (ModBusLocked) { return; }
            ModBusLocked = true;
            if (initModBusClose) { ModBusLocked = false; return; }
            {
                lock (modBusIndexQueue)
                {
                    modBusIndexQueue.Enqueue((sender as ModBusUnit).UnitNum);
                }
            }
            if (portBusy) { ModBusLocked = false; return; }
            ModBusLocked = false;
            SendNewDataRTU();
        }

        public static void EndRequest(object sender, EventArgs e)
        {
            if (closeClicked) { closeClicked = false; CloseModBusPortProcess(); }         
            if (initModBusClose) { return; }

            if (sendModBusData == null) 
            {
                string st = "Код ошибки: 0x02";
                if (ModBusClient.Tresh) { st = st + "00"; } else { st = st + "0x01"; }
                if (ModBusClient.portBusy) { st = st + "00"; } else { st = st + "0x01"; }
                MessageBox.Show("Ошибка при работе с COM-портом!\n" + st + "\nПриложение будет закрыто", "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
            ModBusUnitList[sendModBusData.Tag].EndRequest(modBus.requestError, modBus.readDataRTU);
            portBusy = false;

            if (modBusIndexQueue.Count == 0) { return; }
            if (ModBusLocked) { return; }
            if (closeClicked) { closeClicked = false; CloseModBusPortProcess(); }
            SendNewDataRTU();
        }

        static void SendNewDataRTU()
        {
            int newIndex=0;
            if (!modBusOpened) { return; }
            if (portBusy) { return; } else {portBusy = true; }
           
           newIndex = modBusIndexQueue.Dequeue();

           sendModBusData = new ModBusDataClass(ModBusUnitList[newIndex].modBusData.Tag);
           sendModBusData.StartAddr = ModBusUnitList[newIndex].modBusData.StartAddr;
           sendModBusData.ReadCount = ModBusUnitList[newIndex].modBusData.ReadCount;
           sendModBusData.WriteCount = ModBusUnitList[newIndex].modBusData.WriteCount;
           sendModBusData.WriteData = ModBusUnitList[newIndex].modBusData.WriteData;
           sendModBusData.RequestType = ModBusUnitList[newIndex].modBusData.RequestType;

           if (closeClicked) { closeClicked = false; CloseModBusPortProcess(); return; }
           
           if (sendModBusData.RequestType == 0) { modBus.GetData(sendModBusData.StartAddr, sendModBusData.ReadCount); }
           if (sendModBusData.RequestType == 1) { modBus.SetData(sendModBusData.StartAddr, sendModBusData.WriteCount, sendModBusData.WriteData); }
        }

        static void CancelModBusRequest()
        {
            ushort[] buff = new ushort[80];
            for (int i = 0; i < ModBusUnitList.Count; i++)
            {
                ModBusUnitList[i].EndRequest(true, buff);
            }
        }
    }

}
