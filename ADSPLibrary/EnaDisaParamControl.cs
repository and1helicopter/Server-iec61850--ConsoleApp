using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace ADSPLibrary
{
    public partial class EnaDisaParamControl : UserControl
    {
        public event EventHandler OnParamsApplied;

        List<double> appliedParams = new List<double>();
        public List<double> AppliedParams { get { return appliedParams; } set { } }

        bool enableFlag = true;
        public bool EnableFlag { get { return enableFlag; } set { } }

        List<TextBox> textBoxs = new List<TextBox>();
        List<Label> nameLabels = new List<Label>();
        List<Label> unitLabels = new List<Label>();
        string titlText;


        void UpdateTextParams(int paramCount, bool yesNoMode, List<string> paramNames, List<string> unitNames, bool newEnableFlag, List<string> nowValues)
        {
            int i = 0;
            if ((paramCount != paramNames.Count) || (paramCount != unitNames.Count) || (paramCount != nowValues.Count))
            {
                tableLayoutPanel.Visible = false;
                return;
            }
            if (paramCount > 6) { tableLayoutPanel.Visible = false; return; }

            TableLayoutRowStyleCollection styles =
                this.tableLayoutPanel.RowStyles;

            for (i = 0; i < paramCount; i++)
            {
                styles[i].SizeType = SizeType.Absolute;
                styles[i].Height = 30;
            }

            styles[paramCount].SizeType = SizeType.Absolute;
            styles[paramCount].Height = 103;

            for (i = (paramCount+1); i < 10; i++)
            {
                styles[i].SizeType = SizeType.Absolute;
                styles[i].Height = 0;
            }            

            for (i = 0; i < paramCount; i++)
            {
                nameLabels.Add(new Label());
                nameLabels[i].AutoSize = false;
                nameLabels[i].Text = paramNames[i];
                nameLabels[i].Dock = DockStyle.Fill;
                nameLabels[i].TextAlign = ContentAlignment.MiddleLeft;
                nameLabels[i].Margin = label1.Margin;
                nameLabels[i].Font = label1.Font;
                tableLayoutPanel.Controls.Add(nameLabels[i]);
                tableLayoutPanel.SetColumn(nameLabels[i], 0);
                tableLayoutPanel.SetRow(nameLabels[i], i);

                unitLabels.Add(new Label());
                unitLabels[i].Margin = label1.Margin;
                unitLabels[i].Font = label1.Font;
                unitLabels[i].AutoSize = false;
                unitLabels[i].Text = unitNames[i];
                unitLabels[i].TextAlign = ContentAlignment.MiddleLeft;
                unitLabels[i].Dock = DockStyle.Fill;

                tableLayoutPanel.Controls.Add(unitLabels[i]);
                tableLayoutPanel.SetColumn(unitLabels[i], 2);
                tableLayoutPanel.SetRow(unitLabels[i], i);
                
                textBoxs.Add(new TextBox());
                textBoxs[i].Text = nowValues[i];
                textBoxs[i].Margin = textBox1.Margin;
                textBoxs[i].Font = textBox1.Font;
                textBoxs[i].TextAlign = HorizontalAlignment.Right;
                tableLayoutPanel.Controls.Add(textBoxs[i]);
                tableLayoutPanel.SetColumn(textBoxs[i], 1);
                tableLayoutPanel.SetRow(textBoxs[i], i);

                textBoxs[i].TextChanged += new EventHandler(textBox1_TextChanged);
            }

            
            tableLayoutPanel.SetRow(tableLayoutPanel2, paramCount);
            if (!yesNoMode) { tableLayoutPanel2.Visible = false; return; }

            enableFlag = newEnableFlag;
            if (enableFlag)
            {
                enaBtn.BackColor  = Color.Lime;
                disaBtn.BackColor = Color.LightGray;
            }
            else
            {
                disaBtn.BackColor = Color.Lime;
                enaBtn.BackColor  = Color.LightGray;
            }
        }


        public 
        EnaDisaParamControl(int paramCount, string newTitl, bool nowEnabled, 
                            List<string> paramNames, List<string> unitNames, List<string> nowValues)
        {
            InitializeComponent();
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            titlText = newTitl;
            UpdateTextParams(paramCount, true, paramNames, unitNames, nowEnabled, nowValues);
            this.label1.Text = titlText;
        }

        public 
        EnaDisaParamControl(int paramCount, string newTitl, 
                            List<string> paramNames, List<string> unitNames, List<string> nowValues)
        {
            InitializeComponent();
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            titlText = newTitl;
            UpdateTextParams(paramCount, false, paramNames, unitNames, false,nowValues);
            this.label1.Text = titlText;
        }


        private void enaBtn_Click_1(object sender, EventArgs e)
        {
            if (enableFlag) { return; }
            enableFlag = true;
            enaBtn.BackColor = Color.Lime;
            disaBtn.BackColor = Color.LightGray;
            label1.Text = titlText + "*";
        }

        private void disaBtn_Click_1(object sender, EventArgs e)
        {
            if (!enableFlag) { return; }
            enableFlag = false;
            disaBtn.BackColor = Color.Lime;
            enaBtn.BackColor = Color.LightGray;
            label1.Text = titlText + "*";
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            bool b = true;
            appliedParams = new List<double>();
            double f;
            int i = 0;
            for (i = 0; i < textBoxs.Count; i++)
            {
                f = 0;
                if (!double.TryParse(textBoxs[i].Text, out f))
                {
                    b = false;
                    break;
                }
                appliedParams.Add(f);
            }
            if (!b) { goto applyError; }



            label1.Text = titlText;
            if (OnParamsApplied != null) OnParamsApplied(this, new EventArgs());

            return;
            applyError:
            {
                ErrorParams();
            }

        }

        public void ErrorParams()
        {

            MessageBox.Show("Неверно заданы параметры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = titlText + "*";
            
        }

        private void tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
