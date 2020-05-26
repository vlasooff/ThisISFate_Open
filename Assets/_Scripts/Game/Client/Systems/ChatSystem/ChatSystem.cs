using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ChatSystem : ComponentClient
{
    public override void OnStartClient(NetManager manager, NetPacketProcessor netPacketProcessor)
    {
       
        base.OnStartClient(manager, netPacketProcessor); 
        netPacketProcessor.SubscribeReusable<ChatPacket>(OnMessage);
    }

    private void OnMessage(ChatPacket obj)
    { 
        Debug.Log("[Client] <color=green>[CHAT]</color> " + obj.text);
        SendPacket(obj, DeliveryMethod.ReliableSequenced);
    }
    public override void OnStopClient()
    {
        base.OnStopClient(); 
        if(_packetProcessor != null)
        _packetProcessor.RemoveSubscription<ChatPacket>();
    }
    protected override void OnUpdate()
    {
    }
} 
