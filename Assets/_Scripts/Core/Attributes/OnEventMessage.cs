 
using Network.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnEventMessageAttribute : Attribute
    {
        public PacketSystem system;
        public byte Message; 
        public OnEventMessageAttribute(PacketSystem packetSystem, byte msg)
        {
            Message = msg;
            system = packetSystem;
        }
        public OnEventMessageAttribute()
        {
            Message = 0;
            system = PacketSystem.Default; 
        }
    }
    public enum PacketSystem : byte
    {
        Default, PlayerSystem
    }
}
