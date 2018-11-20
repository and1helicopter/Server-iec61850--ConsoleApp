using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ServerLib.DownloadScope;
using ServerLib.Format;
using ServerLib.ModBus;
using ServerLib.Log;
using ServerLib.Server;

namespace ServerWPF
{
	public partial class MainWindow 
	{
		public MainWindow()
		{
			InitializeComponent();

			Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Hidden; });
			TimerLoadOsc.Elapsed -= TimerLoadOscOnElapsed;
			TimerLoadOsc.Enabled = false;

			ConfigStackPanel.Children.Add(_config);

			var pathDirectory = Directory.GetCurrentDirectory();
			//Установка пути для лог файла
			Log.SetRootPath(pathDirectory);
			
			//Открываем настройки сервера
			if (!ServerIEC61850.ReadConfig(pathDirectory + "\\Settings.xml"))
			{
				Log.Write(@"Settings: ReadSettings finish with status false. Stop server", @"Error");
			}

			FormatConverter.ReadFormats(null);

			if (ServerIEC61850.ServerConfig.Autostart)
			{
				Start_Button_Click(null,null);
			}
			Status();
		}

		private readonly Config _config = new Config();
		
		public static bool CheckedStart;
		private static bool _checkedStop = true;
		private static bool _checkedConfig;
		private static readonly Timer TimerLoadOsc = new Timer()
		{
			Interval = 500
		};

		private void TimerLoadOscOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			//if (StatusLoad(out var count))
			//{
			//	Dispatcher.Invoke(() => { ProgressBar.Value = Math.Round(count, 2);});
			//	Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
			//}
			//else
			//{
			//	Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Hidden; });
			//}
		}
		
		private void Status()
		{
			if (_checkedStop)
			{
				Host.Visibility = Visibility.Hidden;
				Port.Visibility = Visibility.Hidden;
				BaudRate.Visibility = Visibility.Hidden;
				SerialPortParity.Visibility = Visibility.Hidden;
				SerialPortStopBits.Visibility = Visibility.Hidden;
				ComPortName.Visibility = Visibility.Hidden;
				ModBusStatus.Visibility = Visibility.Hidden;
			}
			else if (CheckedStart)
			{
				Host.Visibility = Visibility.Visible;
				Host.Content = ServerIEC61850.ServerConfig.LocalIpAddr;
				Port.Visibility = Visibility.Visible;
				Port.Content = ServerIEC61850.ServerConfig.ServerPort;
				
				if (true) // ModBus.CheckModbus().IsStart
				{
					BaudRate.Visibility = Visibility.Visible;
					BaudRate.Content = ConfigModBus.BaudRate;
					SerialPortParity.Visibility = Visibility.Visible;
					SerialPortParity.Content = ConfigModBus.SerialPortParity;
					SerialPortStopBits.Visibility = Visibility.Visible;
					SerialPortStopBits.Content = ConfigModBus.SerialPortStopBits;
					ComPortName.Visibility = Visibility.Visible;
					ComPortName.Content = ConfigModBus.ComPortName;
					ModBusStatus.Visibility = Visibility.Hidden;
				}
				else
				{
					ModBusStatus.Visibility = Visibility.Visible;
					ModBusStatus.Content = "UpdateModBus not started!";
				}
			}
		}

		private void OpenAnimation()
		{
			Height = 360;
			Width = 700;

			DoubleAnimation openAnimation = new DoubleAnimation
			{
				From = 0,
				To = 500,
				Duration = new Duration(TimeSpan.FromSeconds(0.25))
			};
			ConfigPanel.BeginAnimation(ColumnDefinition.MinWidthProperty, openAnimation);
		}

		private void CloseAnimation()
		{
			DoubleAnimation closeAnimation = new DoubleAnimation
			{
				From = 500,
				To = 0,
				Duration = new Duration(TimeSpan.FromSeconds(0.25))
			};
			ConfigPanel.BeginAnimation(ColumnDefinition.MinWidthProperty, closeAnimation);

			Height = 390;
			Width = 200;
		}

		public void Start_Button_Click(object sender, RoutedEventArgs e)
		{
			if (CheckedStart)
			{
				return;
			}
			
			_checkedStop = false;
			
			//Парсим файл конфигурации
			if (!ServerIEC61850.ParseFile())
			{
				Log.Write(@"ParseFile: Finish with status false. Stop server", @"Error");
				return;
			}
			Log.Write(@"ParseFile: File parse success", @"Success");
			
			if (ConfigDownloadScope.Enable)
			{
				TimerLoadOsc.Enabled = true;
				TimerLoadOsc.Elapsed += TimerLoadOscOnElapsed;
			}

			//Создаем модель сервера
			if (!ServerIEC61850.ConfigServer()) return;

			//Запуск сервера 
			if (!ServerIEC61850.StartServer()) return;

			//if (!UpdateModBus.StartModBus())
			//{
			//	SerialPortStopBits.Content = @"COM Port";
			//	ComPortName.Content = @"Not Open";
			//	SerialPortStopBits.Visibility = Visibility.Visible;
			//	ComPortName.Visibility = Visibility.Visible;
			//	return;
			//}

			CheckedStart = true;

			Start.Background = new SolidColorBrush(Colors.LimeGreen);
			Start.BorderBrush = new SolidColorBrush(Colors.DarkGreen);
			Start.Foreground = new SolidColorBrush(Colors.WhiteSmoke);

			Stop.Background = new SolidColorBrush(Colors.White);
			Stop.BorderBrush = new SolidColorBrush(Colors.LightGray);
			Stop.Foreground = new SolidColorBrush(Colors.Black);

			Status();
		}

		public void Stop_Button_Click(object sender, RoutedEventArgs e)
		{
			if (_checkedStop)
			{
				return;
			}

			ServerIEC61850.StopServer();

			if (ConfigDownloadScope.Enable)
			{
				Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Hidden; });
				TimerLoadOsc.Elapsed -= TimerLoadOscOnElapsed;
				TimerLoadOsc.Enabled = false;
			}

			CheckedStart = false;
			_checkedStop = true;

			Stop.Background = new SolidColorBrush(Colors.Red);
			Stop.BorderBrush = new SolidColorBrush(Colors.DarkRed);
			Stop.Foreground = new SolidColorBrush(Colors.White);

			Start.Background = new SolidColorBrush(Colors.White);
			Start.BorderBrush = new SolidColorBrush(Colors.LightGray);
			Start.Foreground = new SolidColorBrush(Colors.Black);

			Status();
		}

		private void Setting_OnClick_Button_Click(object sender, RoutedEventArgs e)
		{
			_config.InitConfig();
			if (_checkedConfig)
			{
				CloseAnimation();
				_checkedConfig = false;
				Setting.Background = new SolidColorBrush(Colors.White);
				Setting.BorderBrush = new SolidColorBrush(Colors.LightGray);
				Setting.Foreground = new SolidColorBrush(Colors.Black);
			}
			else
			{
				OpenAnimation();
				_checkedConfig = true;
				Setting.Background = new SolidColorBrush(Colors.LimeGreen);
				Setting.BorderBrush = new SolidColorBrush(Colors.DarkGreen);
				Setting.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			ServerIEC61850.StopServer();
		}
	}
}
