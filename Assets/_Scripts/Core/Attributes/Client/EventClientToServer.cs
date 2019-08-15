using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Core.Attributes.Client
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventConnectClient : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Method)]
    public class EventDisconectClient : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Method)]
    public class EventMessageClient : Attribute
    {
        public OperationMessage Message;

        public EventMessageClient(OperationMessage msg)
        {
            Message = msg;
        }
        public EventMessageClient()
        {
            Message = OperationMessage.Null;
        }
    }
}
   