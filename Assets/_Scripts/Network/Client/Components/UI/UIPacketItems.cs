using Community.Client.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Community.Client.Components
{ 
    public class UIPacketItems : MonoBehaviour
    {
        public ItemInventory item;
        public Text TextMassa;
        public Transform content;
        public List<SlotItemUI> slots = new List<SlotItemUI>();
    }
}
