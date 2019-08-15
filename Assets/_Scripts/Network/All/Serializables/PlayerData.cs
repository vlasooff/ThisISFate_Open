using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib.Utils;
using UnityEngine;

namespace Core.Serializables
{
    public struct NetPlayerCommand : INetSerializable
    {
        public NetPlayerInputs inputs;
        public Vector3 position;
        public Quaternion rotation;
        public float TimeStep;
        public NetPlayerCommand(NetDataReader reader)
        {
            inputs = new NetPlayerInputs(reader);
            position = reader.GetVector3();
            rotation = reader.GetQuaternion();
            TimeStep = reader.GetFloat();
        }
        public void Deserialize(NetDataReader reader)
        {
            inputs = new NetPlayerInputs(reader);
            position = reader.GetVector3();
            rotation = reader.GetQuaternion();
            TimeStep = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            inputs.Serialize(writer);
            writer.Put(position);
            writer.Put(rotation);
            writer.Put(TimeStep);
            
        }
    }
    public struct NetPlayerResultat : INetSerializable
    {
        public int indexPlayer;
        public Vector3 position;
        public Quaternion rotation;
        public float timestep;
    
        public void Deserialize(NetDataReader reader)
        {
            indexPlayer = reader.GetInt();
            position = reader.GetVector3();
            rotation = reader.GetQuaternion();
            timestep = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(indexPlayer);
            writer.Put(position);
            writer.Put(rotation);
            writer.Put(timestep);
        }
    }

    public struct NetPlayerInputs : INetSerializable
    {
        public bool forward;
        public bool backward;
        public bool left;
        public bool right;
        public bool jump;
        public bool ctrl;
        public bool shift;

        public float yaw;
        public float pitch;
        public NetPlayerInputs(NetDataReader reader)
        {
            yaw = reader.GetFloat();
            pitch = reader.GetFloat();
            ctrl = reader.GetBool();
            shift = reader.GetBool();
            jump = reader.GetBool();
            forward = reader.GetBool();
            backward = reader.GetBool();
            left = reader.GetBool();
            right = reader.GetBool();
        }
        public void Deserialize(NetDataReader reader)
        {

            yaw = reader.GetFloat();
            pitch = reader.GetFloat();
            ctrl = reader.GetBool();
            shift = reader.GetBool();
            jump = reader.GetBool();
            forward = reader.GetBool();
            backward = reader.GetBool();
            left = reader.GetBool();
            right = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(yaw);
            writer.Put(pitch);
            writer.Put(ctrl);
            writer.Put(shift);
            writer.Put(jump);
            writer.Put(forward);
            writer.Put(backward);
            writer.Put(left);
            writer.Put(right);
        }
    }
}
