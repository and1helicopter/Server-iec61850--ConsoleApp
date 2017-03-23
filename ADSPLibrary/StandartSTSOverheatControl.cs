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
    public partial class StandartSTSOverheatControl : UserControl
    {
        TextBox[] textBoxs = new TextBox[5];
        string titlLabel = "Ограничитель перегрева";

        public StandartSTSOverheatControl()
        {
            InitializeComponent();
            textBoxs[0] = textBox1;
            textBoxs[1] = textBox2;
            textBoxs[2] = textBox3;
            textBoxs[3] = textBox4;
            textBoxs[4] = textBox5;


            textBox1.Text = AdvanceConvert.HexToOverHeatTi(SystemParamsClass.SystemParams[SystemParamsFormat.OverheatTiAddr]);
            textBox2.Text = AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.OverheatDBPosAddr],1);
            textBox3.Text = AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.OverheatDBNegAddr],1);
            textBox4.Text = AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.OverheatDropDownAddr],1);
            textBox5.Text = AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.OverheatForceIfAddr],1);
            label1.Text = titlLabel;
        }

        private void StandartSTSUnitParamControl_LocationChanged(object sender, EventArgs e)
        {

        }

        private void StandartSTSUnitParamControl_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.label1.Text = titlLabel+"*";
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            double[] fValues = new double[5];
            ushort[] uValues = new ushort[5];
            bool b=true;
            //ushort w = 0;
            
            for (int i = 0; i < 5; i++)
            {
                if (!double.TryParse(textBoxs[i].Text, out fValues[i]))
                {
                    b = false;
                    break;
                }
            }
            if (!b) { goto applyError; }

            //Время
            if ((fValues[0] > 5) && (fValues[0] < 65)) { uValues[0] = AdvanceConvert.OverHeatTiToUShort(fValues[0]); } 
            else b = false;

            //DB Heat
            if ((fValues[1] >= 0) && (fValues[1] <= 10)) { uValues[1] = AdvanceConvert.PercentToHex(fValues[1]); } 
            else b = false;

            //DB Cool
            if ((fValues[2] >= -10) && (fValues[2] <= 0)) { uValues[2] = AdvanceConvert.PercentToHex(fValues[2]); } 
            else b = false;

            //Ток охл
            if ((fValues[3]>0 ) && (fValues[3] <= 100)) { uValues[3] = AdvanceConvert.PercentToHex(fValues[3]); } 
            else b = false;

            //Ток охл
            if ((fValues[4] >= 100) && (fValues[4] <= 250)) { uValues[4] = AdvanceConvert.PercentToHex(fValues[4]); }
            else b = false;


            if (!b) { goto applyError; }

            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.OverheatTiAddr, uValues[0]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.OverheatDBPosAddr, uValues[1]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.OverheatDBNegAddr, uValues[2]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.OverheatDropDownAddr, uValues[3]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.OverheatForceIfAddr, uValues[4]);

            label1.Text = titlLabel;


            return;
            applyError:
            {
                MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
