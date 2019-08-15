using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using UnityEngine;

namespace Core.Serializables
{
    
        public struct PlayerData 
    {
        public Vector3 position;
        public Quaternion rotation;
        //  public ushort id;
        public ulong steamid;

        
        public PlayerData Deserialize(NetDataReader reader)
        {
            position = reader.GetVector3();
            rotation = reader.GetQuaternion(); 
     //       id = reader.GetUShort();
            steamid = reader.GetULong();
            return this;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(position);
            writer.Put(rotation); 
        //    writer.Put(id);
            writer.Put(steamid);
        }
    }
}
