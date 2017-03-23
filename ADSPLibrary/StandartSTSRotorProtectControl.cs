using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ADSPLibrary;

namespace ADSPLibrary
{
    public partial class StandartSTSRotorProtectControl : UserControl
    {
        TextBox[] textBoxs = new TextBox[12];
        bool valueChanged = false;
        string titlStr = "Защита от перегрузки ротора";

        public StandartSTSRotorProtectControl()
        {
            int i;
            InitializeComponent();

            textBoxs[0] = textBox1;
            textBoxs[1] = textBox2;
            textBoxs[2] = textBox3;
            textBoxs[3] = textBox4;
            textBoxs[4] = textBox5;
            textBoxs[5] = textBox6;
            textBoxs[6] = textBox7;
            textBoxs[7] = textBox8;
            textBoxs[8] = textBox9;
            textBoxs[9] = textBox10;
            textBoxs[10] = textBox11;
            textBoxs[11] = textBox12;

            for (i = 0; i < 6; i++)
            {
                textBoxs[i].Text = AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRotorProtectAddr+i],1);
                textBoxs[i+6].Text = AdvanceConvert.HexToInt(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRotorProtectTimeAddr + i]);
            }
            valueChanged = false;
            titlLabel.Text = titlStr;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            if (!valueChanged) { return; }

            double[] fValues = new double[12];
            ushort[] uValues = new ushort[12];
            bool b = true;
            //ushort w = 0;
            int i = 0;

            for (i = 0; i < 12; i++)
            {
                if (!double.TryParse(textBoxs[i].Text, out fValues[i]))
                {
                    b = false;
                    break;
                }
            }
            if (!b) { goto applyError; }

            for (i = 0; i < 6; i++)
            {
                if ((fValues[i] >= 100) && (fValues[i] <= 200))
                { uValues[i] = AdvanceConvert.PercentToHex(fValues[i]);}
                else {b=false;}
            }
            if (!b) { goto applyError; }
            
            for (i = 6; i < 12; i++)
            {
                if ((fValues[i] >= 10) && (fValues[i] <= 3600))
                { uValues[i] = (ushort)(fValues[i]);}
                else {b=false;}
            }
            if (!b) { goto applyError; }            

            for (i=0;i<6;i++)
            {
                SystemParamsClass.UpdateSystemParam((ushort)(SystemParamsFormat.StandartSTSRotorProtectAddr+i), uValues[i]);
                SystemParamsClass.UpdateSystemParam((ushort)(SystemParamsFormat.StandartSTSRotorProtectTimeAddr+i), uValues[i+6]);
            }
            valueChanged = false;
            titlLabel.Text = titlStr;

            return;
        applyError:
            {
                MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            titlLabel.Text = titlStr + "*";
            valueChanged = true;
        }
    }
}
