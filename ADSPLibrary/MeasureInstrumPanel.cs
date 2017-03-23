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
    public partial class MeasureInstrumPanel : UserControl
    {
        double minValue=0;
        public double MinValue { get { return minValue; } set { } }

        double maxValue=0;
        public double MaxValue { get { return maxValue; } set { } }

        double coefAbsolute = 2.50;

        double nowValue = 0;

        int floatCount  = 1;
        int floatCount2 = 1;

        int updateTextCounter = 0;


        Pen valuePen = new Pen(Color.Red, 4);
        Pen clearPen = new Pen(Color.FromArgb(255,255,255), 8);

        Point zeroPoint = new Point(150, 389);
        Point valuePoint = new Point(150, 248);
        Point clearValuePoint = new Point(30, 248);

        int arrowLength = 333;

        public void DrawValue(double newValue)
        {
            double drawValue = 0;

            drawValue = newValue;

            if (updateTextCounter == 0)
            {
                updateTextCounter = 4;
                valueLabel1.Text = newValue.ToString("F" + floatCount.ToString());
                valueLabel2.Text = (coefAbsolute * newValue).ToString("F" + floatCount2.ToString());
            }

            if (newValue > maxValue) { drawValue = maxValue; }
            if (newValue < minValue) { drawValue = minValue; }

            if (nowValue == drawValue) { return; }
            nowValue = drawValue;

            int newX, newY;
            double f;

            //f = 50+80.0 * (double)(nowValue - minValue) / (double)(maxValue - minValue); 
            f = 68.8 + 42.4 * (double)(nowValue - minValue) / (double)(maxValue - minValue); 
            newX = (int)(zeroPoint.X - arrowLength * Math.Cos(Math.PI * f / 180.0));
            newY = (int)(zeroPoint.Y - arrowLength * Math.Sin(Math.PI * f / 180.0));
            Graphics g = pictureBox2.CreateGraphics();
            g.DrawLine(clearPen, zeroPoint, clearValuePoint);
            valuePoint.X = newX;
            valuePoint.Y = newY;
            g.DrawLine(valuePen, zeroPoint, valuePoint);

            newX = (int)(zeroPoint.X - (arrowLength+3) * Math.Cos(Math.PI * f / 180.0));
            newY = (int)(zeroPoint.Y - (arrowLength+3) * Math.Sin(Math.PI * f / 180.0));
            clearValuePoint.X = newX;
            clearValuePoint.Y = newY;
        }

        public void DrawPF(double nowP, double nowQ)
        {
            string st;
            double nowAngle = 0;

            if (nowQ >= 0)
            {
                nowAngle = Math.Atan2(nowQ,nowP)*180.0/Math.PI;
                st = Math.Cos(nowAngle * Math.PI / 180.0).ToString("F2");

                if (nowAngle > 66.4) { nowAngle = 66.4; }
                nowAngle = 1 - Math.Cos(nowAngle * Math.PI / 180.0);
                nowAngle = nowAngle / 0.6;
                nowAngle = 90 + 21.2 * nowAngle;
            }

            else
            {
                nowQ = -nowQ;
                nowAngle = Math.Atan2(nowQ , nowP) * 180.0 / Math.PI;
                st = "-" + Math.Cos(nowAngle * Math.PI / 180.0).ToString("F2");
                if (nowAngle > 66.4) { nowAngle = 66.4; }
                nowAngle = 1 - Math.Cos(nowAngle * Math.PI / 180.0);
                nowAngle = nowAngle / 0.6;
                nowAngle = 90 - 21.2 * nowAngle;
            }

            int newX, newY;
            double f;

            f = nowAngle;
            newX = (int)(zeroPoint.X - arrowLength * Math.Cos(Math.PI * f / 180.0));
            newY = (int)(zeroPoint.Y - arrowLength * Math.Sin(Math.PI * f / 180.0));
            Graphics g = pictureBox2.CreateGraphics();
            g.DrawLine(clearPen, zeroPoint, clearValuePoint);
            valuePoint.X = newX;
            valuePoint.Y = newY;
            g.DrawLine(valuePen, zeroPoint, valuePoint);

            newX = (int)(zeroPoint.X - (arrowLength + 3) * Math.Cos(Math.PI * f / 180.0));
            newY = (int)(zeroPoint.Y - (arrowLength + 3) * Math.Sin(Math.PI * f / 180.0));
            clearValuePoint.X = newX;
            clearValuePoint.Y = newY;


            if (updateTextCounter != 0) { return; }
            updateTextCounter = 4;
            valueLabel1.Text = st;
        }

        public MeasureInstrumPanel(double newMinValue, double newMaxValue, string newTitl,Image newImage)
        {
            InitializeComponent();
            minValue = newMinValue;
            maxValue = newMaxValue;
            nowValue = -1;
            DrawValue(0);
            nameLabel.Text = newTitl;
            pictureBox2.Image = newImage;
            floatCount = 1;
        }

        public MeasureInstrumPanel(double newMinValue, double newMaxValue, 
                                    string newTitl, Image newImage, int newFloatCount)
        {
            InitializeComponent();
            minValue = newMinValue;
            maxValue = newMaxValue;
            nowValue = -1;
            DrawValue(0);
            nameLabel.Text = newTitl;
            pictureBox2.Image = newImage;

            floatCount = newFloatCount;
            if (floatCount > 3) { floatCount = 3; }
        }

        public MeasureInstrumPanel(double newMinValue, double newMaxValue,
                                    string newTitl, Image newImage, int newFloatCount1, string unitName1)
        {
            double startX;
            InitializeComponent();
            minValue = newMinValue;
            maxValue = newMaxValue;
            nowValue = -1;
            DrawValue(0);
            nameLabel.Text = newTitl;
            pictureBox2.Image = newImage;

            floatCount = newFloatCount1;
            if (floatCount > 3) { floatCount = 3; }

            valueLabel1.Visible = true;
            unitLabel1.Visible = true;
            unitLabel1.Text = unitName1;

            startX = (300 - valueLabel1.Width - unitLabel1.Width) / 2.0;
            valueLabel1.Left = (int)startX;
            unitLabel1.Left = valueLabel1.Left + valueLabel1.Width;
        }


        public MeasureInstrumPanel(double newMinValue, double newMaxValue,
                                    string newTitl, Image newImage, 
                                    int newFloatCount1, string unitName1,
                                    int newFloatCount2, string unitName2, double newCoefAbsolute
                                  )
        {
            double startX;
            InitializeComponent();
            minValue = newMinValue;
            maxValue = newMaxValue;
            nowValue = -1;
            DrawValue(0);
            nameLabel.Text = newTitl;
            pictureBox2.Image = newImage;

            floatCount = newFloatCount1;
            if (floatCount > 3) { floatCount = 3; }

            floatCount2 = newFloatCount2;
            if (floatCount2 > 3) { floatCount2 = 3; }


            valueLabel1.Visible = true;
            unitLabel1.Visible = true;
            unitLabel1.Text = unitName1;

            valueLabel2.Visible = true;
            unitLabel2.Visible = true;
            unitLabel2.Text = unitName2;

            startX = (150 - valueLabel1.Width - unitLabel1.Width) / 2.0;
            valueLabel1.Left = (int)startX;
            unitLabel1.Left = valueLabel1.Left + valueLabel1.Width;

            startX = 150+(150 - valueLabel1.Width - unitLabel1.Width) / 2.0;
            valueLabel2.Left = (int)startX;
            unitLabel2.Left = valueLabel2.Left + valueLabel2.Width;

            coefAbsolute = newCoefAbsolute;

            nowValue = -1;
            DrawValue(0);
        }

        public MeasureInstrumPanel(double newMinValue, double newMaxValue,
                            string newTitl, Image newImage,
                            int newFloatCount1, string unitName1,
                            int newFloatCount2, string unitName2, double newCoefAbsolute,
                            int updateTimerInterval
                          )
        {
            double startX;
            InitializeComponent();
            minValue = newMinValue;
            maxValue = newMaxValue;
            nowValue = -1;
            DrawValue(0);
            nameLabel.Text = newTitl;
            pictureBox2.Image = newImage;

            floatCount = newFloatCount1;
            if (floatCount > 3) { floatCount = 3; }

            floatCount2 = newFloatCount2;
            if (floatCount2 > 3) { floatCount2 = 3; }


            valueLabel1.Visible = true;
            unitLabel1.Visible = true;
            unitLabel1.Text = unitName1;

            valueLabel2.Visible = true;
            unitLabel2.Visible = true;
            unitLabel2.Text = unitName2;

            startX = (150 - valueLabel1.Width - unitLabel1.Width) / 2.0;
            valueLabel1.Left = (int)startX;
            unitLabel1.Left = valueLabel1.Left + valueLabel1.Width;

            startX = 150 + (150 - valueLabel1.Width - unitLabel1.Width) / 2.0;
            valueLabel2.Left = (int)startX;
            unitLabel2.Left = valueLabel2.Left + valueLabel2.Width;

            coefAbsolute = newCoefAbsolute;

            nowValue = -1;
            timer1.Interval = updateTimerInterval;
            DrawValue(0);
        }

        public void UpdateCoefAbsolute(double newCoefAbsolute)
        {
            coefAbsolute = newCoefAbsolute;
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(valuePen, zeroPoint, valuePoint);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (updateTextCounter > 0) { updateTextCounter--; }
        }




    }
}
