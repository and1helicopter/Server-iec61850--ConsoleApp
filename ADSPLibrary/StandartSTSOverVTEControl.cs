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
    public partial class StandartSTSOverVTEControl : UserControl
    {
        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;

            if ((paramControl.AppliedParams[0] > 100) && (paramControl.AppliedParams[0] < 1000))
            { uValues[0] = AdvanceConvert.Int10ToHex(paramControl.AppliedParams[0]); }
            else b = false;


            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSOverVTEAddr, uValues[0]);
            
        }

        public StandartSTSOverVTEControl()
        {
            InitializeComponent();
        }

        private void StandartSTSOverVTEControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            names.Add("Уставка срабатывания защиты");
            List<string> unitnames = new List<string>();
            unitnames.Add(" В");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToInt10(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSOverVTEAddr])
            );

            InitializeComponent();

            paramControl = new EnaDisaParamControl(1, "Защита от высокого напряжения питания", names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }
    }
}
