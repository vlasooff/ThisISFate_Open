using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using LiteNetLib;
using LiteNetLib.Utils;
using Community.Server.Components;
using System;

namespace Community.Server.Systems
{
    public delegate void OnPeerResponse(EntityPlayerManager manager,NetDataWriter packet);
    public delegate void OnUpdateState(NetDataWriter packet);
    public delegate void OnUpdateForPlayer(); 
    public class SyncStateServer : ComponentServer
    {
        public static OnUpdateState onUpdateMsec;
        public static OnUpdateState onUpdateSec;
        public static OnUpdateState onUpdateMinute;
        public static OnUpdateForPlayer onPlayerMsec;
        public static OnUpdateForPlayer onPlayerSec;
        public static OnUpdateForPlayer onPlayerMinute;
        public static OnPeerResponse OnConnectResponse;
        public static OnPeerResponse OnCreatedPlayerResponse;
        public static OnPeerResponse OnDesConnectResponse;

        private float timeUpdateMSec;
        private float timeUpdateSec;
        private float timeUpdateMinute;
        private ServerProxy m_proxy;

        protected override void OnStartServer(NetPacketProcessor _packetProcessor )
        { 
            m_proxy = ServerManager.manager.serverProxy; 
            ServerCallBlack.onCreatePlayer += OnPlayerCreated;
        }

        NetDataWriter hash_spawnPlayer= new NetDataWriter();
        private void OnPlayerCreated(EntityPlayerManager player, bool isNew)
        {
            if (player != null)
            {
                OnCreatedPlayerResponse?.Invoke(player, hash_spawnPlayer);
                if (hash_spawnPlayer.Length > 0)
                {
                    player.peer.Send(hash_spawnPlayer, DeliveryMethod.ReliableOrdered);
                    
                }
                Debug.Log("Send " + hash_spawnPlayer.Length);
            }
            else Debug.LogError("[S] OnCreate Event not called");
        }


        protected override void OnDisconectedPlayer(NetPeer peer, DisconnectInfo info)
        {
         
            EntityPlayerManager player = (EntityPlayerManager)peer.Tag;
            if (player != null)
            {
                NetDataWriter writer = new NetDataWriter();
                OnDesConnectResponse?.Invoke(player, writer);
                if (writer.Length > 0)
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
            else Debug.LogError("[S] OnDesConnect Event not called");
        }

        protected override void onConnectedPlayer(NetPeer peer)
        {
            EntityPlayerManager player = (EntityPlayerManager)peer.Tag;
            if (player != null)
            {
                NetDataWriter writer = new NetDataWriter();
                OnConnectResponse?.Invoke(player, writer);
                if (writer.Length > 0)
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
            else Debug.LogError("[S] OnConnect Event not called");
        } 
        protected override void OnUpdate()
        { 
            
            if(ServerCallBlack.isServerRun)
            {
               
                if ((Time.time - timeUpdateMSec) > 0.1f)
                {
                    timeUpdateMSec = Time.time;
                    onPlayerMsec?.Invoke();
                    NetDataWriter writer = new NetDataWriter();
                    onUpdateMsec?.Invoke(writer);
                    if (writer.Length > 0)
                        m_proxy._netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                }
                if ((Time.time - timeUpdateSec) > 1f)
                {
                    timeUpdateSec = Time.time;
                    onPlayerSec?.Invoke();

                    NetDataWriter writer = new NetDataWriter();
                    onUpdateSec?.Invoke(writer);
                    if (writer.Length > 0)
                        m_proxy._netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                }
                if ((Time.time - timeUpdateMinute) > 60f)
                {
                    timeUpdateMinute = Time.time;
                    onPlayerMinute?.Invoke();
                    NetDataWriter writer = new NetDataWriter();
                    onUpdateMinute?.Invoke(writer);
                    if(writer.Length >0)
                    m_proxy._netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                }
            }
        }
    }

}