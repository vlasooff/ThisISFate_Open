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

        protected override void onStartedServer(NetManager manager)
        {
            ItemsGlobal = ServerManager.manager.GetComponent<ItemsWorldComponent>();
            SyncStateServer.OnCreatedPlayerResponse += OnConnectedToServer;
            SyncStateServer.onUpdateSec += OnForPlayerSec;
            ServerData._packetProcessor.SubscribeReusable<CommandAddItem, NetPeer>(OnAddItemInventory);
        }

        private void OnForPlayerSec(NetDataWriter packet)
        {

        }

        private void OnConnectedToServer(PlayerManager manager, NetDataWriter writer)
        {
            WritePacket(new WorldItemsPacket() { items = ItemsGlobal.itemWorlds.ToArray() }, writer);
            WritePacket(manager.playerInventory, writer);

        }
        #region ItemWorld
        private void SpawnWorldItem(ushort id, Vector3 position, byte rotY)
        {
            ItemWorld item = new ItemWorld(ItemsGlobal.itemWorlds.Count + 1, id, position, rotY);
            ItemsGlobal.itemWorlds.Add(item);
            item.Index = ItemsGlobal.itemWorlds.IndexOf(item);
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
                AddItemInInventory((PlayerManager)peer.Tag, itemWorld.Id);
                RemoveWorldItem((ushort)command.index);
            }
            else Debug.LogError("[S] ONADDITEM Index != null");
        }
        private void AddItemInInventory(PlayerManager manager, ushort id)
        {
            PacketItems packet = manager.playerInventory.GetMinItems();
            WritePacket(new ResponseAddItem(packet.index, id), manager.hash_response_sec);
        }

        protected override void OnUpdate()
        {
        }

    }

}