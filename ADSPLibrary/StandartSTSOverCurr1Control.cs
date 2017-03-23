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
    public partial class StandartSTSOverCurr1Control : UserControl
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
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSOverCurr1Addr, uValues[0]);

        }

        public StandartSTSOverCurr1Control()
        {
            InitializeComponent();
        }

        private void StandartSTSOverCurr1Control_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            names.Add("Время срабатывания защиты");
            List<string> unitnames = new List<string>();
            unitnames.Add(" с");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToUint1000(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSOverCurr1Addr])
            );

            InitializeComponent();

            paramControl = new EnaDisaParamControl(1, "Защита от превышения тока ротора", names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }
    }
}