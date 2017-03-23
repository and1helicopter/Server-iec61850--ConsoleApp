namespace ADSPLibrary
{
    partial class MeasurePanelSTS
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lampLabel1 = new System.Windows.Forms.Label();
            this.lampLabel2 = new System.Windows.Forms.Label();
            this.lampLabel3 = new System.Windows.Forms.Label();
            this.lampLabel4 = new System.Windows.Forms.Label();
            this.lampLabel5 = new System.Windows.Forms.Label();
            this.lampLabel6 = new System.Windows.Forms.Label();
            this.lampLabel7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.refValueLabel = new System.Windows.Forms.Label();
            this.refLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnARMU = new System.Windows.Forms.Button();
            this.btnARMIre = new System.Windows.Forms.Button();
            this.btnARMPF = new System.Windows.Forms.Button();
            this.btnARMMAN = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1020, 54);
            this.label1.TabIndex = 1;
            this.label1.Text = "ТУРБОГЕНЕРАТОР ТГ5";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel1.Controls.Add(this.lampLabel1);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel2);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel3);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel4);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel5);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel6);
            this.flowLayoutPanel1.Controls.Add(this.lampLabel7);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 54);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1020, 64);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // lampLabel1
            // 
            this.lampLabel1.AutoEllipsis = true;
            this.lampLabel1.BackColor = System.Drawing.Color.White;
            this.lampLabel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel1.CausesValidation = false;
            this.lampLabel1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel1.Location = new System.Drawing.Point(3, 3);
            this.lampLabel1.Margin = new System.Windows.Forms.Padding(3);
            this.lampLabel1.Name = "lampLabel1";
            this.lampLabel1.Size = new System.Drawing.Size(124, 53);
            this.lampLabel1.TabIndex = 0;
            this.lampLabel1.Text = "ГОТОВНОСТЬ";
            this.lampLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lampLabel1.Click += new System.EventHandler(this.LampLabel1_Click);
            // 
            // lampLabel2
            // 
            this.lampLabel2.AutoEllipsis = true;
            this.lampLabel2.BackColor = System.Drawing.Color.White;
            this.lampLabel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel2.CausesValidation = false;
            this.lampLabel2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel2.Location = new System.Drawing.Point(133, 3);
            this.lampLabel2.Margin = new System.Windows.Forms.Padding(3);
            this.lampLabel2.Name = "lampLabel2";
            this.lampLabel2.Size = new System.Drawing.Size(124, 53);
            this.lampLabel2.TabIndex = 1;
            this.lampLabel2.Text = "РАБОТА";
            this.lampLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampLabel3
            // 
            this.lampLabel3.AutoEllipsis = true;
            this.lampLabel3.BackColor = System.Drawing.Color.White;
            this.lampLabel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel3.CausesValidation = false;
            this.lampLabel3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel3.Location = new System.Drawing.Point(263, 3);
            this.lampLabel3.Margin = new System.Windows.Forms.Padding(3);
            this.lampLabel3.Name = "lampLabel3";
            this.lampLabel3.Size = new System.Drawing.Size(124, 53);
            this.lampLabel3.TabIndex = 2;
            this.lampLabel3.Text = "В СЕТИ";
            this.lampLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampLabel4
            // 
            this.lampLabel4.AutoEllipsis = true;
            this.lampLabel4.BackColor = System.Drawing.Color.White;
            this.lampLabel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel4.CausesValidation = false;
            this.lampLabel4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel4.Location = new System.Drawing.Point(393, 3);
            this.lampLabel4.Margin = new System.Windows.Forms.Padding(3);
            this.lampLabel4.Name = "lampLabel4";
            this.lampLabel4.Size = new System.Drawing.Size(124, 53);
            this.lampLabel4.TabIndex = 6;
            this.lampLabel4.Text = "РУЧНОЙ РЕЖИМ";
            this.lampLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampLabel5
            // 
            this.lampLabel5.AutoEllipsis = true;
            this.lampLabel5.BackColor = System.Drawing.Color.White;
            this.lampLabel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel5.CausesValidation = false;
            this.lampLabel5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel5.Location = new System.Drawing.Point(523, 3);
            this.lampLabel5.Margin = new System.Windows.Forms.Padding(3);
            this.lampLabel5.Name = "lampLabel5";
            this.lampLabel5.Size = new System.Drawing.Size(124, 53);
            this.lampLabel5.TabIndex = 3;
            this.lampLabel5.Text = "ОГРАНИ- ЧИТЕЛЬ";
            this.lampLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampLabel6
            // 
            this.lampLabel6.AutoEllipsis = true;
            this.lampLabel6.BackColor = System.Drawing.Color.White;
            this.lampLabel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel6.CausesValidation = false;
            this.lampLabel6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel6.Location = new System.Drawing.Point(653, 3);
            this.lampLabel6.Margin = new System.Windows.Forms.Padding(3);
            this.lampLabel6.Name = "lampLabel6";
            this.lampLabel6.Size = new System.Drawing.Size(124, 53);
            this.lampLabel6.TabIndex = 4;
            this.lampLabel6.Text = "ПРЕДУПРЕЖ-  ДЕНИЕ";
            this.lampLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lampLabel6.Click += new System.EventHandler(this.lampLabel6_Click);
            // 
            // lampLabel7
            // 
            this.lampLabel7.AutoEllipsis = true;
            this.lampLabel7.BackColor = System.Drawing.Color.White;
            this.lampLabel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampLabel7.CausesValidation = false;
            this.lampLabel7.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lampLabel7.Location = new System.Drawing.Point(783, 3);
            this.lampLabel7.Margin = new System.Windows.Forms.Padding(3);
            this.lampLabel7.Name = "lampLabel7";
            this.lampLabel7.Size = new System.Drawing.Size(124, 53);
            this.lampLabel7.TabIndex = 5;
            this.lampLabel7.Text = "АВАРИЯ";
            this.lampLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lampLabel7.Click += new System.EventHandler(this.LampLabel7_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.refValueLabel);
            this.panel1.Controls.Add(this.refLabel);
            this.panel1.Controls.Add(this.flowLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 118);
            this.panel1.Margin = new System.Windows.Forms.Padding(10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1020, 611);
            this.panel1.TabIndex = 3;
            this.panel1.Tag = "0";
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // refValueLabel
            // 
            this.refValueLabel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.refValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.refValueLabel.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.refValueLabel.Location = new System.Drawing.Point(479, 576);
            this.refValueLabel.Name = "refValueLabel";
            this.refValueLabel.Size = new System.Drawing.Size(84, 25);
            this.refValueLabel.TabIndex = 2;
            this.refValueLabel.Text = "100,0";
            this.refValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // refLabel
            // 
            this.refLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.refLabel.Location = new System.Drawing.Point(346, 576);
            this.refLabel.Name = "refLabel";
            this.refLabel.Size = new System.Drawing.Size(111, 25);
            this.refLabel.TabIndex = 1;
            this.refLabel.Text = "Уставка, %";
            this.refLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel2.Controls.Add(this.btnARMU);
            this.flowLayoutPanel2.Controls.Add(this.btnARMIre);
            this.flowLayoutPanel2.Controls.Add(this.btnARMPF);
            this.flowLayoutPanel2.Controls.Add(this.btnARMMAN);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(334, 437);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(254, 107);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // btnARMU
            // 
            this.btnARMU.BackColor = System.Drawing.Color.Lime;
            this.btnARMU.Enabled = false;
            this.btnARMU.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnARMU.Location = new System.Drawing.Point(3, 3);
            this.btnARMU.Name = "btnARMU";
            this.btnARMU.Size = new System.Drawing.Size(119, 46);
            this.btnARMU.TabIndex = 0;
            this.btnARMU.Text = "Рег. U";
            this.btnARMU.UseVisualStyleBackColor = false;
            this.btnARMU.Click += new System.EventHandler(this.btnARMU_Click);
            // 
            // btnARMIre
            // 
            this.btnARMIre.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnARMIre.Enabled = false;
            this.btnARMIre.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnARMIre.Location = new System.Drawing.Point(128, 3);
            this.btnARMIre.Name = "btnARMIre";
            this.btnARMIre.Size = new System.Drawing.Size(119, 46);
            this.btnARMIre.TabIndex = 1;
            this.btnARMIre.Text = "Рег. Ire";
            this.btnARMIre.UseVisualStyleBackColor = false;
            // 
            // btnARMPF
            // 
            this.btnARMPF.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnARMPF.Enabled = false;
            this.btnARMPF.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnARMPF.Location = new System.Drawing.Point(3, 55);
            this.btnARMPF.Name = "btnARMPF";
            this.btnARMPF.Size = new System.Drawing.Size(119, 46);
            this.btnARMPF.TabIndex = 2;
            this.btnARMPF.Text = "Рег. PF";
            this.btnARMPF.UseVisualStyleBackColor = false;
            // 
            // btnARMMAN
            // 
            this.btnARMMAN.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnARMMAN.Enabled = false;
            this.btnARMMAN.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnARMMAN.Location = new System.Drawing.Point(128, 55);
            this.btnARMMAN.Name = "btnARMMAN";
            this.btnARMMAN.Size = new System.Drawing.Size(119, 46);
            this.btnARMMAN.TabIndex = 3;
            this.btnARMMAN.Text = "Рег. If";
            this.btnARMMAN.UseVisualStyleBackColor = false;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 250;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // MeasurePanelSTS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Name = "MeasurePanelSTS";
            this.Size = new System.Drawing.Size(1020, 729);
            this.Tag = "0";
            this.Load += new System.EventHandler(this.MeasurePanelSTS_Load);
            this.VisibleChanged += new System.EventHandler(this.MeasurePanelSTS_VisibleChanged);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lampLabel1;
        private System.Windows.Forms.Label lampLabel2;
        private System.Windows.Forms.Label lampLabel3;
        private System.Windows.Forms.Label lampLabel5;
        private System.Windows.Forms.Label lampLabel6;
        private System.Windows.Forms.Label lampLabel7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lampLabel4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label refValueLabel;
        private System.Windows.Forms.Label refLabel;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button btnARMU;
        private System.Windows.Forms.Button btnARMIre;
        private System.Windows.Forms.Button btnARMPF;
        private System.Windows.Forms.Button btnARMMAN;
    }
}
