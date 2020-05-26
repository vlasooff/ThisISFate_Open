using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class SpawnSystem : ComponentSystem, VVServerNetEvents
{
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        NetServerSystem.netEvents.Add(this);
    }
    public void OnPeerConnected(NetPeer peer)
    { 

    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    { 
    }

    public void OnStartServer(NetManager manager, NetPacketProcessor _packetProcessor)
    {
        Debug.Log("Spawn system active");
    }

    public void OnStopServer()
    { 
    }

    protected override void OnUpdate()
    {

    }
}
