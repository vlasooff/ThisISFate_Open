using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Entities;
using UnityEngine;


[DisableAutoCreation]
[AlwaysUpdateSystem]
public class NetServerSystem : ComponentSystem, INetEventListener
{
    public static List<VVServerNetEvents> netEvents = new List<VVServerNetEvents>();
    public NetManager _netManager;
    public NetPacketProcessor _packetProcessor; 
    private readonly NetDataWriter _cachedWriter = new NetDataWriter();
    private List<Action<NetPacketReader, NetPeer>> message = new List<Action<NetPacketReader, NetPeer>>();
    ServerConfig config;

    public void StartServer(ServerStart info)
    {
        if (_netManager != null)
            return;
        _packetProcessor = new NetPacketProcessor();
        config = new ServerConfig("Test");
        config.Load();
        _netManager = new NetManager(this)
        {
            AutoRecycle = true
        };
        _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetColor32());
        _netManager.DisconnectTimeout = 30000;
        _netManager.SimulateLatency = info.SimulateLatency;
        _netManager.SimulatePacketLoss = info.SimulatePacketLoss;
        _netManager.NatPunchEnabled = true;
        _netManager.Start(info.port);
        for (int i = 0; i < netEvents.Count; i++)
        {
            netEvents[i].OnStartServer(_netManager,_packetProcessor);
        }

    }
    public void StopServer()
    {
        if (_netManager != null) {
            for (int i = 0; i < netEvents.Count; i++)
            {
                netEvents[i].OnStopServer();
            }
            _netManager.Stop();
        }   
    }

    public void OnConnectionRequest(ConnectionRequest request)
    { 
        request.AcceptIfKey("sample_app");
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.LogError("[SERVER] Error " + socketError);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
      
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        byte id = reader.GetByte();
        message[id].Invoke(reader, peer);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryRequest)
        {
            Debug.Log("Received discovery request. Send discovery response");
            _netManager.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);

        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] OnPeerConnected");
        for (int i = 0; i < netEvents.Count; i++)
        {
            netEvents[i].OnPeerConnected(peer);
        }
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] OnPeerDisconnected");
        for (int i = 0; i < netEvents.Count; i++)
        {
            netEvents[i].OnPeerDisconnected(peer, disconnectInfo);
        }
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity ent, ref ServerStart client) =>
        {
            Debug.Log("[Server] Server starting");
            StartServer(client);
            EntityManager.RemoveComponent<ServerStart> (ent);

        });
        if(_netManager != null)
        { 
            if (_netManager.IsRunning)
            { 
                _netManager.PollEvents(); 
            }
        }
    }
    protected override void OnStopRunning()
    {
        config.Save();
        StopServer();
    }
    public NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
    {
        _cachedWriter.Reset();
        _cachedWriter.Put((byte)type);
        packet.Serialize(_cachedWriter);
        return _cachedWriter;
    }

    public NetDataWriter WritePacket<T>(T packet) where T : class, new()
    {
        _cachedWriter.Reset();
        _cachedWriter.Put((byte)PacketType.Serialized);
        _packetProcessor.Write(_cachedWriter, packet);
        return _cachedWriter;
    }
    public NetDataWriter WritePacket<T>(T packet, NetDataWriter writer) where T : class, new()
    {
        if (writer.Length == 0)
        {
            Debug.Log("[S] writer = 0");
            writer = WritePacket(packet);
        }
        else
        {
            Debug.Log("Writet != null");
            _packetProcessor.Write(writer, packet);
        }
        return writer;
    }
}
