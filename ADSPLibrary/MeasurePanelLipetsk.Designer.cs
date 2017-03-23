namespace ADSPLibrary
{
    partial class MeasurePanelLipetsk
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.titlLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lampLabel1 = new System.Windows.Forms.Label();
            this.lampLabel2 = new System.Windows.Forms.Label();
            this.lampLabel3 = new System.Windows.Forms.Label();
            this.lampLabel4 = new System.Windows.Forms.Label();
            this.lampLabel5 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // titlLabel
            // 
            this.titlLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.titlLabel.Location = new System.Drawing.Point(0, 0);
            this.titlLabel.Name = "titlLabel";
            this.titlLabel.Size = new System.Drawing.Size(615, 39);
            this.titlLabel.TabIndex = 1;
            this.titlLabel.Text = "ИЗМЕРЯЕМЫЕ ПАРАМЕТРЫ";
            this.titlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lampLabel1);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel2);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel3);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel4);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel5);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 39);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(615, 42);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // lampLabel1
            // 
            this.lampLabel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lampLabel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel1.Location = new System.Drawing.Point(3, 0);
            this.lampLabel1.Name = "lampLabel1";
            this.lampLabel1.Size = new System.Drawing.Size(115, 39);
            this.lampLabel1.TabIndex = 4;
            this.lampLabel1.Text = "Готовность";
            this.lampLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lampLabel1.Click += new System.EventHandler(this.label6_Click);
            // 
            // lampLabel2
            // 
            this.lampLabel2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lampLabel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel2.Location = new System.Drawing.Point(124, 0);
            this.lampLabel2.Name = "lampLabel2";
            this.lampLabel2.Size = new System.Drawing.Size(115, 39);
            this.lampLabel2.TabIndex = 5;
            this.lampLabel2.Text = "Работа";
            this.lampLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampLabel3
            // 
            this.lampLabel3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lampLabel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel3.Location = new System.Drawing.Point(245, 0);
            this.lampLabel3.Name = "lampLabel3";
            this.lampLabel3.Size = new System.Drawing.Size(115, 39);
            this.lampLabel3.TabIndex = 6;
            this.lampLabel3.Text = "РРВ";
            this.lampLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampLabel4
            // 
            this.lampLabel4.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lampLabel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel4.Location = new System.Drawing.Point(366, 0);
            this.lampLabel4.Name = "lampLabel4";
            this.lampLabel4.Size = new System.Drawing.Size(115, 39);
            this.lampLabel4.TabIndex = 7;
            this.lampLabel4.Text = "Предупреждение";
            this.lampLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lampLabel4.Click += new System.EventHandler(this.label4_Click);
            // 
            // lampLabel5
            // 
            this.lampLabel5.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lampLabel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel5.Location = new System.Drawing.Point(487, 0);
            this.lampLabel5.Name = "lampLabel5";
            this.lampLabel5.Size = new System.Drawing.Size(115, 39);
            this.lampLabel5.TabIndex = 8;
            this.lampLabel5.Text = "Авария";
            this.lampLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lampLabel5.Click += new System.EventHandler(this.label5_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MeasurePanelLipetsk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.titlLabel);
            this.Name = "MeasurePanelLipetsk";
            this.Size = new System.Drawing.Size(615, 101);
            this.Tag = "0";
            this.Load += new System.EventHandler(this.MeasurePanelLipetsk_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label titlLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lampLabel1;
        private System.Windows.Forms.Label lampLabel2;
        private System.Windows.Forms.Label lampLabel3;
        private System.Windows.Forms.Label lampLabel4;
        private System.Windows.Forms.Label lampLabel5;
        private System.Windows.Forms.Timer timer1;
    }
}
