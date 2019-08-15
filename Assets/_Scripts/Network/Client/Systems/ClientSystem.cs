using Community.Client.Components;
using Community.Core;
using Community.Core.Serializables;
using LiteNetLib;
using LiteNetLib.Utils;
using Network.Core;
using Network.Core.Attributes;
using System;
using System.Net;
using System.Net.Sockets;
using Unity.Entities;
using UnityEngine;

namespace Community.Client
{

    public class ClientSystem : ComponentSystem, INetEventListener
    {
        private ushort _lastServerTick;
        private ServerState _cachedServerState;
        private ClientData dataClient;
        private ClientDataSteam dataClientSteam;
        protected override void OnStartRunning()
        {
            dataClient = ClientManager.manager.clientData;
            dataClientSteam = ClientManager.manager.clientDataSteam;
            Debug.Log("Start client");
            dataClient._netManager = new NetManager(this);
            System.Random r = new System.Random();
            _cachedServerState = new ServerState();
            dataClient._userName = Environment.MachineName + " " + r.Next(100000);
            ClientData.LogicTimer = new LogicTimer(OnLogicUpdate);
            dataClient._writer = new NetDataWriter();
            ClientData._packetProcessor = new NetPacketProcessor();
            ClientData._packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3());
            ClientData._packetProcessor.RegisterNestedType<SpawnCurrentPlayer>();
            dataClient._netManager = new NetManager(this)
            {
                AutoRecycle = true
            };
            dataClient._netManager.Start();
            if (ClientManager.manager.isStart) dataClient.Connect();
        }
        private void OnLogicUpdate()
        { }

        protected override void OnStopRunning()
        {
            if (!ClientManager.manager.isClient)
            {
                return;
            }
            dataClient._netManager.Stop(); dataClient.status = ENetworkClient.NoRun;
            base.OnStopRunning();
        }
        protected override void OnUpdate()
        {
            if (!ClientManager.manager.isClient)
            {
                Enabled = false;
                return;
            }
            dataClient._netManager.PollEvents();
            ClientData.LogicTimer.Update();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[C] Connected to server " );
            dataClient.server = peer; 
            ClientData.LogicTimer.Start();
            dataClient.status = ENetworkClient.Run;
            ClientCallBlack.OnConnectedClient?.Invoke();
            EventManager.EventMethod(typeof(EventStartClient), new object[0]);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            EventManager.EventMethod(typeof(EventOnDisconnectedAttribute), new object[] { peer, disconnectInfo });
            dataClient.server = null;
            ClientData.LogicTimer.Stop();
            Debug.Log("[C] Disconnected from server: " + disconnectInfo.Reason);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log("OnNetworkError");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte packetType = reader.GetByte();
            if (packetType >= NetworkGeneral.PacketTypesCount)
                return;
            PacketType pt = (PacketType)packetType;
            switch (pt)
            {
                case PacketType.SystemSync:
                    byte SystemSync = reader.GetByte();
                    UnityEngine.Profiling.Profiler.BeginSample("EventMethodMessage");
                    EventManager.EventMethodMessage(new object[] { reader }, (PacketSystem)SystemSync, reader.GetByte());
                    UnityEngine.Profiling.Profiler.EndSample();
                    break;
                case PacketType.ServerState:
                    _cachedServerState.Deserialize(reader);
                    OnServerState();
                    break;
                case PacketType.Serialized:
                    ClientData._packetProcessor.ReadAllPackets(reader);
                    break;
                default:
                    Debug.Log("Unhandled packet: " + pt);
                    break;
            }
        }
        private void OnServerState()
        {
            //skip duplicate or old because we received that packet unreliably
            if (NetworkGeneral.SeqDiff(_cachedServerState.Tick, _lastServerTick) <= 0)
                return;
            _lastServerTick = _cachedServerState.Tick;
            // _playerManager.ApplyServerState(ref _cachedServerState);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Debug.Log("OnNetworkReceiveUnconnected");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            Debug.Log("OnNetworkLatencyUpdate");
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {

        }

    }
    public enum ENetworkClient : byte
    {
        Run, NoRun
    }
}
