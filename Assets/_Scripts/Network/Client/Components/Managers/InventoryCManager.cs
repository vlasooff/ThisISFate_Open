using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Community.Client.Components 
{
    public class InventoryCManager : MonoBehaviour 
    {  
        public ItemComponent[] itemWorlds = new ItemComponent[0];
        public UIPacketItems[] PacketItems = new UIPacketItems[0];
        public UISlotBody[] UISlotBody = new UISlotBody[7];


        public SlotItemUI[] Arms;

        public UIPacketItems PrefabSlotPacket; 
        public SlotItemUI PrefabSlotItem;
        public Transform ParrentItemsInventory;
        public Transform ParrentNearbyInventory; 

     


        public void GetItem()
        {

        }
    }
 


}
