using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Core.Serializables
{ 
    public class TransformsPacket  
    {
        public TransformPacket[] packets { get; set; }

    }
    public class ServerBaseState
    {
        public ushort ServerTime { get; set; }
        public ushort CountPlayers { get; set; }

        public ServerBaseState(ushort time,ushort count)
        {
            ServerTime = time;
            CountPlayers = count;
        }
        public ServerBaseState()
        {

        }
    }

    public struct TransformPacket  : INetSerializable 
    {
        public ushort Id;
        public Vector3 position;
        public Vector3 velocity;
        public float rotation;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id); 
            writer.Put(position);
            writer.Put(velocity); 
            writer.Put(rotation);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetUShort(); 
            position = reader.GetVector3();
            velocity = reader.GetVector3(); 
            rotation = reader.GetFloat();
        }
    }
    public class PacketFixedPosition
    {
        public Vector3 vector3 { get; set; }
        public ushort ServerTick { get; set; }
    }
    public class PlayerInputPacket 
    {
        public ushort Id { get; set; } 
        public bool[] Keys { get; set; }
        public float Rotation { get; set; }
        public ushort ServerTick { get; set; }

        public float currentSpeed { get; set; }
    }
  
}
