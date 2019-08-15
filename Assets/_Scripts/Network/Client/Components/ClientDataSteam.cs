using Community.Client.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components
{
    public class ClientDataSteam : MonoBehaviour
    {
        public bool isDevelop;
        public readonly uint AppId = 821530;
        public Facepunch.Steamworks.Client client;
        public ulong steamid { get
            {
                if (client != null)
                {
                    return client.SteamId;
                }
                else return (ulong)Time.time;
            }
        }
        public string NickName
        {
            get
            {
                if (client != null)
                {
                    return client.Username;
                }
                else return Environment.MachineName + Time.time;
            }
        }

    }
}
