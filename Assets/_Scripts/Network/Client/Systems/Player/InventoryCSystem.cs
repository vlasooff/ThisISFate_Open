using System;
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
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class InventoryCSystem : ComponentClient
    {
        InventoryCManager Inventory;
        CustomManager customManager;
        ClientData clientData;
        public static InventoryCSystem instance;
        private CharacterCustomBody customBody;
        private CharacterCustomHead customHead;
    

        Entity enitity;
        protected override void OnStartClient()
        { 
         //   ClientData._packetProcessor.RegisterNestedType<ItemInventory>();
            ClientData._packetProcessor.RegisterNestedType<ItemWorld>(); 
            ClientData._packetProcessor.SubscribeReusable<WorldItemsPacket>(OnWorldItems); 
            ClientData._packetProcessor.SubscribeReusable<CharacterDefaultPacket>(OnDefaultCharacter);
            ClientData._packetProcessor.SubscribeReusable<ResponseAddItem>(OnResponseAddItem);
            ClientData._packetProcessor.SubscribeReusable<SpawnItemWorldPacket>(OnSpawnItemWorld);
            ClientData._packetProcessor.SubscribeReusable<DestroyItemWorldPacket>(OnDestroyItemWorld);
            //  ClientData._packetProcessor.SubscribeReusable<PlayerInventory>(OnPlayerInventory);
            enitity = ClientManager.manager.playersManager._clientPlayer.entity;
            Inventory  = ClientManager.manager.inventoryManager;
            customManager = ClientManager.manager.CustomManager;
            instance = this;
        }

       




        #region response
        private void OnResponseAddItem(ResponseAddItem obj)
        {
            AddSlotItem(Inventory.PacketItems[(int)obj.IndexBodyPacket], new ItemInventory(obj.idItem));
        }

        private void AddSlotItem(UIPacketItems uIPacketItems, ItemInventory itemInventory)
        {
            GetFreeSlot(uIPacketItems).SetItem(itemInventory);
        }
        #endregion

        public void GetAddItem(ushort idItem, int index)
        {
            ClientManager.manager.clientData.SendPacket(new CommandAddItem() { index = index }, LiteNetLib.DeliveryMethod.ReliableOrdered);
            DestroyItem(index);
        }
        public void ChangeItem()
        {

        }
        public void RemoveItem(ushort idItem)
        {
          //  ClientManager.manager.clientData.SendPacket(new CommandGiveItem() { id = idItem }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }
        private void OnDefaultCharacter(CharacterDefaultPacket obj)
        {
            customManager.characterDefault = obj;
        }
        #region UI
        private UIPacketItems CreatePacketUI(ItemInventory item,EBodyIndex bodyIndex)
        {
            UIPacketItems  packet = GameObject.Instantiate<UIPacketItems>(Inventory.PrefabSlotPacket, Inventory.ParrentItemsInventory);
            packet.item = item;
            Inventory.UISlotBody[(int)bodyIndex].SetPacket(packet);
            
            return packet;
        }
        private SlotItemUI CreateSlot(UIPacketItems packet)
        {
            SlotItemUI itemUI =  GameObject.Instantiate<SlotItemUI>(Inventory.PrefabSlotItem, packet.content)  ;
            packet.slots.Add(itemUI);
            return itemUI;
        }
        private void DestroySlot(UIPacketItems packet,SlotItemUI slot)
        {
            packet.slots.Remove(slot);
            GameObject.Destroy(slot);
        }
        #endregion
        #region WorldItems
        private void OnSpawnItemWorld(SpawnItemWorldPacket packet)
        { 
            ItemAsset asset = GetItemAsset(packet.id);
            GameObject item = GameObject.Instantiate<GameObject>(new GameObject(), packet.position, Quaternion.Euler(0, packet.rotY, 0));
            ItemComponent component = item.AddComponent<ItemComponent>();
            component.id = packet.id;
            component.index = packet.index;
            item.AddComponent<MeshFilter>().mesh = asset.mesh;
            item.AddComponent<MeshRenderer>().material = asset.material;
            item.AddComponent<BoxCollider>();
            item.tag = "Interact";
            Inventory.itemWorlds[component.index] = component;
        }
        private void OnDestroyItemWorld(DestroyItemWorldPacket packet)
        {
            GameObject.Destroy(Inventory.itemWorlds[packet.index]);
        }

        private void OnWorldItems(WorldItemsPacket obj)
        {
            for (int i = 0; i < obj.items.Length; i++)
            {
                ItemAsset asset = GetItemAsset(obj.items[i].Id);

                GameObject item = GameObject.Instantiate<GameObject>(new GameObject(), obj.items[i].position, Quaternion.Euler(0, obj.items[i].rotation, 0));
                ItemComponent component = item.AddComponent<ItemComponent>();
                component.id = obj.items[i].Id;
                component.index = obj.items[i].Index;
                item.AddComponent<MeshFilter>().mesh = asset.mesh;
                item.AddComponent<MeshRenderer>().material = asset.material;
                item.AddComponent<BoxCollider>();
                item.tag = "Interact";
                Inventory.itemWorlds[component.index] = component;

            }
        }
        protected override void OnShutdown()
        {
            for (int i = 0; i < Inventory.itemWorlds.Length; i++)
            {
                GameObject.Destroy(Inventory.itemWorlds[i]);

            }
            base.OnShutdown();
        }
        #endregion 
        private ItemAsset GetItemAsset(ushort id)
        {
            return Resources.Load<ItemAsset>($"Prefabs/items/item_{id}");
        }
        private Texture2D GetItemIcon(ushort id)
        {
            return Resources.Load<Texture2D>($"Prefabs/items/icons/icon_{id}");
        }
        private SlotItemUI GetFreeSlot(UIPacketItems packet)
        {
            for (int i = 0; i < packet.slots.Count; i++)
            {
                if (packet.slots[i].item == null) return packet.slots[i];
            } 
            return CreateSlot(packet);
        }
        protected override void OnUpdate()
        {



        }
      

        private void DestroyItem(int index)
        {
            GameObject.Destroy(Inventory.itemWorlds[index]);
        }
        private void UpdateDate()
        { 
            EntityManager.GetComponentData<CharacterCustomBody>(enitity);
        }
    }
    public class ItemInventory
    { 
        public ushort id;
        public ushort count;

        public ItemInventory(ushort idItem)
        {
            id = idItem;
        }
        public ItemInventory(ushort idItem,ushort countItem)
        {
            id = idItem;
            count = countItem;
        }
    }

}