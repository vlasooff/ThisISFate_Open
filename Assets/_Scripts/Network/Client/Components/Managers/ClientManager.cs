
using Community.Client.Components;
using Community.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client 
{
    public class ClientManager : Managers
    {
        //Клиент
        public ClientData clientData;
        public ClientDataSteam clientDataSteam;
        //Чат
        public ChatData chatData;
        //Система Игроков
        public PlayersManager playersManager;
        public static ClientManager manager;
        public InventoryCManager inventoryManager;
        public CustomManager CustomManager;
        public UIManager UVManager;
        private void Awake()
        {
            manager = this;
            DontDestroyOnLoad(this);
            if (!clientData) Debug.Log("[S] ClientManager null!");
        }
        public static void SetCursor(CursorLockMode cursorLock, bool active)
        {
            Cursor.lockState = cursorLock;
            Cursor.visible = active;
        }
    } 
}
