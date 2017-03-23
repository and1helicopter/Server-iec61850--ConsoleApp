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
    public partial class MeasurePanelLipetsk : UserControl
    {
        int standartWidth = 615;
        public int StandartWidth { set { } get { return (standartWidth); } }
        int standartHeight = 550;
        public int StandartHeight { set { } get { return (standartHeight); } }

        public bool ResizePossible { set { } get { return (false); } }

        Label[] headValueLabels, valueLabels, unitlabels;
        ModBusUnit modBusUnit;
        private ushort[] measureValues;
        ushort requestStep = 0;
        bool processed = false;

        ListForm readyListForm;
        ListForm alarmListForm;
        ListForm warnListForm;
        ushort[] readyMasks;
        ushort[] warnMasks;
        ushort[] alarmMasks;

        ushort nowLamps = 0;

        public MeasurePanelLipetsk()
        {
            InitializeComponent();
            modBusUnit = new ModBusUnit();
            modBusUnit.RequestFinished += new EventHandler(EndRequest);
        }

        private void MeasurePanelLipetsk_Load(object sender, EventArgs e)
        {
            int startY = 10 + flowLayoutPanel1.Height + titlLabel.Height+2;

            headValueLabels = new Label[EventStructFormat.measureParams.Count];
            valueLabels = new Label[EventStructFormat.measureParams.Count];
            unitlabels = new Label[EventStructFormat.measureParams.Count];

            Font headFont = new Font("Arial", 12);
            Font valueFont = new Font("Arial", 12,FontStyle.Bold);

            for (int i = 0; i < EventStructFormat.measureParams.Count; i++)
            {
                headValueLabels[i] = new Label();
                headValueLabels[i].AutoSize = false;
                headValueLabels[i].Width = 250;
                headValueLabels[i].Height = 20;
                headValueLabels[i].Left = 5;
                headValueLabels[i].Top = startY + 25 * i;
                headValueLabels[i].Text = EventStructFormat.measureParams[i].paramName;
                headValueLabels[i].Font = headFont;
                this.Controls.Add(headValueLabels[i]);

                valueLabels[i] = new Label();
                valueLabels[i].AutoSize = false;
                valueLabels[i].Width = 70;
                valueLabels[i].Height = 20;
                valueLabels[i].Left = 320;
                valueLabels[i].Top = startY + 25 * i;
                valueLabels[i].Text = "0,0";
                valueLabels[i].BorderStyle = BorderStyle.Fixed3D;
                valueLabels[i].Font = valueFont;
                this.Controls.Add(valueLabels[i]);


                unitlabels[i] = new Label();
                unitlabels[i].AutoSize = false;
                unitlabels[i].Width = 250;
                unitlabels[i].Height = 20;
                unitlabels[i].Left = 400;
                unitlabels[i].Top = startY + 25 * i;
                unitlabels[i].Text = EventStructFormat.measureParams[i].paramUnitName;
                unitlabels[i].Font = headFont;
                this.Controls.Add(unitlabels[i]);
            }

            standartHeight = headValueLabels[EventStructFormat.measureParams.Count - 1].Top+25;
            Height = standartHeight;
            measureValues = measureValues = new ushort[EventStructFormat.eventLogCountBlocks * 8];

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

        }

        private string CalcMeasureParam(int paramNum, ushort paramValue)
        {
            string str;
            byte format;
            format = EventStructFormat.measureParams[paramNum].paramFormat;
            str = AdvanceConvert.HexToFormat(paramValue, format);
            return (str);
        }

        delegate void NoParamDelegate();

        private void UpdataLamps(ushort newLamps)
        {
            if (newLamps == nowLamps) { return; }
            nowLamps = newLamps;

            if ((nowLamps & 0x0001) != 0) { lampLabel1.BackColor = Color.Lime; }
            else { lampLabel1.BackColor = Color.White; }

            if ((nowLamps & 0x0002) != 0) { lampLabel2.BackColor = Color.Lime; }
            else { lampLabel2.BackColor = Color.White; }

            if ((nowLamps & 0x0008) != 0) { lampLabel3.BackColor = Color.Yellow ; }
            else { lampLabel3.BackColor = Color.White; }

            if ((nowLamps & 0x0040) != 0) { lampLabel4.BackColor = Color.Yellow; }
            else { lampLabel4.BackColor = Color.White; }

            if ((nowLamps & 0x0080) != 0) { lampLabel5.BackColor = Color.Red; }
            else { lampLabel5.BackColor = Color.White; }

        }
        private void UpdateData()
        {
            int i = 0;
            ushort data = 0;

            for (i = 0; i < EventStructFormat.measureParams.Count; i++)
            {
                data = measureValues[EventStructFormat.measureParams[i].paramAddr];
                valueLabels[i].Text = CalcMeasureParam(i, data);
            }
            UpdataLamps(measureValues[0x14]);
        }
        private void UpdateDataInvoke()
        {
            if (!this.Visible) { return; }
            Invoke(new NoParamDelegate(UpdateData), null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.Parent.Visible) { requestStep = 0; return; }
            if (processed) { return; }
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
                if (requestStep < EventStructFormat.eventLogCountBlocks)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        measureValues[i + requestStep * 8] = modBusUnit.modBusData.ReadData[i];
                    }
                }
                requestStep++;
                if (requestStep == EventStructFormat.eventLogCountBlocks)
                {
                    UpdateDataInvoke();

                    readyMasks[0] = measureValues[EventStructFormat.readyList[0].EventPos]; 
                    readyListForm.UpdateList(readyMasks);

                    warnMasks[0] = measureValues[EventStructFormat.warningsList[0].EventPos]; 
                    warnListForm.UpdateList(warnMasks);

                    alarmMasks[0] = measureValues[EventStructFormat.alarmList[0].EventPos];
                    alarmMasks[1] = measureValues[EventStructFormat.alarmList[1].EventPos]; 
                    alarmListForm.UpdateList(alarmMasks);
                    requestStep = 0; ;
                }


            }
            else
            {
                requestStep = 0;
            }

            processed = false;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            warnListForm.Show();
            warnListForm.TopMost = true;
            warnListForm.TopMost = false;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            alarmListForm.Show();
            alarmListForm.TopMost = true;
            alarmListForm.TopMost = false;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            readyListForm.Show();
            readyListForm.TopMost = true;
            readyListForm.TopMost = false;
        }



    }
}
