using Community.Client.Systems;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities; 
using UnityEngine;

namespace Community.Client.Components
{
    public class ItemComponent : Interactable
    {
        public ushort id;
        public int index;

        public void Start()
        {
            sliderTexts = new SliderText[1];
            sliderTexts[0] = new SliderText("Get", GetItem);
        }
    
        private void GetItem()
        {
            InventoryCSystem.instance.GetAddItem(id,index);
        }
        public override void Execute(int index)
        {
            if (sliderTexts[index].method != null)
            {
                sliderTexts[index].method.Invoke();
            }
            else Debug.Log("[C] Execute test");
        }
    } 
}