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
    public partial class StandartSTSStatismControl : UserControl
    {
        ushort startAddr = 0;
        string titlText;
        public StandartSTSStatismControl()
        {
            InitializeComponent();

            startAddr = SystemParamsFormat.StandartSTSStatismAddr;
            titlText = "Настройка статизма";
            textBox1.Text = AdvanceConvert.HexToSlide(SystemParamsClass.SystemParams[startAddr],1);
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
            if ((fValue >= 0) && (fValue <=150))
            { uValue = AdvanceConvert.SlideToHex(fValue); }
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
