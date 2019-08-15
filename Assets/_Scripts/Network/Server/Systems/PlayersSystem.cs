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
    public delegate void OnCreatePlayer(Components.PlayerManager player, bool isNew);
   

    public class PlayersSystem : ComponentServer
    {
        ushort timeOut = 0; 
        private ServerProxy ServerData => ServerManager.manager.serverProxy;
        private PlayersInfoComponent playersData => ServerManager.manager.playersData;
        
        protected override void onStartedServer(NetManager manager)
        {
            ServerData._packetProcessor.RegisterNestedType<CustomCharacter>();
            ServerData._packetProcessor.RegisterNestedType<CharacterCustomHead>();
            ServerData._packetProcessor.RegisterNestedType<CharacterCustomBody>();
            ServerData._packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);
            ServerData._packetProcessor.RegisterNestedType<UpdatePlayerPacket>();
            ServerData._packetProcessor.RegisterNestedType<ItemWorld>();
            ServerData._packetProcessor.SubscribeReusable<CreateCharacterPacket, NetPeer>(OnCreateCharacter);
            SyncStateServer.onUpdateMsec += UpdatePlayers;
            ServerCallBlack.onDisconnectedPlayer += RemovePlayer;
            playersData.worldConfig = new WorldData();
        }

    

        protected override void OnStartServer(NetManager manager)
        {
            playersData.worldConfig = new WorldData();
            playersData.worldConfig.worldConfig = new WorldConfig();
            playersData.worldConfig.Load();

        }

        private void OnCreateCharacter(CreateCharacterPacket arg1, NetPeer arg2)
        {
            PlayerManager player = (PlayerManager)arg2.Tag;
            if (player != null)
            {
                if (player.isNew)
                {
                    CharacterCustomBody customBody = new CharacterCustomBody(arg1.Color_body, arg1.id_pants, arg1.id_body);
                    CharacterCustomHead customHead = new CharacterCustomHead(arg1.Color_beard, arg1.Color_Hair, arg1.Color_eyes, arg1.Color_lips, arg1.id_hair, arg1.id_beard, arg1.id_head, 0);
                    CustomCharacter custom = new CustomCharacter(0, playersData.worldConfig.worldConfig.massa_Character_default, 0, arg1.Gender);
                    EntityManager.AddComponentData(player.entity, custom);
                    EntityManager.AddComponentData(player.entity, customHead);
                    EntityManager.AddComponentData(player.entity, customBody);
                    ConnectionPlayer(player , arg2); 
                }
                else Debug.LogError("[S] Create character isNew false");
            }
            else
                Debug.LogError("[S] Create character null");
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
                    }
                }
                else
                {
                    peer.Send(WritePacket(new PlayerAccountPacket(EUserState.create)), DeliveryMethod.ReliableOrdered);
                    peer.Send(WritePacket(new CharacterDefaultPacket(playersData.worldConfig.worldConfig)), DeliveryMethod.ReliableOrdered);
                    playerServer.isNew = true;
                    return;
                }
                ConnectionPlayer(  playerServer, peer);
            }
            else Debug.LogError(this + " Error peer != null");
        }
        private void ConnectionPlayer(PlayerManager playerServer, NetPeer peer)
        {
            try
            {
                if (!playerServer.isNew)
                {
                    playersData.players.Add(playerServer.id, playerServer); 
                    playerServer.SpawnLoadInfo();
                    LogDev($"[S] Add  player " + playerServer.id);
                    ServerCallBlack.onCreatePlayer?.Invoke(playerServer, false);
                }
                else
                {
                    playersData.worldConfig.usersCount++;
                    playerServer.id = playersData.worldConfig.usersCount;
                    playersData.players.Add(playerServer.id, playerServer);
                    playerServer.Spawn(new Vector3(1685, 25, 911));
                    LogDev($"[S] Add new player " + playerServer.id);
                    ServerCallBlack.onCreatePlayer?.Invoke(playerServer,true);
                }
           
                NetDataWriter writer = new NetDataWriter();
                writer = WritePacket(SendServerInfo(playerServer));

                playersData.onSendServerData?.Invoke(playerServer,writer);
                peer.Send(writer, DeliveryMethod.ReliableOrdered); //Отправка новому игроку данных сервера, игроков, и его данные
                 ServerData._netManager.SendToAll(WritePacket(playerServer.GetJoinedPacket()), DeliveryMethod.ReliableOrdered, peer);//Отправка базовых данных
                 
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[S] [ADDPLAYER]" + ex.Message);
            }
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
        protected void UpdatePlayers(NetDataWriter packet)
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
                    playerDatas.Add(new PlayerData() { Id = playerID.id, steamid = playerID.SteamID, username = playerID.username.ToString() }); 
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
    public class WorldData
    {
        public ushort usersCount { get; set; }
        string name = "/World.dat";
        public WorldConfig worldConfig;

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
            worldConfig = SaveManager.LoadJSON<WorldConfig>($"{ServerManager.manager.serverInfoProxy.serverFolder}/World.config");
            if (worldConfig == null)
            {
                worldConfig = new WorldConfig(); 
                worldConfig.Radius = 250;
                worldConfig.isRandomMassaCharacter = true;
                worldConfig.massa_Character_default = 20;
                worldConfig.id_pants_default_woman= 6;
                worldConfig.id_shirt_default_woman = 5;
                worldConfig.id_pants_default_man = 4; 
                worldConfig.id_shirt_default_man = 3;
           
            }
             
        }
        public void Save()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(usersCount); 
            SaveManager.Save(writer, ServerManager.manager.serverInfoProxy.serverFolder + name);
            SaveManager.SaveJSON(  worldConfig, $"{ ServerManager.manager.serverInfoProxy.serverFolder } /World.config");
        }
    }
   
}
