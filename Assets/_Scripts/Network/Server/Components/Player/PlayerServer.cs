using Community.Other;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Core.Serializables;
using Network.Core.Attributes;
using UnityEngine;
using LiteNetLib.Utils;

namespace Community.Server.Components
{
    public class PlayerServer : MonoBehaviour
    {
        public int ping;
        public EtypePermission licenze = EtypePermission.player;
        public CharacterController controller;
        public ushort idRegion;
    }
  
}
