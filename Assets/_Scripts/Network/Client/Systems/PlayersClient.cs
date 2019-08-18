
using System;
using System.Collections.Generic;
using Community.Client.Components;
using Community.Core;
using Community.Core.Serializables;
using Network;
using Network.Core;
using UnityEngine;

namespace Community.Client
{
    public delegate void OnSpawnCurrentPlayer(CPlayerManager playerManager);
    public class PlayersClient : ComponentClient
    {
        PlayersManager playersData;
        private ClientDataSteam clientDataSteam;
        public static OnSpawnCurrentPlayer OnSpawnCurrentPlayer;
        private CustomManager custom;
        private readonly List<UpdatePlayersPacket> _buffer = new List<UpdatePlayersPacket>(30);
        protected override void OnStartedClient()
        {
            playersData = ClientManager.manager.playersManager;
            clientDataSteam = ClientManager.manager.clientDataSteam;
            ClientData._packetProcessor.RegisterNestedType<SpawnCurrentPlayer>();
            ClientData._packetProcessor.RegisterNestedType<RemotePlayersData>();
            ClientData._packetProcessor.RegisterNestedType<UpdatePlayerPacket>();
            ClientData._packetProcessor.RegisterNestedType<CustomCharacter>();
            ClientData._packetProcessor.RegisterNestedType<CharacterCustomHead>();
            ClientData._packetProcessor.RegisterNestedType<RemoteCustomPlayer>();
            ClientData._packetProcessor.RegisterNestedType<CharacterCustomBody>();
            ClientData._packetProcessor.RegisterNestedType<PlayerData>();
            ClientData._packetProcessor.SubscribeReusable<PlayerJoinedPacket>(OnPlayerJoined);
            ClientData._packetProcessor.SubscribeReusable<PlayerLeavedPacket>(OnPlayerLeaved); 
            ClientData._packetProcessor.SubscribeReusable<ServerInfoConnect>(OnServerData);
            ClientData._packetProcessor.SubscribeReusable<LoadingPlayerPacket>(OnLoadingPlayer);
            ClientData._packetProcessor.SubscribeReusable<UpdatePlayersPacket>(OnUpdateStatePlayer);
            ClientData._packetProcessor.SubscribeReusable<UnloadingPlayerPacket>(UnloadingPlayer);
            ClientData._packetProcessor.SubscribeReusable<PlayerAccountPacket>(AccauntState);
        }

        private void AccauntState(PlayerAccountPacket obj)
        {

            UIManager uimanager =   ClientManager.manager.UVManager;
            EUserState state = (EUserState)obj.state;
            if (state == EUserState.login)
            {
                uimanager.OnLogin();
            }
            else
            {
                if (state == EUserState.create) uimanager.OnCreate();
                else uimanager.OnProblem();
            }
        }

        private void OnUpdateStatePlayer(UpdatePlayersPacket obj)
        { 
            UpdatePlayersPacket Last = obj;
            if (_buffer.Count != 0)
                 Last = _buffer[_buffer.Count -1]; 
            _buffer.Add(obj); 
            if (Last.updatePlayers != null)
                for (int i = 0; i < Last.updatePlayers.Length; i++)
                {
                    if (playersData.players.ContainsKey(Last.updatePlayers[i].Id))
                    {
                        RPlayerManager playerManager =
                        (RPlayerManager)playersData.players[Last.updatePlayers[i].Id];
                        RemotePlayerUpdate(obj.updatePlayers[i], playerManager);
                        _buffer.Remove(Last);
                    }
                    else Debug.Log("[C] players not find " + Last.updatePlayers[i].Id);
                }
            else Debug.LogError("[C] last players == null");
        }

     
        public void RemotePlayerUpdate(UpdatePlayerPacket playerPacket, RPlayerManager manager)
        {
            if (!manager.IsSpawn)
            {
             
                manager.Spawn(playerPacket.position, playerPacket.rotation);
            }
            else
            {
                RPlayerMotor motor = new RPlayerMotor();

                motor.newPosition = playerPacket.position ;

                motor.newRotation = playerPacket.rotation;
                motor.movingDir = playerPacket.animation;
                motor.speed = playerPacket.speed;
                EntityManager.SetComponentData(manager.entity, motor);
            }  
        }
        private void OnLoadingPlayer(LoadingPlayerPacket Packet)
        {
            SpawnCurrentPlayer state = Packet.playerState;
            if (Packet.PlayerId != playersData._clientPlayer.id)
            {
                RPlayerManager remote = (RPlayerManager)GetPlayer(Packet.PlayerId);
                if (remote != null)
                {

                }
                else
                {
                    Debug.LogError("[C] Player !ContainsKey");
                }
            }
        } 
        private void UnloadingPlayer(UnloadingPlayerPacket Packet)
        {
            if (Packet.Id != playersData._clientPlayer.id)
            {
                Debug.Log("[C] Player  remove: " +
                playersData.players.Remove(Packet.Id));
            }
        }

        private void OnServerData(ServerInfoConnect server)
        {
            Debug.Log($"[C]   OnServerDatar");
            if (playersData._clientPlayer.transform == null)
            {
                playersData._clientPlayer = new CPlayerManager(server.currentPlayer, ClientManager.manager.clientDataSteam.steamid);
                 
                 
                OnSpawnCurrentPlayer?.Invoke(playersData._clientPlayer);
            }
            for (int i = 0; i < server.players.Length; i++)
            {

                RPlayerManager player  = new RPlayerManager(server.players[i]);
                Debug.Log("[C] Add list remote player " + player.id);
                playersData.players.Add(player.id, player);
            }
            Debug.Log($"[C] Remote count {playersData.players.Count}| ServerData players: {server.players.Length}" );
        }
       
        private void OnPlayerJoined(PlayerJoinedPacket Packet)
        {
            Debug.Log($"[C] Player joined: {Packet.UserName}");
            playersData.players.Add(Packet.id, new RPlayerManager(Packet));
        }
        private void OnPlayerLeaved(PlayerLeavedPacket Packet)
        {
            Debug.Log($"[C] Player disconected: {Packet.Id} : {playersData.players.Remove(Packet.Id)}");
        }
        private PlayerManager GetPlayer(ushort id)
        {
            return playersData.players[id];
        }
        private bool IsPlayer(ushort id)
        {
            return playersData.players.ContainsKey(id);
        }



        /// <summary>
        /// Добавляет подключившихся игроков
        /// </summary>
        /// <param name="Packet"></param>
      


        protected override void OnUpdate()
        {

        }

    }
}
