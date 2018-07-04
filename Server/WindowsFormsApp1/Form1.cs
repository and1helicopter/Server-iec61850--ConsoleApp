using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		private ServiceTest service = new ServiceTest();
		public Form1()
		{
			InitializeComponent();
		}

		private void button_Click_Start(object sender, System.EventArgs e)
		{
			service.TestStart(null);
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			service.TestStop();
		}
	}

	class ServiceTest : IEC61850.IEC61850
	{
		public void TestStart(string[] args)
		{
			OnStart(args);
		}

		public void TestStop()
		{
			OnStop();
		}

		public void TestPause()
		{
			OnPause();
		}

		public void TestContinue()
		{
			OnContinue();
		}
	}
}
