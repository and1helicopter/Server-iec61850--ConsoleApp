using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModBusLibrary;

namespace ADSPLibrary
{
    public partial class JogForm : Form
    {
        bool requestStep = false;
        bool readLineBusy = false;
        bool writeLineBusy = false;

        ModBusUnit modBusUnitR;
        ModBusUnit modBusUnitW;

        delegate void SetStringDelegate(string parameter);
        void SetToolStripLabel(string newStr)
        {
            try
            {
                toolStripStatusLabel1.Text = newStr;
            }
            catch { }
        }
        void UpdateToolStripLabel(string newStr)
        {
            if (!this.Visible) { return; }

            try
            {
                Invoke(new SetStringDelegate(SetToolStripLabel), newStr);
            }
            catch { }
        }
       
        void SetNowText1(string newStr)
        {
            try
            {
                nowTextBox1.Text = newStr;
            }
            catch { }
        }
        void SetNowText2(string newStr)
        {
            try
            {
                nowTextBox2.Text = newStr;
            }
            catch { }
        }
        void UpdateNowText1(string newStr)
        {
            if (!this.Visible) { return; }

            Invoke(new SetStringDelegate(SetNowText1), newStr);
        }
        void UpdateNowText2(string newStr)
        {
            if (!this.Visible) { return; }

            Invoke(new SetStringDelegate(SetNowText2), newStr);
        }


        private void EndRequestW(object sender, EventArgs e)
        {
            if (modBusUnitW.modBusData.RequestError)
            {
                UpdateToolStripLabel("Ошибка связи");
                writeLineBusy = false;
                return;
            }
            else
            {
                UpdateToolStripLabel("Связь");
                writeLineBusy = false;
                return;
            }
        }

        private void EndRequestR(object sender, EventArgs e)
        {
            if (modBusUnitR.modBusData.RequestError)
            {
                UpdateToolStripLabel("Ошибка связи");
                requestStep = !requestStep;
                readLineBusy = false;
                return;
            }

            UpdateToolStripLabel("Связь");
            if (requestStep)
            {
                UpdateNowText2(AdvanceConvert.HexToPercent(modBusUnitR.modBusData.ReadData[0]));
            }
            else
            {
                UpdateNowText1(AdvanceConvert.HexToPercent(modBusUnitR.modBusData.ReadData[0]));

            }
            requestStep = !requestStep; 
            readLineBusy = false;
        }

        public JogForm()
        {
            InitializeComponent();
            InitModBusUnits();

        }

        public void InitModBusUnits()
        {
            modBusUnitR = new ModBusUnit();
            modBusUnitR.RequestFinished += new EventHandler(EndRequestR);

            modBusUnitW = new ModBusUnit();
            modBusUnitW.RequestFinished += new EventHandler(EndRequestW);
        }

        private void JogForm_Load(object sender, EventArgs e)
        {

        }

        private void nowTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!this.Visible) {return;}
            if (readLineBusy)  {return;}
            readLineBusy = true;
            if (!requestStep) { modBusUnitR.GetData(0x015C, 1); }
            else { modBusUnitR.GetData(0x015F, 1); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (writeLineBusy) {return;}
            double f;
            if (!double.TryParse(textBox1.Text, out f))
            {
                MessageBox.Show("Неправильно задан параметр!", "Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if ((f>120)||(f<80))
            {
                MessageBox.Show("Неправильно задан параметр!", "Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            ushort[] w = new ushort[1] {(ushort)(f * 40.96)};
            if (writeLineBusy) { return; }
            modBusUnitW.SetData(0x015C, 1, w);
        }

        private void JogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (writeLineBusy) { return; }
            double f;
            if (!double.TryParse(textBox2.Text, out f))
            {
                MessageBox.Show("Неправильно задан параметр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ((f > 120) || (f < 0))
            {
                MessageBox.Show("Неправильно задан параметр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ushort[] w = new ushort[1] { (ushort)(f * 40.96) };
            if (writeLineBusy) { return; }
            modBusUnitW.SetData(0x015F, 1, w);
        }


    }
}
