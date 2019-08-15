using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Core.Serializables
{
    public class ChatPacket
    {
        public ushort Id { get; set; }
        public string text { get; set; }
    }
}
