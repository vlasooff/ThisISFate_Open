using Community.Core;
using Community.Core.Serializables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Server.Components
{
    public class MoveTestComponent : MonoBehaviour 
    { 
        public float speed; 
        public SpawnCurrentPlayer state = new SpawnCurrentPlayer();
    }
}
