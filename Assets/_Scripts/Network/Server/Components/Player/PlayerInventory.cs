using Community.Core;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Server.Components
{
    public class PlayerInventory
    {
        public PacketItems[] itemPackets;
        public ItemInventory[] slotsHand;
        public ushort AllMassa = 0;
 
        public void AddNewPackets(PacketItems packet, EBodyIndex bodyIndex)
        {
            itemPackets[(int)bodyIndex] = packet;
        }
        public void CombinePackets(PacketItems packet,EBodyIndex bodyIndex)
        {
            itemPackets[(int)bodyIndex].CombinePacker(packet);
        }
        public PacketItems GetMinItems()
        {
            PacketItems packet = itemPackets[0];
            for (int i = 0; i < itemPackets.Length; i++)
            {
                if (packet.massaPacket < itemPackets[i].massaPacket) packet = itemPackets[i];

            }
            return packet;
        }
        public RemoteCustomPlayer GetCharacterBody()
        {
            ushort[] idlist = new ushort[itemPackets.Length];
            for (int i = 0; i < itemPackets.Length; i++)
            {
                idlist[i] = itemPackets[i].id;
            }
            RemoteCustomPlayer remoteCustom = new RemoteCustomPlayer(idlist);
            return remoteCustom;
            
        }
    }
    [System.Serializable]
    public class PacketItems : ItemInventory
    {
        public ushort id;
        public EBodyIndex index;
        public ushort massaPacket;
        private List<ItemPacket> items;
        public PacketItems(ushort idItem, ushort countItem, EBodyIndex bodyIndex) : base(idItem,countItem )
        {
            id = idItem;
            items = new List<ItemPacket>();
            index = bodyIndex;
        }
        
        public void CombinePacker(PacketItems newpacket)
        {
            items.Add(new ItemPacket(id, count, asset));
            id = newpacket.id;
            massaPacket += newpacket.massaPacket;
            for (int i = 0; i < newpacket.items.Count; i++)
            {
                items.Add(newpacket.items[i]);
            }
        }

        public void AddItem(ItemInventory item)
        {
            massaPacket += (ushort)(item.asset.massa * count);
            if (IsItem(id))
            {
                GetItem(id).count += count;
            }
            else
            {
                ItemPacket packet = (ItemPacket)item;
                items.Add(packet);
                packet.index = (ushort)items.IndexOf(packet); 
            }
        }
        public bool IsItem(ushort id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == id) return true;
            }
            Debug.LogError("[S] Inventory packet not fined item " + id);
            return false;
        }
        public ItemInventory GetItem(ushort id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == id) return items[i];
            }
            Debug.LogError("[S] Inventory packet not fined item " + id);
            return null;
        }
        public void RemoveItem(ItemAsset asset, ushort id, ushort count)
        {
            massaPacket -= (ushort)(asset.massa * count);
            ItemPacket item = (ItemPacket)GetItem(id);
            if((item.count - count) > 0)
            {
                item.count -= count;
            }
            else
            {
                items.Remove(item);
            }
        }
       
    }
    [System.Serializable]
    public class ItemInventory  
    {
        public ushort id;
        public readonly ItemAsset asset;
        public ushort count;
        public ItemInventory(ushort idItem )
        {
            id = idItem;
            count = 1;
            asset = Resources.Load($"Prefabs/items/item_{id}") as ItemAsset;
        }
        public ItemInventory(ushort idItem,ushort countItem)
        {
            id = idItem;
            count = countItem;
            asset = Resources.Load($"Prefabs/items/item_{id}") as ItemAsset;
        }
    }
    public class ItemPacket : ItemInventory
    { 
        public ushort index;

        public ItemPacket(ushort idItem, ushort countItem,ItemAsset ItemAsset) : base(idItem,countItem)
        { 
        }
    }
}
