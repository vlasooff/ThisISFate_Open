using Community.Core;
using Community.Other;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Systems
{
    public delegate void OnAutchPeer(EntityPlayerManager player);

    public class AutchSystem : ComponentServer
    {
        public static OnAutchPeer onAutchPeer;
        private EntitysManager EntitysManager;
        private ServerProxy ServerData => ServerManager.manager.serverProxy;
        protected override void onStartedServer(NetPacketProcessor _packetProcessor)
        {

            ServerData._packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);
            EntitysManager = ServerManager.manager.GetManager<EntitysManager>(EManagers.entitysnet);
        }

        private void OnJoinReceived(JoinPacket packet, NetPeer peer)
        {
            if (peer != null)
            {

                EntityPlayerManager entityPlayer = (EntityPlayerManager)peer.Tag;
                if (entityPlayer != null)
                {
                    entityPlayer.username = packet.UserName;

                    EntityManager.AddComponentData(entityPlayer.entityWorld, new EntityName(entityPlayer.username));
                    EntityManager.AddComponentData(entityPlayer.entityWorld, new EntitySteamID(packet.steamid));
                    EntityManager.SetName(entityPlayer.entityWorld, "Player_" + packet.UserName);
                  
                }
                else Debug.LogError("[S] Entity player null for set name");
                AutchUser user = LoadInfoEntity(packet.UserName);
                if (user != null)
                {
                    if (user.password != packet.password)
                    {
                        peer.Send(WritePacket(new PlayerAccountPacket(EUserState.wrongPassword)), DeliveryMethod.ReliableOrdered);

                        return;
                    }
                    else
                    {
                        peer.Send(WritePacket(new PlayerAccountPacket(EUserState.login)), DeliveryMethod.ReliableOrdered);

                    }
                }
                else
                {
                    peer.Send(WritePacket(new PlayerAccountPacket(EUserState.create)), DeliveryMethod.ReliableOrdered);
                    SaveEntity(new AutchUser(packet));
                    return;
                }
            }
            else Debug.LogError(this + " Error peer != null");
        }

        private AutchUser LoadInfoEntity(string username)
        {
            return SaveManager.LoadJSON<AutchUser>($"{ServerManager.manager.serverInfoProxy.serverFolder}/Players/player_{username}/User_{username}.dat");

        }
        private void SaveEntity(AutchUser user)
        {
            SaveManager.CreateFolder($"{ServerManager.manager.serverInfoProxy.serverFolder}/Players/player_{user.UserName}/");
            SaveManager.SaveJSON(user, $"{ServerManager.manager.serverInfoProxy.serverFolder}/Players/player_{user.UserName}/User_{user.UserName}.dat");
        }
        protected override void OnUpdate()
        { 
        }
    }

    [System.Serializable]
    public class AutchUser
    {
        public ulong steamid;
        public string UserName;
        public string password;
        public AutchUser(JoinPacket packet)
        {
            steamid = packet.steamid;
            UserName = packet.UserName;
            password = packet.password;
        }
    }
}
