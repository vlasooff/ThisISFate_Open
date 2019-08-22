using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Community.Server.Components 
{
    public class EntityPlayerManager : EntityNetManager
    {
        public readonly NetPeer peer;
        public EntityPlayerManager(ushort id,Entity entity, NetPeer netPeer) : base(  id,entity )
        {
            peer = netPeer;
        }
     
    }
}
