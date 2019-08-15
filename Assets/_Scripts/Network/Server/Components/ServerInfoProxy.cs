using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Components
{
   
    public class ServerInfoProxy : MonoBehaviour
    {

        public ServerConfig config = new ServerConfig(); 
        public static ushort _serverTick;
        public bool SimulateLatency;
        public bool SimulatePacketLoss;

        public string serverFolder { get { return "Servers/Server_" + config.port; } }

    } 
}
