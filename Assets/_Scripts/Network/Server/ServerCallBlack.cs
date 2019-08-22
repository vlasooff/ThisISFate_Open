using Community.Core;
using Community.Core.Serializables;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Community.Server
{
    public static class ServerCallBlack
    {
        public static OnStartedServer onStartedServer;
        public static OnStartServer onStartServer;
        public static bool isServerRun = false;
        public static OnConnectedPlayer onConnectedPlayer;
        public static OnDisconnectedPlayer onDisconnectedPlayer;    
        public static OnShutdownServer onShutdownServer;
        public static OnUpdateState onUpdateState;
        public static OnCreatePlayer onCreatePlayer;

        public static void RegisterNestedTypes(NetPacketProcessor _packetProcessor)
        {
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), r => r.GetVector3());
            // регистрируем auto serializable PlayerState
            _packetProcessor.RegisterNestedType<SpawnCurrentPlayer>();
            _packetProcessor.RegisterNestedType<PlayerData>();
            _packetProcessor.RegisterNestedType<RemotePlayersData>(); 
        }
    }
    public delegate void OnConnectedPlayer(NetPeer peer);
    public delegate void OnDisconnectedPlayer(NetPeer peer, DisconnectInfo info);
    public delegate void OnShutdownServer();
    public delegate void OnUpdateState(NetDataWriter dataWriter);
    public delegate void OnStartedServer(NetPacketProcessor _packetProcessor);
    public delegate void OnStartServer(NetPacketProcessor _packetProcessor);
}
