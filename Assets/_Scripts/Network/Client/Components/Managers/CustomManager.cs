using Community.Core;
using Community.Core.Serializables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components
{
    public class CustomManager : MonoBehaviour
    {
        public CharacterType currentCharater;

        public ManData Man;
        public WomanData Women;
        public CharacterDefaultPacket characterDefault;

        public void Send()
        {

        }

    }
    [System.Serializable]
    public struct ManData
    {
        public CustomItem hatsDefault;
        public CustomItem headDefault;
        public CustomItem bodyDefault;
        public CustomItem pantsDefault;
    }
    [System.Serializable]
    public struct WomanData
    {
        public CustomItem hatsDefault;
        public CustomItem headDefault;
        public CustomItem bodyDefault;
        public CustomItem pantsDefault;
    }
}
