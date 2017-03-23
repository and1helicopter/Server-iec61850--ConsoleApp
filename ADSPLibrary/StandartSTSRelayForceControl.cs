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
    public partial class StandartSTSRelayForceControl : UserControl
    {
        bool relayForceEnabled = true;
        TextBox[] textBoxs = new TextBox[3];
        string titlText = "Релейная форсировка";
        public StandartSTSRelayForceControl()
        {
            InitializeComponent();
            textBoxs[0] = textBox1;
            textBoxs[1] = textBox2;
            textBoxs[2] = textBox3;

            textBox1.Text =
            AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRelayRefEnaAddr],1);

            textBox2.Text =
            AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRelayRefDisaAddr],1);

            textBox3.Text =
            AdvanceConvert.HexToUint((ushort)(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRelayDelayAddr]-1));

            if (SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRelayEnabledAddr] != 0) { relayForceEnabled = true; }
            else { relayForceEnabled = false; }

            if (relayForceEnabled)
            {
                enaBtn.BackColor = Color.Lime;
                disaBtn.BackColor = Color.LightGray;
            }
            else
            {
                disaBtn.BackColor = Color.Lime;
                enaBtn.BackColor = Color.LightGray;
            }

            label1.Text = titlText;
        }

        private void enaBtn_Click(object sender, EventArgs e)
        {
            if (relayForceEnabled) { return; }
            relayForceEnabled = true;
            enaBtn.BackColor = Color.Lime;
            disaBtn.BackColor = Color.LightGray;
            label1.Text = titlText + "*";
                 
        }

        private void disaBtn_Click(object sender, EventArgs e)
        {
            if (!relayForceEnabled) { return; }
            relayForceEnabled = false;
            disaBtn.BackColor = Color.Lime;
            enaBtn.BackColor = Color.LightGray;
            label1.Text = titlText + "*";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = titlText + "*";
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            double[] fValues = new double[3];
            ushort[] uValues = new ushort[4];
            bool b = true;

            for (int i = 0; i < 3; i++)
            {
                if (!double.TryParse(textBoxs[i].Text, out fValues[i]))
                {
                    b = false;
                    break;
                }
            }
            if (!b) { goto applyError; }


            //Ena
            if ((fValues[0] > 70) && (fValues[0] < fValues[1]))
            { uValues[0] = AdvanceConvert.PercentToHex(fValues[0]); }
            else b = false;

            //Disa
            if ((fValues[1] > fValues[0]) && (fValues[1] < 110))
            { uValues[1] = AdvanceConvert.PercentToHex(fValues[1]); }
            else b = false;

            //Delay
            if ((fValues[2] > 0) && (fValues[2] < 1000))
            { uValues[2] = (ushort)((ushort)(fValues[2]) + 1); }
            else b = false;

            if (relayForceEnabled) { uValues[3] = 1; } else { uValues[3] = 0; }


            if (!b) { goto applyError; }


            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSRelayRefEnaAddr, uValues[0]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSRelayRefDisaAddr, uValues[1]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSRelayDelayAddr, uValues[2]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSRelayEnabledAddr, uValues[3]);

            label1.Text = titlText;
            return;
        applyError:
            {
                MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
