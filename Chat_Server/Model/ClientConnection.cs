using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Server.Model
{
    public class ClientConnection
    {
        public Socket socket { get; set; } = null;
        public List<MessageString> str = new List<MessageString>();
        public ClientConnection() { }
        public ClientConnection(Socket socket, MessageString msg)
        {
            this.socket = socket;
            str.Add(msg);
        }
        public ClientConnection(Socket socket)
        {
            this.socket = socket;

        }
    }
}
