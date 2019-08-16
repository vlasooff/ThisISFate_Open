using Community.Other;
using Community.Server.Components;
using Community.Server.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Server
{
    public class ServerManager : Managers 
    {
        //Сервер
        public ServerProxy serverProxy;
        public ServerInfoProxy serverInfoProxy;
        //Чат
        public ChatComponent chatData;
        //Система Игроков
        public PlayersInfoComponent playersData;
        public RegionsComponent regionsComponent; 
        public static ServerManager manager;

        private void Awake()
        {

 
            manager = this; 

             DontDestroyOnLoad(this);
            if (!serverProxy) Debug.Log("[S] ServerManager " +
                "null!");
        }
     
    }
     
}
