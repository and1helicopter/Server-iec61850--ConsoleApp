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
    public partial class StandartSTSTransAlarmControl : UserControl
    {

        EnaDisaParamControl paramControl;
        ushort addr1, addr2;

        private void ParamsApplied(object sender, EventArgs e)
        {
            ushort[] uValues = new ushort[3];
            bool b = true;

            if ((paramControl.AppliedParams[0] > 0.2) && (paramControl.AppliedParams[0] < 70))
            { uValues[0] = AdvanceConvert.TransAlarmToHex(paramControl.AppliedParams[0]); }
            else b = false;

            if ((paramControl.AppliedParams[1] > 1) && (paramControl.AppliedParams[1] < 5000))
            { uValues[1] = (ushort)(paramControl.AppliedParams[1]); }
            else b = false;

            if (!b) { paramControl.ErrorParams(); return; }
            SystemParamsClass.UpdateSystemParam(addr1, uValues[0]);
            SystemParamsClass.UpdateSystemParam(addr2, uValues[1]);
        }
        public StandartSTSTransAlarmControl(int refNum)
        {
            InitializeComponent();
            if (refNum == 0)
            {
                addr1 = SystemParamsFormat.StandartSTSTransAlarmAddr;
                addr2 = SystemParamsFormat.StandartSTSTransAlarmTimeAddr;
            }
            else
            {
                addr1 = SystemParamsFormat.StandartSTSTransAlarm2Addr;
                addr2 = SystemParamsFormat.StandartSTSTransAlarm2TimeAddr;
            }
        }

        private void StandartSTSTransAlarmControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();
            names.Add("Уставка срабатывания защиты");
            names.Add("Задержка на срабатывание");
            List<string> unitnames = new List<string>();
            unitnames.Add(" А");
            unitnames.Add(" мс");

            List<string> nowValues = new List<string>();
            nowValues.Add(
            AdvanceConvert.HexToTransAlarm(SystemParamsClass.SystemParams[addr1])
            );

            nowValues.Add(
            AdvanceConvert.HexToInt(SystemParamsClass.SystemParams[addr2])
            );

            InitializeComponent();

            paramControl = new EnaDisaParamControl(2, "МТЗ трансформатора", names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }
    }
}
