using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Community.Client.Components
{
    public class Interactable : MonoBehaviour
    {
        public SliderText[] sliderTexts;

        public virtual void Execute(int index)
        {
            if (sliderTexts[index].method != null)
            {
                sliderTexts[index].method.Invoke();
            }
            else Debug.Log("[C] Execute test");
        }
    }
    [System.Serializable]
    public struct SliderText
    {
        [Multiline]
        public string Text;
        public Action method;

        public SliderText(string text, Action _method)
        {
            Text = text;
            method = _method;
        }
       
    }
}