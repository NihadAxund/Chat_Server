using Chat_Server.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chat_Server.View
{
    /// <summary>
    /// Interaction logic for Chat_Client.xaml
    /// </summary>
    public partial class Chat_Client : Window
    {
        public ChatViewModel cvm;
        public Chat_Client(ClientConnection clinent)
        {
            InitializeComponent();
            var data = new ChatViewModel(this, clinent);
            DataContext = new ChatViewModel(this, clinent);
        }
    }
}
