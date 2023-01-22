using Chat_Server.Command;
using Chat_Server.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Label = System.Windows.Controls.Label;

namespace Chat_Server.ViewModel
{
    public class ChatViewModel : BaseViewModel
    {
        private ClientConnection _TCP_CLient { get; set; }
        private Chat_Client _CC { get; set; }
        public RelayCommand Send_Btn { get; set; }
        private void MessageList(List<string> str)
        {
            _CC.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var item in str)
                {
                    _CC.list_box.Items.Add(item);
                }
            }));
        }
        public ChatViewModel(Chat_Client cc, ClientConnection tcpClient) {
            
            _CC= cc;
            _TCP_CLient= tcpClient;
            Send_Btn = new RelayCommand(CanSend, AllwaysTrue);
            Action ac = new Action(() =>
            {
                if (tcpClient.str.Count!=0)
                {
                    MessageList(tcpClient.str);
                }
                Connection(tcpClient.socket);
            });
            Task t1 = new Task(ac);
            t1.Start();
       
        }
        
        public void SendUI(string msg)
        {
         //   _CC.Dispatcher.BeginInvoke(new Action(() =>
         //   {
                _CC.Dispatcher.Invoke(new Action(() =>
                {
                     Label lbl= new Label();
                    lbl.Foreground = new SolidColorBrush(Colors.BlueViolet);
                     lbl.Content= msg; lbl.HorizontalAlignment = HorizontalAlignment.Right;
                    _TCP_CLient.str.Add(msg);
                    _CC.list_box.Items.Add(lbl);
                    _CC.TXT_BOX.Text = "";
                }));

         //   }));
        }

        private bool AllwaysTrue(object parametr)
        {
            return _CC.TXT_BOX.Text.Length > 0;
        }
        private void CanSend (object parametr)
        {
           // MessageBox.Show("a");
            if (_TCP_CLient.socket.Connected)
            {
               
                var text = _CC.TXT_BOX.Text;
                try
                {
                    var writer = Task.Run(() =>
                    {
                          var bytes = Encoding.ASCII.GetBytes(text);
                        _TCP_CLient.socket.Send(bytes);

                        MessageBox.Show(text);
                        SendUI(text);
                    //    MessageBox.Show(text);


                    }).Wait(50);
                    //Task.WaitAll(writer);
                }
                catch (Exception)
                {

                    _TCP_CLient.socket.Shutdown(SocketShutdown.Both);
                    _TCP_CLient.socket.Close();
                    _CC.Close();
                }
            }
        }
        private void Connection(Socket tcp)
        {

            //var ip = IPAddress.Loopback;
            //var port = 27001;
            //var ep = new IPEndPoint(ip, port);


            try
            {
               // tcp.Connect(tcp.RemoteEndPoint);
            //    _TCP_CLient.Connect(ep);
               // if (tcp.Connect())
             //   {
                    //var writer = Task.Run(() =>
                    //{
                    //    while (true)
                    //    {
                    //        var text = Console.ReadLine();
                    //        var stream = _TCP_CLient.GetStream();
                    //        var bw = new BinaryWriter(stream);
                    //        bw.Write(text);
                    //    }
                    //});
                    //MessageBox.Show("a");
                    var reader = Task.Run(() =>
                    {
                            var length = 0;
                       //    tcp.Accept();
                        while (true)
                        {
                            try
                            {
                                var bytes = new byte[1024];
                                length = tcp.Receive(bytes);
                                var msg = Encoding.UTF8.GetString(bytes, 0, length);
                                _TCP_CLient.str.Add(msg);
                                _CC.Dispatcher.Invoke(new Action(() =>
                                {
                                    _CC.list_box.Items.Add(msg);
                                }));

                            }
                            catch (Exception es)
                            {

                              //  MessageBox.Show(es.Message);
                                //_TCP_CLient.socket.Shutdown(SocketShutdown.Both);
                               // _TCP_CLient.socket.Close();
                            }
           
                          //  Console.WriteLine($"From Server : {br.ReadString()}");
                        }
                    });

                    //Task.WaitAll(reader);
             //   }
            }
            catch (Exception)
            {
               
            }

        }


    }
}
