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
    public partial class StandartSystemStabControl : UserControl
    {
        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;
            for (int i = 0; i < 3; i++)
            {
                if ((paramControl.AppliedParams[i] > -127) && (paramControl.AppliedParams[i] < 127))
                { uValues[i] = AdvanceConvert.F8_8ToHex(paramControl.AppliedParams[i]); }
                else b = false;
            }
            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabCoef1Addr, uValues[0]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabCoef2Addr, uValues[1]);
            SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabCoef3Addr, uValues[2]);

            if (paramControl.EnableFlag) { SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabEnaAddr, 1); }
            else { SystemParamsClass.UpdateSystemParam(SystemParamsFormat.SystemStabEnaAddr, 0); }


        }

        public StandartSystemStabControl()
        {
            List<string> names = new List<string>();
            names.Add("Коэффициент по отклонению частоты");
            names.Add("Коэффициент по отклонению производной частоты");
            names.Add("Коэффициент по производной тока ротора");
            List<string> unitnames = new List<string>();
            unitnames.Add(" ");
            unitnames.Add(" ");
            unitnames.Add(" ");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[SystemParamsFormat.SystemStabCoef1Addr], 0)
            );
            nowValues.Add(
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[SystemParamsFormat.SystemStabCoef2Addr], 0)
            );
            nowValues.Add(
            AdvanceConvert.HexTo8_8(SystemParamsClass.SystemParams[SystemParamsFormat.SystemStabCoef3Addr], 0)
            );

            bool b;
            if (SystemParamsClass.SystemParams[SystemParamsFormat.SystemStabEnaAddr] != 0)
            {
                b = true;

            }
            else { b = false; }

            InitializeComponent();

            paramControl = new EnaDisaParamControl(3, "Системный стабилизатор", b, names,unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }

        private void StandartSystemStabControl_Load(object sender, EventArgs e)
        {
            

        }
    }
}
