using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMenuControl : MonoBehaviour
{
    public List<Button> buttons = new List<Button>();
    public Button currentButton; 
    public virtual void UpdateMenu()
    {
         
    }
    
}