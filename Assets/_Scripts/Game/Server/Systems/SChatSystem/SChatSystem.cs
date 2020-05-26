
using System;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Entities;
using UnityEngine;

public class SChatSystem : ComponentServer
{
    public override void OnStartServer(NetManager managerNet, NetPacketProcessor packetProcessor)
    {
        base.OnStartServer(managerNet, packetProcessor); 
        packetProcessor.SubscribeReusable<ChatPacket, NetPeer>(OnMessage);
    }

    private void OnMessage(ChatPacket chat, NetPeer peer)
    {

    }
    public override void OnPeerConnected(NetPeer peer)
    {   
        base.OnPeerConnected(peer); 
        manager.SendToAll(WritePacket(new ChatPacket() { Id = 0 , text = "Connected"}), DeliveryMethod.ReliableOrdered);
    }

    protected override void OnUpdate()
    { 
    }  
}