using Unity.Entities;
using UnityEngine;

namespace Community.Server.Components
{
    [System.Serializable]
    public struct PlayerID : IComponentData
    {
        public readonly ushort id;
        public readonly ulong SteamID;
        public NativeString64 username;
        public PlayerID(ushort UserID, ulong steamid,string name)
        {
            id = UserID;
            SteamID = steamid;
            username = new NativeString64(name);
        }
    }
    [System.Serializable]
    public struct PlayerMotor : IComponentData
    {
        public float _speed;
        public readonly byte jumpForge;
        public Vector3 _position;
        public float _rotation;
        public Vector3 movingDir;
        public bool IsSimulate;

        public PlayerMotor(Vector3 position, float rotation)
        {
            _position = position;
            _rotation = rotation;
            _speed = 2;
            jumpForge = 3;
            movingDir = Vector3.zero;
            IsSimulate = false;
        }
    }
    [System.Serializable]
    public struct PlayerRegion : IComponentData
    {
        public ushort id; 

        public PlayerRegion(ushort idRegion)
        {
            id = idRegion;
        }
    }
    [System.Serializable]
    public struct PlayerCitizen : IComponentData
    {

        public byte permissionID;

        public PlayerCitizen(byte permID)
        {
            permissionID = permID;
        }
    }
    [System.Serializable]
    public struct PlayerCustom : IComponentData
    {

        public byte permissionID;

        public PlayerCustom(byte permID)
        {
            permissionID = permID;
        }
    }
}