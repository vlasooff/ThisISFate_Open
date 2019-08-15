using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Core.Serializables
{
    public class NetDataEventWriter : NetDataWriter
    { 
        public NetDataEventWriter(ushort idEvent)
        {
            Put((byte)OperationMessage.Event);
            Put(idEvent);
        }

    }
    public class NetDataEventReader : NetDataReader
    {
        public NetPeer peer;
        public NetDataReader reader;
        public ushort id;

        public NetDataEventReader(NetPeer connect, NetDataReader msg,ushort idEvent)
        {
            peer = connect;
            reader = msg;
            id = idEvent;
        }

    }
}
