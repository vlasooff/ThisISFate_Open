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
        public ushort id;
        public ushort count;
        public Text countText;
        Color backcolor;

  
   



        public void OnPointerEnter(PointerEventData eventData)
        {
            Mask.color = color; 

        }
        public void SetIcon(Texture2D icon, ushort iditem)
        {
            backcolor = Mask.color;
            therawImage.texture = icon;
            id = iditem;
            //        tooltip.text = string.Format("{0} Massa: {1}", InventoryUI.instance.player.theInventory.info.ItemsAll[iditem].worldItem.nameItem, InventoryUI.instance.player.theInventory.info.ItemsAll[iditem].massa);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Mask.color = backcolor;  
        }


        public void OnPointerClick(PointerEventData eventData)
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
