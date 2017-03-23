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
    public partial class StandartSTSSystemStabControl : UserControl
    {
        string titlText = "Системный стабилизатор";
        TextBox[] textBoxs = new TextBox[3];

        public StandartSTSSystemStabControl()
        {
            InitializeComponent();

            textBoxs[0] = textBox1;
            textBoxs[1] = textBox2;
            textBoxs[2] = textBox3;

            textBox1.Text =
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[SystemParamsFormat.SystemStabCoef1Addr], 0);
            textBox2.Text =
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[SystemParamsFormat.SystemStabCoef2Addr], 0);
            textBox3.Text =
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[SystemParamsFormat.SystemStabCoef3Addr], 0);
            

            if (SystemParamsClass.SystemParams[SystemParamsFormat.SystemStabEnaAddr] != 0)
            {
                radioButton1.Checked = true;

            }
            else { radioButton2.Checked = true; }

            label1.Text = titlText;
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            double[] fValues = new double[3];
            ushort[] uValues = new ushort[3];
            bool b = true;
            int i = 0;
            for (i = 0; i < 3; i++)
            {
                if (!double.TryParse(textBoxs[i].Text, out fValues[i]))
                {
                    b = false;
                    break;
                }
            }
            if (!b) { goto applyError; }

            for (i = 0; i < 3; i++)
            {
                if ((fValues[i] > -127) && (fValues[i] < 127))
                { uValues[i] = AdvanceConvert.F8_8ToHex(fValues[i]); }
                else b = false;

            }

            if (!b) { goto applyError; }


            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabCoef1Addr, uValues[0]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabCoef2Addr, uValues[1]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabCoef3Addr, uValues[2]);

            if (radioButton1.Checked) { SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabEnaAddr, 1); }
            else { SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabEnaAddr, 0); }

            label1.Text = titlText;
            return;
            applyError:
            {
                ErrorParams();
            }
        }

        public void ErrorParams()
        {
            MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = titlText + "*";
        }
    }
}
