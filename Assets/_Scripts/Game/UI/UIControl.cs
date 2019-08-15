using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public Button[] buttons;
    public UICustom custom;
    [EasyButtons.Button]
    public void GetButtons()
    {
        buttons = GetComponents<Button>();
    }
    [EasyButtons.Button]
    public void UpdateButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].colors = custom.ButtonBlock;
        }
    }
}
