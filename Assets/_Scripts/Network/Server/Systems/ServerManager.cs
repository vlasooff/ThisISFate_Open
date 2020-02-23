using Community.Other;
using Community.Server.Components;
using Community.Server.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Server
{
    public class ServerManager : Managers 
    {
        //Сервер
        public ServerProxy serverProxy;
        public ServerInfoProxy serverInfoProxy;
        //Чат
        public ChatComponent chatData;
        //Система Игроков
        public PlayersInfoComponent playersData;
        public RegionsComponent regionsComponent; 
        public static ServerManager manager;
        private IManager[] managers = new IManager[2];
        public static Entity EntityServer;
        private void Awake()
        {
            managers[0] = GetComponentInChildren<EntitysManager>();
            managers[1] = GetComponentInChildren<ItemsWorldComponent>();
            if (managers[0] == null) Debug.LogError("EROR");
            manager = this;
            EntityServer =  World.Active.EntityManager.CreateEntity();
            AddManagers();
             DontDestroyOnLoad(this);
            if (!serverProxy) Debug.Log("[S] ServerManager " +
                "null!");
        }
        public T GetManager<T>(EManagers id)
        {

            return (T)managers[(int)id];


        }
        private void AddManagers()
        {
           // AddComponent(new ItemsWorldComponent() { itemWorlds = new Unity.Collections.NativeList<Core.ItemWorld>(Unity.Collections.Allocator.None) }); ;
        }
        public static void AddComponent<T>(T data) where T :struct, IComponentData
        {
            World.Active.EntityManager.AddComponentData(EntityServer, data);
        }
     
    }
    public enum EManagers : int
    {
        entitysnet,ItemsWorld
    }
    public interface IManager
    {
        
    }
     
}
