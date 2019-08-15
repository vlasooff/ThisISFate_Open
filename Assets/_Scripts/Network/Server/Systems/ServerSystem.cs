
using Community.Core;
using Community.Core.Serializables;
using Community.Other;
using Community.Server.Components;
using Community.Server.Systems;
using LiteNetLib;
using LiteNetLib.Utils;
using Network.Core;
using Network.Core.Attributes;
using Network.Core.Serializables; 
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Unity.Entities;
using UnityEngine;

namespace Community.Server
{
    [DisableAutoCreation]
    public class ServerSystem : ComponentServer, INetEventListener
    {
        ServerProxy proxy;
        ServerInfoProxy info;

        private ServerState m_serverState;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            World.Active.GetOrCreateSystem<MoveSystem>().Enabled = true;
        }
        [EventStartServer]
        public void OnStartServer()
        {  
           // PlayersInfoComponent.OnEventPlayer += (PlayerManager player, bool isNew) =>
          //  {
          //      NetDataWriter writer = new NetDataWriter();
          //      m_serverState.Serialize(writer);
          //      player.peer.Send(writer, DeliveryMethod.ReliableOrdered);
          //  };

            Debug.Log("Start server!");
        }

        protected override void OnStartRunning()
        {
            proxy = ServerManager.manager.serverProxy;
            info = ServerManager.manager.serverInfoProxy;
            if (!proxy || !info)
            {
                Debug.LogError("Server components not found!");
                Enabled = false;
            }
            base.OnStartRunning();
            proxy._packetProcessor = new NetPacketProcessor();
            //   _playerManager = new ServerPlayerManager(this);

            //register auto serializable vector3
            proxy._packetProcessor.RegisterNestedType((w, v) => w.Put(v), r => r.GetVector3());
            // регистрируем auto serializable PlayerState
            proxy._packetProcessor.RegisterNestedType<SpawnCurrentPlayer>();
            proxy._packetProcessor.RegisterNestedType<PlayerData>();
          //  proxy._packetProcessor.RegisterNestedType<PlayersData>();

            proxy._netManager = new NetManager(this)
            {
                AutoRecycle = true
            };
            proxy._netManager.DisconnectTimeout = 30000;
            if (ServerManager.manager.isStart) proxy.StartServer(info);
        }
        protected override void OnStopRunning()
        {
            if (Active)
            {
                proxy._netManager.Stop(); 
                proxy._logicTimer.Stop();
            }
        }
        protected override void OnUpdate()
        {
            Debug.LogWarning("SERVER");
            if (Active)
                proxy._netManager.PollEvents();

        }

        public void OnPeerConnected(NetPeer peer)
        { 

            LogDev("OnPeerConnected");
            EventManager.EventMethod(typeof(EventOnConnectedAttribute), new object[] { peer });
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            LogDev("OnPeerDisconnected " + disconnectInfo);
            if (peer.Tag != null)
            {
                var plp = new PlayerLeavedPacket { Id = (ushort)peer.Id };
                proxy._netManager.SendToAll(WritePacket(plp), DeliveryMethod.ReliableOrdered);
            }
            EventManager.EventMethod(typeof(EventOnDisconnectedAttribute), new object[] { peer, disconnectInfo });
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            LogModule("error " + socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte id = reader.GetByte();
            LogDev("OnNetworkReceive " + id);
            switch ((PacketType)id)
            {
                case PacketType.Event:

                    var methods = EventManager.EventMethod(typeof(OnEventMessageAttribute));
                    break;
                case PacketType.Serialized:
                    proxy._packetProcessor.ReadAllPackets(reader, peer);
                    break;
                default:
                    Debug.Log("Unhandled packet: " + (PacketType)id);
                    break;
            }



        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryRequest)
            {
                LogModule("Received discovery request. Send discovery response");
                // _Manager.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);

            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("sample_app");
        }


    }
    [System.Serializable]
    public struct ServerConfig
    {
        public string name;
        public ushort port;
        public byte MaxPlayers;
        public string pathServer;

        public ServerConfig(string _name)
        {
            name = _name;
            port = 10515;
            MaxPlayers = 64;
            pathServer =  $"/Servers/Server_{port}/"; 

        }
        public void Load()
        {
          //  Directory.CreateDirectory(pathServer);
            Directory.CreateDirectory(Environment.CurrentDirectory + pathServer);
            SaveManager.LoadJSON<ServerConfig>($"{pathServer}Server.config");
            pathServer = $"/Servers/Server_{port}/";
        }

        public void Save()
        { 
            SaveManager.SaveJSON(this, $"{pathServer}Server.config");
        }
    }
}