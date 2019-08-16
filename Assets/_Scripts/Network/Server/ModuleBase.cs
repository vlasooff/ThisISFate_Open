using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.Server.Components
{
    public class ModuleBase : IModule
    {
        public byte id { get { return id_module; } }
        private byte id_module;

        public void Install(byte idModule)
        {
            id_module = idModule;
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
    public interface IModule
    {
        byte id { get; }
        void Install(byte id);
        void Shutdown();
        void Load();
        void Save();
    }
}
