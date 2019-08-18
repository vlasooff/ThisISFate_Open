 
using Community.Core;
using Community.Core.Serializables;
using Community.Other;
using Community.Server.Systems;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.IO;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Components
{

    public class PlayerManager : IPlayerBase
    {
        public ushort id;
        public ulong steamID;
        public string username;
        public string password;
        public readonly NetPeer peer; 
        public Transform transform;  
        public bool isFixed = false;
        public Entity entity;
        public CharacterController controller;
        public bool isNew = false;
        public DateTime createUser;
        PlayerServerData playerdata;
        public PlayerInventory playerInventory;
        public NetDataWriter hash_response_sec;

        public PlayerManager(JoinPacket packet, ushort _id, NetPeer _peer)
        {
            id = _id;
            entity = World.Active.EntityManager.CreateEntity();
           
            World.Active.EntityManager.AddComponentData(entity, new PlayerID(id, packet.steamid, packet.UserName));
            World.Active.EntityManager.SetName(entity, "PlayerServer_" + packet.UserName);
            steamID = packet.steamid;
            username = packet.UserName;
            password = packet.password;
            peer = _peer;
            peer.Tag = this;
            Debug.Log($"[S] Create playermanager ID {id} STEAMID {packet.steamid}");
            SyncStateServer.onPlayerSec += OnUpdState;


        }

        private void OnUpdState()
        {
            if(hash_response_sec.Length > 0)
            { 
                peer.Send(hash_response_sec, DeliveryMethod.ReliableOrdered);
                hash_response_sec.Reset();
            }
        }

        /// <summary>
        /// Загрузка и создание аккаунта
        /// </summary>
        /// <returns>Возвращает новый аккаунт или нет</returns>
        public virtual bool Load()
        {
         SaveManager.LoadJSON<PlayerServerData>($"{ServerManager.manager.serverInfoProxy.serverFolder}/Players/player_{username}/Player_{username}.dat");
            if (playerdata == null)
            {
                SaveManager.CreateServerFolder("Players"); 
                SaveManager.CreateServerFolder($"Players/player_{username}");
                createUser = DateTime.Now; 
                return false;
            }
            else
                password = playerdata.password;

            World.Active.EntityManager.AddComponentData(entity, playerdata.CustomCharacter);
            World.Active.EntityManager.AddComponentData(entity, playerdata.CharacterCustomHead);
            World.Active.EntityManager.AddComponentData(entity, playerdata.CharacterCustomBody);
            return true;

        }
        public virtual void Save()
        {
             
            if (transform != null)
            {
                playerdata = new PlayerServerData() { position = transform.position, rotation = transform.rotation.y, UserName = username, password = password, DataCreateAccount = createUser.ToString() };
                playerdata.CustomCharacter =   World.Active.EntityManager.GetComponentData<CustomCharacter>(entity );
                playerdata.CharacterCustomHead = World.Active.EntityManager.GetComponentData<CharacterCustomHead>(entity);
                playerdata.CharacterCustomBody = World.Active.EntityManager.GetComponentData<CharacterCustomBody>(entity);
                SaveManager.SaveJSON(playerdata, $"{ ServerManager.manager.serverInfoProxy.serverFolder }/Players/player_{username}/Player_{username}.dat");

            }
          
        }
        #region GetState

        public SpawnCurrentPlayer GetSpawnCurrentPlayer()
        {
            SpawnCurrentPlayer currentPlayer = new SpawnCurrentPlayer();
            currentPlayer.username = username;
            currentPlayer.PlayerId = id;
            if (transform == null) Debug.LogError("[S] Player Transform null");
            currentPlayer.Position = transform.position;
            currentPlayer.Rotation = transform.rotation.y;

            currentPlayer.custom = World.Active.EntityManager.GetComponentData<CustomCharacter>(entity);
            currentPlayer.head = World.Active.EntityManager.GetComponentData<CharacterCustomHead>(entity);
            currentPlayer.body = World.Active.EntityManager.GetComponentData<CharacterCustomBody>(entity);
            return currentPlayer;
        }
        public PlayerJoinedPacket GetJoinedPacket()
        {
            PlayerJoinedPacket playerJoined = new PlayerJoinedPacket() { id = id, NewPlayer = isNew, steamid = steamID, UserName = username };
            playerJoined.custom   = World.Active.EntityManager.GetComponentData<CustomCharacter>(entity);
            playerJoined.customHead = World.Active.EntityManager.GetComponentData<CharacterCustomHead>(entity);
            playerJoined.customBody = World.Active.EntityManager.GetComponentData<CharacterCustomBody>(entity);
            playerJoined.player = playerInventory.GetCharacterBody();

            return playerJoined;
        }


        #endregion
        public virtual void SpawnLoadInfo()
        {
            //_buffer.FastClear(); 
            Spawn(playerdata.position,playerdata.rotation);
        }
        public virtual void Spawn(Vector3 position)
        {
            //_buffer.FastClear(); 
            Spawn(position, 0);
        }
        public virtual void Spawn(Vector3 position, float rot)
        {
            //  _buffer.FastClear();
            Quaternion rotation = Quaternion.Euler(0, rot, 0);
            GameObject pl = GameObject.Instantiate(Resources.Load("playerServer"), position, rotation) as GameObject;
            controller = pl.GetComponent<CharacterController>();
            transform = pl.transform; 
            World.Active.EntityManager.AddComponentData(entity, new PlayerMotor(position, rot)); 
            World.Active.EntityManager.AddComponentData(entity, new PlayerRegion(0));
            World.Active.EntityManager.AddComponentData(entity, new PlayerCitizen(0));
        }

        public void StopPlayer()
        {
            Save();
        }
        [System.Serializable]
        public class PlayerServerData
        {
            public float rotation;
            public Vector3 position;
            public string UserName;
            public string DataCreateAccount;
            public string password;
            public CustomCharacter CustomCharacter;
            public CharacterCustomHead CharacterCustomHead;
            public CharacterCustomBody CharacterCustomBody;
        }
    }
    

}
