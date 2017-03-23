using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModBusLibrary;

namespace ADSPLibrary
{
    public partial class MeasurePanelSTS : UserControl, IMeasurePanel
    {
        bool formLoaded = false;

        double uteCoef = 1;

        MeasureInstrumPanel statorVoltagePanel, statorCurrentPanel, rotorCurrentPanel,
                            activePowerPanel, reactivePowerPanel,
                            cosFiPanel, utePanel, rotorVoltagePanel;

        ModBusUnit modBusUnit;
        ModBusUnit modBusUnit2;
        ModBusUnit modBusUnit3;

        bool processed = false;
        bool processed2 = false;

        private ushort[] measureValues;
        private ushort[] measureValues2;

        private ushort lamps = 0;
        private ushort oldlamps = 0;

        private ushort nowARMType = 0;
        private int nowARMIndex = 0;

        ListForm readyListForm;
        ListForm alarmListForm;
        ListForm warnListForm;
        ushort[] readyMasks;
        ushort[] warnMasks;
        ushort[] alarmMasks;

 
        public MeasurePanelSTS()
        {
            
            InitializeComponent();

            this.AutoSize = false;
            this.Height = 760;
            this.Width = 927;

            modBusUnit = new ModBusUnit();
            modBusUnit.RequestFinished += new EventHandler(EndRequest);

            modBusUnit2 = new ModBusUnit();
            modBusUnit2.RequestFinished += new EventHandler(EndRequest2);

            modBusUnit3 = new ModBusUnit();
            modBusUnit3.RequestFinished += new EventHandler(EndRequest3);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!formLoaded) { return; }
            if (!this.Parent.Visible) { return; }
            if (processed) { return; }
           
            if (measureValues != null)
            {
                statorVoltagePanel.DrawValue(((short)measureValues[0x00]) / 40.96);
                statorCurrentPanel.DrawValue(((short)measureValues[0x01]) / 40.96);
                rotorCurrentPanel.DrawValue((short)measureValues[0x02]/40.96);
                activePowerPanel.DrawValue((short)measureValues[0x03] / 40.96);
                reactivePowerPanel.DrawValue((short)measureValues[0x04] / 40.96);
                utePanel.DrawValue(uteCoef*((short)measureValues[0x05]));
                rotorVoltagePanel.DrawValue((short)measureValues[0x06] / 40.96);
                cosFiPanel.DrawPF((short)measureValues[0x03], (short)measureValues[0x04]);
            }
            measureValues = new ushort[8];
            processed = true;
            modBusUnit.GetData((ushort)(0x2800), 8);
        }

        private void EndRequest(object sender, EventArgs e)
        {
            if (!modBusUnit.modBusData.RequestError)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (measureValues!=null)
                    measureValues[i] = modBusUnit.modBusData.ReadData[i];
                }
            }
            processed = false;
        }

        private void EndRequest2(object sender, EventArgs e)
        {
            if (!modBusUnit2.modBusData.RequestError)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (measureValues2 != null)
                    measureValues2[i] = modBusUnit2.modBusData.ReadData[i];
                }
            }
            processed2 = false;

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!formLoaded) { return; }
            if (!this.Parent.Visible) { return; }
            if (processed2) { return; }

            if (measureValues2 != null)
            {
                UpdateButtons(measureValues2[0], measureValues2[1]);
                UpdateRefReg();
                UpdateListForms();
            }
            measureValues2 = new ushort[12];
            processed2 = true;
            modBusUnit2.GetData((ushort)(0x2810), 12);
        }

        void UpdateButtons(ushort newLamps, ushort newARMType)
        {
            lamps = newLamps;

            if ((lamps == oldlamps)&&( newARMType == nowARMType)) {return; }
            nowARMType = newARMType;
            
            //Готовность
            if ((lamps & 0x0001) != 0) { lampLabel1.BackColor = Color.PaleGreen; }
            else { lampLabel1.BackColor = Color.White; }

            //Работа
            if ((lamps & 0x0002) != 0) { lampLabel2.BackColor = Color.PaleGreen; }
            else { lampLabel2.BackColor = Color.White; }

            //В сети
            if ((lamps & 0x0004) != 0) { lampLabel3.BackColor = Color.PaleGreen; }
            else { lampLabel3.BackColor = Color.White; }

            //РРВ
            if ((lamps & 0x0008) != 0) { lampLabel4.BackColor = Color.Yellow; }
            else { lampLabel4.BackColor = Color.White; }

            //ОГРАНИЧИТЕЛЬ
            if ((lamps & 0x0010) != 0) { lampLabel5.BackColor = Color.Yellow; }
            else { lampLabel5.BackColor = Color.White; }
            
            //Нет связи
            
            //Предупреждение
            if ((lamps & 0x0040) != 0) { lampLabel6.BackColor = Color.Yellow; }
            else { lampLabel6.BackColor = Color.White; }

            //Авария
            if ((lamps & 0x0080) != 0) { lampLabel7.BackColor = Color.Red; }
            else { lampLabel7.BackColor = Color.White; }
            oldlamps = lamps;


            if ((lamps & 0x0008) != 0)
            {
                btnARMIre.BackColor = btnARMPF.BackColor = btnARMU.BackColor = Color.WhiteSmoke;
                btnARMMAN.BackColor = Color.PaleGreen;
                refLabel.Text = "Уставка, %";
                nowARMIndex = 3;
            }
            else
            {
                switch (nowARMType)
                {
                    case 1:
                        {
                            btnARMIre.BackColor = btnARMMAN.BackColor = btnARMU.BackColor = Color.WhiteSmoke;
                            btnARMPF.BackColor = Color.PaleGreen;
                            refLabel.Text = "Уставка, °";
                            nowARMIndex = 1;

                        }break;

                    case 2:
                        {
                            btnARMPF.BackColor = btnARMMAN.BackColor = btnARMU.BackColor = Color.WhiteSmoke;
                            btnARMIre.BackColor = Color.PaleGreen;
                            refLabel.Text = "Уставка, %";
                            nowARMIndex = 2;
                        }break;

                    default:
                        {
                            btnARMPF.BackColor = btnARMMAN.BackColor = btnARMIre.BackColor = Color.WhiteSmoke;
                            btnARMU.BackColor = Color.PaleGreen;
                            refLabel.Text = "Уставка, %";
                            nowARMIndex = 0;
                        }break;
                }
            }
        }

        void UpdateRefReg()
        {
            if (nowARMIndex != 1)
            {
                refValueLabel.Text = AdvanceConvert.HexToPercent(measureValues2[2 + nowARMIndex], 1);
            }
            else
            {
                refValueLabel.Text = "";
            }

        }

        void UpdateListForms()
        {
            int i;

            try
            {
                for (i = 0; i < EventStructFormat.readyList.Count; i++)
                {
                    readyMasks[0 + i] = measureValues2[6 + i];
                }


                for (i = 0; i < EventStructFormat.warningsList.Count; i++)
                {
                    warnMasks[0 + i] = measureValues2[8 + i];
                }


                alarmMasks[0] = measureValues2[10];
                alarmMasks[1] = measureValues2[11];


            }
            catch { return; }
            readyListForm.UpdateList(readyMasks);
            warnListForm.UpdateList(warnMasks);
            alarmListForm.UpdateList(alarmMasks);
        }

        private void LampLabel1_Click(object sender, EventArgs e)
        {
            readyListForm.Left = this.Parent.Left + 20;
            readyListForm.Top = this.Parent.Top + 20;
            readyListForm.Show();
            readyListForm.TopMost = true;
            readyListForm.TopMost = false;
        }
      
        private void lampLabel6_Click(object sender, EventArgs e)
        {
            warnListForm.Left = this.Parent.Left + 20;
            warnListForm.Top = this.Parent.Top + 20;
            warnListForm.Show();
            warnListForm.TopMost = true;
            warnListForm.TopMost = false;
        }

        private void LampLabel7_Click(object sender, EventArgs e)
        {
            alarmListForm.Left = this.Parent.Left + 20;
            alarmListForm.Top = this.Parent.Top + 20;
            alarmListForm.Show();
            alarmListForm.TopMost = true;
            alarmListForm.TopMost = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
            
                    
        }

        private void MeasurePanelSTS_Load(object sender, EventArgs e)
        {
            formLoaded = false;

            string[] readyStrings = new string[EventStructFormat.readyList.Count * 16];
            for (int i = 0; i < (EventStructFormat.readyList.Count); i++)
            {
                for (int i1 = 0; i1 < 16; i1++)
                {
                    readyStrings[i * 16 + i1] = EventStructFormat.readyList[i].NameStrings[i1];
                }
            }
            readyListForm = new ListForm(EventStructFormat.readyList.Count, readyStrings, "Причины неготовности");
            readyMasks = new ushort[EventStructFormat.readyList.Count];

            string[] warnStrings = new string[EventStructFormat.warningsList.Count * 16];
            for (int i = 0; i < (EventStructFormat.warningsList.Count); i++)
            {
                for (int i1 = 0; i1 < 16; i1++)
                {
                    warnStrings[i * 16 + i1] = EventStructFormat.warningsList[i].NameStrings[i1];
                }
            }
            warnListForm = new ListForm(EventStructFormat.warningsList.Count, warnStrings, "Предупреждения");
            warnMasks = new ushort[EventStructFormat.warningsList.Count];

            string[] alarmStrings = new string[EventStructFormat.alarmList.Count * 16];
            for (int i = 0; i < (EventStructFormat.alarmList.Count); i++)
            {
                for (int i1 = 0; i1 < 16; i1++)
                {
                    alarmStrings[i * 16 + i1] = EventStructFormat.alarmList[i].NameStrings[i1];
                }
            }
            alarmListForm = new ListForm(EventStructFormat.alarmList.Count, alarmStrings, "Аварии");
            alarmMasks = new ushort[EventStructFormat.alarmList.Count];


            Image instrImage;
            instrImage = ADSPLibrary.Properties.Resources.Scale_Corel_0_120;
            statorVoltagePanel = new MeasureInstrumPanel(0, 120, "Напряжение статора", instrImage, 1, "%", 0, "В", 63);
            statorVoltagePanel.Top = 5;
            statorVoltagePanel.Left = 5;
            panel1.Controls.Add(statorVoltagePanel);


            instrImage = ADSPLibrary.Properties.Resources.Scale_Corel_0_120;
            statorCurrentPanel = new MeasureInstrumPanel(0, 120, "Ток статора", instrImage, 1, "%", 0, "A", 10);
            statorCurrentPanel.Top = 215;
            statorCurrentPanel.Left = 5;
            panel1.Controls.Add(statorCurrentPanel);

            instrImage = ADSPLibrary.Properties.Resources.Scale_Corel_0_300;
            rotorCurrentPanel = new MeasureInstrumPanel(0, 300, "Ток ротора", instrImage, 1, "%", 0, "А", 20);
            rotorCurrentPanel.Top = 5;
            rotorCurrentPanel.Left = 310;
            panel1.Controls.Add(rotorCurrentPanel);

            instrImage = ADSPLibrary.Properties.Resources.Scale_Corel_0_120;
            activePowerPanel = new MeasureInstrumPanel(0, 120, "Активная мощность", instrImage, 1, "%", 1, "МВт", 2.50);
            activePowerPanel.Top = 5;
            activePowerPanel.Left = 615;
            panel1.Controls.Add(activePowerPanel);

            instrImage = ADSPLibrary.Properties.Resources.Scale_Corel__120_120;
            reactivePowerPanel = new MeasureInstrumPanel(-120, 120, "Реактивная мощность", instrImage, 1, "%", 1, "МВар", 2.50);
            reactivePowerPanel.Top = 215;
            reactivePowerPanel.Left = 615;
            panel1.Controls.Add(reactivePowerPanel);

            instrImage = ADSPLibrary.Properties.Resources.Scale_Corel_PF;
            cosFiPanel = new MeasureInstrumPanel(-66.42, 66.42, "Коэффициент мощности", instrImage, 1, "");
            cosFiPanel.Top = 425;
            cosFiPanel.Left = 615;
            panel1.Controls.Add(cosFiPanel);

            instrImage = ADSPLibrary.Properties.Resources.Scale_Corel_0_120;
            utePanel = new MeasureInstrumPanel(0, 120, "Напряжение синхронизации", instrImage, 1, "%", 0, "В", 3);
            utePanel.Top = 425;
            utePanel.Left = 5;
            panel1.Controls.Add(utePanel);

            instrImage = ADSPLibrary.Properties.Resources.Scale_Corel_0_300;
            rotorVoltagePanel = new MeasureInstrumPanel(0, 300, "Напряжение ротора", instrImage, 1, "%", 0, "В", 3);
            rotorVoltagePanel.Top = 215;
            rotorVoltagePanel.Left = 310;
            panel1.Controls.Add(rotorVoltagePanel);
            GetNominalParams();
        }

        private void EndRequest3(object sender, EventArgs e)
        {
            int nomI, nomU, nomUf, nomUte;
            int nomIfSys, kIf;
            double nomIf;
            double nomPower;

            if (!modBusUnit3.modBusData.RequestError)
            {
                nomU = (int)modBusUnit3.modBusData.ReadData[0];
                nomI = (int)modBusUnit3.modBusData.ReadData[1];
                nomPower = 1.73 * nomI * nomU / 1000000.0;
                
                nomUf = (int)modBusUnit3.modBusData.ReadData[2];
                nomIfSys = (int)modBusUnit3.modBusData.ReadData[6];
                kIf = (int)modBusUnit3.modBusData.ReadData[5];

                try
                {
                    nomIf = ((double)(nomIfSys) * 512.0) / ((double)kIf);
                }
                catch
                {
                    timer1.Enabled = false;
                    timer2.Enabled = false;
                    return;
                }

                try
                {
                    nomUte = (int)modBusUnit3.modBusData.ReadData[3];
                    uteCoef = 100.0 / (double)(nomUte);
                }
                catch
                {
                    timer1.Enabled = false;
                    timer2.Enabled = false;
                    return;
                }

                try
                {
                    rotorCurrentPanel.UpdateCoefAbsolute(nomIf / 100.0);

                    statorVoltagePanel.UpdateCoefAbsolute(nomU / 100.0);
                    statorCurrentPanel.UpdateCoefAbsolute(nomI / 100.0);

                    activePowerPanel.UpdateCoefAbsolute(nomPower / 100.0);
                    reactivePowerPanel.UpdateCoefAbsolute(nomPower / 100.0);
                    rotorVoltagePanel.UpdateCoefAbsolute(nomUf / 1000.0);

                    utePanel.UpdateCoefAbsolute(0.1 / uteCoef);
                }
                catch { }
                formLoaded = true;
            }
            else
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
            }
        }

        private void GetNominalParams()
        {
            modBusUnit3.GetData(0x0540, 8);

        }

        private void btnARMU_Click(object sender, EventArgs e)
        {

        }

        private void MeasurePanelSTS_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                timer1.Enabled = true;
                timer2.Enabled = true;
            }
        }

        //Методы интерфейса
        #region InterFaceMethods

        public int NeedWidth
        {
            get { return this.Width; }
            set { }
        }

        public int NeedHeight
        {
            get { return this.Height; }
            set { }         
        }

        public bool ResizePossible
        {
            get { return false; }
            set { }

        }

        public void InitModBusUnits()
        {
            modBusUnit = new ModBusUnit();
            modBusUnit.RequestFinished += new EventHandler(EndRequest);

            modBusUnit2 = new ModBusUnit();
            modBusUnit2.RequestFinished += new EventHandler(EndRequest2);

            modBusUnit3 = new ModBusUnit();
            modBusUnit3.RequestFinished += new EventHandler(EndRequest3);
        }

        #endregion



    }
}
