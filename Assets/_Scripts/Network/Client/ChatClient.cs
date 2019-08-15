using Community.Client.Components;
using Network; 
using UnityEngine;
using System;
using Network.Core.Serializables;

namespace Community.Client
{

    public class ChatClient : ComponentClient
    {
        ChatData chat;
        PlayersManager playersManager;
        protected override void OnStartedClient()
        {
            chat = ClientManager.manager.chatData;
            playersManager = ClientManager.manager.playersManager;
            ClientData._packetProcessor.SubscribeReusable<ChatPacket>(OnMessageFromPlayer);
        }
        
        private void OnMessageFromPlayer(ChatPacket obj)
        {
            Debug.Log("[CLIENT] [CHAT] Message user " + obj.Id);
            chat.text.text += (Environment.NewLine + playersManager.players[obj.Id].id + obj.text);
        } 

        protected override void OnUpdate()
        {
            if(ClientCallBlack.isConnected)
            {
                if (Input.GetKeyDown(chat.keyCode))
                {
                    if (chat.isActive)
                    {
                        chat.isActive = false;
                        chat.InputField.gameObject.SetActive(false);
                        ClientManager.SetCursor(CursorLockMode.Locked, false);
                    }
                    else
                    {
                        chat.isActive = true;
                        chat.InputField.gameObject.SetActive(true);
                        ClientManager.SetCursor(CursorLockMode.None, true);
                    }
                }
            }
        }
    }
}
