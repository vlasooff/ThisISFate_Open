using Community.Server;
using LiteNetLib;
using Network.Core;
using Network.Core.Attributes;
using Network.Core.Serializables;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Network.Modules
{
    public static class SyncManager
    {
        public static Dictionary<ushort, MethodSync> methodsSync = new Dictionary<ushort, MethodSync>();
        public static void SendEvent(NetDataEventWriter writer)
        {

        }
    }
    [DisableAutoCreation]
    public class SyncSystem : ComponentServer
    { 
        [EventStartServer] 
        public void OnInstalizeServer()
        {
            LogDev("Instalize");
            var methods = EventManager.EventMethod(typeof(SyncAttribute));
            foreach (var method in methods) // iterate through all found methods
            {
                object[] obj = methods.GetType().GetCustomAttributes(typeof(SyncAttribute), false);
                if(obj != null)
                {
                    foreach (var item in obj)
                    {
                        SyncAttribute at = (SyncAttribute)obj[0];
                        LogDev($"Method add: {at.id} name: {method.Name}");
                        SyncManager.methodsSync.Add(at.id, new MethodSync(at, method));
                    }
                }
            } 
        } 
       
        [Sync(0)]
        public void Execute(NetPeer peer, NetPacketReader reader)
        {

        }
        protected override void OnUpdate()
        {
        }
    }
    public struct MethodSync
    {
        public SyncAttribute atrebute;
        public MethodInfo method;

        public MethodSync(SyncAttribute at, MethodInfo info)
        {
            atrebute = at;
            method = info;
        }
    }
}
