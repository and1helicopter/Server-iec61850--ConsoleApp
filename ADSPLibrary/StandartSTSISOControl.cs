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
    public partial class StandartSTSISOControl : UserControl
    {
        EnaDisaParamControl paramControl;

        private void ParamsApplied(object sender, EventArgs e)
        {
            if (paramControl.EnableFlag) 
            {
                SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSISOAddr, 1); 
            }
            else
            {
                SystemParamsClass.UpdateSystemParam(SystemParamsFormat.StandartSTSISOAddr, 0); 
            }
        }

        public StandartSTSISOControl()
        {
            InitializeComponent();
        }

        private void StandartSTSISOControl_Load(object sender, EventArgs e)
        {
            List<string> names = new List<string>();

            List<string> unitnames = new List<string>();

            List<string> nowValues = new List<string>();

            bool b;

            if (SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSISOAddr] != 0)
            {
                b = true;
            }
            else 
            {   
                b = false; 
            }

            InitializeComponent();

            paramControl = new EnaDisaParamControl(0, "Контроль изоляции", b, names, unitnames, nowValues);
            paramControl.Dock = DockStyle.Fill;
            this.Controls.Add(paramControl);
            paramControl.OnParamsApplied += new EventHandler(ParamsApplied);
        }
    }
}

