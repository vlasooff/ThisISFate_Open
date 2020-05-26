
using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct ServerStart : IComponentData
{
    public ushort port;
    public NativeString64 serverIp;
    public bool SimulateLatency;
    public bool SimulatePacketLoss;
}
