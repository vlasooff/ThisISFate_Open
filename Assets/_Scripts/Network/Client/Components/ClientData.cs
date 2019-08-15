using Community.Core;
using Community.Core.Serializables;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components
{
    public class ClientData : MonoBehaviour
    {
        public NetManager _netManager;
        public NetDataWriter _writer;
        public static NetPacketProcessor _packetProcessor; 
        public ENetworkClient status;
        public string _userName;
        public NetPeer server;
        private int _ping;
        public ServerBaseState state;
        public bool Connected => server?.ConnectionState == ConnectionState.Connected;
        public static LogicTimer LogicTimer { get;  set; }
        public ushort port = 10515;
        public string serverIp = "localhost";
         

       

        public void Connect(string _serverIp, int _port)
        {
            ClientCallBlack.onStartedClient?.Invoke();
            _netManager.Connect(_serverIp, _port, "sample_app");
            ClientCallBlack.OnStartClient?.Invoke();
        }
        public void Connect(string _serverIp)
        {
           Connect(_serverIp, port);
        }
        [EasyButtons.Button]
        public void Connect()
        {
            Connect(serverIp, port);
        } 
        [EasyButtons.Button]
        public void  Disconnected()
        {
            _netManager.DisconnectPeer(_netManager.FirstPeer);
        }
        public void SendPacketSerializable<T>(PacketType type, T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            if ( server == null)
                return;
           _writer.Reset();
             _writer.Put((byte)type);
            packet.Serialize( _writer);
          server.Send( _writer, deliveryMethod);
        }

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
        {
            if ( _netManager == null)
                return;
        _writer.Reset();
             _writer.Put((byte)PacketType.Serialized);
           _packetProcessor.Write( _writer, packet);
          _netManager.FirstPeer.Send( _writer, deliveryMethod);
        }

    }
}

 