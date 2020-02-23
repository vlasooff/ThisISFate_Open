using Community.Core.Serializables;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Community.Server.Systems
{ 
    public class MoveSystem : ComponentServer
    {
        ServerProxy serverProxy;
        PlayersInfoComponent playersManager;
        ServerInfoProxy info;
        TransformsPacket state;
        private EntitysManager entitysManager;

        protected override void onStartedServer(NetPacketProcessor _packetProcessor)
        {
            playersManager = ServerManager.manager.playersData;
            serverProxy = ServerManager.manager.serverProxy;
            info = ServerManager.manager.serverInfoProxy;
            base.OnStartRunning();
            serverProxy._packetProcessor.RegisterNestedType<TransformPacket>();
            serverProxy._packetProcessor.SubscribeReusable<PlayerInputPacket>(OnPlayerInput);
            serverProxy._packetProcessor.SubscribeReusable<PacketFixedPosition, NetPeer>(OnPlayerFixed);
            entitysManager = ServerManager.manager.GetManager<EntitysManager>(EManagers.entitysnet);
        }

        private void OnPlayerFixed(PacketFixedPosition obj, NetPeer peer)
        {
            EntityPlayerManager player = (EntityPlayerManager)peer.Tag;
            player.isFixed = true;
        //    Debug.Log("[S] pos: " + player.player.transform.position + " New: " + obj.vector3);
            player.transform.position = obj.vector3;
    //    Debug.Log("[S] pos 2: " + player.player.transform.position);
        }

        private void OnPlayerInput(PlayerInputPacket packet)
        {
            MoveEntity(packet);

        }

        protected override void OnUpdate()
        {
            if (!ServerCallBlack.isServerRun) return;
            Entities.ForEach((ref EntityNetID playerID, ref EntityMotor playerMotor) =>
            {
                EntityNetManager entityNet =  entitysManager.GetEntity(playerID.id);
               if (!entityNet.isFixed)
                    entityNet.controller.Move(playerMotor.movingDir);
            });
            Entities.ForEach((ref PlayerID playerID, ref PlayerMotor playerMotor) =>
            {
                PlayerManager player = playersManager.players[playerID.id];
                if (!player.isFixed)
                    player.controller.Move(playerMotor.movingDir);
            });
        }
        protected virtual void Move(PlayerInputPacket packet)
        {
            PlayerManager player = playersManager.players[packet.Id];
            player.isFixed = false;
            Transform transform = player.transform;
            PlayerMotor playerMotor = EntityManager.GetComponentData<PlayerMotor>(player.entity);
            var movingDir = Vector3.zero;

            if (packet.Keys[0] ^ packet.Keys[1])
            {
                movingDir.z = packet.Keys[0] ? +1 : -1;
            }

            if (packet.Keys[2] ^ packet.Keys[3])
            {
                movingDir.x = packet.Keys[3] ? +1 : -1;
            }

            if (movingDir.x != 0 || movingDir.z != 0)
            {
                movingDir = Vector3.Normalize(Quaternion.Euler(0, packet.Rotation, 0) * movingDir);
            }
            playerMotor._speed = Mathf.Clamp(packet.currentSpeed, 0, 5f);
            float speed = playerMotor._speed;
            if (packet.Keys[4])
            {
                speed = 5;
            }
            if (packet.Keys[5] && player.controller.isGrounded)
            {
                movingDir.y += playerMotor.jumpForge;

            }
            else
            {
                if (!player.controller.isGrounded)
                {

                    movingDir.y -= 2;
                }
            }


            // set local rotation 

            transform.localRotation = Quaternion.Euler(0, packet.Rotation, 0);
            movingDir *= speed * Time.deltaTime;
            playerMotor.movingDir = movingDir;
            playerMotor._rotation = packet.Rotation;
            EntityManager.SetComponentData<PlayerMotor>(player.entity, playerMotor);

        }
        protected virtual void MoveEntity(PlayerInputPacket packet)
        { 
            EntityPlayerManager player =entitysManager.GetEntityPlayer(packet.Id);
            player.isFixed = false;
            Transform transform = player.transform;
            EntityMotor playerMotor = EntityManager.GetComponentData<EntityMotor>(player.entityWorld);
       
            NativeArray<bool> keys = new NativeArray<bool>(packet.Keys, Allocator.TempJob);
            NativeArray<Vector3> moving = new NativeArray<Vector3>(new Vector3[1] { Vector3.zero }, Allocator.TempJob);
            NativeArray<float> rot = new NativeArray<float>(new float[2] { packet.Rotation, packet.currentSpeed}, Allocator.TempJob);
            JobHandle handle = new JobHandle();
            JobEntityMove job = new JobEntityMove()
            {
                deltaTime = Time.deltaTime,
                keys = keys,
                moving  = moving,
                rotation = rot
            }; 
            handle = job.Schedule();
            handle.Complete();
            if (handle.IsCompleted)
            {

                playerMotor.movingDir = moving[0];
                moving.Dispose();
                keys.Dispose();
                rot.Dispose();
                // set local rotation 
                if (packet.Keys[5] && player.controller.isGrounded)
                {
                    playerMotor.movingDir.y += playerMotor.jumpForge;

                }
                else
                {
                    if (!player.controller.isGrounded)
                    {

                        playerMotor.movingDir.y -= 2;
                    }
                } 
            }
            transform.localRotation = Quaternion.Euler(0, packet.Rotation, 0); 
            playerMotor._rotation = packet.Rotation;
            EntityManager.SetComponentData(player.entityWorld, playerMotor);
          
        }
    }
    [BurstCompile]
    public struct JobEntityMove : IJob
    {
        public NativeArray<Vector3> moving;
    
        public NativeArray<bool> keys;
 
        public NativeArray<float> rotation;
 
        public float deltaTime;
        public void Execute()
        {
            var movingDir = Vector3.zero;

            if (keys[0] ^ keys[1])
            {
                movingDir.z = keys[0] ? +1 : -1;
            }

            if (keys[2] ^ keys[3])
            {
                movingDir.x = keys[3] ? +1 : -1;
            }

            if (movingDir.x != 0 || movingDir.z != 0)
            {
                movingDir = Vector3.Normalize(Quaternion.Euler(0,  rotation[0], 0) * movingDir);
            }
            rotation[1] = Mathf.Clamp(rotation[1], 0, 5f); //speed
            float speed = rotation[1];
            if (keys[4])
            {
                speed = 5;
            }
            movingDir *= speed *  deltaTime;
            moving[0] = movingDir;

        }
    }
}
