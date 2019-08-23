using Community.Server.Systems;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Components 
{
    [System.Serializable]
    public class EntityNetManager 
    {
        public readonly Entity entityWorld;
        public readonly ushort id;
        public CharacterController controller;
        public PlayerInventory playerInventory;
        public Transform transform;
        public EntityNetManager(ushort idEntity,Entity entity )
        {
            entityWorld = entity;
            id = idEntity;
        }
        public   virtual EntityData GetSave()
        {
            return new EntityData(transform.position, transform.rotation.y);
        }
        public virtual void DestroyEntity()
        {

        }
    }
}
