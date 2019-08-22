using Community.Core;
using Community.Core.Serializables;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using Network.Core;
using Network.Core.Attributes;
using Network.Core.Serializables; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Jobs;
using UnityEngine;
using Unity.Entities;

namespace Community.Server
{
    [DisableAutoCreation]
    public class JobGlobalServer : ComponentServer, INetEventListener
    {

        ServerProxy m_proxy;
        ServerInfoProxy m_info;
        private bool m_isRunServer = false;
        protected override bool IsLog => false;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            ServerCallBlack.onStartServer += (NetPacketProcessor _packetProcessor) =>
            {
                Debug.Log($"[SERVER] Server started: {StartedServer()}");
            };
        }
        public bool StartedServer()
        {
            try
            {
                if (ServerCallBlack.isServerRun) return false;
                m_proxy = ServerManager.manager.serverProxy;
                m_info = ServerManager.manager.serverInfoProxy;
                if (!m_proxy || !m_info)
                {
                    Debug.LogError("Server components not found!");
                    Enabled = false;
                }
                m_proxy._packetProcessor = new NetPacketProcessor();
                m_proxy._logicTimer = new LogicTimer(OnLogicUpdate);
                m_proxy._logicTimer.Start();
                m_proxy._netManager = new NetManager(this)
                {
                    AutoRecycle = true
                };
                m_proxy._netManager.DisconnectTimeout = 30000;
                ServerCallBlack.RegisterNestedTypes(m_proxy._packetProcessor);
                m_isRunServer = true;
                ServerCallBlack.isServerRun = true;
                ServerCallBlack.onStartedServer?.Invoke(m_proxy._packetProcessor);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[SERVER] " + ex);
                return false;
            }
        }
        public void StopServer()
        {
            m_proxy._logicTimer.Stop();
            m_proxy._netManager.Stop();
            ServerCallBlack.onShutdownServer?.Invoke();
            ServerCallBlack.onShutdownServer?.Invoke();
        }

        private void OnLogicUpdate()
        {
            ServerInfoProxy._serverTick = (ushort)((ServerInfoProxy._serverTick + 1) % NetworkGeneral.MaxGameSequence);

            if (ServerInfoProxy._serverTick % 2 == 0)
            {
                //  state = new TransformsPacket();
                //state.ServerTime = ServerInfoProxy._serverTick;
                // state.packets = new TransformPacket[playersManager.Count];
                // for (int i = 0; i < playersManager.Count; i++)
                // {
                //    state.packets[i] = playersManager.players.ToList()[i].Value._buffer;
                //}
                NetDataWriter writer = m_proxy.WritePacket(new ServerBaseState(ServerInfoProxy._serverTick, (ushort)m_proxy._netManager.PeersCount));
                ServerCallBlack.onUpdateState?.Invoke(writer);
                m_proxy._netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        #region Events
        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("sample_app");
        }


        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.LogError("[SERVER] Error " + socketError);
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


        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            NetJobReader jobReader = reader.GetJob();

            byte id = reader.GetByte();
            LogDev("OnNetworkReceive " + id);
            switch ((PacketType)id)
            {
                case PacketType.Event:

                  
                    break;
                case PacketType.Serialized:

                    m_proxy._packetProcessor.ReadAllPackets(reader, peer);
                    break;
                default:
                    Debug.Log("Unhandled packet: " + (PacketType)id);
                    break;
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            LogDev("[SERVER] OnPeerConnected");
            ServerCallBlack.onConnectedPlayer?.Invoke(peer);
            EventManager.EventMethod(typeof(EventOnConnectedAttribute), new object[] { peer });
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            LogDev("[SERVER]  OnPeerDisconnected " + disconnectInfo);
            if (peer.Tag != null)
            {
                var plp = new PlayerLeavedPacket { Id = (ushort)peer.Id };
                m_proxy._netManager.SendToAll(WritePacket(plp), DeliveryMethod.ReliableOrdered);
            }
            EventManager.EventMethod(typeof(EventOnDisconnectedAttribute), new object[] { peer, disconnectInfo });
        }
        #endregion

        protected override void OnUpdate()
        {
            if (m_isRunServer)
            {
                m_proxy._netManager.PollEvents();
                m_proxy._logicTimer.Update();
            }
        }
        protected override void OnStopRunning()
        {
            StopServer();
        }
    }
    public struct JobOnMessage : IJob
    {
        public NetJobReader reader;
        public ulong hash;
        public void Execute()
        {
            byte id = reader.GetByte();
            switch ((PacketType)id)
            {
                case PacketType.Event:

                    Debug.Log("Event packet: ");
                    break;
                case PacketType.Serialized:
                    while (reader.AvailableBytes > 0)
                    {
                        hash = reader.GetULong();
                        
                    }
                    break;
                default:
                    Debug.Log("Unhandled packet: " + (PacketType)id);
                    break;
            }
        }
    }

}

