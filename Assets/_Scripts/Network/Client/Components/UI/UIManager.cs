using Community.Core;
using Community.Core.Serializables;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Community.Client.Components
{
    public class UIManager : MonoBehaviour
    {
        public UISlider slider;
        public InputField username;
        public InputField passworld;
        public GameObject createCharater;
        public GameObject login;
        public GameObject UIServer;
        public ClientData m_dataClient;
        public ClientDataSteam m_dataClientSteam;
        public Text message;
        CreateCharacterPacket createCharacter = new CreateCharacterPacket();
        public void Login()
        {
            m_dataClient.SendPacket(new JoinPacket { UserName = username.text, steamid = m_dataClientSteam.steamid , password = passworld.text }, DeliveryMethod.ReliableOrdered);
        }
        public void RandomPlayerButton()

        {
            byte eyes = (byte)UnityEngine.Random.Range(0, 200);
            byte beard = (byte)UnityEngine.Random.Range(0, 200); 
            createCharacter.Color_eyes = new Color32(eyes, eyes, eyes,1);
            createCharacter.Color_beard = new Color32(beard, beard, beard, 1);
            createCharacter.Color_Hair = createCharacter.Color_beard;
            createCharacter.Color_lips = Color.red;
            createCharacter.beard_size = (byte)UnityEngine.Random.Range(0, 200);
            createCharacter.id_beard = (byte)UnityEngine.Random.Range(0, 1);
            createCharacter.id_hair = (byte)UnityEngine.Random.Range(0, 1);
            createCharacter.id_body = 1;
            createCharacter.id_pants = 1;
            createCharacter.id_head = 1;
            createCharacter.id_head = 1;
            createCharacter.id_head = 1;
            createCharacter.id_head = 1; 
            m_dataClient.SendPacket(createCharacter, DeliveryMethod.ReliableOrdered);
        }
        public void OnSetMan()
        {
            createCharacter.Gender = false;

        }
        public void OnSetWoman()
        {
            createCharacter.Gender = true;

        }
        public void OnLogin()
        {
            UIServer.SetActive(false);
        }
        public void Play()
        {
            m_dataClient.SendPacket(new JoinPacket { UserName = username.text, steamid = m_dataClientSteam.steamid, password = passworld.text }, DeliveryMethod.ReliableOrdered);
        }
        public void OnCreate()
        {
            createCharater.SetActive(true);
            login.SetActive(false);
        }
        public void OnProblem()
        {
            message.text = "Wrong password!";
            message.color = Color.red;
        }

    }
  
}
