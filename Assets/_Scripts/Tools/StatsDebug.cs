using Community.Client;
using Community.Client.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Tools
{
    
    public class StatsDebug : MonoBehaviour
    {
        public bool isDevelope;
      
        GUIStyle style = new GUIStyle(); 
        float deltaTime = 0.0f;
        ClientData clientData;
        PlayersManager playersData;
        void Start()
        {
            if (ClientManager.manager != null)
            {

                clientData = ClientManager.manager.clientData;
                if(ClientManager.manager.playersManager)
                    playersData = ClientManager.manager.playersManager;
            }

            style.normal.textColor = Color.white;
            style.fontSize = 23;
            style.fontStyle = FontStyle.Bold;
        }

        void OnGUI()
        {
            if (!isDevelope) return;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            if (ClientManager.manager != null)
            GUI.Label(new Rect(10, 10, 100, 34), $" FPS: {fps} \n PING: { clientData._netManager.FirstPeer.Ping} \n PCounts: { clientData._netManager.FirstPeer.PacketsCountInReliableQueue} \n Time: { (Time.deltaTime - clientData._netManager.FirstPeer.RemoteTimeDelta)} ", style);
        if(playersData != null) GUI.Label(new Rect(10, 160, 100, 34), $" Players: {playersData.players.Count}   ", style);
        }
         
  


        void Update()
        {
            if(Input.GetKey(KeyCode.F10))
            {
                if (isDevelope) isDevelope = false;
                else isDevelope = true;
            }
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }
 
    }

}
