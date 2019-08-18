using Community.Core;
using Community.Core.Serializables;
using LiteNetLib;
using LiteNetLib.Utils;
using Network.Core;
using Network.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Components
{ 
    public class ServerProxy : MonoBehaviour 
    {
        public NetManager _netManager;
        public  NetPacketProcessor _packetProcessor;
        public LogicTimer _logicTimer;
        private readonly NetDataWriter _cachedWriter = new NetDataWriter();

        public void StartServer(ServerInfoProxy info)
        {

            ServerCallBlack.onStartedServer?.Invoke(_netManager);
            info.config = new ServerConfig("Test_1");
            info.config.Load();
            if (_netManager.IsRunning)
                return;
            _netManager.SimulateLatency = info.SimulateLatency;
            _netManager.SimulatePacketLoss = info.SimulatePacketLoss; 
            _netManager.MergeEnabled = true;
            _netManager.NatPunchEnabled = true;
            _netManager.Start(info.config.port);

            EventManager.EventMethod(typeof(EventStartServer), new object[0]);
        }

        public NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
        {
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)type);
            packet.Serialize(_cachedWriter);
            return _cachedWriter;
        }
        public NetDataWriter WriteSerializable<T>(PacketSystem type,byte typeSystem, T packet) where T : struct, INetSerializable
        {
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)PacketType.SystemSync);
            _cachedWriter.Put((byte)type);
            _cachedWriter.Put(typeSystem);
            packet.Serialize(_cachedWriter);
            return _cachedWriter;
        }
        public NetDataWriter WritePacket<T>(T packet) where T : class, new()
        {
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)PacketType.Serialized);
            _packetProcessor.Write(_cachedWriter, packet);
            return _cachedWriter;
        }
        public NetDataWriter WritePacket<T>(T packet,NetDataWriter writer) where T : class, new()
        { 
            _packetProcessor.Write(writer, packet);
            return _cachedWriter;
        }
    } 
}
