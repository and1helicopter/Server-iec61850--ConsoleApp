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
    public partial class InputValueForm : Form
    {
        public static Point FormLocation = new Point(120,60);
        public static Font ValueFont = new Font("Arial", 13, FontStyle.Bold);
        public static Font ButtonFont = new Font("Arial",15,FontStyle.Bold);
        public static Size FormSize = new Size(400, 360);
        public static string MinString = "Мин";
        public static string MaxString = "Макс";
        public static Font TitlFont = new Font("Arial", 12, FontStyle.Bold);

        void SetFonts()
        {
            button1.Font =
                button2.Font =
                button3.Font =
                button4.Font =
                button5.Font =
                button6.Font =
                button7.Font =
                button8.Font =
                button9.Font =
                button10.Font =
                pointButton.Font = signButton.Font = ButtonFont;
            label2.Font = label3.Font = label4.Font = label5.Font = label6.Font = valueLabel.Font = ValueFont;
            label2.Text = MinString;
            label3.Text = MaxString;
            label6.Font = TitlFont;
        }

        public InputValueForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Location = FormLocation;
            valueLabel.Text = "0";
            SetFonts();
        }

        public InputValueForm(int ShowMode, InputValueFormSettings ShowSettings)
        {
            InitializeComponent();
            valueLabel.Text = "0";
            SetFonts();



            if (ShowMode == 1)
            {
                this.TopMost = true;
                this.StartPosition = FormStartPosition.Manual;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Location = FormLocation;
                this.Size = FormSize;

                this.SecretMode = false;
                this.FloatCount = ShowSettings.DigitCount;
                this.FloatPossible = (ShowSettings.DigitCount != 0);
                this.SignPossible = ShowSettings.SignPossible;
                this.MinValue = ShowSettings.Min;
                this.MaxValue = ShowSettings.Max;
            }

        }
        public bool FloatPossible
        {
            get { return (pointButton.Visible); }
            set
            {
                if (value)
                {
                    pointButton.Visible = true;
                }
                else
                {
                    pointButton.Visible = false;
                    FloatCount = 0;
                }
            }
        }

        public bool SignPossible
        {
            get { return (signButton.Visible); }
            set
            {
                signButton.Visible = value;
            }
        }

        string valueStr = "";
        public double OutValue { get; set; }

        public string TitlString
        {
            get { return label6.Text; }
            set { label6.Text = value; }
        }

        bool secretMode = false;
        public bool SecretMode
        {
            get { return secretMode; }
            set
            {
                secretMode = value;
                if (!secretMode)
                {
                    label2.Visible
                        = label3.Visible
                        = label4.Visible
                        = label5.Visible
                        = signButton.Visible
                        = pointButton.Visible = true;

                }
                else
                {
                    label2.Visible
                        = label3.Visible
                        = label4.Visible
                        = label5.Visible
                        = signButton.Visible
                        = pointButton.Visible = false;

                }
                UpdateValueStr();
            }
        }

        int floatCount = 1;
        public int FloatCount
        {
            get { return floatCount; }
            set
            {
                floatCount = value;
                label5.Text = maxValue.ToString("F" + floatCount.ToString());
                label4.Text = minValue.ToString("F" + floatCount.ToString());
            }
        }

        double maxValue = 0;
        public double MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                label5.Text = maxValue.ToString("F" + floatCount.ToString());
            }
        }

        double minValue = 0;
        public double MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                minValue = value;
                label4.Text = minValue.ToString("F" + floatCount.ToString());
            }
        }

        void UpdateValueStr()
        {
            if (secretMode)
            {
                valueLabel.Text = "";
                for (int i = 0; i < valueStr.Length; i++)
                {
                    valueLabel.Text = (string)(valueLabel.Text) + "*";
                }
            }
            else
            {
                valueLabel.Text = valueStr;
                double f;
                if (!double.TryParse(valueStr, out f))
                {
                    return;
                }


                if (f > maxValue)
                {
                    //MessageBox.Show("1");
                    label5.BackColor = Color.Yellow;
                }
                else
                {
                    // MessageBox.Show("2");
                    label5.BackColor = Color.White;
                }

                if (f < minValue)
                {
                    label4.BackColor = Color.Yellow;
                }
                else
                {
                    label4.BackColor = Color.White;
                }
            }
        }

        private void digitButtonClick(object sender, EventArgs e)
        {
            valueStr = valueStr + (sender as Button).Text.ToString();
            UpdateValueStr();
        }

        private void signButton_Click(object sender, EventArgs e)
        {
            double f;
            if (!double.TryParse(valueStr, out f)) { return; }
            if (f < 0) { valueStr = valueStr.Remove(0, 1); }
            if (f > 0) { valueStr = "-" + valueStr; }
            UpdateValueStr();
        }

        private void pointButton_Click(object sender, EventArgs e)
        {
            Char separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            if (valueStr.Contains(separator)) { return; }
            if (valueStr.Length > 0)
            {
                valueStr = valueStr + separator;
            }
            else
            {
                valueStr = "0" + separator;
            }
            UpdateValueStr();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            double f;

            if (valueLabel.Text.ToString() == "0") { valueStr = "0"; }

            if (double.TryParse(valueStr, out f))
            {
                if ((f >= minValue) && (f <= maxValue))
                {
                    OutValue = f;
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                }
                //Close();
            }
            else
            {
                //  label2.ForeColor = Color.Red;
            }
        }

        private void backSpaceBtn_Click(object sender, EventArgs e)
        {
            if (valueStr.Length < 1)
            {
                return;
            }
            valueStr = valueStr.Remove(valueStr.Length - 1, 1);
            UpdateValueStr();
            if (valueStr.Length < 1)
            {
                label4.BackColor = label5.BackColor = Color.White;
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            valueStr = "";
            valueLabel.Text = "";
            label4.BackColor = label5.BackColor = Color.White;
        }
    }
}
