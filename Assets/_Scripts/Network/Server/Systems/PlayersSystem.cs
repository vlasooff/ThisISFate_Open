using Community.Core;
using Community.Other;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Community.Server.Systems;
using System;

namespace Community.Server
{
    public delegate void OnCreatePlayer(Components.EntityPlayerManager player, bool isNew);
   
    [DisableAutoCreation]
    public class PlayersSystem : ComponentServer
    {
        ushort timeOut = 0; 
        private ServerProxy ServerData => ServerManager.manager.serverProxy;
        private PlayersInfoComponent playersData => ServerManager.manager.playersData;
        
        protected override void onStartedServer(NetPacketProcessor _packetProcessor)
        {
            _packetProcessor.RegisterNestedType<CustomCharacter>();
            _packetProcessor.RegisterNestedType<CharacterCustomHead>();
            _packetProcessor.RegisterNestedType<CharacterCustomBody>();
            _packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);
            _packetProcessor.RegisterNestedType<UpdatePlayerPacket>();
            _packetProcessor.RegisterNestedType<ItemWorld>();
            SyncStateServer.onPlayerMsec += UpdatePlayersMsec;
            SyncStateServer.onPlayerSec += OnSyncForPlayer;
           // CustomizeSystem.onCreateCharacter += ConnectionPlayer;
            ServerCallBlack.onDisconnectedPlayer += RemovePlayer;
            SyncStateServer.OnCreatedPlayerResponse += OnResponseCreatePlayer;
            playersData.worldConfig = new WorldData();
        }

        private void OnSyncForPlayer()
        {
            for (int i = 0; i < playersData.players.Count; i++)
            {
                PlayerManager playerCurrent = playersData.players.ElementAt(i).Value; 
                playerCurrent.peer.Send(WritePacket(GetUpdatePlayers(playerCurrent)), DeliveryMethod.ReliableOrdered); 
            }
        }

        private void OnResponseCreatePlayer(EntityPlayerManager manager, NetDataWriter packet)
        {
            Debug.Log("[S]OnResponseCreatePlayer");
          //  manager.peer.Send(WritePacket(SendServerInfo(manager)), DeliveryMethod.ReliableOrdered);
            //Отправка новому игроку данных сервера, игроков, и его данные 
        }

        protected override void OnStartServer(NetPacketProcessor _packetProcessor)
        {
            playersData.worldConfig = new WorldData(); 
            playersData.worldConfig.Load();

        }

        
        public void OnJoinReceived(JoinPacket join , NetPeer peer)
        {
            if (peer != null)
            {
                int id = playersData.players.Values.Count + 1;
                PlayerManager playerServer = new PlayerManager(join, (ushort)id, peer);
                if (playerServer.Load())
                {
                    if (playerServer.password != join.password)
                    {
                        peer.Send(WritePacket(new PlayerAccountPacket(EUserState.wrongPassword)), DeliveryMethod.ReliableOrdered);
                        
                        return;
                    }
                    else 
                    {
                        peer.Send(WritePacket(new PlayerAccountPacket(EUserState.login)), DeliveryMethod.ReliableOrdered);
                        ConnectionPlayer(playerServer);
                    }
                }
                else
                {
                    peer.Send(WritePacket(new PlayerAccountPacket(EUserState.create)), DeliveryMethod.ReliableOrdered);
                   // 
                    playerServer.isNew = true;
                    return;
                }
            }
            else Debug.LogError(this + " Error peer != null");
        }
        private void ConnectionPlayer(PlayerManager playerServer)
        {
           
                if (!playerServer.isNew)
                {
                    playersData.players.Add(playerServer.id, playerServer); 
                    playerServer.SpawnLoadInfo();
                    LogDev($"[S] Add  player " + playerServer.id);
                   // ServerCallBlack.onCreatePlayer?.Invoke(playerServer, false);
                }
                else
                {
                    playersData.worldConfig.usersCount++;
                    playerServer.id = playersData.worldConfig.usersCount;
                    playersData.players.Add(playerServer.id, playerServer);
                    playerServer.Spawn(new Vector3(1685, 25, 911));
                    LogDev($"[S] Add new player " + playerServer.id);
                 //   ServerCallBlack.onCreatePlayer?.Invoke(playerServer,true);
                   
                } 
                 ServerData._netManager.SendToAll(WritePacket(playerServer.GetJoinedPacket()), DeliveryMethod.ReliableOrdered,playerServer.peer);//Отправка базовых данных
          
        }
        protected override void onUpdateStateServer(NetDataWriter dataWriter)
        {
            if ((ServerInfoProxy._serverTick - timeOut) > 5)
            {
                timeOut = ServerInfoProxy._serverTick; 
            //    UpdatePlayers();
            }
            else if (ServerInfoProxy._serverTick == 0) timeOut = 10;
        }
        private void RemovePlayer(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            PlayerManager player = (PlayerManager)peer.Tag;
            player.StopPlayer();
            playersData.players.Remove(player.id);
        }
        protected override void OnUpdate()
        {
        }
        protected override void OnShutdown()
        {
            Debug.Log("[S] Shutdown server!");
            
 
                playersData.worldConfig.Save();
           

        }
        protected void UpdatePlayersMsec()
        { 
            for (int i = 0; i < playersData.players.Count; i++)
            {
                PlayerManager playerCurrent = playersData.players.ElementAt(i).Value;
                 
                playerCurrent.peer.Send(WritePacket(GetUpdatePlayers(playerCurrent)), DeliveryMethod.ReliableOrdered);
                 
            }
        }

        protected virtual ServerInfoConnect SendServerInfo(PlayerManager playerServer)
        {
            ServerInfoConnect inform = new ServerInfoConnect();
            inform.title = "[RUS] SERVER TEST";
            inform.currentPlayer = playerServer.GetSpawnCurrentPlayer();
                

            Debug.Log("[S] Server  players :" + playersData.players.Count);
            List<PlayerData> playerDatas = new List<PlayerData>(); 
            Entities.ForEach((ref PlayerID playerID) =>
            { 
                if (playerID.id != playerServer.id)
                {
                    playerDatas.Add(new PlayerData() { Id = playerID.id, steamid = playerID.SteamID, username = new NativeString64(playerID.username.ToString()) }) ; 
                }
            });
          
            inform.players = playerDatas.ToArray();

            Debug.Log("[S] Server info connect and remote players :" + inform.players.Length);
            return inform;
        }
       
        private UpdatePlayersPacket GetUpdatePlayers(PlayerManager player)
        {
            UpdatePlayersPacket playersPacket = new UpdatePlayersPacket();
            playersPacket.ServerTick = ServerInfoProxy._serverTick;

            List<UpdatePlayerPacket> playerDatas = new List<UpdatePlayerPacket>();

            Entities.ForEach((ref PlayerID playerID,ref PlayerMotor playerMotor) =>
            {
                PlayerManager manager = playersData.players[playerID.id];

                if (playerID.id != player.id)
                {
                    playerDatas.Add(new UpdatePlayerPacket(playerID.id, manager.transform.position, new Vector2(playerMotor.movingDir.x, playerMotor.movingDir.z), playerMotor._rotation, playerMotor._speed));
                }
            });
         
            playersPacket.updatePlayers = playerDatas.ToArray();
            return playersPacket;
        }
    }
    [System.Serializable]
    public class WorldData
    {
        public ushort usersCount;
        string name = "/World.dat"; 

        public void Load()
        {
            NetDataReader reader = SaveManager.Load(ServerManager.manager.serverInfoProxy.serverFolder + name);
            if (reader == null)
            {
                usersCount = 0; 
            }
            else
            {
                usersCount = reader.GetUShort(); 
            }
          
             
        }
        public void Save()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(usersCount); 
            SaveManager.Save(writer, ServerManager.manager.serverInfoProxy.serverFolder + name); 
        }
    }
   
}
