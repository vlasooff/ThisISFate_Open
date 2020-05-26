using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Entities;
using UnityEngine;


[DisableAutoCreation]
[AlwaysUpdateSystem]
public class NetWorkClientSystem : ComponentSystem, INetEventListener
{
    public static List<VVClientNetEvents> netEvents = new List<VVClientNetEvents>();
    private NetManager _netManager;
    private NetDataWriter _writer;
    public static NetPacketProcessor _packetProcessor;
    private bool Connected => server?.ConnectionState == ConnectionState.Connected;
    private NetPeer server;


    private List<Action<NetPacketReader>> message = new List<Action<NetPacketReader>>();


    public bool StartedClient()
    {
        try
        {  
            _netManager = new NetManager(this);
            System.Random r = new System.Random();
           // m_cachedServerState = new ServerState(); 
            //  ClientData.LogicTimer = new LogicTimer(OnLogicUpdate);
            _writer = new NetDataWriter();
            _packetProcessor = new NetPacketProcessor();
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetColor32());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3());
 
            _netManager = new NetManager(this)
            {
                AutoRecycle = true
            };
            _netManager.Start();
            for (int i = 0; i < netEvents.Count; i++)
            {
                netEvents[i].OnStartClient(_netManager,_packetProcessor);
            }
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
     

    public void OnConnectionRequest(ConnectionRequest request)
    { 
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    { 
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    { 
    }


    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        Debug.Log("[client]Message");
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
        _packetProcessor.ReadAllPackets(reader);
    }
    public void MsgSerialized(NetPacketReader reader)
    {
        _packetProcessor.ReadAllPackets(reader);
    }
    public void MsgServerState(NetPacketReader reader)
    { 
    }
    #endregion
    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    { 
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] Connected");
        for (int i = 0; i < netEvents.Count; i++)
        {
            netEvents[i].OnPeerConnected(peer);
        } 
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] Disconnected");
        for (int i = 0; i < netEvents.Count; i++)
        {
            netEvents[i].OnPeerDisconnected(peer,disconnectInfo);
        }
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        StartedClient();
    }
    protected override void OnStopRunning()
    {
        StopClient();
    }
    protected override void OnUpdate()
    {
        if(_netManager.FirstPeer != null)
        { 
            _netManager.PollEvents(); 
        }
        else
        {
            Entities.ForEach((Entity ent, ref CmdConnectClient client) =>
            {
                Debug.Log("[Client] cmd connect");
                client.ip = new Unity.Collections.NativeString64("localhost");
                Connect(ent,client);

            });
        }
    }
    public void Connect(Entity ent,CmdConnectClient connect)
    {
        _netManager.Connect(connect.ip.ToString(),connect.port, "sample_app");
        EntityManager.RemoveComponent<CmdConnectClient>(ent);
    }

    public void Connect(string _serverIp, int _port)
    { 
        _netManager.Connect(_serverIp, _port, "sample_app"); 
    }
    public void Connect(string _serverIp)
    {
        Connect(_serverIp, 25000);
    }  
    public void Disconnected()
    {
        _netManager.DisconnectPeer(_netManager.FirstPeer);
    }
    public void StopClient()
    {
        for (int i = 0; i < netEvents.Count; i++)
        {
            netEvents[i].OnStopClient();
        }
        if (_netManager != null)
         _netManager.Stop();  

    }
    public void SendPacketSerializable<T>(PacketType type, T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
    {
        if (server == null)
            return;
        _writer.Reset();
        _writer.Put((byte)type);
        packet.Serialize(_writer);
        server.Send(_writer, deliveryMethod);
    }

    public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
    {
        if (_netManager == null)
            return;
        _writer.Reset();
        _writer.Put((byte)PacketType.Serialized);
        _packetProcessor.Write(_writer, packet);
        _netManager.FirstPeer.Send(_writer, deliveryMethod);
    }

}