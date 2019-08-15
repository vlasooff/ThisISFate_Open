using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Community.Tools
{
    public class MaterialUt : MonoBehaviour
    {

        public Material defMaterial;

        public Transform parret;

        public MeshRenderer[] meshs;

        [EasyButtons.Button]
        public void UpdatePrefab()
        {
            meshs =   FindObjectsOfType<MeshRenderer>();

            foreach (var item in meshs)
            {
                for (int i = 0; i < item.sharedMaterials.Length; i++)
                {
                   if(item.material = null)
                    {
                        item.material = defMaterial;
                    }


                } 
            }
        }
        [EasyButtons.Button]
        public void UpdateMaterial()
        {

            MeshRenderer[] meshRenderers = FindObjectsOfType<MeshRenderer>();
            foreach (var item in meshRenderers)
            {
                for (int i = 0; i < item.materials.Length; i++)
                { 
                    
                        item.material = defMaterial;
                  
                }
            }
        }
    }
}
