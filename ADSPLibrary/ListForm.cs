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
    public partial class ListForm : Form
    {
        Label[] listLabels;
        int blockCount = 0;
        ushort[] oldMasks;
        ushort[] newMasks;

        public ListForm(int newBlockCount, string[] listTexts,string newTitl)
        {
            blockCount = newBlockCount;
            oldMasks = new ushort[blockCount];
            newMasks = new ushort[blockCount];
            InitializeComponent();
            this.Text = newTitl;
            titlLabel.Text = newTitl;
            Size size = new Size(flowLayoutPanel.Width - 30, 15);
            listLabels = new Label[blockCount * 16];
            //MessageBox.Show(listTexts[0]);

            for (int i = 0; i < blockCount; i++)
            {
                oldMasks[i] = 0;
            }
            for (int i = 0; i < (blockCount * 16); i++)
            {
                listLabels[i] = new Label();
                listLabels[i].AutoSize = true;
                listLabels[i].Visible = false;
                listLabels[i].Height = 0;
                listLabels[i].MinimumSize = size;
                listLabels[i].MaximumSize = size;
                listLabels[i].Text = listTexts[i];
                flowLayoutPanel.Controls.Add(listLabels[i]);
            }
        }

        public void UpdateList(ushort[] masks)
        {

            newMasks = masks;
        }

        private void ListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void ListForm_Load(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ushort mask;
            for (int i = 0; i < blockCount; i++)
            {
                mask = newMasks[i];
                if (mask != oldMasks[i])
                {
                    //MessageBox.Show(mask.ToString());
                    oldMasks[i] = mask;
                    for (int i1 = 0; i1 < 16; i1++)
                    {
                        if ((mask & 0x0001) != 0) { listLabels[i * 16 + i1].Visible = true; }
                        else { listLabels[i * 16 + i1].Visible = false; }
                        mask = (ushort)(mask >> 1);
                    }

                }
            }
        }
    }
}
