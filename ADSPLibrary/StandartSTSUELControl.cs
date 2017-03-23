using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ADSPLibrary
{
    public partial class StandartSTSUELControl : UserControl
    {
        double[] PValues = new double[6];
        double[] QValues = new double[6];

        TextBox[] PTextBoxs = new TextBox[6];
        TextBox[] QTextBoxs = new TextBox[6];

        string titlText = "Ограничитель минимального возбуждения";

        public bool CheckUELCValues()
        {
            double[] tmpPValues = new double[6];
            double[] tmpQValues = new double[6];
            int i;
            bool b = true;
            for (i = 0; i < 6; i++)
            {
                if (!double.TryParse(PTextBoxs[i].Text, out tmpPValues[i]))
                {
                    b = false;
                    break;
                }

                if (!double.TryParse(QTextBoxs[i].Text, out tmpQValues[i]))
                {
                    b = false;
                    break;
                }
            }

            if (!b) { return b; }

 
            for (i = 5; i > 0; i--)
            {

                if ((tmpPValues[i - 1] - tmpPValues[i]) < 2)
                {
                    b = false;
                    break;
                }
            }
            for (i = 0; i < 6; i++)
            {

                if ((tmpPValues[i] > 100) || (tmpPValues[i] < 0) || (tmpQValues[i] > 0) || (tmpQValues[i] < -100))
                {
                    b = false;
                    break;
                }
            }
            if (!b) { return b; }
            PValues = tmpPValues;
            QValues = tmpQValues;

            return true;
        }

        public void LoadUEL()
        {
            for (int i = 0; i < 6; i++)
            {
                PTextBoxs[i].Text = AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.StartUELAddr+2*i], 0);
                QTextBoxs[i].Text = AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.StartUELAddr + 2 * i+1], 0);
            }
            DrawGraph();
        }

        public void DrawGraph()
        {
            int i = 0;

            chart1.ChartAreas[0].AxisY2.Minimum = 0;
            chart1.ChartAreas[0].AxisY2.Maximum = 100;
            chart1.ChartAreas[0].AxisY2.Title = "P, %";
            

            chart1.ChartAreas[0].AxisX.Minimum = -100;
            chart1.ChartAreas[0].AxisX.Maximum = 0;
            chart1.ChartAreas[0].AxisX.Title = "Q, %";

            chart1.Series[0].Points.Clear();

            if (!CheckUELCValues()) { chart1.Series[0].Points.AddXY(0, 0); return; }
            for (i = 0; i < 6; i++)
            {
                chart1.Series[0].YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
                chart1.Series[0].Points.AddXY(QValues[i], PValues[i]);
            }

        }

        public StandartSTSUELControl()
        {
            InitializeComponent();
            PTextBoxs[0] = textBoxP1; PTextBoxs[1] = textBoxP2; PTextBoxs[2] = textBoxP3;
            PTextBoxs[3] = textBoxP4; PTextBoxs[4] = textBoxP5; PTextBoxs[5] = textBoxP6;

            QTextBoxs[0] = textBoxQ1; QTextBoxs[1] = textBoxQ2; QTextBoxs[2] = textBoxQ3;
            QTextBoxs[3] = textBoxQ4; QTextBoxs[4] = textBoxQ5; QTextBoxs[5] = textBoxQ6;
        
        }

        private void chart1_Click(object sender, EventArgs e)
        {


        }

        private void StandartSTSUELControl_Load(object sender, EventArgs e)
        {
            LoadUEL();
            label1.Text = titlText;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckUELCValues()) 
            {
                MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ushort startAddr = SystemParamsFormat.StartUELAddr;

            for (int i = 0; i < 6; i++)
            {
                SystemParamsClass.UpdateSystemParam((ushort)(startAddr+2*i),AdvanceConvert.PercentToHex(PValues[i]));
                SystemParamsClass.UpdateSystemParam((ushort)(startAddr + 2 * i+1), AdvanceConvert.PercentToHex(QValues[i]));
            }

            label1.Text = titlText;

        }

        private void textBoxQ6_TextChanged(object sender, EventArgs e)
        {
            DrawGraph();
            label1.Text = titlText+"*";
        }
    }
}
