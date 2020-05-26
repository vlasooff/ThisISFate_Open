using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct ClientConnection : IComponentData
{
    public ushort port;
    public NativeString64 serverIp;

}
