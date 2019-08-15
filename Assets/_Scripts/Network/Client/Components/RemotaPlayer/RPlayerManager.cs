using Community.Core.Serializables;
using Community.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Entities;
using Community.Core;

namespace Community.Client.Components 
{
  

    [Serializable]
    public class RPlayerManager : PlayerManager
    {  
     
        public Animator animator;
        public CharacterCustom custom; 

        public RPlayerManager(PlayerJoinedPacket packet)
        {
            baseData = new PlayerBase(packet.UserName, packet.steamid, packet.id);
            entity = World.Active.EntityManager.CreateEntity();
            World.Active.EntityManager.AddComponentData(entity, new RPlayerID(id));
            World.Active.EntityManager.AddComponentData(entity, new PlayerInfo(packet.steamid,packet.UserName));
            World.Active.EntityManager.AddComponentData(entity, packet.custom);
            World.Active.EntityManager.AddComponentData(entity, packet.customHead);
            World.Active.EntityManager.AddComponentData(entity, packet.customBody);
        }
        public RPlayerManager(PlayerData packet)
        {
            baseData = new PlayerBase(packet.username, packet.steamid, packet.Id);
        }

        public bool IsSpawn;

        public virtual void Spawn(Vector3 position)
        {
            //_buffer.FastClear(); 
            Spawn(position, 0);
        } 
        public virtual void Spawn(Vector3 position, float rot)
        {
            IsSpawn = true;
            //  _buffer.FastClear();
            Quaternion rotation = Quaternion.Euler(0, rot, 0);
            Debug.Log("[C] Spawn remote player");
            GameObject pl = GameObject.Instantiate(Resources.Load("playerRemote"), position, rotation) as GameObject; 
            transform = pl.GetComponent<Transform>();
            animator= pl.GetComponent<Animator>();
            custom = pl.GetComponent<CharacterCustom>();
            World.Active.EntityManager.AddComponentData(entity, new RPlayerMotor(Vector3.zero,position,rot));
            

        }
    }
}
