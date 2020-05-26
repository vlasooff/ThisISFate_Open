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
public class NetEntitySystem : ComponentClient
{
    public override void OnStartClient(NetManager manager, NetPacketProcessor netPacketProcessor)
    {
        base.OnStartClient(manager, netPacketProcessor);
        netPacketProcessor.SubscribeReusable<EntityYourJoin>(OnYouJoin);
        netPacketProcessor.SubscribeReusable<EntitySomeJoin>(OnGhostJoin);
        netPacketProcessor.SubscribeReusable<ChatPacket>(OnChatJoin);
    }

    private void OnChatJoin(ChatPacket obj)
    {
        Debug.Log("OnChatJoin");
    }

    private void OnGhostJoin(EntitySomeJoin obj)
    {

        Debug.Log("EntityYourJoin");
    }

    private void OnYouJoin(EntityYourJoin packet)
    {
        Debug.Log("EntityYourJoin");
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        base.OnPeerConnected(peer);

        Debug.Log("Send entity joinng packet");
    } 
} 
