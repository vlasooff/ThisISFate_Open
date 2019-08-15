using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Other;
using LiteNetLib;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Systems
{
    public class ObjectSystem : ComponentServer
    {
       
        protected override void onStartedServer(NetManager manager)
        {
            return;
            base.onStartedServer(manager);
            WorldObjects objects =  SaveManager.LoadJSON<WorldObjects>($"{ServerManager.manager.serverInfoProxy.serverFolder}/WorldServer.dat");
            for (int i = 0; i < objects.objectServerWorlds.Length; i++)
            {
                SpawnObject(objects.objectServerWorlds[i]);
            }
            SpawnObject(new ObjectServerWorld { id = 0, position = Vector3.zero, rotation = Quaternion.identity });
            Debug.Log("[S] Spawn server object");
        }
        private void SpawnObject(ObjectServerWorld component)
        {
            Entity obj  = EntityManager.CreateEntity();
            EntityManager.AddComponentData(obj, component); 
        }
        protected override void OnShutdown()
        {
            base.OnShutdown();
            List<ObjectServerWorld> objectServers = new List<ObjectServerWorld>();
            Entities.ForEach((ref ObjectServerWorld obj) =>
            {
                objectServers.Add(obj);
            });
            SaveManager.SaveJSON(new WorldObjects() { objectServerWorlds = objectServers.ToArray() }, $"{ServerManager.manager.serverInfoProxy.serverFolder}/WorldServer.dat");
        }
        protected override void OnUpdate()
        { 
           
        }
    }
    [System.Serializable]
    public class WorldObjects
    {
        public ObjectServerWorld[] objectServerWorlds;
    }
   
    [System.Serializable]
    public struct ObjectServerWorld : IComponentData
    {
        public Vector3 position;
        public Quaternion rotation;
        public ushort id;
    }
}
