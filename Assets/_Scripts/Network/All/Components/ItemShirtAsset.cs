using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    [CreateAssetMenu(fileName = "ShirtName", menuName = "Items/ShirtAsset")]
    public class ItemShirtAsset : ItemAsset
    {
        public Mesh CharacterMesh;
        
    }
}
