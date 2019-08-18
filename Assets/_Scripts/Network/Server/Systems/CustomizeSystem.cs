using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Core;
using Community.Core.Serializables;
using Community.Other;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Systems
{
    public delegate void OnCreateCharacter(PlayerManager manager);
    public class CustomizeSystem : ComponentServer
    {
        public static OnCreateCharacter onCreateCharacter;
        private CharacterConfig config;
        private ServerProxy ServerData => ServerManager.manager.serverProxy;
        protected override void OnStartServer(NetManager manager)
        {
            base.OnStartServer(manager);
            SyncStateServer.OnConnectResponse += OnConnResponse;
            ServerData._packetProcessor.RegisterNestedType<RemoteCustomPlayer>();  
            ServerData._packetProcessor.SubscribeReusable<CreateCharacterPacket, NetPeer>(OnCreateCharacter);
            config = new CharacterConfig();
        }

     

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            ServerData._packetProcessor.RemoveSubscription<CreateCharacterPacket>();
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
                    CustomCharacter custom = new CustomCharacter(0, config.massa_Character_default, 0, arg1.Gender);
                    EntityManager.AddComponentData(player.entity, custom);
                    EntityManager.AddComponentData(player.entity, customHead);
                    EntityManager.AddComponentData(player.entity, customBody);
                }
                else Debug.LogError("[S] Create character isNew false");
            }
            else
                Debug.LogError("[S] Create character null");
            onCreateCharacter?.Invoke(player);
        }

        private void OnConnResponse(PlayerManager manager, NetDataWriter packet)
        { 
            manager.peer.Send(WritePacket(config.GetDefaultPacket()), DeliveryMethod.ReliableOrdered);
        }

        protected override void OnUpdate()
        {

        }
    }
    public class CharacterConfig
    {
        public ushort Radius;
        public bool isRandomMassaCharacter;
        public byte massa_Character_default;
        public ushort id_shirt_default_man;
        public ushort id_pants_default_man;
        public ushort id_shirt_default_woman;
        public ushort id_pants_default_woman;

        public CharacterConfig()
        {
            Load();
        }
        public CharacterDefaultPacket GetDefaultPacket()
        {
            CharacterDefaultPacket defaultPacket = new CharacterDefaultPacket();
            if ( isRandomMassaCharacter) defaultPacket.massa = (byte)UnityEngine.Random.Range(0, 100);
            else
                defaultPacket.massa =  massa_Character_default;
            defaultPacket.id_pants_man =  id_pants_default_man;
            defaultPacket.id_shirt_man =  id_shirt_default_man;
            defaultPacket.id_shirt_F =  id_shirt_default_man;
            defaultPacket.id_pants_F =  id_pants_default_man;
            return defaultPacket;
        }
        public void Load()
        {
            CharacterConfig newConfig = SaveManager.LoadJSON<CharacterConfig>($"{ServerManager.manager.serverInfoProxy.serverFolder}/World.config");
            if (newConfig == null)
            {
                Radius = 250;
                isRandomMassaCharacter = true;
                massa_Character_default = 20;
                id_pants_default_woman = 6;
                id_shirt_default_woman = 5;
                id_pants_default_man = 4;
                id_shirt_default_man = 3;

            }
            else
            {
                var the = this;
                the = newConfig;
            }
        }
        public void Save()
        { 
            SaveManager.SaveJSON(this, $"{ ServerManager.manager.serverInfoProxy.serverFolder } /Character.config");
        }
    }
}
