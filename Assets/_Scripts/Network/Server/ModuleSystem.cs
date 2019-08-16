using Community.Server.Components;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Community.Server.Systems
{

    public class ModuleSystem
    {
        public IModule[] modules = new IModule[0];

        public ModuleSystem()
        {
            ServerCallBlack.onStartServer += InstallModules;
            ServerCallBlack.onShutdownServer += ShutdownModules;
        }

        private void ShutdownModules()
        {
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].Shutdown();
            }
        }

        public void InstallModules(NetManager manager)
        {
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].Install((byte)i);
            }
        }

    }
}
