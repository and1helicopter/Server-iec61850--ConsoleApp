
using System.Linq;
using System.Windows.Controls;
using Server.ModBus;

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



            var sad = BaudRateComboBox.SelectionBoxItem;

            //BaudRateComboBox.SelectedItem = from x in BaudRateComboBox.Items
            //                                where x.Content == ConfigModBus.BaudRate.ToString()
            //                                select x;

            //BaudRateComboBox.SelectionBoxItem
        }
    }
}
