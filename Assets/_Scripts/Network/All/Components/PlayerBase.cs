using Community.Core.Serializables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Other
{
    public class PlayerBase : IPlayerBase
    {
        public ushort id { get; protected set; }
        public string username;
        public ulong steamid { get; protected set; }
        
        public PlayerBase(string user,ulong steamId,ushort Id)
        {
            username = user;
            id = Id;
            steamid = steamId;
        }  
    }
    public interface IPlayerBase { }
}
