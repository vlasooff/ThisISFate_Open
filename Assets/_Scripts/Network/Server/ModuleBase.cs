using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.Server.Components
{
    public class ModuleBase : IModule
    { 
        private int index;

        public virtual void Install(int idModule)
        {
            index = idModule;
        }

        public virtual void Load()
        { 

        }

        public virtual void Save()
        { 
        }

        public virtual void Shutdown()
        { 
        }
    }
    public interface IModule
    { 
        void Install(int idModule);
        void Shutdown();
        void Load();
        void Save();
    }
}
