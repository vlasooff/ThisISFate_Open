using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Community.Server.Components
{
    [System.Serializable]
    public struct EntityNetID : IComponentData
    {
        public readonly ushort id;
        public EntityNetID(ushort UserID)
        {
            id = UserID;
        }
    }
    [System.Serializable]
    public struct EntitySteamID : IComponentData
    {
        public readonly  ulong id;
        public EntitySteamID(ulong UserID)
        {
            id = UserID;
        }
    }
    [System.Serializable]
    public struct EntityName : IComponentData
    {
        public readonly NativeString64 username;
        public EntityName(string user )
        {
            username = new NativeString64(user);
        }
    }
    public struct EntityNetPlayer : IComponentData
    {

    }
    public struct EntityPosition : IComponentData
    {
        public Vector3 position;
        public EntityPosition(Vector3 vector3) { position = vector3;  }

    }
    [System.Serializable] 
    public struct EntityMotor : IComponentData
    {
        public float _speed;
        public readonly byte jumpForge;
        public Vector3 _position;
        public float _rotation;
        public Vector3 movingDir;
        public bool IsSimulate;

        public EntityMotor(Vector3 position, float rotation)
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
    public struct EntityRegion : IComponentData
    {
        public ushort id;

        public EntityRegion(ushort idRegion)
        {
            id = idRegion;
        }
    }
    [System.Serializable]
    public struct EntityCitizen : IComponentData
    {

        public byte permissionID;

        public EntityCitizen(byte permID)
        {
            permissionID = permID;
        }
    }
    [System.Serializable]
    public struct ShirtCustomEntity : IComponentData
    {
        public ushort id_shirt;  

        public ShirtCustomEntity(ushort shirt)
        {
            id_shirt = shirt;
        }
    }
    [System.Serializable]
    public struct PantsCustomEntity : IComponentData
    {
        public ushort id;

        public PantsCustomEntity(ushort Pants)
        {
            id = Pants;
        }
    }
    [System.Serializable]
    public struct HatCustomEntity : IComponentData
    {
        public ushort id;

        public HatCustomEntity(ushort ids)
        {
            id = ids;
        }
    }
    [System.Serializable]
    public struct GlassesCustomEntity : IComponentData
    {
        public ushort id;

        public GlassesCustomEntity(ushort ids)
        {
            id = ids;
        }
    }
    [System.Serializable]
    public struct MaskCustomEntity : IComponentData
    {
        public ushort id;

        public MaskCustomEntity(ushort ids)
        {
            id = ids;
        }

    }
    [System.Serializable]
    public struct VestCustomEntity : IComponentData
    {
        public ushort id;

        public VestCustomEntity(ushort ids)
        {
            id = ids;
        }
    }
    [System.Serializable]
    public struct BackpackCustomEntity : IComponentData
    {
        public ushort id;

        public BackpackCustomEntity(ushort ids)
        {
            id = ids;
        }
    }
}