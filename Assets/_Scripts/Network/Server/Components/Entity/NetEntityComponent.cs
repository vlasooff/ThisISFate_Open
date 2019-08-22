using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Community.Server.Components 
{
    [System.Serializable]
    public struct EntityNetID : IComponentData
    {
        public readonly ushort id; 
        public EntityNetID(ushort UserID )
        {
            id = UserID; 
        }
    } 
    public struct EntityNetPlayer : IComponentData
    {
        
    }
}
