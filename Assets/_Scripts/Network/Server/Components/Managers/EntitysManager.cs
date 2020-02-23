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
        [SerializeField]
        private List<EntityNetManager> entitys = new List<EntityNetManager>();

        public EntityNetManager GetEntity(ushort id)
        {
            
            if (entitys.ElementAt(id) != null) return entitys.ElementAt(id);
            else Debug.LogError("[S] Enitity  not find " + id);
            return null;
        }
        public EntityPlayerManager GetEntityPlayer(ushort id)
        {
            return (EntityPlayerManager)GetEntity(id);
        }
        public void Add(EntityNetManager manager)
        {
            entitys.Add(manager);
        }
        public void SetIndex(EntityNetManager manager)
        {
            entitys[manager.id] = manager;
        }
        public ushort AddAndID(EntityNetManager manager)
        {
            entitys.Add(manager);
            return (ushort)entitys.IndexOf(manager);
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
