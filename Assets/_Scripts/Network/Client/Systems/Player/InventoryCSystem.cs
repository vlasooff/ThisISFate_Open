﻿using System;
using Community.Client.Components;
using Community.Core;
using Community.Core.Serializables;
using LiteNetLib.Utils;
using Network;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Community.Client.Systems
{
    public class InventoryCSystem : ComponentClient
    {
        InventoryCManager InventoryCManager;
        CustomManager customManager;
        ClientData clientData;
        public static InventoryCSystem instance;

        Entity enitity;
        protected override void OnStartClient()
        {
            base.OnStartClient();


            ClientData._packetProcessor.RegisterNestedType<InventoryPlayer>();
            ClientData._packetProcessor.RegisterNestedType<ItemWorld>(); 
            ClientData._packetProcessor.SubscribeReusable<WorldItemsPacket>(OnWorldItems);
            ClientData._packetProcessor.SubscribeReusable<InventoryItemsPacket>(OnItemsPacket);
            ClientData._packetProcessor.SubscribeReusable<CharacterDefaultPacket>(OnDefaultCharacter);
            enitity = ClientManager.manager.playersManager._clientPlayer.entity;
            InventoryCManager = ClientManager.manager.inventoryManager;
            customManager = ClientManager.manager.CustomManager;
            instance = this;
        }

     

        public void GetAddItem(ushort idItem, int index)
        {
            ClientManager.manager.clientData.SendPacket(new CommandAddItem() { id = idItem, index = index }, LiteNetLib.DeliveryMethod.ReliableOrdered);
            DestroyItem(index);
        }
        public void RemoveItem(ushort idItem)
        {
            ClientManager.manager.clientData.SendPacket(new CommandRemoveItem() { id = idItem }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }
        private void OnDefaultCharacter(CharacterDefaultPacket obj)
        {
            customManager.characterDefault = obj;
        }

        private void OnItemsPacket(InventoryItemsPacket obj)
        {
            if (EntityManager.HasComponent<InventoryPlayer>(enitity))
                EntityManager.AddComponentData(enitity, obj.inventory);
            else EntityManager.SetComponentData(enitity, obj.inventory);
        }

        private void OnWorldItems(WorldItemsPacket obj)
        {
            for (int i = 0; i < obj.items.Length; i++)
            {
                ItemAsset asset = Resources.Load<ItemAsset>($"Prefabs/items/item_{obj.items[i].Id}");

                GameObject item = GameObject.Instantiate<GameObject>(new GameObject(), obj.items[i].position, Quaternion.Euler(0, obj.items[i].rotation, 0));
                ItemComponent component = item.AddComponent<ItemComponent>();
                component.id = obj.items[i].Id;
                component.index = obj.items[i].Index;
                item.AddComponent<MeshFilter>().mesh = asset.mesh;
                item.AddComponent<MeshRenderer>().material = asset.material;
                item.AddComponent<BoxCollider>();
                item.tag = "Interact";
                InventoryCManager.itemWorlds[component.index] = component;
            }
        }

        protected override void OnUpdate()
        {



        }
        private void AddSlotPacket(ItemPacket packet)
        {
            UIPacketItems packetItems = InstancePacket();
            for (int i = 0; i < packet.items.Length; i++)
            {
                AddSlotItem(packetItems, packet.items[i]);
            }
        }
        private void AddSlotItem(UIPacketItems packetItems, ItemInventory item)
        {
            SlotItemUI itemUI = InstanceItem();
            itemUI.id = item.id;
            itemUI.count = item.count;
            itemUI.countText.text = item.count.ToString();
            packetItems.slots.Add(itemUI);
        }
        private SlotItemUI InstanceItem()
        {

            SlotItemUI  itemUI = GameObject.Instantiate(InventoryCManager.PrefabIconItem, InventoryCManager.ParrentItemsInventory).GetComponent<UIPacketItems>();

            return itemUI;

        }
        private UIPacketItems InstancePacket()
        {
            
            UIPacketItems packetItems =  GameObject.Instantiate(InventoryCManager.PrefabIconPacket, InventoryCManager.ParrentItemsInventory).GetComponent<UIPacketItems>();
            InventoryCManager.PacketItems.Add(packetItems);
            return packetItems;
           
        }

        private void DestroyItem(int index)
        {
            GameObject.Destroy(InventoryCManager.itemWorlds[index]);
        }
    }


}