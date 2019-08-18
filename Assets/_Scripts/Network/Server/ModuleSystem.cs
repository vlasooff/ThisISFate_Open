using Community.Server.Components;
using Community.Server.Systems;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace  Community.Server 
{

    public class ModuleSystem
    {
        public List<IModule> modules = new List<IModule>();
     

        public ModuleSystem()
        {
            ServerCallBlack.onStartServer += InstallModules;
            ServerCallBlack.onShutdownServer += ShutdownModules; 
        }
        public void InstallModules(NetManager manager)
        {
            modules.Add(new CharacterModule());
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Install(i);
            }
        }
        private void ShutdownModules()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Shutdown();
            }
        }

  

    }
}
