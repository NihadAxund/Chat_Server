using Chat_Server.Command;
using Chat_Server.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static System.Net.Mime.MediaTypeNames;

namespace Chat_Server.ViewModel
{
   public class ClientConnection
   {
       public Socket socket { get; set; } = null;
       public List<string> str = new List<string>();
        public ClientConnection() { }
       public ClientConnection(Socket socket, string msg)
       {
            this.socket = socket;
            str.Add(msg);
       }
       public ClientConnection(Socket socket)
       {
            this.socket = socket;

       }
    }
    public class MainViewModel : BaseViewModel
    {
        private MainWindow _MW { get; set; }
        static int index = -1;
        Chat_Client CC { get; set; } 
        public RelayCommand Start_Stop_Btn { get; set; }
        public RelayCommand Exit_Btn { get; set; }
        public List<ClientConnection> ClientConnections = new List<ClientConnection>();
  //      List<TcpClient> Clients {get; set; }    
        private bool Isokay { get; set; } = true;
        Task t1 { get; set; }
        public MainViewModel(MainWindow MW) {
            Start_Stop_Btn = new RelayCommand(CanClick, CanTrue);
            Exit_Btn = new RelayCommand(CanClose, AlwaysTrue);
            _MW = MW;
            _MW.Client_Server_list.SelectionChanged += Client_Server_list_Selected;
        }
        private bool AlwaysTrue(object parametr) => true;
        private bool CanTrue(object parametr)
        {
            return Isokay;
        }
        private void Client_Server_list_Selected(object sender, RoutedEventArgs e)
        {
            if (_MW.Client_Server_list.SelectedItem is Client_uc uc)
            {
                index = _MW.Client_Server_list.SelectedIndex;
                 CC = new Chat_Client(uc.TCC);
                //  uc.TCC.Dispose();
                _MW.Client_Server_list.SelectedIndex = -1;
                //     t1.Dispose();
                // CC.ShowDialog();
                MessageBox.Show(index.ToString()+"AAA");
                if (!CC.ShowDialog().Value)
                {
                    index = -1;
                }
                //MessageBox.Show(CC.ShowDialog().ToString());
            }
        }
        private void  DispacherMethod(ClientConnection tc)
        {
            _MW.Dispatcher.BeginInvoke(new Action(() =>
            {
                var UC = new Client_uc(tc);
                _MW.Client_Server_list.Items.Add(UC);
            }));
        }
        private void AgainSend(Socket socke,string msg)
        {
            var bytes = Encoding.ASCII.GetBytes(msg);
            socke.Send(bytes);
        }
        private  void ConnectionMethod()
        {
            var idAddres = IPAddress.Loopback;
            var port = 27009;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var EP = new IPEndPoint(idAddres, port);
                socket.Bind(EP);
                socket.Listen(10);
                while (true)
                {
                   // var client1 = socket.Accept();
                    var ClientCon = new ClientConnection(socket.Accept());
                    //       MessageBox.Show($"Listen ever {client.LocalEndPoint}");
                    Task.Run(() =>
                    {
                 //        MessageBox.Show($"{ClientCon.socket.RemoteEndPoint} connected");
                        if (index == -1)
                        {
                            ClientConnections.Add(ClientCon);
                            DispacherMethod(ClientCon);
                            var length = 0;
                            var bytes = new byte[1024];
                            do
                            {
                                lock (ClientCon)
                                {

                                    try
                                    {
                                        if (index != -1 && ClientCon.socket.RemoteEndPoint == ClientConnections[index].socket.RemoteEndPoint)
                                        {
                                            continue;
                                        }
                                        length = ClientCon.socket.Receive(bytes);
                                        var msg = Encoding.UTF8.GetString(bytes, 0, length);
                                        if (index==-1 &&_MW.Client_Server_list.Items[ClientConnections.IndexOf(ClientCon)] is Client_uc uc)
                                        {
                                            uc.Badge_Count_inc();
                                          //  MessageBox.Show(msg);
                                        }
                                        else
                                        {
                                            //       AgainSend(ClientConnections[index].socket, " ");
                                            continue;
                                        }
                                        ClientCon.str.Add(msg);
                                        //MessageBox.Show($"Client: {client.RemoteEndPoint}: {msg}");

                                    }
                                    catch (Exception)
                                    {
                                       // MessageBox.Show(client.RemoteEndPoint.ToString());
                                        int index = ClientConnections.IndexOf(ClientCon);
                                        ClientCon.socket.Shutdown(SocketShutdown.Both);
                                        ClientCon.socket.Dispose();
                                        ClientConnections.RemoveAt(index);
                                        _MW.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            _MW.Client_Server_list.Items.RemoveAt(index);
                                        }));
                                        break;
                                    }

                                }
                                //if (msg == "Exit")
                                //{
                                //    client.Shutdown(SocketShutdown.Both);
                                //    client.Dispose();
                                //    break;
                                //}
                            } while (true);

                        }
                    });
                }
                //   socket.Close();
            }
        

        ////////////////////////////////////////
        //    Clients = new List<TcpClient>();
        //    TcpListener listener = null;
        //    BinaryReader br = null;
        //    BinaryWriter bw = null;
        //    var ip = IPAddress.Loopback;

        //    //  var ip = IPAddress.Loopback;
        //    var port = 27001;

        //    var ep = new IPEndPoint(ip, port);
        //    listener = new TcpListener(ep);
        //    listener.Start();
        //    MessageBox.Show($"Listening on {listener.LocalEndpoint}");
        //    while (true)
        //    {
        //        var client = listener.AcceptTcpClient();
        //        Clients.Add(client);
        //        await DispacherMethod(client);
        //        var reader = Task.Run(() =>
        //        {
        //            foreach (var item in Clients)
        //            {
        //                int index = Clients.IndexOf(item);
        //                //if (_MW.Client_Server_list.SelectedItem is Client_uc uc && uc.TCC == item)
        //                //{
        //                //    MessageBox.Show(_MW.Client_Server_list.SelectedIndex.ToString());
        //                //    continue;
        //                //}

        //                Task.Run(() =>
        //                {
        //                    //    var stream = item.GetStream();
        //                    //  MessageBox.Show("Send " + item.Client.RemoteEndPoint + " " + Clients.Count);
        //                    // int i = Clients.IndexOf(item);
        //                    // var br = new BinaryReader(stream);
        //                    while (true)
        //                    {
        //                        if (_MW.Client_Server_list.SelectedItem is Client_uc uc)
        //                        {
        //                            MessageBox.Show("a5");
        //                            break;
        //                        }
        //                        try { item.GetStream().ReadByte(); }
        //                        catch
        //                        {
        //                            //   MessageBox.Show("a");
        //                            //    int index = Clients.IndexOf(item);
        //                            //MessageBox.Show(ex.Message);
        //                            Clients.RemoveAt(index);
        //                            _MW.Dispatcher.BeginInvoke(new Action(() =>
        //                            {
        //                                //  var UC = new Client_uc(client);
        //                                _MW.Client_Server_list.Items.RemoveAt(index);

        //                            }));

        //                            //       MessageBox.Show($"{item.Client.RemoteEndPoint}  disconnected");

        //                        }
        //                    }
        //                }).Wait(100);

        //            }

        //        });
        //}   }
        /////
        //private  void ConnectionMethod()
        //{
        //    Clients = new List<TcpClient>();
        //    TcpListener listener = null;
        ////    BinaryWriter bw = null;
        // //   BinaryReader br = null;


        //    var ip = IPAddress.Loopback;
        //    var port = 27001;

        //    var ep = new IPEndPoint(ip, port);
        //    listener = new TcpListener(ep);

        //    listener.Start();
        //    //         MessageBox.Show($"Listening on {listener.LocalEndpoint}");
        //    object obj = null;
        //    while (true)
        //    {
        //        var client = listener.AcceptTcpClient();
        //        Clients.Add(client);
        //        _MW.Dispatcher.BeginInvoke(new Action(() =>
        //        {
        //            var UC = new Client_uc(client);
        //            _MW.Client_Server_list.Items.Add(UC);
        //        }));
        //   //     MessageBox.Show($"{client.Client.RemoteEndPoint}");
        //         Task.Run(() =>
        //        {
        //            var reader = Task.Run(() =>
        //            {

        //                foreach (var item in Clients)

        //               {
        //                    int index = Clients.IndexOf(item);
        //                    //if (_MW.Client_Server_list.SelectedItem is Client_uc uc && uc.TCC == item)
        //                    //{
        //                    //    MessageBox.Show(_MW.Client_Server_list.SelectedIndex.ToString());
        //                    //    continue;
        //                    //}

        //                    Task.Run(() =>
        //                    {
        //                    //    var stream = item.GetStream();
        //                        //  MessageBox.Show("Send " + item.Client.RemoteEndPoint + " " + Clients.Count);
        //                       // int i = Clients.IndexOf(item);
        //                       // var br = new BinaryReader(stream);
        //                        while (true)
        //                        {
        //                            if (_MW.Client_Server_list.SelectedItem is Client_uc uc)
        //                            {
        //                                MessageBox.Show("a");
        //                                break;
        //                            }
        //                            try {item.GetStream().ReadByte();}
        //                            catch 
        //                            {
        //                             //   MessageBox.Show("a");
        //                           //    int index = Clients.IndexOf(item);
        //                              //MessageBox.Show(ex.Message);
        //                                Clients.RemoveAt(index);
        //                                _MW.Dispatcher.BeginInvoke(new Action(() =>
        //                                {
        //                                  //  var UC = new Client_uc(client);
        //                                    _MW.Client_Server_list.Items.RemoveAt(index);

        //                                }));

        //                         //       MessageBox.Show($"{item.Client.RemoteEndPoint}  disconnected");

        //                            }
        //                        }
        //                    }).Wait(100);
        //               }
        //            });
        //        });
        //    }
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
