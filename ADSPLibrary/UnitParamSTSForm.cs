using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ADSPLibrary
{
    public partial class UnitParamSTSForm : Form
    {
        public UnitParamSTSForm()
        {
            InitializeComponent();
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            if (!AdvanceConvert.StrToInt(textBox1.Text)) 
            {
                MessageBox.Show("Ошибка");
                return;
            }
            SystemParamsClass.UpdateSystemParam(0x20, AdvanceConvert.uValue);
            this.Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UnitParamSTSForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = AdvanceConvert.HexToInt(SystemParamsClass.SystemParams[0x20]);
        }


    }
}
