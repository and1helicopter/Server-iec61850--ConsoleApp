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
    public partial class StandartSTSOverVControl : UserControl
    {
        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;

            if ((paramControl.AppliedParams[0] > 99) && (paramControl.AppliedParams[0] < 140))
            { uValues[0] = AdvanceConvert.PercentToHex(paramControl.AppliedParams[0]); }
            else b = false;

            if ((paramControl.AppliedParams[1] > 1) && (paramControl.AppliedParams[1] < 2000))
            { uValues[1] = (ushort)(paramControl.AppliedParams[1]); }
            else b = false;

            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSOverVAddr, uValues[0]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSOverVTimeAddr, uValues[1]);
        }

        public StandartSTSOverVControl()
        {
            InitializeComponent();
        }

        private void StandartSTSOverVControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            names.Add("Уставка срабатывания защиты");
            names.Add("Задержка на срабатывание");
            List<string> unitnames = new List<string>();
            unitnames.Add(" %");
            unitnames.Add(" мс");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToPercent(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSOverVAddr], 1)
            );

            nowValues.Add(
            AdvanceConvert.HexToUint(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSOverVTimeAddr])
            );

            InitializeComponent();

            paramControl = new EnaDisaParamControl(2, "Защита от перенапряжения статора", names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);


        }
    }
}
