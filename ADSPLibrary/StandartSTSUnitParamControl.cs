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
    public partial class StandartSTSUnitParamControl : UserControl
    {
        bool valueChanged = false;
        TextBox[] textBoxs = new TextBox[6];


        public StandartSTSUnitParamControl()
        {
            ushort w1,w2;
            InitializeComponent();
            textBoxs[0] = textBox1;
            textBoxs[1] = textBox2;
            textBoxs[2] = textBox3;
            textBoxs[3] = textBox4;
            textBoxs[4] = textBox5;
            textBoxs[5] = textBox6;
            
            
            textBox1.Text = AdvanceConvert.HexToInt(SystemParamsClass.SystemParams[SystemParamsFormat.NomUAddr]);
            textBox2.Text = AdvanceConvert.HexToInt(SystemParamsClass.SystemParams[SystemParamsFormat.NomIAddr]);
            w1=SystemParamsClass.SystemParams[SystemParamsFormat.KIfAddr];
            w2=SystemParamsClass.SystemParams[SystemParamsFormat.NomIfAddr];
            textBox3.Text = AdvanceConvert.CalcNomIf(w1, w2);
            textBox4.Text = AdvanceConvert.HexToInt10(SystemParamsClass.SystemParams[SystemParamsFormat.NomUfAddr]);
            textBox5.Text = AdvanceConvert.HexToTT(SystemParamsClass.SystemParams[SystemParamsFormat.KttAddr]);
            textBox6.Text = AdvanceConvert.HexToInt10(SystemParamsClass.SystemParams[SystemParamsFormat.NomUTEAddr]);
            label1.Text = "Номинальные параметры генератора";
            valueChanged = false;
        }

        private void StandartSTSUnitParamControl_LocationChanged(object sender, EventArgs e)
        {

        }

        private void StandartSTSUnitParamControl_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (valueChanged) { return; }
            valueChanged = true;

            this.label1.Text += "*";
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            double[] fValues = new double[6];
            ushort[] uValues = new ushort[6];
            bool b=true;
            ushort w = 0;
            
            for (int i = 0; i < 6; i++)
            {
                if (!double.TryParse(textBoxs[i].Text, out fValues[i]))
                {
                    b = false;
                    break;
                }
            }
            if (!b) { goto applyError; }

            //Напряжение статора
            if ((fValues[0] > 100) && (fValues[0] < 20000)) { uValues[0] = (ushort)(fValues[0]); } else b = false;

            //Ток статора
            if ((fValues[1] > 100) && (fValues[1] < 20000)) { uValues[1] = (ushort)(fValues[1]); } else b = false;

            //Ток ротора
            w = SystemParamsClass.SystemParams[SystemParamsFormat.NomIfAddr];
            if ((fValues[2] >= (w * 0.4)) && (fValues[2] <= w))
            { uValues[2] = AdvanceConvert.CalcKIf(fValues[2], w); }      else b = false;

            //Напряжение ротора
            if ((fValues[3] > 100) && (fValues[3] < 800)) { uValues[3] = AdvanceConvert.Int10ToHex(fValues[3]); } else b = false;

            //TT
            if ((fValues[4] >= 2.0) && (fValues[4] <= 5.0))
            {
                uValues[4] = AdvanceConvert.TTToHex(fValues[4]);
            }
            else b = false;

            //Напряжение питания
            if ((fValues[5] > 100) && (fValues[5] < 800)) { uValues[5] = AdvanceConvert.Int10ToHex(fValues[5]); } else b = false;

            if (!b) { goto applyError; }

            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.NomUAddr, uValues[0]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.NomIAddr, uValues[1]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.KIfAddr, uValues[2]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.NomUfAddr, uValues[3]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.KttAddr, uValues[4]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.NomUTEAddr, uValues[5]);

            label1.Text = "Номинальные параметры генератора";
            valueChanged = false;

            return;
            applyError:
            {
                MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
