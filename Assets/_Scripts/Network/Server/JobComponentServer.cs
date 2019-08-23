using Community.Core;
using LiteNetLib;
using LiteNetLib.Utils;
using Network.Core.Attributes;
using Unity.Entities;
using UnityEngine;

namespace Community.Server
{/// <summary>
/// Плохо или хорошо?
/// </summary>
    public abstract class ComponentJobServer : JobComponentSystem
    {
        public bool Active = false;
        protected ComponentJobServer()
        { }

        /// <summary> 
        /// Доп отправка логов
        /// </summary>
        protected virtual bool IsLog
        {
            get
            {
                return true;
            }

        }

        /// <summary>
        /// Имя системы
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.ToString();
            }
        }
        /// <summary>
        /// Тестовый logger
        /// </summary>
        /// <param name="text"></param>
        public virtual void LogDev(string text)
        {
            if (IsLog)
                LogModule(text);
        }
        protected override void OnStartRunning()
        {
            base.OnStartRunning(); 
            ServerCallBlack.onStartServer += OnStartServer;
            ServerCallBlack.onStartedServer += onStartedServer;
            ServerCallBlack.onConnectedPlayer += onConnectedPlayer;
            ServerCallBlack.onDisconnectedPlayer += OnDisconectedPlayer;
            ServerCallBlack.onUpdateState += onUpdateStateServer;
            ServerCallBlack.onShutdownServer += OnShutdown;
        }
        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            ServerCallBlack.onStartServer -= OnStartServer;
            ServerCallBlack.onStartedServer-= onStartedServer;
            ServerCallBlack.onConnectedPlayer -= onConnectedPlayer;
            ServerCallBlack.onDisconnectedPlayer -= OnDisconectedPlayer;
            ServerCallBlack.onUpdateState -= onUpdateStateServer;
            ServerCallBlack.onShutdownServer -= OnShutdown;
        }

        protected virtual void OnShutdown()
        {
        }

        protected virtual void OnDisconectedPlayer(NetPeer peer, DisconnectInfo info)
        {
        }

        protected virtual void onConnectedPlayer(NetPeer peer)
        {
        }
        

        protected virtual void onStartedServer(NetPacketProcessor _packetProcessor)
        {
        }
        protected virtual void onUpdateStateServer(NetDataWriter dataWriter)
        {
            
        }

        protected virtual void OnStartServer(NetPacketProcessor _packetProcessor)
        {
        }

        /// <summary>
        /// Logger системы
        /// </summary>
        /// <param name="text"></param>
        public virtual void LogModule(object text)
        {
            Debug.Log(text);
        }
        public virtual NetDataWriter WritePacket<T>(T packet) where T : class, new()
        {
            return ServerManager.manager.serverProxy.WritePacket(packet);

        }
        public virtual NetDataWriter WritePacket<T>(T packet,NetDataWriter writer) where T : class, new()
        {
            return ServerManager.manager.serverProxy.WritePacket(packet, writer); 

        }
        public virtual NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
        {
            return ServerManager.manager.serverProxy.WriteSerializable(type, packet);
        }
        public virtual NetDataWriter WriteSerializable<T>(PacketSystem system, byte typeSystem, T packet) where T : struct, INetSerializable
        {
            return ServerManager.manager.serverProxy.WriteSerializable(system, typeSystem, packet);
        }

    }
}
