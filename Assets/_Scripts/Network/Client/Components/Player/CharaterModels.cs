using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components
{
    [CreateAssetMenu(fileName = "CharaсterModels",menuName = "Character/Models")]
    public class CharaterModels : ScriptableObject
    {
 

    }
    [Serializable]
    public class CustomItem 
    {
        public ushort Id;
        public Mesh mesh; 
        
    }
}
