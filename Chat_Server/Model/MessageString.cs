using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Server.Model
{
    public class MessageString
    {
        public string Message { get; set; }
        public bool IsSent { get; set; }
        public MessageString(string Message, bool isok = false)
        {
            this.Message = Message; IsSent = isok;
        }
    }
}
