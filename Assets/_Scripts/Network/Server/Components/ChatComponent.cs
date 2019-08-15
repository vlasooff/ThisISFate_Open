using Community.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Components
{ // Serializable attribute is for editor support.
 
    public class ChatComponent : MonoBehaviour
    {
        public static OnSayChat EventSay;
        public static OnCommandChat EventCommand ;
        public  readonly int MAX_MESSAGE_LENGTH = 127; 
        public  Dictionary<string, ICommandChat> commands = new Dictionary<string, ICommandChat>();

        ServerProxy serverData => ServerManager.manager.serverProxy;
    }
}
