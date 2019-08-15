using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventOnDisconnectedAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class EventOnConnectedAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class EventStartServer : Attribute
    {

    }
}
