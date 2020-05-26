 
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
[System.Serializable]
[GenerateAuthoringComponent]
public struct CmdConnectClient : IComponentData
{  
    public NativeString64 ip;
    public ushort port;
}
 
 