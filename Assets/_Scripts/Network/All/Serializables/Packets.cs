
using LiteNetLib.Utils;
using Unity.Collections;
using Unity.Entities;
using UnityEngine; 

namespace Community.Core
{
    public class PlayerLeavedPacket
    {
        public ushort Id { get; set; }
    }
    public enum PacketType : byte
    {
        SystemSync,
        ServerState,
        Serialized,
        Shoot,
        Event,

    }
 
    public class JoinPacket
    {
        public string UserName { get; set; }
        public ulong steamid { get; set; }
        public string password { get; set; }
    }

    public class PlayerJoinedPacket
    {
        public string UserName { get; set; }
        public bool NewPlayer { get; set; }
        public ushort id { get; set; }
        public ulong steamid { get; set; }
        public RemoteCustomPlayer player { get; set; }
        public CustomCharacter custom { get; set; }
        public CharacterCustomBody customBody { get; set; }
        public CharacterCustomHead customHead { get; set; }
    }
    public class PlayerAccountPacket
    {
        public PlayerAccountPacket(EUserState Estate)
        {
            state = (byte)Estate;
        }
        public byte state { get; set; }
        public PlayerAccountPacket()
        {
        }

    }
    public enum EUserState : byte
    {
        login, create, wrongPassword
    }
    #region EditorPlayer
   
     
    public class CharacterDefaultPacket
    {
        public byte massa { get; set; }
        public ushort id_shirt_man { get; set; }
        public ushort id_pants_man { get; set; }
        public ushort id_shirt_F { get; set; }
        public ushort id_pants_F { get; set; }
        public CharacterDefaultPacket()

        { }
   
    }
    public class CreateCharacterPacket
    {
        public Color32 Color_beard { get; set; }
        public Color32 Color_Hair { get; set; }
        public Color32 Color_eyes { get; set; }
        public Color32 Color_lips { get; set; }
        public Color32 Color_body { get; set; }
        public byte id_hair { get; set; } //причёстка 
        public byte id_beard { get; set; } //борода
        public byte id_head { get; set; } //лицо
        public byte id_body { get; set; } //лицо
        public byte id_pants { get; set; } 
        public byte beard_size { get; set; }//лицо
        public bool Gender { get; set; } 
    }
    #endregion
    //Manual serializable packets
    #region Inventory 


    public class CommandAddItem
    { 
        public int index { get; set; }

    }
    public class CommandGiveItem
    {
        public ushort id { get; set; }

    }
    public class SpawnItemWorldPacket
    {
        public ushort id { get; set; }
        public ushort index { get; set; }

    }
    public class DestroyItemWorldPacket
    {
        public ushort index { get; set; } 

    }
    [System.Serializable]
    public struct ItemWorld : INetSerializable
    {
        public int Index;
        public ushort Id;
        public Vector3 position;
        public float rotation;


        public ItemWorld(int index, ushort id, Vector3 vector3, float Y)
        {
            Index = index;
            Id = id;
            position = vector3;
            rotation = Y;
        }
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Index);
            writer.Put(Id);
            writer.Put(position);
            writer.Put(rotation);
        }

        public void Deserialize(NetDataReader reader)
        {
            Index = reader.GetInt();
            Id = reader.GetUShort();
            position = reader.GetVector3();
            rotation = reader.GetFloat();
        }

    }
    public class WorldItemsPacket
    {
        public ItemWorld[] items { get; set; }

    }
     

   
    public enum EBodyIndex : byte
    {
        hats,mask,glasses,shirt,vest,backpack,pants
    }
   

    #endregion

    #region playeysSystem
    public class UpdatePlayersPacket
    {
        public ushort ServerTick { get; set; }
        public UpdatePlayerPacket[] updatePlayers { get; set; }
    }
    public struct UpdatePlayerPacket : INetSerializable
    {
        public ushort Id { get; set; }
        public Vector3 position { get; set; }
        public float rotation { get; set; }
        public Vector2 animation { get; set; }
        public byte speed { get; set; }


        public UpdatePlayerPacket(ushort id, Vector3 vector3, Vector2 vector, float Y, float cSpeed)
        {
            Id = id;
            position = vector3;
            rotation = Y;
            animation = vector;
            speed = (byte)cSpeed;
        }
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(position);
            writer.Put(rotation);
            writer.Put(animation);
            writer.Put(speed);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetUShort();
            position = reader.GetVector3();
            rotation = reader.GetFloat();
            animation = reader.GetVector2();
            speed = reader.GetByte();
        }

    }
    public class LoadingPlayerPacket
    {
        public ushort PlayerId { get; set; }
        public SpawnCurrentPlayer playerState { get; set; }
    }
    public class UnloadingPlayerPacket
    {
        public ushort Id { get; set; }
    }
    public class NewPlayerJoined
    {
        public string username { get; set; }
        public ushort PlayerId { get; set; }
        public ulong steamid { get; set; }
    }

    public struct SpawnCurrentPlayer : INetSerializable
    {
        public string username;
        public ushort PlayerId;
        public Vector3 Position;
        public float Rotation;
        public CustomCharacter custom;
        public CharacterCustomHead head;
        public CharacterCustomBody body;

     
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(username);
            writer.Put(PlayerId);
            writer.Put(Position);
            writer.Put(Rotation);
            custom.Serialize(writer);
            head.Serialize(writer);
            body.Serialize(writer);

        }

        public void Deserialize(NetDataReader reader)
        {
            username = reader.GetString();
            PlayerId = reader.GetUShort();
            Position = reader.GetVector3();
            Rotation = reader.GetFloat();
            custom.Deserialize(reader);
            head.Deserialize(reader);
            body.Deserialize(reader);
        }
    }
    #endregion

    [System.Serializable]
    public struct CustomCharacter : IComponentData, INetSerializable
    {

        public byte massa_m;
        public byte massa;
        public bool Gender;
        public byte time; 

        public CustomCharacter(byte m_masssa, byte _massa, byte _time,bool gender)
        {
            massa_m = m_masssa;
            massa = _massa;
            time = _time;
            Gender = gender;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(massa_m);
            writer.Put(massa);
            writer.Put(time);
            writer.Put(Gender);
        }
        public void Deserialize(NetDataReader reader)
        {
            massa_m = reader.GetByte();
            massa = reader.GetByte();
            time = reader.GetByte();
            Gender = reader.GetBool();
        }

    }
    [System.Serializable]
    public struct CharacterCustomHead : IComponentData, INetSerializable
    {
        public byte beardSize; //лицо
        public Color32 Color_beard;
        public Color32 Color_Hair;
        public Color32 Color_eyes;
        public Color32 Color_lips;
        public byte id_hair; //причёстка 
        public byte id_beard; //борода
        public byte id_head;  //лицо

        public CharacterCustomHead(Color32 beard, Color32 Hair, Color32 eyes, Color32 lips, byte idhair, byte idbeard, byte idHead, byte SizeBeard)
        {
            Color_beard = beard;
            Color_Hair = Hair;
            Color_eyes = eyes;
            Color_lips = lips;
            id_beard = idbeard;
            id_hair = idhair;
            id_head = idHead;
            beardSize = SizeBeard;

        }
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(beardSize);
            writer.Put(Color_beard);
            writer.Put(Color_eyes);
            writer.Put(Color_Hair);
            writer.Put(Color_lips);
            writer.Put(id_hair);
            writer.Put(id_head);
            writer.Put(id_beard);
        }

        public void Deserialize(NetDataReader reader)
        {
            beardSize = reader.GetByte();
            Color_beard = reader.GetColor32();
            Color_eyes = reader.GetColor32();
            Color_Hair = reader.GetColor32();
            Color_lips = reader.GetColor32();
            id_hair = reader.GetByte();
            id_head = reader.GetByte();
            id_beard = reader.GetByte();

        }

    }
    [System.Serializable]
    public struct RemoteCustomPlayer : INetSerializable
    { 
        public ushort[] idlist;
            
        public RemoteCustomPlayer( ushort[] m_idlist)
        { 
            idlist = m_idlist;
        }
       
        public void Deserialize(NetDataReader reader)
        { 
            idlist =  reader.GetUShortArray();
        }

        public void Serialize(NetDataWriter writer)
        { 
                  writer.PutArray(idlist); 
             
        }
    }
    [System.Serializable]
    public struct CharacterCustomBody : IComponentData, INetSerializable
    {

        public Color32 Color_body;

        public byte id_body; //тело
        public byte id_pants; //штаны   

        public CharacterCustomBody(Color body, byte idPants, byte idBody)
        {
            Color_body = body;
            id_body = idBody;
            id_pants = idPants;
        }

        public void Deserialize(NetDataReader reader)
        {
            Color_body = reader.GetColor32();
            id_body = reader.GetByte();
            id_pants = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Color_body);
            writer.Put(id_body);
            writer.Put(id_pants);
        }
    }
}
