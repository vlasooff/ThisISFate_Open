using Network.Core.Serializables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Community.Client.Components
{
    public class ChatData : MonoBehaviour
    {
        [SerializeField]
        ClientData client;

        public InputField InputField;
        public Text text;
        public bool isActive = false;
        public KeyCode keyCode = KeyCode.Return;
        public void Send(string message)
        {
           if(ClientCallBlack.isConnected)
            {
                if (InputField.text.Length < 1) return;
                client.SendPacket(new ChatPacket() { Id = ClientManager.manager.playersManager._clientPlayer.id, text = InputField.text }, LiteNetLib.DeliveryMethod.ReliableOrdered);
                InputField.text = "";
                
            }
        }
    }
}
