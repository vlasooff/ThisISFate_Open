using Community.Client;
using Community.Other;
using Community.Server;
using Community.Server.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using Network.All;
using Network.Core;
using Network.Core.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{ 
    public class Utility : MonoBehaviour
    {
        private bool isSnow = false;
        private bool isChat = false;
        [SerializeField]
        private bool isHost = false;
        public ushort[] ids;
        public ServerInfoProxy info; 
        string command = "localhost";
        public string test = "Vitaxa sADddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddaaaaaaaadadadadadadadadada";
        public  struct Test
        {
            public byte[] bytes;

           
        }
        [EasyButtons.Button]
        public void TestComp()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(new Color(25, 25, 25));
            writer.Put(new Color(21, 12, 13));
            byte[] compression = CompManager.Compress(writer.Data);
       
            byte[]  text = CompManager.Decompress(compression);
            NetDataReader reader = new NetDataReader(text);
            Debug.Log(writer.Length  + " = Comp: " + compression.Length);
            Debug.Log("Test: " + reader.GetColor()); Debug.Log("Test: " + reader.GetColor());
        }
        [EasyButtons.Button]
        public void Test2()
        {
         
        }
        public static IEnumerable<IPAddress> GetIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return (from ip in host.AddressList where !IPAddress.IsLoopback(ip) select ip).ToList();
        }
        private void Update()
        {
          
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isSnow) isSnow = false;
                else isSnow = true;
            }
        }
        public static IPAddress GetIPAddress2()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).Where(address =>
            address.AddressFamily == AddressFamily.InterNetwork).First();
        }  
        public void OnGUI()
        {
            if(isSnow)
            {
                GUILayout.BeginVertical();
                if (GUILayout.Button("Exit"))
                {
                    Application.Quit();
                }
                GUILayout.EndVertical();
            }
            if (!ClientCallBlack.isConnected)
            {
                GUILayout.BeginHorizontal();
                  GUILayout.Label("Commands line", GUILayout.Width(380));
                command = GUILayout.TextField(command);
                if (GUILayout.Button("Connect"))
                {
                    ClientManager.manager.clientData.Connect(command);
                }
                if (isHost) 
                if (GUILayout.Button("Host"))
                {
                    ServerManager.manager.serverProxy.StartServer(info);
                }
             
                GUILayout.EndHorizontal();
            }

        }
    }
    [System.Serializable]
    public class TestJson
    {
        public string nameServer;

        public ushort port;

        
    }
}
