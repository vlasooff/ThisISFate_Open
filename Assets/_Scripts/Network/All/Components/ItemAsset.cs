using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace   Network 
{
    [CreateAssetMenu(fileName = "ItemName",menuName = "Items/ItemAsset")]
    public class ItemAsset : ScriptableObject
    {
        public ushort id;
        public Mesh mesh;
        public Material material; 
      
        
    }
    
}
