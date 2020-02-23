using System;
using Community.Core;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Community.Server.Systems
{
    public class SyncEntitysSystem : ComponentServer
    {
        private EntitysManager entitysManager;

        protected override void OnStartServer(NetPacketProcessor _packetProcessor)
        {
            SyncStateServer.onPlayerSec += OnSyncForPlayer;
            entitysManager = ServerManager.manager.GetManager<EntitysManager>(EManagers.entitysnet);
            _packetProcessor.RegisterNestedType<UpdatePlayerPacket>();
            EntitysSystem.onSpawnEntity += OnSpawnEntityOrPlayer;
        }

        private void OnSpawnEntityOrPlayer(EntityNetManager manager)
        {
            EntityPlayerManager entityPlayer = (EntityPlayerManager)manager;
            if (entityPlayer != null)
            {
                entityPlayer.peer.Send(WritePacket(
                SendServerInfo(entityPlayer)),DeliveryMethod.ReliableOrdered);
            }
            else Debug.LogError("[S] OnSpawnEntity не игрок или не найден.");

        }

        private void OnSyncForPlayer()
        {
            Entities.ForEach((ref EntityNetID entityID, ref EntityNetPlayer pl) =>
                {
                    EntityPlayerManager player = entitysManager.GetEntityPlayer(entityID.id);
                    player.peer.Send(WritePacket(GetUpdatePlayers(entityID.id)), DeliveryMethod.ReliableOrdered);
                });
        }

        protected override void OnUpdate()
        {
            if(ServerCallBlack.isServerRun)
            {
                Entities.ForEach((ref EntityNetID EntityID, ref EntityPosition pos) =>
                {
                    EntityNetManager player = entitysManager.GetEntity(EntityID.id);
                    if (player != null)
                        pos.position = player.transform.position;
                });
            }
        }

        private UpdatePlayersPacket GetUpdatePlayers(ushort idCurrentPlayer)
        {
            UpdatePlayersPacket playersPacket = new UpdatePlayersPacket();
            playersPacket.ServerTick = ServerInfoProxy._serverTick;

            NativeList<UpdatePlayerPacket> packets = new NativeList<UpdatePlayerPacket>(Allocator.TempJob);
            JobHandle handle = new JobHandle();
            JobGetPlayer job = new JobGetPlayer
            {
                idCurrent = idCurrentPlayer,
                playerPackets = packets
            };

            handle = job.Schedule(this, handle);
            handle.Complete();
            if (handle.IsCompleted)
            {
                playersPacket.updatePlayers = packets.ToArray();
                packets.Dispose();
            }
            return playersPacket;
        }
        protected virtual ServerInfoConnect SendServerInfo(EntityPlayerManager playerManager)
        {
            ServerInfoConnect inform = new ServerInfoConnect();
            inform.title = "[RUS] SERVER TEST";
            inform.currentPlayer = playerManager.GetSpawnCurrentPlayer();
            NativeList<PlayerData> packets = new NativeList<PlayerData>(Allocator.TempJob);
            JobHandle handle = new JobHandle();
            JobGetPlayerData job = new JobGetPlayerData
            {
                idCurrent = playerManager.id,
                playerDatas = packets
            };

            handle = job.Schedule(this, handle);
            handle.Complete();
            if (handle.IsCompleted)
            {
                 inform.players = packets.ToArray();
                packets.Dispose();
            }
             

            Debug.Log("[S] Server info connect and remote players :" + inform.players.Length);
            return inform;
        }

        [BurstCompile]
        public struct JobGetPlayer : IJobForEach<EntityNetID, EntityMotor, EntityPosition>
        {
            [ReadOnly]
            public NativeList<UpdatePlayerPacket> playerPackets;
            public ushort idCurrent;
            public void Execute(ref EntityNetID entity, ref EntityMotor motor, ref EntityPosition pos)
            {

                if (idCurrent != entity.id)
                {
                    playerPackets.Add(new UpdatePlayerPacket(entity.id, pos.position, new Vector2(motor.movingDir.x, motor.movingDir.z), motor._rotation, motor._speed));
                }
            }
        } 
        public struct JobGetPlayerData : IJobForEach<EntityNetID, EntitySteamID, EntityName>
        {
            [ReadOnly]
            public NativeList<PlayerData> playerDatas;
            public ushort idCurrent;
            public void Execute(ref EntityNetID entity, ref EntitySteamID steamID, ref EntityName user )
            {
                if (idCurrent != entity.id)
                {
                    playerDatas.Add(new PlayerData() { Id = entity.id, steamid = steamID.id, username = new NativeString64(user.username.ToString()) });
                }
            }
        }
    }
}
