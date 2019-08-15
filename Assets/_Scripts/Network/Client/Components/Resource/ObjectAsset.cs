using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components 
{
    [CreateAssetMenu(fileName = "ObjectScena", menuName = "Asset/Object")]
    public class ObjectAsset : ScriptableObject
    {
        public Mesh Mesh;
        public ushort id;
        public Material[] materials;
    }
}
