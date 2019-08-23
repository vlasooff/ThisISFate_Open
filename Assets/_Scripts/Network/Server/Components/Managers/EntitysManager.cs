using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Server.Components
{
    public class EntitysManager : MonoBehaviour, IManager
    {
        public Transform SpawnEntity;
        private List<EntityNetManager> entitys = new List<EntityNetManager>();

        public EntityNetManager GetEntity(ushort id)
        {
            return entitys[id];
        }
        public EntityPlayerManager GetEntityPlayer(ushort id)
        {
            return (EntityPlayerManager)entitys[id];
        }
        public void Add(EntityNetManager manager)
        {
            entitys.Add(manager);
        }
        public void Remove(EntityNetManager manager)
        {
            entitys.Remove(manager);
        }
        public ushort IndexOf(EntityNetManager manager)
        {
            return (ushort)entitys.IndexOf(manager);
        }
    }
}
