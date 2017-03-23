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
    public partial class StandartSTSAsynchProtectControl : UserControl
    {

        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;

            if ((paramControl.AppliedParams[0] > 1) && (paramControl.AppliedParams[0] < 30))
            { uValues[0] = (ushort)(paramControl.AppliedParams[0]); }
            else b = false;

            if ((paramControl.AppliedParams[1] > 1) && (paramControl.AppliedParams[1] < 30))
            { uValues[1] = AdvanceConvert.Uint1000ToHex(paramControl.AppliedParams[1]); }
            else b = false;

            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSAsynchAddr, uValues[0]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSAsynchTimeAddr, uValues[1]);
        }


        public StandartSTSAsynchProtectControl()
        {
            InitializeComponent();
        }

        private void StandartSTSAsynchProtectControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            names.Add("Количество переворотов");
            names.Add("Задержка на сброс счетчика переворотов");
            List<string> unitnames = new List<string>();
            unitnames.Add("");
            unitnames.Add("сек");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToUint(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSAsynchAddr])
            );

            nowValues.Add(
            AdvanceConvert.HexToUint1000(SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSAsynchTimeAddr])
            );

            InitializeComponent();

            paramControl = new EnaDisaParamControl(2, "Защита от асинхронного хода", names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }
    }
}
