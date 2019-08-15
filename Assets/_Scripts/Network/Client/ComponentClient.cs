using Community.Client;
using Community.Core;
using Community.Core.Serializables; 
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Network
{
    public abstract class ComponentClient : ComponentSystem
    {
        protected ComponentClient()
        { }

        /// <summary>
        /// Доп отправка логов
        /// </summary>
        public virtual bool isLog
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Имя системы
        /// </summary>
        public virtual string Name => this.ToString();
        /// <summary>
        /// Тестовый logger
        /// </summary>
        /// <param name="text"></param>
        public virtual void LogDev(string text)
        {
            if (isLog)
                LogModule(text);
        }

        /// <summary>
        /// Logger системы
        /// </summary>
        /// <param name="text"></param>
        public virtual void LogModule(object text)
        {
            Debug.Log(text);
        }
        public void SendPacketSerializable<T>(PacketType type, T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            ClientManager.manager.clientData.SendPacketSerializable(type, packet, deliveryMethod);
        }

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
        {
            ClientManager.manager.clientData.SendPacket(packet, deliveryMethod);
        }
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            Debug.Log("[CLIENT]" + Name + " Started!");
            ClientCallBlack.OnStartClient += OnStartClient;
            ClientCallBlack.onStartedClient += OnStartedClient;
            ClientCallBlack.OnConnectedClient += OnConnectedPlayer;
            ClientCallBlack.OnDisconnectedClient += OnDisconectedPlayer;
            ClientCallBlack.OnShutdownClient += OnShutdown;

        }
        protected virtual void OnStartedClient()
        {

        }
        protected virtual void OnStartClient()
        {

        }
        protected virtual void OnConnectedPlayer()
        {

        }
        protected virtual void OnDisconectedPlayer()
        {

        }
        protected virtual void OnShutdown()
        {

        }
    }
} 