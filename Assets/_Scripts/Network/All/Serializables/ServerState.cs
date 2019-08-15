using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Core.Serializables
{

    public struct ServerState : INetSerializable
    {
        public ushort Tick;
        public ushort LastProcessedCommand;

        public int PlayerStatesCount;
        public int StartState; //server only

        public SpawnCurrentPlayer[] PlayerStates;

        //tick
        public const int HeaderSize = sizeof(ushort) * 2;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Tick);
            writer.Put(LastProcessedCommand);

            for (int i = 0; i < PlayerStatesCount; i++)
                PlayerStates[StartState + i].Serialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            Tick = reader.GetUShort();
            LastProcessedCommand = reader.GetUShort();

        //    PlayerStatesCount = reader.AvailableBytes / SpawnCurrentPlayer.Size;
         //   if (PlayerStates == null || PlayerStates.Length < PlayerStatesCount)
         //       PlayerStates = new SpawnCurrentPlayer[PlayerStatesCount];
         //   for (int i = 0; i < PlayerStates.Length; i++)
           //     PlayerStates[i].Deserialize(reader);
        }
    } 
}
