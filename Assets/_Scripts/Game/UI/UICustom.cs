using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UISitting",menuName = "UI")]
[System.Serializable]
public class UICustom : ScriptableObject
{
    public Color[] colorsSprites = new Color[0]; 
   [Header("Button sitting")]
    public ColorBlock ButtonBlock;


}
