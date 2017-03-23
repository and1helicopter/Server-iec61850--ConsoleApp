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
    public partial class MeasurePanelPenza : UserControl
    {
        private ushort nomUstat = 6300;
        private ushort nomIstat = 700;
        private ushort nomIf = 2000;
        private ushort nomUf = 200;
        private ushort[] measureValues;
        private ushort lamps = 0;
        private ushort oldlamps = 0;

        ListForm readyListForm;
        ListForm alarmListForm;
        ListForm warnListForm;
        ushort[] readyMasks;
        ushort[] warnMasks;
        ushort[] alarmMasks;


        void UpdateButtons()
        {
            if (lamps == oldlamps) { return; }
            if ((lamps & 0x0001) != 0) { readyBtn.BackColor = Color.Lime; }
            else { readyBtn.BackColor = Color.White; }

            if ((lamps & 0x0002) != 0) { workBtn.BackColor = Color.Lime; }
            else { workBtn.BackColor = Color.White; }

            if ((lamps & 0x0008) != 0) { rrvBtn.BackColor = Color.Yellow; }
            else { rrvBtn.BackColor = Color.White; }

            if ((lamps & 0x0040) != 0) { warnBtn.BackColor = Color.Yellow; }
            else { warnBtn.BackColor = Color.White; }

            if ((lamps & 0x0080) != 0) { alarmBtn.BackColor = Color.Red; }
            else { alarmBtn.BackColor = Color.White; }
            oldlamps = lamps;

        }

        private string CalcU(ushort value)
        {
            int i = value;
            double f = i / 4096.0;
            if (f < 0.02) { return ("0"); }
            f = f * nomUstat;
            return (f.ToString("F0"));
        }
        private string CalcUf(ushort value)
        {
            int i = value;
            double f = i / 4096.0;
            if (f < 0.02) { return ("0"); }
            f = f * nomUf;
            return (f.ToString("F0"));
        }
        private string CalcI(ushort value)
        {
            int i = value;
            double f = i / 4096.0;
            if (f < 0.02) { return ("0"); }
            f = f * nomIstat;
            return (f.ToString("F0"));
        }
        private string CalcIf(ushort value)
        {
            int i = value;
            double f = i / 4096.0;
            if (f < 0.02) { return ("0"); }
            f = f * nomIf;
            return (f.ToString("F0"));
        }
        private string CalcPower(ushort value)
        {
            double f = 1.7321*nomUstat * nomIstat;
            f = (int)value * f / 40960000.0;
            return (f.ToString("F2"));
        }
        ModBusUnit modBusUnit;
        ushort requestStep = 0;
        bool processed = false;
        
        public MeasurePanelPenza()
        {
            InitializeComponent();
            modBusUnit = new ModBusUnit();
            modBusUnit.RequestFinished += new EventHandler(EndRequest);
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.Parent.Visible) { requestStep = 0; return; }
            if (processed) { return; }
            if (requestStep == 0) { measureValues = new ushort[EventStructFormat.eventLogCountBlocks * 8]; }
            processed = true;
            if (requestStep < EventStructFormat.eventLogCountBlocks)
            {
                modBusUnit.GetData((ushort)(0x0200 + (requestStep) * 8), 8);
            }
            else
            {
                modBusUnit.GetData(0x042C, 1);
            }
        }

        private void EndRequest(object sender, EventArgs e)
        {
            if (!modBusUnit.modBusData.RequestError)
            {
                if (requestStep< EventStructFormat.eventLogCountBlocks)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        measureValues[i + requestStep * 8] = modBusUnit.modBusData.ReadData[i];
                    }
                }
                requestStep++;
                if (requestStep == EventStructFormat.eventLogCountBlocks)
                {
                    valueLabel1.Text = CalcU(measureValues[0x09]);
                    valueLabel2.Text = CalcI(measureValues[0x0A]);
                    valueLabel3.Text = CalcPower(measureValues[0x0B]);
                    valueLabel4.Text = CalcPower(measureValues[0x0c]);
                    valueLabel5.Text = CalcI(measureValues[0x0D]);
                    valueLabel6.Text = CalcI(measureValues[0x0E]);
                    valueLabel7.Text = CalcIf(measureValues[0x10]);
                    valueLabel8.Text = CalcUf(measureValues[0x25]);

                    readyMasks[0] = 0;                  readyListForm.UpdateList(readyMasks);
                    warnMasks[0] = measureValues[0x21]; warnListForm.UpdateList(warnMasks);
                    alarmMasks[0] = measureValues[0x22];
                    alarmMasks[1] = measureValues[0x23];alarmListForm.UpdateList(alarmMasks);
                }
                if (requestStep == (EventStructFormat.eventLogCountBlocks + 1))
                {
                    lamps = modBusUnit.modBusData.ReadData[0];
                    UpdateButtons();
                    requestStep = 0; 
                }

            }
            else
            {
                requestStep = 0;
            }

            processed = false;
        }

        private void alarmBtn_Click(object sender, EventArgs e)
        {
            alarmListForm.Left = this.Parent.Left + 20;
            alarmListForm.Top = this.Parent.Top + 20;
            alarmListForm.Show();
            alarmListForm.TopMost = true;
            alarmListForm.TopMost = false;
        }

        private void warnBtn_Click(object sender, EventArgs e)
        {
            warnListForm.Left = this.Parent.Left + 20;
            warnListForm.Top = this.Parent.Top + 20;
            warnListForm.Show();
            warnListForm.TopMost = true;
            warnListForm.TopMost = false;
        }

        private void MeasurePanelPenza_Resize(object sender, EventArgs e)
        {
            Font font;
            if (Width < 1000)
            {
                font = new Font("Arial", 15, FontStyle.Bold | FontStyle.Italic);
            }
            else
            {

                font = new Font("Arial", 25, FontStyle.Bold | FontStyle.Italic);

            }
            label1.Font = font;

            if (Width < 1000)
            {
                font = new Font("Arial", 12, FontStyle.Bold);
            }
            else
            {

                font = new Font("Arial", 20, FontStyle.Bold);

            }
        }

        private void readyBtn_Click(object sender, EventArgs e)
        {
            readyListForm.Left = this.Parent.Left + 20;
            readyListForm.Top = this.Parent.Top + 20;
            readyListForm.Show();
            readyListForm.TopMost = true;
            readyListForm.TopMost = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void MeasurePanelPenza_Load(object sender, EventArgs e)
        {
            string[] readyStrings = new string[EventStructFormat.readyList.Count * 16];
            for (int i =0; i<(EventStructFormat.readyList.Count); i++)
            {
                for (int i1=0; i1<16; i1++)
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
            alarmListForm = new ListForm(EventStructFormat.alarmList.Count, alarmStrings,"Аварии");
            alarmMasks = new ushort[EventStructFormat.alarmList.Count];
        }

    }
}
