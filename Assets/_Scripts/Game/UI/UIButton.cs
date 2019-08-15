using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Community.Client.Components
{

    public class UIButton : Button
    {
        [Space(100)]
        public Text textButton;
        [Range(0,2)]
        public float range;

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            textButton.color = colors.highlightedColor;
            Debug.Log("OnSubmit");
        }
       
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            textButton.color = colors.selectedColor; 
        }
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            textButton.color = colors.normalColor;
            
        }
    }
}
