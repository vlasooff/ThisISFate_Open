using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Community.Client.Components; 

namespace Network.All
{
    [CreateAssetMenu(fileName = "CustomName", menuName = "Custom/MaskAsset")]

    public class CustomMaskAsset : CustomAsset
    {
        public override bool Set(CharacterCustom character)
        {
            try
            {

                character.mesh_mask.sharedMesh = mesh;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public override bool Delete(CharacterCustom character, Mesh bodyDefault)
        {
            try
            {

                character.mesh_mask.sharedMesh = bodyDefault;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
