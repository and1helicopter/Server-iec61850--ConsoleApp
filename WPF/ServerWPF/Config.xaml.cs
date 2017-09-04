using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Controls;
using Microsoft.Win32;
using Server.ModBus;
using Server.Settings;

namespace ServerWPF
{
	/// <summary>
	/// Interaction logic for Setting.xaml
	/// </summary>
	public partial class Config 
	{
		public Config()
		{
			InitializeComponent();
		}

		public void InitConfig()
		{
			PortTextBox.Text = Server.Server.Server.ServerConfig.PortServer.ToString();
			HostTextBox.Text = Server.Server.Server.ServerConfig.LocalIPAddr;
			AutostartCheckBox.IsChecked = Server.Server.Server.ServerConfig.Autostart;
			PathTextBox.Text = Server.Server.Server.ServerConfig.NameConfigFile;

			foreach (var item in BaudRateComboBox.Items)
			{
				if (((ComboBoxItem)item).Content.ToString() == ConfigModBus.BaudRate.ToString())
				{
					BaudRateComboBox.SelectedIndex = BaudRateComboBox.Items.IndexOf(item);
					break;
				}
			}

			foreach (var item in SerialPortParityComboBox.Items)
			{
				if (((ComboBoxItem)item).Content.ToString() == ConfigModBus.SerialPortParity.ToString())
				{
					SerialPortParityComboBox.SelectedIndex = SerialPortParityComboBox.Items.IndexOf(item);
					break;
				}
			}           

			foreach (var item in SerialPortStopBitsComboBox.Items)
			{
				if (((ComboBoxItem)item).Content.ToString() == ConfigModBus.SerialPortStopBits.ToString())
				{
					SerialPortStopBitsComboBox.SelectedIndex = SerialPortStopBitsComboBox.Items.IndexOf(item);
					break;
				}
			}

			List <string> portList =new List <string>();
			string[] portStrList = SerialPort.GetPortNames();
			portList.Clear();
			foreach (string port in portStrList)
			{
				portList.Add(port);
			}
			portList.Sort();

			foreach (var port in portList)
			{
				ComPortNameComboBox.Items.Add(new ComboBoxItem{Content = port});
			}

			foreach (var item in ComPortNameComboBox.Items)
			{
				if (((ComboBoxItem)item).Content.ToString() == ConfigModBus.ComPortName)
				{
					ComPortNameComboBox.SelectedIndex = ComPortNameComboBox.Items.IndexOf(item);
					break;
				}
			}

			DownloadScopeCheckBox.IsChecked = ConfigDownloadScope.Enable;
			RemoveAfterDownloadCheckBox.IsChecked = ConfigDownloadScope.Remove;

			foreach (var item in TypeScopeComboBox.Items)
			{
				if (((ComboBoxItem)item).Content.ToString() == ConfigDownloadScope.Type)
				{
					TypeScopeComboBox.SelectedIndex = TypeScopeComboBox.Items.IndexOf(item);
					break;
				}
			}

			foreach (var item in ComtradeTypeComboBox.Items)
			{
				if (((ComboBoxItem)item).Content.ToString() == ConfigDownloadScope.ComtradeType)
				{
					ComtradeTypeComboBox.SelectedIndex = ComtradeTypeComboBox.Items.IndexOf(item);
					break;
				}
			}

			ConfigurationAddrTextBox.Text = "0x" + ConfigDownloadScope.ConfigurationAddr.ToString("X4");
			OscilCmndAddrTextBox.Text = "0x" + ConfigDownloadScope.OscilCmndAddr.ToString("X4");
			PathScopeTextBox.Text = ConfigDownloadScope.PathScope?.Replace("\\", "");
			OscilNominalFrequencyTextBox.Text = ConfigDownloadScope.OscilNominalFrequency;

			OldFormatLabel.Content = Server.Server.Server.ServerConfig.OldFormat ? @"OLD" : @"NEW";
		}

		private void Ok_Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!MainWindow.CheckedStart)
			{
				SetSettings();
				Settings.SaveSettings();
			}
		}

		private void SetSettings()
		{
			Server.Server.Server.ServerConfig.PortServer = Convert.ToInt32(PortTextBox.Text);
			Server.Server.Server.ServerConfig.LocalIPAddr = HostTextBox.Text;
			Server.Server.Server.ServerConfig.Autostart = Convert.ToBoolean(AutostartCheckBox.IsChecked.ToString());
			Server.Server.Server.ServerConfig.NameConfigFile = Convert.ToString(PathTextBox.Text);

			ConfigModBus.InitConfigModBus(
				Convert.ToInt32(BaudRateComboBox.Text), 
				SerialPortParityComboBox.Text, 
				SerialPortStopBitsComboBox.Text,
				ComPortNameComboBox.Text, 
				ConfigModBus.TimeUpdate);

			ConfigDownloadScope.InitConfigDownloadScope(
				Convert.ToString(DownloadScopeCheckBox.IsChecked),
				Convert.ToString(RemoveAfterDownloadCheckBox.IsChecked),
				TypeScopeComboBox.Text,
				ComtradeTypeComboBox.Text,
				Convert.ToString(int.Parse(ConfigurationAddrTextBox.Text.Remove(0,2), System.Globalization.NumberStyles.HexNumber)),
				Convert.ToString(int.Parse(OscilCmndAddrTextBox.Text.Remove(0, 2), System.Globalization.NumberStyles.HexNumber)),
				PathScopeTextBox.Text + "\\",
				Convert.ToString(OscilNominalFrequencyTextBox.Text)
				);
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog
			{
				DefaultExt = ".icd",
				Filter = "ICD|*.icd"
			};

			if (ofd.ShowDialog() == true)
			{
				PathTextBox.Text =  ofd.SafeFileName;
			}
		}
	}
}
