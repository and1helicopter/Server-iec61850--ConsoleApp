using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ServerLib.DownloadScope;
using ServerLib.ModBus;
using ServerLib.Settings;

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
			PortTextBox.Text = ServerLib.Server.ServerIEC61850.ServerConfig.ServerPort.ToString();
			HostTextBox.Text = ServerLib.Server.ServerIEC61850.ServerConfig.LocalIpAddr;
			AutostartCheckBox.IsChecked = ServerLib.Server.ServerIEC61850.ServerConfig.Autostart;
			PathTextBox.Text = ServerLib.Server.ServerIEC61850.ServerConfig.NameConfigFile;

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

			OldFormatLabel.Content = ServerLib.Server.ServerIEC61850.ServerConfig.AdditionalParams ? @"OLD" : @"NEW";
		}

		private void Ok_Button_Click(object sender, RoutedEventArgs e)
		{
			if (MainWindow.CheckedStart)
			{
				if (MessageBox.Show(@"Перезапустить сервер с новыми параметрами?", @"Сервер запущен", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					App._mainWindow.Stop_Button_Click(null, null);

					SetSettings();
					Settings.SaveSettings();

					App._mainWindow.Start_Button_Click(null, null);
				}
			}
			else
			{
				SetSettings();
				Settings.SaveSettings();
			}
		}

		private void SetSettings()
		{
			ServerLib.Server.ServerIEC61850.ServerConfig.ServerPort = Convert.ToInt32(PortTextBox.Text);
			ServerLib.Server.ServerIEC61850.ServerConfig.LocalIpAddr = HostTextBox.Text;
			ServerLib.Server.ServerIEC61850.ServerConfig.Autostart = Convert.ToBoolean(AutostartCheckBox.IsChecked.ToString());
			ServerLib.Server.ServerIEC61850.ServerConfig.NameConfigFile = Convert.ToString(PathTextBox.Text);

			ModBus.InitConfigModBus(
				BaudRateComboBox.Text, 
				SerialPortParityComboBox.Text, 
				SerialPortStopBitsComboBox.Text,
				ComPortNameComboBox.Text, 
				ConfigModBus.AddrPort);

			DownloadScope.InitConfigDownloadScope(
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

		private void Button_Click(object sender, RoutedEventArgs e)
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
