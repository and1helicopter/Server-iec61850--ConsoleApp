using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestService
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ServiceTest service = new ServiceTest();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			service.TestStart(null);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			service.TestStop();
		}
	}

	class ServiceTest : ServiceIec61850.Service
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
