using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chat_Server.Network_Helper
{
    public class Network
    {
        private List<Socket> Sockets = new List<Socket>();
        public void Connection()
        {
            var idAddres = IPAddress.Loopback;
            var port = 27001;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var EP = new IPEndPoint(idAddres, port);
                socket.Bind(EP);
                socket.Listen(10);
                while (true)
                {
                    var client = socket.Accept();
           //       MessageBox.Show($"Listen ever {client.LocalEndPoint}");
                    Task.Run(() =>
                    {
                        MessageBox.Show($"{client.RemoteEndPoint} connected");
                         Sockets.Add(client);
                        var length = 0;
                        var bytes = new byte[1024];
                        do
                        {
                            try
                            {
                                length = client.Receive(bytes);
                                var msg = Encoding.UTF8.GetString(bytes, 0, length);
                                MessageBox.Show($"Client: {client.RemoteEndPoint}: {msg}");

                            }
                            catch (Exception)
                            {
                                MessageBox.Show(client.RemoteEndPoint.ToString());
                                Sockets.Remove(client);
                                client.Shutdown(SocketShutdown.Both);
                                client.Dispose();
                                break;
                            }
                            //if (msg == "Exit")
                            //{
                            //    client.Shutdown(SocketShutdown.Both);
                            //    client.Dispose();
                            //    break;
                            //}
                        } while (true);
                    });
                }
                //   socket.Close();
            }
        }
        public Network()
        {

        }
















        //////////////////////////
        //public static string IP = "192.168.100.98";
        //public static int PORT = 27001;
        //static TcpListener listener = null;
        //static BinaryWriter bw = null;
        //static BinaryReader br = null;
        //public static List<TcpClient> Clients { get; set; }

        //public static void Connect()
        //{
        //    var ip = IPAddress.Parse(IP);
        //    Clients = new List<TcpClient>();

        //    var ep = new IPEndPoint(ip, PORT);
        //    listener = new TcpListener(ep);
        //    listener.Start();
        //    MessageBox.Show($"Listening on {listener.LocalEndpoint}");
        //    while (true)
        //    {
        //        var client = listener.AcceptTcpClient();
        //        Clients.Add(client);

        //        Task.Run(() =>
        //        {

        //            var reader = Task.Run(() =>
        //            {
        //                foreach (var item in Clients)
        //                {
        //                    Task.Run(() =>
        //                    {
        //                        var stream = item.GetStream();
        //                        br = new BinaryReader(stream);
        //                        while (true)
        //                        {
        //                            try
        //                            {
        //                                var msg = br.ReadString();
        //                                //MessageBox.Show($"CLIENT : {client.Client.RemoteEndPoint} : {msg}");
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                Console.WriteLine($"{item.Client.RemoteEndPoint}  disconnected");
        //                                Clients.Remove(item);
        //                            }
        //                        }
        //                    }).Wait(50);
        //                }



        //            });

        //        });
        //    }
        //}
    }
}
