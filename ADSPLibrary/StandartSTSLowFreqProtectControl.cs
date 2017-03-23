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
    public partial class StandartSTSLowFreqProtectControl : UserControl
    {
        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;
            for (int i = 0; i < 1; i++)
            {
                if ((paramControl.AppliedParams[i] > 5) && (paramControl.AppliedParams[i] < 60))
                { uValues[i] = AdvanceConvert.FreqToHex(paramControl.AppliedParams[i]); }
                else b = false;
            }
            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSLowFreqProtectAddr, uValues[0]);

            if (paramControl.EnableFlag) { SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSLowFreqProtectEnaAddr, 1); }
            else { SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSLowFreqProtectEnaAddr, 0); }


        }

        public StandartSTSLowFreqProtectControl()
        {
            List<string> names = new List<string>();
            names.Add("Допустимая частота");
            List<string> unitnames = new List<string>();
            unitnames.Add("Гц");


            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToFreq(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSLowFreqProtectAddr])
            );


            bool b;
            if (SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSLowFreqProtectEnaAddr] != 0)
            {
                b = true;

            }
            else { b = false; }

            InitializeComponent();

            paramControl = new EnaDisaParamControl(1, "Защита от низкой частоты на ХХ", b, names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }

        private void StandartSTSLowFreqProtectControl_Load(object sender, EventArgs e)
        {

        }
    }
}
