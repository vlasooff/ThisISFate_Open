using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components 
{
    public class InventoryCManager : MonoBehaviour 
    { 
        public CharaterModels models; 
        public ItemComponent[] itemWorlds = new ItemComponent[0];
        public List<UIPacketItems> PacketItems = new List<UIPacketItems>();

        public SlotItemUI[] slotsCharacter;
        public Action[] slotsAction;

        public SlotItemUI[] Arms;

        public GameObject PrefabIconPacket; 
        public GameObject PrefabIconItem;
        public Transform ParrentItemsInventory;
        public Transform ParrentNearbyInventory;

        
      

        public void GetItem()
        {

        }
    }
}
