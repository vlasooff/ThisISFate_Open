using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Other
{
    public class ObjectManager : MonoBehaviour
    {
        public List<ObjectDataWorld> objects = new List<ObjectDataWorld>();
    }

    public class ObjectDataWorld : ScriptableObject
    { 
        public Mesh meshLod0;
        public ushort Id;
        public Material[] materials; 
    }
}
