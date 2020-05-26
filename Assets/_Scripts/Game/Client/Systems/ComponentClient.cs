using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Entities;
using UnityEngine;

public abstract class ComponentClient : ComponentSystem, VVClientNetEvents
{
    protected ComponentClient()
    { }
    protected NetManager _manager;
    protected NetPacketProcessor _packetProcessor;
    protected readonly NetDataWriter _cachedWriter = new NetDataWriter();

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        NetWorkClientSystem.netEvents.Add(this);
    }
    public virtual void OnDisconnected()
    { 

    }

    public virtual void OnPeerConnected(NetPeer peer)
    { 

    }

    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    { 
    }

    protected override void OnUpdate()
    { 
    }

    public virtual void OnStartClient(NetManager manager, NetPacketProcessor netPacketProcessor)
    {
        Debug.Log("Start client");
        _manager = manager;
        _packetProcessor = netPacketProcessor; 
    }

    public virtual void OnStopClient()
    {
    }
    public void SendPacketSerializable<T>(PacketType type, T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
    {
        if (_manager == null)
            return;
        _cachedWriter.Reset();
        _cachedWriter.Put((byte)type);
        packet.Serialize(_cachedWriter);
        _manager.FirstPeer.Send(_cachedWriter, deliveryMethod);
    }

    public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
    {
        if (_manager == null)
            return;
        _cachedWriter.Reset();
        _cachedWriter.Put((byte)PacketType.Serialized);
        _packetProcessor.Write(_cachedWriter, packet);
        _manager.FirstPeer.Send(_cachedWriter, deliveryMethod);
    }
}