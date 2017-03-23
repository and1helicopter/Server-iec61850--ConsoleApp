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
    public partial class StandartInvCurrControl : UserControl
    {
        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            if (paramControl.EnableFlag) 
            {
                SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSInvCurrAddr, 0); 
            }
            else
            {
                SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSInvCurrAddr, 1); 
            }
        }

        public StandartInvCurrControl()
        {
            InitializeComponent();
        }

        private void StandartInvCurrControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();

            List<string> unitnames = new List<string>();

            List<string> nowValues = new List<string>();

            bool b;

            if (SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSInvCurrAddr] != 0)
            {
                b = false;
            }
            else 
            {   
                b = true; 
            }

            InitializeComponent();

            paramControl = new EnaDisaParamControl(0, "Инвертирование токов", b, names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);

        }
    }
}
