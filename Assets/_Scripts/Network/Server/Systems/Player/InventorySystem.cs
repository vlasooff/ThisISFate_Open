using Community.Core;
using Community.Core.Serializables;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace Community.Server.Systems
{

    public class InventorySystem : ComponentServer
    {
        private ItemsWorldComponent  inventoryManager; 

        protected override void onStartedServer(NetManager manager)
        {
            inventoryManager = ServerManager.manager.GetComponent<ItemsWorldComponent>(); 
        //    ServerCallBlack.onCreatePlayer += onCreatePlayer;
                SyncStateServer.OnCreatedPlayerResponse   += OnConnectedToServer;
        }

        private void OnConnectedToServer(PlayerManager manager, NetDataWriter writer)
        {
             WritePacket(new WorldItemsPacket() { items = inventoryManager.itemWorlds.ToArray() },writer);
             WritePacket(manager.playerInventory, writer);

        } 
        protected override void OnUpdate()
        { 
        }
        
    }

}