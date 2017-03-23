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
    public partial class MessageWindowForms : Form
    {
        public static Font ElementsFont= new Font("Arial",12,FontStyle.Bold);
        public static string OkString = "Да";
        public static string NoString = "Нет";
        public static string CloseString = "Закрыть";



        public MessageWindowForms(string Text, double ParentTop, double ParentLeft, double ParentWidth, double ParentHeight, int WindowMode)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Width = (int)(0.8 * ParentWidth);
            this.Height = (int)(0.2 * ParentHeight);
            this.Top = (int)(ParentTop + 0.4 * ParentHeight);
            this.Left = (int)(ParentLeft + 0.1 * ParentWidth);
            label1.Text = Text;
            label1.Font = ElementsFont;
            textButton3.Font = textButton1.Font = textButton2.Font = label1.Font;

            textButton1.Text = OkString;
            textButton2.Text = NoString;
            textButton3.Text = CloseString;
            if (WindowMode == 0)
            {
                textButton2.Visible = textButton1.Visible = false;
                textButton3.Visible = true;
                tableLayoutPanel1.SetRow(textButton3, 1);
            }
            else
            {
                textButton2.Visible = textButton1.Visible = true;
                textButton3.Visible = false;
            }

            tableLayoutPanel1.SetColumnSpan(label1, 6);
        }


    }
}
