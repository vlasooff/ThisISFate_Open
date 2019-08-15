using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components
{ 
    public class UIPacketItems : SlotItemUI
    {
        public Transform content;
        public List<SlotItemUI> slots = new List<SlotItemUI>();
    }
}
