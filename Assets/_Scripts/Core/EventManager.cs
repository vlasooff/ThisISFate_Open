 
using Network.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Network.Core
{
    public enum OperationMessage : byte
    {
        Null = 0, Spawn = 1, Chat = 2, Logining = 3, Event, Sync, Error,
        Serialized
    }
    public enum OperationError : byte
    {
        Error, NotFoundCommand
    }
    public static class EventManager
    {
        //Server on message
        public static void EventMethodMessage(object[] param, PacketSystem message,byte idMethod)
        {
            var methods = EventMethod(typeof(OnEventMessageAttribute));
            foreach (var method in methods) // iterate through all found methods
            {
                var obj = Activator.CreateInstance(method.DeclaringType); // Instantiate the class

                object[] messageconfig = method.GetCustomAttributes(typeof(OnEventMessageAttribute), false);
                foreach (var item in messageconfig)
                {
                    OnEventMessageAttribute at = (OnEventMessageAttribute)item;
                    if (at.system == PacketSystem.Default) method.Invoke(obj, param); // invoke the method
                    else if (at.system == message && at.Message == idMethod) method.Invoke(obj, param);
                }
            }

        }
        public static IEnumerable<MethodInfo> EventMethod(Type type)
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
             .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
             .Where(x => x.IsClass) // only yields classes
             .SelectMany(x => x.GetMethods()) // returns all methods defined in those classes
              .Where(x => x.GetCustomAttributes(type, false).FirstOrDefault() != null); // returns only methods that have the InvokeAttribute

            return methods;
        }
        public static void EventMethod(Type type, object[] param)
        {
            var methods = EventMethod(type);
            foreach (var method in methods) // iterate through all found methods
            {
                var obj = Activator.CreateInstance(method.DeclaringType); // Instantiate the class
                method.Invoke(obj, param); // invoke the method
            }
        }
        public static IEnumerable<Type> EventSyncInterfaces(Type type)
        {
            var Interfaces = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
               .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
               .Where(x => x.IsClass) // only yields classes
               .SelectMany(x => x.GetInterfaces()) // returns all methods defined in those classes
                .Where(x => x.GetCustomAttributes(type, false).FirstOrDefault() != null); // returns only methods that have the InvokeAttribute
            return Interfaces;
            //   foreach (var interfacee in Interfaces) // iterate through all found methods
            //  {
            //      INetSerializable inter = (INetSerializable)interfacee;
            //     inter.
            // var obj = Activator.CreateInstance(method.DeclaringType); // Instantiate the class
            //  method.Invoke(obj, param); // invoke the method
            //  }
        }
    }
}
