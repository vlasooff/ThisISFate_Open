using Community.Core.Serializables;
using Community.Other;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Server.Components
{ 
    public class PlayersInfoComponent : MonoBehaviour
    {
        public delegate void OnSendServerData(PlayerManager manager, NetDataWriter writer);
        public OnSendServerData onSendServerData;
        public Dictionary<ushort, PlayerManager> players = new Dictionary<ushort, PlayerManager>();
        public List<PlayerManager> playersList => players.Values.ToList();
        // public int TimeSend = 10;
        public WorldData worldConfig; 
        public ushort Count => (ushort)players.Count;
     
    }
}
