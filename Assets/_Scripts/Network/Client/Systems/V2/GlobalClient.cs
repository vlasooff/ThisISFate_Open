using Community.Client.Components;
using Community.Core;
using Community.Core.Serializables;
using LiteNetLib;
using LiteNetLib.Utils;
using Network;
using Network.Core;
using Network.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Net;        
using System.Net.Sockets;
using UnityEngine;

namespace Community.Client
{
    public class GlobalClient : ComponentClient, INetEventListener
    {
        private ushort m_lastServerTick;
        private ServerState m_cachedServerState;    
        private ClientData m_dataClient;
        private bool m_isRunClient = false;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            ClientCallBlack.onStartedClient += () =>
            {
                Debug.Log("[C] Started: " + StartedClient());
            };

        }

        public bool StartedClient()
        {
            try
            {
                m_dataClient = ClientManager.manager.clientData; 
                m_dataClient._netManager = new NetManager(this);
                System.Random r = new System.Random();
                m_cachedServerState = new ServerState();
                m_dataClient._userName = Environment.MachineName + " " + r.Next(100000);
                //  ClientData.LogicTimer = new LogicTimer(OnLogicUpdate);
                m_dataClient._writer = new NetDataWriter();
                ClientData._packetProcessor = new NetPacketProcessor();
                ClientData._packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetColor32());
                ClientData._packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3());
                ClientData._packetProcessor.RegisterNestedType<SpawnCurrentPlayer>();
                ClientData._packetProcessor.RegisterNestedType<RemotePlayersData>();
                ClientData._packetProcessor.RegisterNestedType<PlayerData>();
                ClientData._packetProcessor.SubscribeReusable<ServerBaseState>(OnBaseState);
                m_dataClient._netManager = new NetManager(this)
                {
                    AutoRecycle = true
                };
                m_dataClient._netManager.Start();
                m_isRunClient = true;
                ClientCallBlack.isClientRun = true;
                //Mesagee 
                message.Add(MsgSyncSytstem);
                message.Add(MsgServerState);
                message.Add(MsgSerialized);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[CLIENT]" + ex);
                return false;
            }
        }

        private void OnBaseState(ServerBaseState obj)
        {
            m_dataClient.state = obj;
            ClientCallBlack.ServerTick = obj.ServerTime;
        }

        private void OnServerState()
        {
            //skip duplicate or old because we received that packet unreliably
            if (NetworkGeneral.SeqDiff(m_cachedServerState.Tick, m_lastServerTick) <= 0)
                return;
            m_lastServerTick = m_cachedServerState.Tick;
            // _playerManager.ApplyServerState(ref _cachedServerState);
        }


        #region Events
        public void OnConnectionRequest(ConnectionRequest request)
        {
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        { 
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        { 
        }
        private List<Action<NetPacketReader>> message = new List<Action<NetPacketReader>>();
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            
            byte packetType = reader.GetByte(); 
            if (packetType >= NetworkGeneral.PacketTypesCount)
            {
                Debug.Log("Unhandled packet: " + packetType + "/" + message.Count);
                return;
            }
              message[packetType].Invoke(reader);
            return;
            /*
            PacketType pt = (PacketType)packetType;
            switch (pt)
            {
                case PacketType.SystemSync:
                    byte SystemSync = reader.GetByte(); 
                    EventManager.EventMethodMessage(new object[] { reader }, (PacketSystem)SystemSync, reader.GetByte()); 
                    break;
                case PacketType.ServerState:
                    break;
                case PacketType.Serialized:
                    break;
                default:
                    break;
            }*/
        }
        #region onMesage
        public void MsgSyncSytstem(NetPacketReader reader)
        { 
            ClientData._packetProcessor.ReadAllPackets(reader);
        }
        public void MsgSerialized(NetPacketReader reader)
        { 
            ClientData._packetProcessor.ReadAllPackets(reader);
        }
        public void MsgServerState(NetPacketReader reader)
        { 
            m_cachedServerState.Deserialize(reader);
            OnServerState();
        }
        #endregion
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Debug.Log("OnNetworkReceiveUnconnected");
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[C] Connected to server: " + peer.EndPoint);

            m_dataClient.server = peer;  
            ClientData.LogicTimer.Start();
            m_dataClient.status = ENetworkClient.Run;
            ClientCallBlack.OnConnectedClient?.Invoke();
            ClientCallBlack.isConnected = true;
            EventManager.EventMethod(typeof(EventStartClient), new object[0]);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ClientCallBlack.OnDisconnectedClient?.Invoke();
            EventManager.EventMethod(typeof(EventOnDisconnectedAttribute), new object[] { peer, disconnectInfo });
            m_dataClient.server = null;
            ClientData.LogicTimer.Stop();
            StopClient();
            Debug.Log("[C] Disconnected from server: " + disconnectInfo.Reason);
        }
        #endregion
        protected override void OnUpdate()
        {
            if (m_isRunClient)
            { 
                m_dataClient._netManager.PollEvents();
                ClientData.LogicTimer.Update();
            }
        }
        public void StopClient()
        {
            m_dataClient._netManager.Stop();
            m_dataClient.status = ENetworkClient.NoRun;
            m_isRunClient = false;
            ClientCallBlack.isClientRun = false;
            ClientCallBlack.OnShutdownClient?.Invoke();

        }
    }

}