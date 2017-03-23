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
    public partial class StandartSTSRefMANControl : UserControl
    {
        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;
            for (int i = 0; i < 1; i++)
            {
                if ((paramControl.AppliedParams[i] > 0) && (paramControl.AppliedParams[i] < 110))
                { uValues[i] = AdvanceConvert.PercentToHex(paramControl.AppliedParams[i]); }
                else b = false;
            }
            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSRefMANDefault, uValues[0]);
        }

        public StandartSTSRefMANControl()
        {
            InitializeComponent();
        }

        private void StandartSTSRefMANControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            names.Add("Уставка по умолчанию в РРВ");
            List<string> unitnames = new List<string>();
            unitnames.Add(" сек");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSRefMANDefault], 1)
            );

            InitializeComponent();

            paramControl = new EnaDisaParamControl(1, "Уставка по умолчанию в РРВ", names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }
    }
}
