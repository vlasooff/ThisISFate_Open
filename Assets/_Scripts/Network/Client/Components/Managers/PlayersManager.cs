using Community.Core.Serializables;
using Community.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components
{
    public class PlayersManager : MonoBehaviour
    {
        public CPlayerManager _clientPlayer;

        public int Count => players.Count;
        public List<RPlayerManager> playersList => players.Values.ToList();
        public Dictionary<ushort, RPlayerManager> players = new Dictionary<ushort, RPlayerManager>(); 

        [EasyButtons.Button]
        public void CheackPlayers()
        {
            Debug.Log($"[C] Client ID: {_clientPlayer.id} Player name : {_clientPlayer.baseData.username} ");
            foreach (var item in  players)
            {
                Debug.Log($"[C] Player ID: {item.Key} Player name : {item.Value.baseData.username}  ");
            }
        }
    }
}
