using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface VVClientNetEvents
{
     
    /// <summary>
    /// New remote peer connected to host, or client connected to remote host
    /// </summary>
    /// <param name="peer">Connected peer object</param>
    void OnPeerConnected(NetPeer peer);

    /// <summary>
    /// Peer disconnected
    /// </summary>
    /// <param name="peer">disconnected peer</param>
    /// <param name="disconnectInfo">additional info about reason, errorCode or data received with disconnect message</param>
    void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);

    /// <summary>
    /// Network error (on send or receive)
    /// </summary>
    /// <param name="endPoint">From endPoint (can be null)</param>
    /// <param name="socketError">Socket error</param>

    void OnDisconnected();

    void OnStartClient(NetManager manager,NetPacketProcessor netPacketProcessor);
    void OnStopClient();
}

public interface VVServerNetEvents
{

    /// <summary>
    /// New remote peer connected to host, or client connected to remote host
    /// </summary>
    /// <param name="peer">Connected peer object</param>
    void OnPeerConnected(NetPeer peer);

    /// <summary>
    /// Peer disconnected
    /// </summary>
    /// <param name="peer">disconnected peer</param>
    /// <param name="disconnectInfo">additional info about reason, errorCode or data received with disconnect message</param>
    void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);

    /// <summary>
    /// Network error (on send or receive)
    /// </summary>
    /// <param name="endPoint">From endPoint (can be null)</param>
    /// <param name="socketError">Socket error</param>

    void OnStopServer();
    void OnStartServer(NetManager manager, NetPacketProcessor _packetProcessor);
}