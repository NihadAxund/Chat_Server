using Chat_Server.Command;
using Chat_Server.Model;
using Chat_Server.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using Label = System.Windows.Controls.Label;

namespace Chat_Server.ViewModel
{
    public class ChatViewModel : BaseViewModel
    {
        private ClientConnection _TCP_CLient { get; set; }
        private Chat_Client _CC { get; set; }
        private Task task { get; set; }
        public bool IsOKay { get; set; } = true;
        public RelayCommand Send_Btn { get; set; }
        private void MessageList(List<MessageString> str)
        {
            _CC.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var item in str)
                {
                    if (item.IsSent)
                    {
                        var cl = new Server_Label(item.Message);
                        cl.Margin = new Thickness(10, 0, 0, 0);
                        cl.HorizontalAlignment = HorizontalAlignment.Right;
                        _CC.Chat_list.Children.Add(cl);
                    }
                    else
                    {
                        var cl = new Client_Label(item.Message);
                        cl.Margin = new Thickness(10, 0, 0, 0);
                        cl.HorizontalAlignment= HorizontalAlignment.Left;
                        _CC.Chat_list.Children.Add(cl);
                    }
                }
            }));
        }
        public ChatViewModel(Chat_Client cc, ClientConnection tcpClient) {
            
            _CC= cc;
            _TCP_CLient= tcpClient;
            _CC.TXT_BOX.KeyUp += TXT_BOX_KeyUp;
            Send_Btn = new RelayCommand(CanSend, CanTrue);
            Action ac = new Action(() =>
            {
                if (_TCP_CLient.str.Count!=0)
                    MessageList(_TCP_CLient.str);
                Connection();
            });
            Task t1 = new Task(ac);
            t1.Start();
       
        }

        private void TXT_BOX_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendData();
            }
        }

        public void SendUI(MessageString msg)
        {
                _CC.Dispatcher.BeginInvoke(new Action(() =>
                {
                        _TCP_CLient.str.Add(msg);
                    if (msg.IsSent)
                    {
                        Server_Label lbl = new Server_Label(msg.Message);
                        lbl.Margin = new Thickness(0,0,15,0);
                          lbl.HorizontalAlignment= HorizontalAlignment.Right;
                        _CC.Chat_list.Children.Add(lbl);
                    }
                    else
                    {
                        Client_Label lbl = new Client_Label(msg.Message);
                        lbl.Margin = new Thickness(0, 0, 15, 0);
                        lbl.HorizontalAlignment = HorizontalAlignment.Left;
                        _CC.Chat_list.Children.Add(lbl);
                    }
                    _CC.TXT_BOX.Text = "";
                    _CC.Scrollviwer.ScrollToEnd();
                        
                }));
        }



        private bool CanTrue(object parametr)
        {
            return _CC.TXT_BOX.Text.Length > 0&& _CC.TXT_BOX.Text!=" ";
        }
        private void SendData()
        {
            if (_TCP_CLient.socket.Connected&& _CC.TXT_BOX.Text.Length > 0 && _CC.TXT_BOX.Text != " ")
            {

                var text = _CC.TXT_BOX.Text;
                try
                {
                    var writer = Task.Run(() =>
                    {


                        var bytes = Encoding.ASCII.GetBytes(text);
                        _TCP_CLient.socket.Send(bytes);

                        //   MessageBox.Show(text);
                        SendUI(new MessageString(text, true));
                        //    MessageBox.Show(text);


                    }).Wait(50);
                    //Task.WaitAll(writer);
                }
                catch (Exception)
                {

                    //_TCP_CLient.socket.Shutdown(SocketShutdown.Both);
                    //_TCP_CLient.socket.Close();
                    // _CC.Close();
                }
            }
        }
        private void CanSend (object parametr)
        {
            SendData();
        }
        private void ReadData(Socket tcp)
        {
            var length = 0;
            while (IsOKay)
            {
                try
                {
                    var bytes = new byte[1024];
                    length = tcp.Receive(bytes);
                    var msg = Encoding.UTF8.GetString(bytes, 0, length);
                    _TCP_CLient.str.Add(new MessageString(msg));
                    if (!IsOKay) { return; }
             //       MessageBox.Show(msg + "|"+IsOKay.ToString());
                    _CC.Dispatcher.Invoke(new Action(() =>
                    {
                        //MessageBox.Show();
                        Client_Label cl = new Client_Label(msg);
                        cl.Margin = new Thickness(10, 0, 0, 0);
                        cl.HorizontalAlignment = HorizontalAlignment.Left;
                        _CC.Chat_list.Children.Add(cl);
                        _CC.Scrollviwer.ScrollToEnd();
                    }));
                 //   MessageBox.Show(msg);

                }
                catch (Exception)
                {
                  //  MessageBox.Show(es.Message);
                  //  IsOKay = false;
                   
                }

            }
        }
        public void CloseTas()
        {
            IsOKay= false;
        }
        private void Connection()
        {
            try
            {
                task = new Task(() =>
                {
                    ReadData(_TCP_CLient.socket);

                });
                task.Start();
            }
            catch (Exception)
            {

            }

        }


    }
}
