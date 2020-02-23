
using Community.Other;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Core 
{
   
    public class ServerInfoConnect 
    {
        public string title { get; set; } 
        public SpawnCurrentPlayer currentPlayer { get; set; }
        public PlayerData[] players { get; set; }

 
        public ServerInfoConnect()
        {

        }
    }
    [System.Serializable]
    public struct PlayerData : INetSerializable
    {
        public string username;
        public ushort Id;
        public ulong steamid;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(steamid);
            writer.Put(username.ToString());
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetUShort();
            steamid = reader.GetULong();
            username = reader.GetString();
        }
    }
    public struct RemotePlayersData : INetSerializable
    { 
        public ushort count;
        public PlayerData[] players; 
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(count);
            for (int i = 0; i < players.Length; i++)
            {
                players[i].Serialize(writer);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            count = reader.GetUShort();
            for (int i = 0; i < count; i++)
            {
                players[i].Deserialize(reader);
            }
        }
    }
    

}
