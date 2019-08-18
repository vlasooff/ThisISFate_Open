using Community.Client.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Systems
{
    public delegate void OnPeerResponse(PlayerManager manager, NetDataWriter packet);
    public delegate void OnUpdateState(NetDataWriter packet);
    public delegate void OnUpdateForPlayer();
    public class SyncStateServer : ComponentClient
    {
        public static OnUpdateState onUpdateMsec;
        public static OnUpdateState onUpdateSec;
        public static OnUpdateState onUpdateMinute;

        private float timeUpdateMSec;
        private float timeUpdateSec;
        private float timeUpdateMinute;
        private PlayersManager playersManager;
        private NetDataWriter Hash_UpdateMsec;
        private NetDataWriter Hash_UpdateSec;
        private NetDataWriter Hash_UpdateMinute;

        protected override void OnStartClient()
        {
            base.OnStartClient();
            playersManager = ClientManager.manager.playersManager;

        }

        private void UpdateSec()
        {
        }

        protected override void OnUpdate()
        {
            if (ClientCallBlack.isConnected)
            {
                if ((Time.time - timeUpdateMSec) > 0.1f)
                {
                    timeUpdateMSec = Time.time;
                    onUpdateMsec?.Invoke(Hash_UpdateMsec);
                    if (Hash_UpdateMsec.Length > 0)
                    { 
                        SendPacket(Hash_UpdateMsec, DeliveryMethod.ReliableOrdered);
                        Hash_UpdateMinute.Reset();
                    }
                }
                if ((Time.time - timeUpdateSec) > 1f)
                {
                    timeUpdateSec = Time.time; 
                    onUpdateSec?.Invoke(Hash_UpdateSec);
                    if (Hash_UpdateSec.Length > 0)
                    { 
                        SendPacket(Hash_UpdateSec, DeliveryMethod.ReliableOrdered);
                        Hash_UpdateMinute.Reset();
                    }
                }
                if ((Time.time - timeUpdateMinute) > 60f)
                {
                    timeUpdateMinute = Time.time;
                    onUpdateMinute?.Invoke(Hash_UpdateMinute);
                    if (Hash_UpdateMinute.Length > 0)
                    { 
                        SendPacket(Hash_UpdateMinute, DeliveryMethod.ReliableOrdered);
                        Hash_UpdateMinute.Reset();
                    }
                }
            }
        }
    }
}
