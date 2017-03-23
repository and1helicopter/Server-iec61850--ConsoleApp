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
    public partial class StandartSTSVHZControl : UserControl
    {
        bool vHzEnabled = true;
        TextBox[] textBoxs = new TextBox[1];
        string titlText = "V/Hz - ограничение";
        public StandartSTSVHZControl()
        {
            InitializeComponent();
            textBoxs[0] = textBox1;

            textBox1.Text =
            AdvanceConvert.LongHexToVHz
            (SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSVHZHiAddr],
             SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSVHZLoAddr]
            );

            if (SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSVHZEnabledAddr] != 0) { vHzEnabled = true; }
            else { vHzEnabled = false; }



            if (vHzEnabled)
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
            if (vHzEnabled) { return; }
            vHzEnabled = true;
            enaBtn.BackColor = Color.Lime;
            disaBtn.BackColor = Color.LightGray;
            label1.Text = titlText + "*";
                 
        }

        private void disaBtn_Click(object sender, EventArgs e)
        {
            if (!vHzEnabled) { return; }
            vHzEnabled = false;
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
            double[] fValues = new double[1];
            ushort[] uValues = new ushort[3];
            bool b = true;

            for (int i = 0; i < 1; i++)
            {
                if (!double.TryParse(textBoxs[i].Text, out fValues[i]))
                {
                    b = false;
                    break;
                }
            }
            if (!b) { goto applyError; }


            //
            if ((fValues[0] >= 40) && (fValues[0] <= 50))
            {   
                AdvanceConvert.VHzToLongHex(fValues[0],out uValues[0],out uValues[1]); 
                
            }   
            else b = false;

            if (vHzEnabled) { uValues[2] = 1; } else { uValues[2] = 0; }


            if (!b) { goto applyError; }


            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSVHZLoAddr, uValues[0]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSVHZHiAddr, uValues[1]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSVHZEnabledAddr, uValues[2]);
            

            label1.Text = titlText;
            return;
        applyError:
            {
                MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
