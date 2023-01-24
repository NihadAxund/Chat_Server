using Chat_Server.Command;
using Chat_Server.Model;
using Chat_Server.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static System.Net.Mime.MediaTypeNames;

namespace Chat_Server.ViewModel
{


    /// <summary>
    /// //////
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private MainWindow _MW { get; set; }
        Chat_Client CC { get; set; } 
        public RelayCommand Start_Stop_Btn { get; set; }
        public RelayCommand Exit_Btn { get; set; }
        public List<ClientConnection> ClientConnections = new List<ClientConnection>();    
        private bool Isokay { get; set; } = true;
        private Task t1 { get; set; }
        private bool IsShowDialog { get; set; } = false;
        static int index = -1;
        public MainViewModel(MainWindow MW) {
            _MW = MW;
       //      ct = src.Token;
            Start_Stop_Btn = new RelayCommand(CanClick, CanTrue);
            Exit_Btn = new RelayCommand(CanClose, AlwaysTrue);
           
            _MW.Client_Server_list.SelectionChanged += Client_Server_list_Selected;
        }
        private bool AlwaysTrue(object parametr) => true;
        private bool CanTrue(object parametr) => Isokay;
        private void Client_Server_list_Selected(object sender, RoutedEventArgs e)
        {
            if (_MW.Client_Server_list.SelectedItem is Client_uc uc)
            {
           

                index = _MW.Client_Server_list.SelectedIndex;
                 CC = new Chat_Client(uc.TCC);
                IsShowDialog = true;
                _MW.Client_Server_list.SelectedIndex = -1;

                if (!CC.ShowDialog().Value)
                {
                    index = -1;
                    IsShowDialog = false;
                    CC.cvm.CloseTas();
                    uc.Badgesi.Visibility= Visibility.Hidden;
                    uc.Badge_Count = 0;
                }
            }
        }
        private void  DispatcherMethod(ClientConnection tc)
        {
            _MW.Dispatcher.BeginInvoke(new Action(() =>
            {
                var UC = new Client_uc(tc);
                _MW.Client_Server_list.Items.Add(UC);
            }));
        }
        private  void ConnectionMethod()
        {
            var idAddres = IPAddress.Any;
            var port = 27009;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var EP = new IPEndPoint(idAddres, port);
                socket.Bind(EP);
                socket.Listen(10);
                while (true)
                {
                    var ClientCon = new ClientConnection(socket.Accept());
                    Task.Run(() =>
                    {
                        if (index == -1)
                        {
                            ClientConnections.Add(ClientCon);
                            DispatcherMethod(ClientCon);
                            var length = 0;
                            var bytes = new byte[1024];

                         //   lock (ClientCon)
                            {
                                do
                                {
                                   try
                                   {
                                       if (index != -1 && ClientCon.socket.RemoteEndPoint == ClientConnections[index].socket.RemoteEndPoint)
                                       {
                                           continue;
                                       }
                                       length = ClientCon.socket.Receive(bytes);
                                       var msg = Encoding.UTF8.GetString(bytes, 0, length);
                                       if (IsShowDialog)
                                       {
                                            CC.cvm.SendUI(new MessageString(msg));
                                       }
                                       else if (_MW.Client_Server_list.Items[ClientConnections.IndexOf(ClientCon)] is Client_uc uc)
                                           uc.Badge_Count_inc();      
                                     
                                       ClientCon.str.Add(new MessageString(msg));
                                   }
                                   catch (Exception)
                                   {
                                       int ind = ClientConnections.IndexOf(ClientCon);
                                       ClientCon.socket.Shutdown(SocketShutdown.Both);
                                       ClientCon.socket.Dispose();
                                       ClientConnections.RemoveAt(ind);
                                       _MW.Dispatcher.BeginInvoke(new Action(() =>
                                       {
                                           _MW.Client_Server_list.Items.RemoveAt(ind);
                                       }));
                                       break;
                                   }
                                } while (true);
                            }
                        }
                    });
                }
            }
        
        }
        private void CanClick(object parameter)
        {
            t1 = new Task(ConnectionMethod,TaskCreationOptions.LongRunning);
            Isokay= false;
            t1.Start();
        }
        private void CanClose (object parameter) { 
            _MW.Close();
        }
    }
}
