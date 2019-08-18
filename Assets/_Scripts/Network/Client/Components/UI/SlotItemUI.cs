using Community.Client.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Community.Client.Components
{
    public class SlotItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private RawImage therawImage;
        public Image Mask; 
        [SerializeField]
        private Color color;
        public ItemInventory item; 
        public Text countText;
        Color backcolor; 

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Mask.color = color; 

        }
        public virtual void SetItem(ItemInventory newitem)
        {
            if(item == null)
            { 
                backcolor = Mask.color;
                therawImage.enabled = true;
                therawImage.texture = GetItemIcon(newitem.id);
                item = newitem;
            } 
            //        tooltip.text = string.Format("{0} Massa: {1}", InventoryUI.instance.player.theInventory.info.ItemsAll[iditem].worldItem.nameItem, InventoryUI.instance.player.theInventory.info.ItemsAll[iditem].massa);

        }
        private Texture2D GetItemIcon(ushort id)
        {
            return Resources.Load<Texture2D>($"Prefabs/items/icons/icon_{id}");
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Mask.color = backcolor;  
        }


        public virtual void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnSubmit");
          //  if (MenuPanel.instance.isActive)
          //  {
          //      MenuPanel.instance.Close();

          //  }
           // MenuPanel.instance.Open(); 

        }
    }

}
