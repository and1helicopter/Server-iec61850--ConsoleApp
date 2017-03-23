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
    public partial class StandartSTSSlowRegControl : UserControl
    {
        int regNum = 0;
        ushort startAddr = 0;
        string titlText;
        public StandartSTSSlowRegControl(int newRegNum)
        {
            InitializeComponent();
            regNum = newRegNum;
            switch (regNum)
            {
                case 0: { titlText = "Регулятор реактивного тока"; startAddr = SystemParamsFormat.StandartSTSRegSlowIreAddr; } break;
                default: { titlText = "Регулятор cosФ"; startAddr = SystemParamsFormat.StandartSTSRegSlowPFAddr; } break;
            }

            textBox1.Text = AdvanceConvert.HexToSlowTi(SystemParamsClass.SystemParams[startAddr]);
            label1.Text = titlText;
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            double fValue;
            ushort uValue=0;
            bool b = true;

            if (!double.TryParse(textBox1.Text, out fValue))
            {
                b = false;
            }

            if (!b) { goto applyError; }


            //Ti
            if ((fValue >= 0) && (fValue <=5.0))
            { uValue = AdvanceConvert.SlowTiToHex(fValue); }
            else b = false;


            if (!b) { goto applyError; }


            SystemParamsClass.UpdateSystemParam(startAddr, uValue);


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
