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
    public partial class MainMenuPanelPenza : UserControl, IMeasurePanel, ISysParams
    {
        int nowSelected = -1;
        UserControl subUserControl;
        public int StandartWidth { set { } get { return (tableLayoutPanel1.Width); } }
        public int StandartHeight { set { } get { return (tableLayoutPanel1.Height+label1.Height); } }

        public MainMenuPanelPenza()
        {
            InitializeComponent();
        }

        public void MainMenuDisable()
        {
            this.Enabled = false;
            panel1.Controls.Clear();
            if (subUserControl != null) { subUserControl.Dispose(); }
            nowSelected = -1;
        }

        private void MainMenuPanelPenza_Load(object sender, EventArgs e)
        {

        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            object o;

            try
            {
                o = treeView1.SelectedNode.Tag;
            }
            catch { return; }
            try
            {
                if (!(int.TryParse(o.ToString(), out nowSelected))) { return; }
            }
            catch { return; }
            
            panel1.Controls.Clear();
            if (subUserControl != null) { subUserControl.Dispose(); }
            switch (nowSelected)
            {
                case 0:
                {
                    subUserControl = new StandartSTSUnitParamControl();
                    subUserControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(subUserControl);
                }break;

                case 1:
                {
                    subUserControl = new StandartSTSRegUControl();
                    subUserControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(subUserControl);
                } break;

                case 2:
                    {
                        subUserControl = new StandartSTSRegControl(0);
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 3:
                    {
                        subUserControl = new StandartSTSRegControl(1);
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 4:
                    {
                        subUserControl = new StandartSTSSlowRegControl(0);
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 5:
                    {
                        subUserControl = new StandartSTSSlowRegControl(1);
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 6:
                    {
                        subUserControl = new StandartSTSStatismControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 7:
                    {
                        subUserControl = new StandartSystemStabControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 8:
                    {
                        subUserControl = new StandartSTSSelectAVRControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 9:
                    {
                        subUserControl = new StandartSTSRegControl(2);
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 10:
                    {
                        subUserControl = new StandartSTSOverheatControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;
                case 11:
                    {
                        subUserControl = new StandartSTSUELControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 12:
                case 13:
                case 14:
                case 15:
                    {
                        subUserControl = new StandartSTSRefLimitControl(nowSelected-12);
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);

                    }break;

                case 16:
                    {
                        subUserControl = new StandartSTSRelayForceControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);

                    } break;

                case 17:
                    {
                        subUserControl = new StandartSTSVHZControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);

                    } break;

                case 18:
                    {
                        subUserControl = new StandartStartExcitControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 19:
                    {
                        subUserControl = new StandartInvCurrControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 20:
                    {
                        subUserControl = new StandartSTSISOControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 21:
                    {
                        subUserControl = new StandartSTSRefMANControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 22:
                    {
                        subUserControl = new StandartSTSOverVControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;
                
                case 23:
                    {
                        subUserControl = new StandartSTSOverCurrVDControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 24:
                    {
                        subUserControl = new StandartSTSTransAlarmControl(0);
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;             
                case 25:
                    {
                        subUserControl = new StandartSTSOverVTEControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 26:
                    {
                        subUserControl = new StandartSTSLEControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 27:
                    {
                        subUserControl = new StandartSTSOverCurr1Control();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 28:
                    {
                        subUserControl = new StandartSTSAsynchProtectControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 29:
                    {
                        subUserControl = new StandartSTSTransAlarmControl(1);
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;

                case 30:
                    {
                        subUserControl = new StandartSTSLowFreqProtectControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;
            
                case 31:
                    {
                        subUserControl = new StandartSTSRotorProtectControl();
                        subUserControl.Dock = DockStyle.Fill;
                        panel1.Controls.Add(subUserControl);
                    } break;        
            
            }
            


            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }


        //Функции от интерфейса
        public int NeedWidth { get {return StandartWidth;} set {}}
        public int NeedHeight { get { return StandartHeight; } set { } }
        public bool ResizePossible { get { return true; } set { } }

        public void InitModBusUnits()
        {
        }

        public int GetPassword 
        {
            get { return (SystemParamsClass.SystemParams[SystemParamsFormat.StandartSTSPasswordAddr]); } 
            set { } 
        }
    }
}
