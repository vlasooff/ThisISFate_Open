using Community.Core;
using Community.Core.Serializables;
using Community.Other;
using Network.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Client.Components
{
    [System.Serializable]
    public struct PlayerID : IComponentData
    {
        public readonly ushort id;
        public PlayerID(ushort UserID)
        {
            id = UserID;
        }
      
    }
    [System.Serializable]
    public struct PlayerMotor : IComponentData
    {
        public float scroll;
        public Vector3 positionCamera;
        public float rotation;
        public Vector3 movingDir; 
    }
    [System.Serializable]
    public struct RPlayerID : IComponentData
    {
        public readonly ushort id;
        public RPlayerID(ushort UserID)
        {
            id = UserID;
        }
    }
    [System.Serializable]
    public struct PlayerInfo : IComponentData
    { 
        public readonly ulong SteamID;
        public NativeString64 username;
        public PlayerInfo(ulong steamid, string name)
        { 
            SteamID = steamid;
            username = new NativeString64(name);
        }
    } 
    [System.Serializable]
    public struct RPlayerMotor : IComponentData
    {
        public Vector3 movingDir;
        public Vector3 newPosition;
        public float newRotation;
        public Vector2 smoothDeltaPosition;
        public Vector2 velocity;
        public byte speed;
        public RPlayerMotor(Vector3 moving,Vector3 Position,float  rotation)
        {
            movingDir = moving;
            newPosition = Position;
            newRotation = rotation;
            smoothDeltaPosition = Vector2.zero;
            velocity = Vector2.zero;
            speed = 2;
        }
    }
}
 
