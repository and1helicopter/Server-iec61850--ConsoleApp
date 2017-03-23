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
    public partial class StandartSTSRegControl : UserControl
    {
        int regNum;
        string titlText;
        ushort startAddr = 0;
        TextBox[] textBoxs = new TextBox[2];
        public StandartSTSRegControl(int newRegNum)
        {
            InitializeComponent();
            regNum = newRegNum;
            switch (regNum)
            {
                case 0: { titlText = "Регулятор тока возбуждения"; startAddr = SystemParamsFormat.StandartSTSRegIfAddr; } break;
                default: { titlText = "Регулятор ОМВ (реактивного тока)"; startAddr = SystemParamsFormat.StandartSTSRegIreAddr; } break;
            }
            
            textBoxs[0] = textBox1;
            textBoxs[1] = textBox2;

            textBox1.Text =
            AdvanceConvert.HexToTi(SystemParamsClass.SystemParams[startAddr]);
            textBox2.Text =
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[startAddr + 1], 0);

            label1.Text = titlText;
        }

        private void applyBtn_Click_1(object sender, EventArgs e)
        {
            double[] fValues = new double[2];
            ushort[] uValues = new ushort[2];
            bool b = true;
            
            for (int i = 0; i < 2; i++)
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
            { uValues[0] = AdvanceConvert.TiToHex(fValues[0]); }
            else b = false;

            //Kp
            if ((fValues[1] > -30) && (fValues[1] < 30))
            { uValues[1] = AdvanceConvert.F8_8ToHex(fValues[1]); }
            else b = false;


            if (!b) { goto applyError; }


            SystemParamsClass.UpdateSystemParam(startAddr, uValues[0]);
            SystemParamsClass.UpdateSystemParam((ushort)(startAddr + 1), uValues[1]);

            label1.Text = titlText;
            return;
            applyError:
            {
                MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = titlText + "*";
        }

    }
}
