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
    public partial class StandartSTSRegUControl : UserControl
    {
        string titlText = "Регулятор напряжения статора";
        TextBox[] textBoxs = new TextBox[3];

        public StandartSTSRegUControl()
        {
            InitializeComponent();

            textBoxs[0] = textBox1;
            textBoxs[1] = textBox2;
            textBoxs[2] = textBox3;

            textBox1.Text = 
            AdvanceConvert.HexToTi(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRegUAddr]);
            textBox2.Text = 
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRegUAddr+1], 0);
            textBox3.Text =
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRegUKdAddr], 0);
            label1.Text = titlText;
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            double[] fValues = new double[3];
            ushort[] uValues = new ushort[3];
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
            

            //Ti
            if ((fValues[0] > 0) && (fValues[0] < 3000)) 
            { uValues[0] = AdvanceConvert.TiToHex(fValues[0]); } else b = false;

            //Kp
            if ((fValues[1] > -30) && (fValues[1] < 30)) 
            { uValues[1] = AdvanceConvert.F8_8ToHex(fValues[1]); } else b = false;

            //Kd
            if ((fValues[2] > -30) && (fValues[2] < 30)) 
            { uValues[2] = AdvanceConvert.F8_8ToHex(fValues[2]); } else b = false;

            if (!b) { goto applyError; }


            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSRegUAddr, uValues[0]);
            SystemParamsClass.UpdateSystemParam((ushort)(SystemParamsFormat.StandartSTSRegUAddr + 1), uValues[1]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSRegUKdAddr,uValues[2]);

            label1.Text = titlText;
            return;
            applyError:
            {
                MessageBox.Show("Неверно заданы параметры!","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = titlText + "*";
        }


    }
}
