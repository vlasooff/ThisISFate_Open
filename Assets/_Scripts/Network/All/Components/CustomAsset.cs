using Community.Client.Components;
using Community.Core;
using System;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
namespace Network.All
{
    [CreateAssetMenu(fileName = "CustomName", menuName = "Custom/Asset")]
    public class CustomAsset : ScriptableObject
    {
        public Mesh mesh;
        public virtual bool Set(CharacterCustom character)
        {
            return false;
        }
        public virtual bool Delete(CharacterCustom character)
        {
            return false;
        }
        public virtual bool Delete(CharacterCustom character,Mesh DefMesh)
        {
            return false;
        }

    }
 
}
