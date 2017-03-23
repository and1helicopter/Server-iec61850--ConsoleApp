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
    public partial class DigitPanelWinForms : UserControl
    {
        Label[] ellipses = new Label[16];
        Label[] textBlocks = new Label[16];
        Button[] setButtons = new Button[16];
        Button[] resetButtons = new Button[16];

        bool[] seted = new bool[16];
        bool[] reseted = new bool[16];


        public DigitPanelWinForms()
        {
            InitializeComponent();

            for (int i = 0; i < 16; i++)
            {
                ellipses[i] = new Label();
                ellipses[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                ellipses[i].Margin = new Padding(2, 2, 2, 2);
                ellipses[i].BackColor = Color.White;
                ellipses[i].Dock = DockStyle.Fill;
                ellipses[i].Width = ellipses[i].Height;
                tableLayoutPanel1.Controls.Add(ellipses[i], 0, i);

                textBlocks[i] = new Label();
                textBlocks[i].TextAlign = ContentAlignment.MiddleLeft;
                textBlocks[i].Text = "Дискрет. №" + (i + 1).ToString();
                textBlocks[i].Margin = new Padding(2, 2, 2, 2);
                textBlocks[i].Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(textBlocks[i], 1, i);

                setButtons[i] = new Button();
                setButtons[i].BackColor = Color.LightGray;
                setButtons[i].Margin = new Padding(0);
                setButtons[i].Dock = DockStyle.Fill;
                setButtons[i].Width = setButtons[i].Height;
                setButtons[i].Click +=new EventHandler(SetButtonClick);
                tableLayoutPanel1.Controls.Add(setButtons[i], 2, i);

                resetButtons[i] = new Button();
                resetButtons[i].BackColor = Color.LightGray;
                resetButtons[i].Margin = new Padding(0);
                resetButtons[i].Dock = DockStyle.Fill;
                resetButtons[i].Width = resetButtons[i].Height;
                resetButtons[i].Click += new EventHandler(ResetButtonClick);
                tableLayoutPanel1.Controls.Add(resetButtons[i], 3, i);
            }

        }

        private void DigitPanelWinForms_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                ellipses[i].Width = ellipses[i].Height;
                setButtons[i].Width = setButtons[i].Height;
                resetButtons[i].Width = resetButtons[i].Height;
                setButtons[i].Tag = resetButtons[i].Tag = i;
            }
        }

        public void SetDigitNames(List<String> NewDigitNames)
        {
            int i;
            for (i = 0; ((i < NewDigitNames.Count) && (i < 16)); i++)
            {
                textBlocks[i].Text = NewDigitNames[i];
            }
        }

        ushort digitValue = 0;
        public ushort DigitValue
        {
            get { return digitValue; }
            set
            {
                if (value == digitValue) { return; }
                digitValue = value;
                ushort w = 1;
                int i;
                for (i = 0; i < 16; i++)
                {
                    if ((digitValue & w) != 0) { ellipses[i].BackColor = Color.Lime; }
                    else { ellipses[i].BackColor = Color.White; }
                    w = (ushort)(w << 1);
                }
            }
        }

        public bool SetupMode
        {
            get
            {
                return (setButtons[0].Visible);
            }
            set
            {
                int i;
                if (value)
                {
                    for (i = 0; i < 16; i++)
                    {
                        setButtons[i].Visible = true;
                        resetButtons[i].Visible = true;
                    }
                }
                else
                {
                    for (i = 0; i < 16; i++)
                    {
                        setButtons[i].Visible = false;
                        resetButtons[i].Visible = false;
                    }
                }
            }
        }

        private void SetButtonClick(object sender, EventArgs e)
        {
            int i;
            if (!int.TryParse((sender as Button).Tag.ToString(), out i)) { return; }
            seted[i] = !(seted[i]);
            reseted[i] = reseted[i] & (!(seted[i]));

            if (seted[i]) setButtons[i].BackColor = Color.Yellow;
            else setButtons[i].BackColor = Color.LightGray;

            if (reseted[i]) resetButtons[i].BackColor = Color.Yellow;
            else resetButtons[i].BackColor = Color.LightGray;
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            int i;
            if (!int.TryParse((sender as Button).Tag.ToString(), out i)) { return; }
            reseted[i] = !(reseted[i]);
            seted[i] = seted[i] & (!(reseted[i]));

            if (seted[i]) setButtons[i].BackColor = Color.Yellow;
            else setButtons[i].BackColor = Color.LightGray;

            if (reseted[i]) resetButtons[i].BackColor = Color.Yellow;
            else resetButtons[i].BackColor = Color.LightGray;
        }

        public ushort MaskOR
        {
            get
            {
                ushort u = 0;
                int i;
                ushort s = 1;
                for (i = 0; i < 16; i++)
                {
                    if (seted[i]) { u = (ushort)(u | s); }
                    s = (ushort)(s << 1);
                }

                return u;
            }
            set { }
        }
        public ushort MaskAND
        {
            get
            {
                ushort u = 0xFFFF;
                int i;
                ushort s = 1;
                for (i = 0; i < 16; i++)
                {
                    if (reseted[i]) { u = (ushort)(u ^ s); }
                    s = (ushort)(s << 1);
                }
                return u;
            }
            set { }
        }
    }
}
