using Community.Core.Serializables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.Server.Components
{
    public class PlayerMovement
    {
        public List<ushort> idPlayerInRadius = new List<ushort>();
        public float timeUpdateRadius = 0;
        public PlayerServer player;
        internal float newRotation;

        public PlayerMovement(PlayerServer plr)
        {
            player = plr;
        }
       

       
    }
}
