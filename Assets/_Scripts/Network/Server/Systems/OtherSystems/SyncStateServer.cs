using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using LiteNetLib;
using LiteNetLib.Utils;
using Community.Server.Components;

namespace Community.Server.Systems
{
    public delegate void OnUpdateState(NetDataWriter packet); 
    public class SyncStateServer : ComponentServer
    {
        public static OnUpdateState onUpdateMsec;
        public static OnUpdateState onUpdateSec;
        public static OnUpdateState onUpdateMinute;

        private float timeUpdateMSec;
        private float timeUpdateSec;
        private float timeUpdateMinute;
        private ServerProxy m_proxy;

        protected override void OnStartServer(NetManager manager)
        {
            base.OnStartServer(manager);
            m_proxy = ServerManager.manager.serverProxy;

        }

        protected override void OnUpdate()
        { 
            
            if(ServerCallBlack.isServerRun)
            {
                if ((Time.time - timeUpdateMSec) > 0.1f)
                {
                    timeUpdateMSec = Time.time;
                    NetDataWriter writer = new NetDataWriter();
                    onUpdateMsec?.Invoke(writer);
                    if (writer.Length > 0)
                        m_proxy._netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                }
                if ((Time.time - timeUpdateSec) > 1f)
                {
                    timeUpdateSec = Time.time;
                    NetDataWriter writer = new NetDataWriter();
                    onUpdateSec?.Invoke(writer);
                    if (writer.Length > 0)
                        m_proxy._netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                }
                if ((Time.time - timeUpdateMinute) > 60f)
                {
                    timeUpdateMinute = Time.time;
                    NetDataWriter writer = new NetDataWriter();
                    onUpdateMinute?.Invoke(writer);
                    if(writer.Length >0)
                    m_proxy._netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                }
            }
        }
    }

}