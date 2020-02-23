using Community.Core;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Community.Server.Systems
{
    public class InventorySystem : ComponentServer
    {
        private ItemsWorldComponent ItemsGlobal;
        private ServerProxy ServerData => ServerManager.manager.serverProxy;

        protected override void OnStartServer(NetPacketProcessor _packetProcessor)
        {
        ItemsGlobal = ServerManager.manager.GetManager<ItemsWorldComponent>(EManagers.ItemsWorld); 
            SyncStateServer.onUpdateSec += OnForPlayerSec; 
            ServerData._packetProcessor.SubscribeReusable<CommandAddItem, NetPeer>(OnAddItemInventory);
            _packetProcessor.RegisterNestedType<ItemWorld>();
        }

        private void OnForPlayerSec(NetDataWriter packet)
        {
            
        }

        protected override void onConnectedPlayer(NetPeer peer)
        {
             
            peer.Send(WritePacket(new WorldItemsPacket() { items = ItemsGlobal.itemWorlds.ToArray() }), DeliveryMethod.ReliableOrdered);
            Debug.Log("[S] Send items world");
        }
        #region ItemWorld
        private void SpawnWorldItem(ushort id, Vector3 position, byte rotY)
        {
            ItemWorld item = new ItemWorld(ItemsGlobal.itemWorlds.Count + 1, id, position, rotY);
            ItemsGlobal.itemWorlds.Add(item);
            
            ServerData._netManager.SendToAll(WritePacket(new SpawnItemWorldPacket() { index = (ushort)item.Index, id = id , position = position,rotY = rotY}), DeliveryMethod.ReliableOrdered);
        }
        private void RemoveWorldItem(ushort index)
        {
            ItemsGlobal.itemWorlds.RemoveAt(index);
            ServerData._netManager.SendToAll(WritePacket(new DestroyItemWorldPacket() { index = index }), DeliveryMethod.ReliableOrdered);
        }
        #endregion

        private void OnAddItemInventory(CommandAddItem command, NetPeer peer)
        {
            ItemWorld itemWorld = ItemsGlobal.itemWorlds[command.index];
            if (itemWorld.Index == command.index)
            {
                AddItemInInventory((EntityPlayerManager)peer.Tag, itemWorld.Id);
                RemoveWorldItem((ushort)command.index);
            }
            else Debug.LogError("[S] ONADDITEM Index != null");
        }
        private void AddItemInInventory(EntityPlayerManager manager, ushort id)
        {
            if (manager.playerInventory == null) Debug.Log("Player == null");
            PacketItems packet = manager.playerInventory.GetMinItems();
            if(packet == null)
            {
                
            }
            manager.peer.Send(WritePacket(new ResponseAddItem(packet.index, id)), DeliveryMethod.ReliableOrdered);
        
        }

        protected override void OnUpdate()
        {
        }

    }

}