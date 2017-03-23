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
    public partial class StandartStartExcitControl : UserControl
    {

        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;
            for (int i = 0; i < 1; i++)
            {
                if ((paramControl.AppliedParams[i] > 0) && (paramControl.AppliedParams[i] < 30))
                { uValues[i] = AdvanceConvert.Uint1000ToHex(paramControl.AppliedParams[i]); }
                else b = false;
            }
            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSReleNETime, uValues[0]);
        }

        public StandartStartExcitControl()
        {
            InitializeComponent();
        }

        private void StandartStartExcitControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            names.Add("Время начального возбуждения");
            List<string> unitnames = new List<string>();
            unitnames.Add(" сек");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToUint1000(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSReleNETime])
            );

            InitializeComponent();

            paramControl = new EnaDisaParamControl(1, "Начальное возбуждение", names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }
    }
}
