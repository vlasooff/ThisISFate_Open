using Community.Core;
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
    public class EntityPlayerManager : EntityNetManager
    {
        public readonly NetPeer peer;
        public EntityPlayerManager(ushort id,Entity entity, NetPeer netPeer) : base(  id,entity)
        {
            peer = netPeer;
        }
     
        public SpawnCurrentPlayer GetSpawnCurrentPlayer()
        {
            SpawnCurrentPlayer currentPlayer = new SpawnCurrentPlayer();
            currentPlayer.username = username;
            currentPlayer.PlayerId = id;
            if (transform == null) Debug.LogError("[S] Player Transform null");
            currentPlayer.Position = World.Active.EntityManager.GetComponentData<EntityPosition>(entityWorld).position;
            currentPlayer.Rotation = transform.rotation.y;

            currentPlayer.custom = World.Active.EntityManager.GetComponentData<CustomCharacter>(entityWorld);
            currentPlayer.head = World.Active.EntityManager.GetComponentData<CharacterCustomHead>(entityWorld);
            currentPlayer.body = World.Active.EntityManager.GetComponentData<CharacterCustomBody>(entityWorld);
            return currentPlayer;
        }
    }
}
