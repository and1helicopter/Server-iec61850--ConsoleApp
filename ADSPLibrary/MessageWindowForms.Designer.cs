namespace ADSPLibrary
{
    partial class MessageWindowForms
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textButton1 = new System.Windows.Forms.Button();
            this.textButton2 = new System.Windows.Forms.Button();
            this.textButton3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(557, 170);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.Controls.Add(this.textButton1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textButton2, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.textButton3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(553, 166);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // textButton1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textButton1, 2);
            this.textButton1.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.textButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textButton1.Location = new System.Drawing.Point(98, 86);
            this.textButton1.Name = "textButton1";
            this.textButton1.Size = new System.Drawing.Size(174, 72);
            this.textButton1.TabIndex = 0;
            this.textButton1.Text = "button1";
            this.textButton1.UseVisualStyleBackColor = true;
            // 
            // textButton2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textButton2, 2);
            this.textButton2.DialogResult = System.Windows.Forms.DialogResult.No;
            this.textButton2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textButton2.Location = new System.Drawing.Point(278, 86);
            this.textButton2.Name = "textButton2";
            this.textButton2.Size = new System.Drawing.Size(174, 72);
            this.textButton2.TabIndex = 1;
            this.textButton2.Text = "button2";
            this.textButton2.UseVisualStyleBackColor = true;
            // 
            // textButton3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textButton3, 2);
            this.textButton3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.textButton3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textButton3.Location = new System.Drawing.Point(188, 8);
            this.textButton3.Name = "textButton3";
            this.textButton3.Size = new System.Drawing.Size(174, 72);
            this.textButton3.TabIndex = 2;
            this.textButton3.Text = "button3";
            this.textButton3.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(8, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 78);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MessageWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 170);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MessageWindow";
            this.Text = "MessageWindow";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button textButton1;
        private System.Windows.Forms.Button textButton2;
        private System.Windows.Forms.Button textButton3;
        private System.Windows.Forms.Label label1;
    }
}