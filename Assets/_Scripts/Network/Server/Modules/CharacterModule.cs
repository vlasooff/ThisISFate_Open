using Community.Server.Components;
using Community.Server.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Server
{
    public class CharacterModule : ModuleBase
    {
        private CustomizeSystem customizeSystem;
        public override void Install(int idModule)
        {
            base.Install(idModule); 
            Debug.Log("[S] Module create");
        }
        public override void Shutdown()
        { 
            World.Active.DestroySystem(customizeSystem);
        }

    }
}
