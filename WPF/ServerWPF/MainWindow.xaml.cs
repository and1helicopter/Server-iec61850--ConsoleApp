using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Server.Settings;
using Server.ModBus;
using Server.Log;
using Server.Parser;

namespace ServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            ConfigStackPanel.Children.Add(_config);
            Status();
            //Открываем настройки сервера
            if (!Settings.ReadSettings())
            {
                Log.Write(@"Settings: ReadSettings finish with status false. Stop server", @"Error");
            }
        }

        private readonly Config _config = new Config();

        private bool _checkedStart;
        private bool _checkedStop = true;
        private bool _checkedConfig;

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
                ModBus_status.Visibility = Visibility.Hidden;
            }
            else if (_checkedStart)
            {
                Host.Visibility = Visibility.Visible;
                Host.Content = Server.Server.Server.ServerConfig.LocalIPAddr;
                Port.Visibility = Visibility.Visible;
                Port.Content = Server.Server.Server.ServerConfig.PortServer;

                if (ModBus.StartPort)
                {
                    BaudRate.Visibility = Visibility.Visible;
                    BaudRate.Content = ConfigModBus.BaudRate;
                    SerialPortParity.Visibility = Visibility.Visible;
                    SerialPortParity.Content = ConfigModBus.SerialPortParity;
                    SerialPortStopBits.Visibility = Visibility.Visible;
                    SerialPortStopBits.Content = ConfigModBus.SerialPortStopBits;
                    ComPortName.Visibility = Visibility.Visible;
                    ComPortName.Content = ConfigModBus.ComPortName;
                    ModBus_status.Visibility = Visibility.Hidden;
                }
                else
                {
                    ModBus_status.Visibility = Visibility.Visible;
                    ModBus_status.Content = "ModBus not started!";
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

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_checkedStart)
            {
                return;
            }

            _checkedStart = true;
            _checkedStop = false;


            Settings.SaveSettings();

            //Парсим файл конфигурации
            if (!Parser.ParseFile())
            {
                Log.Write(@"ParseFile: Finish with status false. Stop server", @"Error");
                return;
            }
            Log.Write(@"ParseFile: File parse success", @"Success");

            //ModBus.CloseModBus();
            //ModBus.InitConfigDownloadScope("true", "false", "comtrade", "1999", "512", "4096", "Scope\\", "50");
            //ModBus.InitConfigModBus("115200", "Odd", "One", "COM1");
            ModBus.ConfigModBusPort();
            ModBus.StartModBus();

            //Создаем модель сервера
            if (!Server.Server.Server.ConfigServer()) return;

            //Запуск сервера 
            if (!Server.Server.Server.StartServer()) return;

            Start.Background = new SolidColorBrush(Colors.LimeGreen);
            Start.BorderBrush = new SolidColorBrush(Colors.DarkGreen);
            Start.Foreground = new SolidColorBrush(Colors.WhiteSmoke);

            Stop.Background = new SolidColorBrush(Colors.White);
            Stop.BorderBrush = new SolidColorBrush(Colors.LightGray);
            Stop.Foreground = new SolidColorBrush(Colors.Black);

            Status();


        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_checkedStop)
            {
                return;
            }

            _checkedStart = false;
            _checkedStop = true;

            Server.Server.Server.StopServer();

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
            }
            else
            {
                OpenAnimation();
                _checkedConfig = true;
            }
        }
    }
}
