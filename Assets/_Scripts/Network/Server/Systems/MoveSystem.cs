using Community.Core.Serializables;
using Community.Server.Components;
using LiteNetLib;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Systems
{
    [UpdateAfter(typeof(ServerSystem))]
    public class MoveSystem : ComponentServer
    {
        ServerProxy serverProxy;
        PlayersInfoComponent playersManager;
        ServerInfoProxy info;
        TransformsPacket state;
        protected override void onStartedServer(NetManager manager)
        {
            playersManager = ServerManager.manager.playersData;
            serverProxy = ServerManager.manager.serverProxy;
            info = ServerManager.manager.serverInfoProxy;
            base.OnStartRunning();
            serverProxy._packetProcessor.RegisterNestedType<TransformPacket>();
            serverProxy._packetProcessor.SubscribeReusable<PlayerInputPacket>(OnPlayerInput);
            serverProxy._packetProcessor.SubscribeReusable<PacketFixedPosition, NetPeer>(OnPlayerFixed);
        }

        private void OnPlayerFixed(PacketFixedPosition obj, NetPeer peer)
        {
            PlayerManager player = (PlayerManager)peer.Tag;
            player.isFixed = true;
        //    Debug.Log("[S] pos: " + player.player.transform.position + " New: " + obj.vector3);
            player.transform.position = obj.vector3;
    //    Debug.Log("[S] pos 2: " + player.player.transform.position);
        }

        private void OnPlayerInput(PlayerInputPacket packet)
        {
            Move(packet);
        }

        protected override void OnUpdate()
        {
            if (!ServerCallBlack.isServerRun) return;
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
    }
}
