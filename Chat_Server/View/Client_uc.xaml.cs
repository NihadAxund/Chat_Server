using Chat_Server.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat_Server.View
{
    /// <summary>
    /// Interaction logic for Client_uc.xaml
    /// </summary>
    public partial class Client_uc : UserControl
    {
        public ClientConnection TCC {get; set; }
        public int Badge_Count = 0;


        public void Badge_Count_inc()
        {
            
            if (Badge_Count >= 30) { return; }
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Badgesi.Visibility != Visibility.Visible)
                    Badgesi.Visibility = Visibility.Visible;
                ++Badge_Count;
                Badgesi.Badge = Badge_Count.ToString();
            }));
        }
        public Client_uc(ClientConnection TC)
        {
            InitializeComponent();
            Badgesi.Badge = Badge_Count.ToString();
            TCC = TC;
            Client_Name_txt.Text = TC.socket.RemoteEndPoint.ToString();
        }

 
    }
}
