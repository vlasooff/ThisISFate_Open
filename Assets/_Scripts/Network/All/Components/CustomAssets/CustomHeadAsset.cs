using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Community.Client.Components;

namespace Network.All
{
    [CreateAssetMenu(fileName = "CustomName", menuName = "Custom/HeadAsset")]

    public class CustomHeadAsset : CustomAsset
    {
        public override bool Set(CharacterCustom character)
        {
            try
            {

                character.mesh_head.sharedMesh = mesh;
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
                if (bodyDefault)
                    character.mesh_head.sharedMesh = bodyDefault;
                else character.mesh_head.sharedMesh = null;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}