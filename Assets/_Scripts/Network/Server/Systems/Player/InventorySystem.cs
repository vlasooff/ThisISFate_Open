using Community.Core;
using Community.Core.Serializables;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Jobs;

namespace Community.Server.Systems
{ 
    public class InventorySystem : ComponentServer
    {
        private ItemsWorldComponent  ItemsGlobal;
        private ServerProxy ServerData => ServerManager.manager.serverProxy;

        protected override void onStartedServer(NetManager manager)
        {
            ItemsGlobal = ServerManager.manager.GetComponent<ItemsWorldComponent>();  
                SyncStateServer.OnCreatedPlayerResponse   += OnConnectedToServer;
            ServerData._packetProcessor.SubscribeReusable<CommandAddItem, NetPeer>(OnAddItemInventory);
        }

        private void OnConnectedToServer(PlayerManager manager, NetDataWriter writer)
        {
            WritePacket(new WorldItemsPacket() { items = ItemsGlobal.itemWorlds.ToArray() }, writer);
            WritePacket(manager.playerInventory, writer);

        }
        #region ItemWorld
        private void SpawnItem(ushort id, Vector3 position,byte rotY)
        {
            ItemWorld item = new ItemWorld(ItemsGlobal.itemWorlds.Count + 1, id, position, rotY);
            ItemsGlobal.itemWorlds.Add(item);
            item.Index = ItemsGlobal.itemWorlds.IndexOf(item);
            ServerData._netManager.SendToAll(WritePacket(new SpawnItemWorldPacket() { index = (ushort)item.Index, id = id}), DeliveryMethod.ReliableOrdered);
        }
        private void RemoveItem(ushort index)
        {
             ItemsGlobal.itemWorlds.RemoveAt(index);
            ServerData._netManager.SendToAll(WritePacket(new DestroyItemWorldPacket() { index = index }),DeliveryMethod.ReliableOrdered);
        }
        #endregion

        private void OnAddItemInventory(CommandAddItem command, NetPeer peer)
        {
             ItemWorld itemWorld =       ItemsGlobal.itemWorlds[command.index];
            if (itemWorld.Index == command.index)
            {
                AddItemInInventory((PlayerManager)peer.Tag, itemWorld.Id);


                RemoveItem((ushort)command.index);
            }
            else Debug.LogError("[S] ONADDITEM Index != null");
        }
        private void AddItemInInventory(PlayerManager manager,ushort id)
        {
            ItemAsset asset = GetItemAsset(id);
            manager.playerInventory.GetMinItems().AddItem(new ItemInventory());
        }
        private ItemAsset GetItemAsset(ushort id)
        {
            return
        }
        protected override void OnUpdate()
        { 
        }
        
    }

}