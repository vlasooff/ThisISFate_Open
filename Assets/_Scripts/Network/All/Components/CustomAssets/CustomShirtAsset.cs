using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Client.Components;
using UnityEngine;

namespace  Network.All
{
    [CreateAssetMenu(fileName = "CustomName", menuName = "Custom/ShirtAsset")]
    public class CustomShirtAsset : CustomAsset
    {
        public override bool Set(CharacterCustom character)
        {
            try
            {

                character.mesh_body.sharedMesh = mesh;
                return true;
            }
            catch  
            {
                return false;
            }
        }
        public override bool Delete(CharacterCustom character,Mesh bodyDefault)
        {
            try
            {

                character.mesh_body.sharedMesh = bodyDefault;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
