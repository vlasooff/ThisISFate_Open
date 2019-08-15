using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Core.Serializables;
using Community.Server.Components;
using LiteNetLib;

namespace Community.Server.Systems
{
    public class CustomizeSystem : ComponentServer
    {
        private ServerProxy ServerData => ServerManager.manager.serverProxy;
        protected override void OnStartServer(NetManager manager)
        {
            base.OnStartServer(manager);
            
        }


        protected override void OnUpdate()
        {

        }
    }
}
