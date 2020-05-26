using LiteNetLib;
using LiteNetLib.Utils;
using System;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Плохо или хорошо?
/// </summary>
public abstract class ComponentServer : ComponentSystem, VVServerNetEvents
{
    public bool Active = false;
    protected ComponentServer()
    { }

    protected NetManager manager;
    protected NetPacketProcessor _packetProcessor;
    protected readonly NetDataWriter _cachedWriter = new NetDataWriter();


    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        NetServerSystem.netEvents.Add(this);
    }
    protected override void OnStopRunning()
    {
        NetServerSystem.netEvents.Remove(this);
    } 

    public virtual void OnPeerConnected(NetPeer peer)
    { 
    }

    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    { 
    }

    public virtual void OnStopServer()
    { 
    }

    public virtual void OnStartServer(NetManager managerNet, NetPacketProcessor packetProcessor)
    {
        manager = managerNet;
        _packetProcessor = packetProcessor;
    }

    public virtual NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
    {
        _cachedWriter.Reset();
        _cachedWriter.Put((byte)type);
        packet.Serialize(_cachedWriter);
        return _cachedWriter;
    }

    public virtual NetDataWriter WritePacket<T>(T packet) where T : class, new()
    {
        _cachedWriter.Reset();
        _cachedWriter.Put((byte)PacketType.Serialized);
        _packetProcessor.Write(_cachedWriter, packet);
        return _cachedWriter;
    }
    public  NetDataWriter WritePacket<T>(T packet, NetDataWriter writer) where T : class, new()
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