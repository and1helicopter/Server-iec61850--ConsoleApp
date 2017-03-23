namespace ADSPLibrary
{
    partial class MeasureInstrumPanel
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.valueLabel1 = new System.Windows.Forms.Label();
            this.valueLabel2 = new System.Windows.Forms.Label();
            this.unitLabel1 = new System.Windows.Forms.Label();
            this.unitLabel2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.BackColor = System.Drawing.Color.White;
            this.nameLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.nameLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nameLabel.Location = new System.Drawing.Point(0, 178);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(300, 27);
            this.nameLabel.TabIndex = 12;
            this.nameLabel.Text = "Активная мощность";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // valueLabel1
            // 
            this.valueLabel1.BackColor = System.Drawing.Color.White;
            this.valueLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.valueLabel1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.valueLabel1.Location = new System.Drawing.Point(61, 152);
            this.valueLabel1.Name = "valueLabel1";
            this.valueLabel1.Size = new System.Drawing.Size(60, 20);
            this.valueLabel1.TabIndex = 14;
            this.valueLabel1.Text = "label1";
            this.valueLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.valueLabel1.Visible = false;
            // 
            // valueLabel2
            // 
            this.valueLabel2.BackColor = System.Drawing.Color.White;
            this.valueLabel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.valueLabel2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.valueLabel2.Location = new System.Drawing.Point(202, 152);
            this.valueLabel2.Name = "valueLabel2";
            this.valueLabel2.Size = new System.Drawing.Size(60, 20);
            this.valueLabel2.TabIndex = 15;
            this.valueLabel2.Text = "label1";
            this.valueLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.valueLabel2.Visible = false;
            // 
            // unitLabel1
            // 
            this.unitLabel1.AutoSize = true;
            this.unitLabel1.BackColor = System.Drawing.Color.Transparent;
            this.unitLabel1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.unitLabel1.ForeColor = System.Drawing.Color.White;
            this.unitLabel1.Location = new System.Drawing.Point(127, 154);
            this.unitLabel1.Name = "unitLabel1";
            this.unitLabel1.Size = new System.Drawing.Size(47, 16);
            this.unitLabel1.TabIndex = 16;
            this.unitLabel1.Text = "label1";
            this.unitLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.unitLabel1.Visible = false;
            // 
            // unitLabel2
            // 
            this.unitLabel2.AutoSize = true;
            this.unitLabel2.BackColor = System.Drawing.Color.Transparent;
            this.unitLabel2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.unitLabel2.ForeColor = System.Drawing.Color.White;
            this.unitLabel2.Location = new System.Drawing.Point(250, 154);
            this.unitLabel2.Name = "unitLabel2";
            this.unitLabel2.Size = new System.Drawing.Size(47, 16);
            this.unitLabel2.TabIndex = 17;
            this.unitLabel2.Text = "label1";
            this.unitLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.unitLabel2.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ADSPLibrary.Properties.Resources.Scale_Corel_0_300;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(300, 145);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ADSPLibrary.Properties.Resources.ScaleCorel1;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 205);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MeasureInstrumPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.unitLabel2);
            this.Controls.Add(this.unitLabel1);
            this.Controls.Add(this.valueLabel2);
            this.Controls.Add(this.valueLabel1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.pictureBox1);
            this.Name = "MeasureInstrumPanel";
            this.Size = new System.Drawing.Size(300, 205);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label valueLabel1;
        private System.Windows.Forms.Label valueLabel2;
        private System.Windows.Forms.Label unitLabel1;
        private System.Windows.Forms.Label unitLabel2;
        private System.Windows.Forms.Timer timer1;

    }
}
