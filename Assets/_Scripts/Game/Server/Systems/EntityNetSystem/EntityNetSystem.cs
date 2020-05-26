using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Entities;
using UnityEngine;

public delegate void NewEntityEvent(NetPeer peer);
[DisableAutoCreation]
[AlwaysUpdateSystem]
public class EntityNetSystem : ComponentServer
{
    private byte count;
    public override void OnStartServer(NetManager managerNet, NetPacketProcessor packetProcessor)
    {
        base.OnStartServer(managerNet, packetProcessor); 
        packetProcessor.SubscribeReusable<EntityJoining, NetPeer>(OnJoinEntity);
    }

    private void OnJoinEntity(EntityJoining packet, NetPeer peer)
    {
        Entity entity = World.EntityManager.CreateEntity();
        count++;
        EntityNet entityNet = new EntityNet() { id = count};
        EntityUserName entityUserName = new EntityUserName() { name = new Unity.Collections.NativeString32(packet.UserName) };
        EntitySteamID entitySteamID = new EntitySteamID() { id = packet.steamid };
        peer.Tag = entity;
        EntityManager.AddSharedComponentData<EntityNet>(entity, entityNet);
        EntityManager.AddSharedComponentData<EntitySteamID>(entity, entitySteamID);
        EntityManager.AddSharedComponentData<EntityUserName>(entity, entityUserName);
        EntityManager.SetName((Entity)peer.Tag, packet.UserName);
        manager.SendToAll(WritePacket(new EntitySomeJoin() ),DeliveryMethod.Sequenced,peer);
        Debug.Log("[Server] OnJoinEntity " + packet.UserName);
    }
    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    { 
        if (peer.Tag != null)
        {
            count--; 
            manager.SendToAll(WritePacket(new EntityUnjoin() { id =
            EntityManager.GetSharedComponentData<EntityNet>((Entity)peer.Tag).id
            }), DeliveryMethod.Sequenced);
            EntityManager.DestroyEntity((Entity)peer.Tag);
        }
        else Debug.LogError("[SERVER] Peer tag null");
        Debug.Log("[Server] OnDisconnectedEntity " + disconnectInfo);
        
    }
    public override void OnPeerConnected(NetPeer peer)
    {

    }
    protected override void OnUpdate()
    { 

    }
    
} 