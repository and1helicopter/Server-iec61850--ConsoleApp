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
    public partial class StandartSTSRefLimitControl : UserControl
    {
        int regNum = 0;
        string titlText = "";
        string unitText = "";
        ushort startAddr = 0;
        double hiValue, loValue;
        public StandartSTSRefLimitControl(int newRegNum)
        {
            InitializeComponent();
            switch (newRegNum)
            {
                case 1:  
                    { 
                        titlText = "Ограничение уставки регулятора тока ротора"; 
                        unitText = "%"; regNum = 1;
                        hiValue = 110; loValue = 0;
                        startAddr = SystemParamsFormat.StandartSTSLimRefIfAddr;} break;
                case 2: 
                    {
                        titlText = "Ограничение уставки регулятора угла Ф"; 
                        unitText = "°"; regNum = 2;
                        hiValue = 45; loValue = -45;
                        startAddr = SystemParamsFormat.StandartSTSLimRefFiAddr;} break;
                case 3: 
                    {
                        titlText = "Ограничение уставки регулятора реактивного тока"; 
                        unitText = "%"; regNum = 3;
                        hiValue = 100; loValue = -50;
                        startAddr = SystemParamsFormat.StandartSTSLimRefIreAddr;} break;
                default: 
                    {
                        titlText = "Ограничение уставки регулятора напряжения"; 
                        unitText = "%"; regNum = 0;
                        hiValue = 115; loValue = 80;
                        startAddr = SystemParamsFormat.StandartSTSLimRefUAddr;} break;
            }

            if (regNum == 2)
            {
                textBox1.Text =
                AdvanceConvert.HexToInt10(SystemParamsClass.SystemParams[startAddr + 1]);

                textBox2.Text =
                AdvanceConvert.HexToInt10(SystemParamsClass.SystemParams[startAddr]);


            }
            else
            {
                textBox1.Text =
                AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[startAddr+1],1);

                textBox2.Text =
                AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[startAddr],1);

            }



            label1.Text = titlText;
            unitLabel1.Text = unitText;
            unitLabel2.Text = unitText;
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            double minValue, maxValue;
            ushort uMinValue, uMaxValue;
            bool b = true;

            if (!double.TryParse(textBox1.Text, out minValue))
            {
                b = false;
            }

            if (!double.TryParse(textBox2.Text, out maxValue))
            {
                b = false;
            }

            if (maxValue <= minValue) { b = false; }

            if (maxValue > hiValue) { b = false; }
            if (minValue < loValue) { b = false; }

            if (!b)
            {
                MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (regNum == 2)
            {
                uMinValue = AdvanceConvert.Int10ToHex(minValue);
                uMaxValue = AdvanceConvert.Int10ToHex(maxValue);

                ushort uMinValue2, uMaxValue2, startAddr2;
                uMinValue2 = AdvanceConvert.CalcAngle(minValue);
                uMaxValue2 = AdvanceConvert.CalcAngle(maxValue);

                startAddr2 = SystemParamsFormat.StandartSTSLimRefFi2Addr;
                SystemParamsClass.UpdateSystemParam((ushort)(startAddr + 1), uMinValue);
                SystemParamsClass.UpdateSystemParam((ushort)(startAddr), uMaxValue);
                SystemParamsClass.UpdateSystemParam((ushort)(startAddr2 + 1), uMinValue2);
                SystemParamsClass.UpdateSystemParam((ushort)(startAddr2), uMaxValue2);
            }
            else
            {
                uMinValue = AdvanceConvert.PercentToHex(minValue);
                uMaxValue = AdvanceConvert.PercentToHex(maxValue);

                SystemParamsClass.UpdateSystemParam((ushort)(startAddr + 1), uMinValue);
                SystemParamsClass.UpdateSystemParam((ushort)(startAddr), uMaxValue);
            }
            label1.Text = titlText;



        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = titlText+"*";
        }
    }
}
