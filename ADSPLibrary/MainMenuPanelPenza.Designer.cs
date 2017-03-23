namespace ADSPLibrary
{
    partial class MainMenuPanelPenza
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Номинальные параметры генератора");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Настройка ПИД-регулятора");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Системный стабилизатор");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Статизм");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Регулятор напряжения", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Регулятор тока возбуждения");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Регулятор ОМВ (реактивного тока)");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Регулятор реактивного тока");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Регулятор cosФ");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Медленнодействующие регуляторы", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Настройка ограничителя");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Ограничитель перегрева", new System.Windows.Forms.TreeNode[] {
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Ограничитель минимального возбуждения");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Настройка регулятора", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode10,
            treeNode12,
            treeNode13});
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Регулятор напряжения статора");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Регулятор тока ротора");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Регулятор угла Ф");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Регулятор реактивного тока");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Ограничение уставок", new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18});
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Релейная форсировка");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("V/Hz - ограничение");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Начальное возбуждение");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Инвертирование токов");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Контроль изоляции");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Уставка по умолчанию в РРВ");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("Конфигурация системы", new System.Windows.Forms.TreeNode[] {
            treeNode20,
            treeNode21,
            treeNode22,
            treeNode23,
            treeNode24,
            treeNode25});
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Превышение напряжения статора");
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("МТЗ тиристорного преобразователя");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("МТЗ трансформатора");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("МТЗ трансформатора 2");
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Превышение напряжения питания");
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Обрыв цепей возбуждения");
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Превышение тока ротора");
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Асинхронный ход");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Защита от низкой частоты на ХХ");
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("Защита от перегрузки ротора");
            System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("Настройка защит и предупреждений", new System.Windows.Forms.TreeNode[] {
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30,
            treeNode31,
            treeNode32,
            treeNode33,
            treeNode34,
            treeNode35,
            treeNode36});
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(809, 36);
            this.label1.TabIndex = 1;
            this.label1.Text = "НАСТРОЙКА ПАРАМЕТРОВ СИСТЕМЫ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 241F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 36);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(809, 454);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Узел0";
            treeNode1.Tag = "0";
            treeNode1.Text = "Номинальные параметры генератора";
            treeNode2.Name = "Узел2";
            treeNode2.Tag = "1";
            treeNode2.Text = "Настройка ПИД-регулятора";
            treeNode3.Name = "Узел9";
            treeNode3.Tag = "7";
            treeNode3.Text = "Системный стабилизатор";
            treeNode4.Name = "Узел8";
            treeNode4.Tag = "6";
            treeNode4.Text = "Статизм";
            treeNode5.Name = "Узел0";
            treeNode5.Text = "Регулятор напряжения";
            treeNode6.Name = "Узел0";
            treeNode6.Tag = "2";
            treeNode6.Text = "Регулятор тока возбуждения";
            treeNode7.Name = "Узел1";
            treeNode7.Tag = "3";
            treeNode7.Text = "Регулятор ОМВ (реактивного тока)";
            treeNode8.Name = "Узел4";
            treeNode8.Tag = "4";
            treeNode8.Text = "Регулятор реактивного тока";
            treeNode9.Name = "Узел5";
            treeNode9.Tag = "5";
            treeNode9.Text = "Регулятор cosФ";
            treeNode10.Name = "Узел3";
            treeNode10.Text = "Медленнодействующие регуляторы";
            treeNode11.Name = "Узел13";
            treeNode11.Tag = "10";
            treeNode11.Text = "Настройка ограничителя";
            treeNode12.Name = "Узел11";
            treeNode12.Text = "Ограничитель перегрева";
            treeNode13.Name = "Узел14";
            treeNode13.Tag = "11";
            treeNode13.Text = "Ограничитель минимального возбуждения";
            treeNode14.Name = "Узел1";
            treeNode14.Text = "Настройка регулятора";
            treeNode15.Name = "Узел1";
            treeNode15.Tag = "12";
            treeNode15.Text = "Регулятор напряжения статора";
            treeNode16.Name = "Узел2";
            treeNode16.Tag = "13";
            treeNode16.Text = "Регулятор тока ротора";
            treeNode17.Name = "Узел3";
            treeNode17.Tag = "14";
            treeNode17.Text = "Регулятор угла Ф";
            treeNode18.Name = "Узел4";
            treeNode18.Tag = "15";
            treeNode18.Text = "Регулятор реактивного тока";
            treeNode19.Name = "Узел0";
            treeNode19.Tag = "";
            treeNode19.Text = "Ограничение уставок";
            treeNode20.Name = "Узел3";
            treeNode20.Tag = "16";
            treeNode20.Text = "Релейная форсировка";
            treeNode21.Name = "Узел5";
            treeNode21.Tag = "17";
            treeNode21.Text = "V/Hz - ограничение";
            treeNode22.Name = "Узел1";
            treeNode22.Tag = "18";
            treeNode22.Text = "Начальное возбуждение";
            treeNode23.Name = "Узел2";
            treeNode23.Tag = "19";
            treeNode23.Text = "Инвертирование токов";
            treeNode24.Name = "Узел6";
            treeNode24.Tag = "20";
            treeNode24.Text = "Контроль изоляции";
            treeNode25.Name = "Узел7";
            treeNode25.Tag = "21";
            treeNode25.Text = "Уставка по умолчанию в РРВ";
            treeNode26.Name = "Узел0";
            treeNode26.Text = "Конфигурация системы";
            treeNode27.Name = "Узел9";
            treeNode27.Tag = "22";
            treeNode27.Text = "Превышение напряжения статора";
            treeNode28.Name = "Узел10";
            treeNode28.Tag = "23";
            treeNode28.Text = "МТЗ тиристорного преобразователя";
            treeNode29.Name = "Узел11";
            treeNode29.Tag = "24";
            treeNode29.Text = "МТЗ трансформатора";
            treeNode30.Name = "Узел0";
            treeNode30.Tag = "29";
            treeNode30.Text = "МТЗ трансформатора 2";
            treeNode31.Name = "Узел12";
            treeNode31.Tag = "25";
            treeNode31.Text = "Превышение напряжения питания";
            treeNode32.Name = "Узел13";
            treeNode32.Tag = "26";
            treeNode32.Text = "Обрыв цепей возбуждения";
            treeNode33.Name = "Узел14";
            treeNode33.Tag = "27";
            treeNode33.Text = "Превышение тока ротора";
            treeNode34.Name = "Узел15";
            treeNode34.Tag = "28";
            treeNode34.Text = "Асинхронный ход";
            treeNode35.Name = "Узел0";
            treeNode35.Tag = "30";
            treeNode35.Text = "Защита от низкой частоты на ХХ";
            treeNode36.Name = "Защита от перегрузки ротора";
            treeNode36.Tag = "31";
            treeNode36.Text = "Защита от перегрузки ротора";
            treeNode37.Name = "Узел8";
            treeNode37.Text = "Настройка защит и предупреждений";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode14,
            treeNode19,
            treeNode26,
            treeNode37});
            this.treeView1.Size = new System.Drawing.Size(235, 448);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(244, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(562, 448);
            this.panel1.TabIndex = 1;
            // 
            // MainMenuPanelPenza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Name = "MainMenuPanelPenza";
            this.Size = new System.Drawing.Size(809, 490);
            this.Load += new System.EventHandler(this.MainMenuPanelPenza_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
    }
}
