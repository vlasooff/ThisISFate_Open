using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Core;
using Community.Other;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Systems
{
    [DisableAutoCreation]
    public class AutchSystem : ComponentServer
    {
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
            throw new NotImplementedException();
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
