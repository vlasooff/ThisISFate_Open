using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandChat : Attribute
    {
        public string Name;
        public EtypePermission License;

        public CommandChat(string name)
        {
            Name = name;
            License = EtypePermission.player;
        }
        public CommandChat(string name,EtypePermission lic)
        {
            Name = name;
            License = lic;
        }
    }
    public enum EtypePermission : byte
    {
        player,moderator,admin
    }

}
