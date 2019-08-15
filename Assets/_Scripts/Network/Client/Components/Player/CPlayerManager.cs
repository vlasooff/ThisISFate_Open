using Community.Client.Components;
using Community.Core;
using Community.Core.Serializables;
using Community.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Client.Components
{
    [Serializable]
    public class PlayerManager
    {
        public Entity entity;
        public ushort id => baseData.id;
        public PlayerBase baseData;
        public Transform transform;
    }
    [Serializable]
    public class CPlayerManager : PlayerManager
    {
        public PlayerClient player;
        public Animator animator;
        public PlayerIK movement;
        public CharacterController controller;
        public CharacterCustom custom;
         
        public float _speed = 2;
        public  float jumpForge = 5;
        public ushort idRegion;
      
        public CPlayerManager(SpawnCurrentPlayer packet,ulong csteamid)
        {
            baseData = new PlayerBase(packet.username, csteamid, packet.PlayerId);
            entity = World.Active.EntityManager.CreateEntity();
            World.Active.EntityManager.AddComponentData(entity, new PlayerID(id));
            World.Active.EntityManager.AddComponentData(entity, new PlayerInfo(csteamid, packet.username));
            World.Active.EntityManager.AddComponentData(entity, packet.custom);
            World.Active.EntityManager.AddComponentData(entity, packet.head);
            World.Active.EntityManager.AddComponentData(entity, packet.body);
            Spawn(packet.Position, packet.Rotation);
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
            Debug.Log("[C] Spawn player"); 
                GameObject pl = GameObject.Instantiate(Resources.Load("player"), position, rotation) as GameObject;
            player = pl.GetComponent<PlayerClient>();
            transform = pl.GetComponent<Transform>();
           custom = pl.GetComponent<CharacterCustom>();
            controller = pl.GetComponent<CharacterController>();
            animator = pl.GetComponent<Animator>();
            movement = pl.GetComponent<PlayerIK>();
            World.Active.EntityManager.AddComponentData(entity, new PlayerMotor()); 
             player.cameraTransform = Camera.main.transform;
            if (player)
                if (player.cameraTransform)
                {
                    player.cameraTransform.SetParent(player.cameraParrent);
                    player.cameraTransform.rotation = Quaternion.identity;
                    player.cameraTransform.localPosition = Vector3.zero;
                }
                else Debug.LogError("camera null");
            else Debug.LogError("Player null");
        }
    }
}
