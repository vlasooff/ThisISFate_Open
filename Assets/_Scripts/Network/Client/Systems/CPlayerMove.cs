using Community.Client;
using Community.Client.Components;
using Community.Core.Serializables;
using Network;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace Component.Client.Systems
{
    public class CPlayerMove : ComponentClient
    {
        CPlayerManager client => players._clientPlayer;
        ClientData server;
        PlayersManager players;
        PlayerInputPacket buffer = new PlayerInputPacket();
        float outTime = 0;
        private float outTime2;

        protected override void OnStartedClient()
        {
            ClientData._packetProcessor.RegisterNestedType<TransformPacket>(); 

        }



        protected override void OnConnectedPlayer()
        {
            server = ClientManager.manager.clientData;
            players = ClientManager.manager.playersManager;
        }

        

        PlayerInputPacket PollKeys(bool mouse, PlayerMotor motor)
        {
            PlayerInputPacket packet = new PlayerInputPacket();
            packet.Keys = new bool[8];
            packet.Keys[0] = Input.GetKey(KeyCode.W);
            packet.Keys[1] = Input.GetKey(KeyCode.S);
            packet.Keys[2] = Input.GetKey(KeyCode.A);
            packet.Keys[3] = Input.GetKey(KeyCode.D);
            packet.Keys[4] = Input.GetKey(KeyCode.LeftShift);
            packet.Keys[5] = Input.GetKey(KeyCode.Space);
            packet.Keys[6] = Input.GetMouseButton(1);
            packet.Keys[7] = Input.GetMouseButton(0);
            packet.ServerTick = ClientCallBlack.ServerTick;
            packet.Id = client.id;
            if (Input.GetKey(KeyCode.LeftAlt))
            {

                client._speed += Input.GetAxis("Mouse ScrollWheel");
                client._speed = Mathf.Clamp(client._speed, 2, 5f);
            }
            packet.currentSpeed = client._speed;
            if (mouse)
            {
                motor.rotation += (Input.GetAxisRaw("Mouse X") * 2);
               client.movement.MouseY -= (Input.GetAxisRaw("Mouse Y") * 2);
                client.movement.MouseY = Mathf.Clamp(client.movement.MouseY, -35f, 35f);
                packet.Rotation = motor.rotation;
                packet.Rotation %= 360f;
                EntityManager.SetComponentData(client.entity, motor);
            }
            return packet;
        }
        protected virtual void SimulationMove(PlayerInputPacket packet, PlayerMotor motor)
        {
            Transform transform = client.transform;
            var moving = false;
            var movingDir = Vector3.zero;

            if (packet.Keys[0] ^ packet.Keys[1])
            {
                movingDir.z = packet.Keys[0] ? +1 : -1;
            }

            if (packet.Keys[2] ^ packet.Keys[3])
            {
                movingDir.x = packet.Keys[3] ? +1 : -1;
            }
            client.animator.SetFloat("speed", client._speed);
            client.animator.SetFloat("X", movingDir.z);
            client.animator.SetFloat("Z", movingDir.x);
            //client.animator.SetFloat("mouse", client.movement.MouseY);
            if (movingDir.x != 0 || movingDir.z != 0)
            {
                moving = true;
                movingDir = Vector3.Normalize(Quaternion.Euler(0, packet.Rotation, 0) * movingDir);
            }
            if (packet.Keys[4])
            {
                client._speed = 5;
            }
            else if (client._speed == 5) client._speed = 2;
            if (packet.Keys[5])
            {
                client.animator.SetBool("Jump", true);
                movingDir.y += client.jumpForge;
            }
            else
            {
                client.animator.SetBool("Jump", false);
                if (!client.controller.isGrounded)
                {

                    movingDir.y -= 2;
                }
            }
            // set local rotation 
            motor.movingDir = movingDir;
            client.player.cameraTransform.localRotation = Quaternion.Euler(client.movement.MouseY / 2, 0, 0);
            transform.localRotation = Quaternion.Euler(0, packet.Rotation, 0);
            if (moving)
            {
                motor.movingDir *= client._speed * Time.deltaTime;
            }
            client.controller.Move(motor.movingDir);
            // done 
        }
        private void UpdateRemote(RPlayerMotor player, RPlayerID playerID)
        {
            RPlayerManager manager = (RPlayerManager)players.players[playerID.id];
            Vector3 worldDeltaPosition = player.newPosition - manager.transform.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot(manager.transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(manager.transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dx, dy);

            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            player.smoothDeltaPosition = Vector2.Lerp(player.smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if time advances
            if (Time.deltaTime > 1e-5f)
                player.velocity = player.smoothDeltaPosition / Time.deltaTime;

            // bool shouldMove = player.player.velocity.magnitude > 0.5f && 0.2f > 0.2f;

            if (worldDeltaPosition.magnitude > 0.2f)//0.2f radius player
                manager.transform.position = player.newPosition - 0.9f * worldDeltaPosition;
            // Update animation parameters

            manager.transform.position = Vector3.Lerp(manager.transform.position, player.newPosition, 0.2f);

            manager.transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(manager.transform.localRotation.y, player.newRotation, 0.2f), 0);
            manager.animator.SetFloat("speed", player.speed);
            manager.animator.SetFloat("X", player.velocity.x);
            manager.animator.SetFloat("Z", player.velocity.y);
            EntityManager.SetComponentData(manager.entity, player);
        }

        protected override void OnUpdate()
        {
            if (ClientCallBlack.isConnected)
            {
                Entities.ForEach((ref PlayerMotor motor) =>
                {

                    PlayerInputPacket packet = PollKeys(true, motor);
                    SimulationMove(packet, motor);
                    if ((Time.time - outTime2) > 1.0)
                    {
                        PacketFixedPosition packetFixed = new PacketFixedPosition();
                        packetFixed.vector3 = client.transform.position;
                        packetFixed.ServerTick = ClientCallBlack.ServerTick;
                        outTime2 = Time.time;
                        server.SendPacket(packetFixed, LiteNetLib.DeliveryMethod.ReliableOrdered);

                    }
                    else
                    if ((Time.time - outTime) > 0.01)
                    {
                        outTime = Time.time;
                        server.SendPacket(packet, LiteNetLib.DeliveryMethod.ReliableOrdered);

                    }

                });
                Entities.ForEach((ref RPlayerID playerID, ref RPlayerMotor playerMotor) =>
                {

                    UpdateRemote(playerMotor, playerID); 
                });

            } 
          

     
        }
      

    }
}

