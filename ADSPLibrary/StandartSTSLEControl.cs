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
    public partial class StandartSTSLEControl : UserControl
    {
        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;

            if ((paramControl.AppliedParams[0] > 0.1) && (paramControl.AppliedParams[0] < 32))
            { uValues[0] = AdvanceConvert.Uint1000ToHex(paramControl.AppliedParams[0]); }
            else b = false;


            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSLEAddr, uValues[0]);

        }

        public StandartSTSLEControl()
        {
            InitializeComponent();
        }

        private void StandartSTSLEControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            names.Add("Время срабатывания защиты");
            List<string> unitnames = new List<string>();
            unitnames.Add(" с");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToUint1000(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSLEAddr])
            );

            InitializeComponent();

            paramControl = new EnaDisaParamControl(1, "Защита от потери возбуждения", names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }
    }
}
