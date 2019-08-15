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
        private InventoryManager  inventoryManager;
        private PlayersInfoComponent playerManager;

        protected override void onStartedServer(NetManager manager)
        {
            inventoryManager = ServerManager.manager.inventoryManager;
            playerManager = ServerManager.manager.playersData;
        //    ServerCallBlack.onCreatePlayer += onCreatePlayer;
            playerManager.onSendServerData += OnConnectedServer;
        }

        private void OnConnectedServer(PlayerManager manager, NetDataWriter writer)
        {
             WritePacket(new WorldItemsPacket() { items = inventoryManager.itemWorlds.ToArray() },writer);
        }

        

      

        protected override void OnUpdate()
        { 
        }
        
    }

}