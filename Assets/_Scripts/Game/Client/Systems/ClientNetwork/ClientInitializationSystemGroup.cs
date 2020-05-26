using System.Collections.Generic;
using Unity.Entities;
using Unity.Profiling;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ClientInitializationSystemGroup : InitializationSystemGroup
{
    protected override void OnUpdate()
    {
#pragma warning disable 618
        // we're keeping World.Active until we can properly remove them all
        var defaultWorld = World.Active;
        World.Active = World;
        base.OnUpdate();
        World.Active = defaultWorld;
#pragma warning restore 618
    }
}



[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ClientSimulationSystemGroup : ComponentSystemGroup
{
    private BeginSimulationEntityCommandBufferSystem m_beginBarrier;
    private NetworkReceiveSystemGroup m_NetworkReceiveSystemGroup;  
    private uint m_previousServerTick;
    private float m_previousServerTickFraction;
    private ProfilerMarker m_fixedUpdateMarker;

    protected override void OnCreate()
    {
        m_beginBarrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_fixedUpdateMarker = new ProfilerMarker("ClientFixedUpdate");
    }

    protected List<ComponentSystemBase> m_systemsInGroup = new List<ComponentSystemBase>();

    public override IEnumerable<ComponentSystemBase> Systems => m_systemsInGroup;

    protected override void OnUpdate()
    {


#pragma warning disable 618
        var defaultWorld = World.Active;
        World.Active = World;
#pragma warning restore 618
        m_beginBarrier.Update();
        if(m_NetworkReceiveSystemGroup != null)
        m_NetworkReceiveSystemGroup.Update();


        base.OnUpdate();


    }
}

#if !UNITY_SERVER
#if !UNITY_CLIENT || UNITY_SERVER || UNITY_EDITOR 
#endif
[AlwaysUpdateSystem]
public class TickClientInitializationSystem : ComponentSystemGroup
{
    public override void SortSystemUpdateList()
    {
    }
}
#endif
#if !UNITY_SERVER
#if !UNITY_CLIENT || UNITY_SERVER || UNITY_EDITOR 
#endif
[AlwaysUpdateSystem]
public class NetworkReceiveSystemGroup : ComponentSystemGroup
{
    public override void SortSystemUpdateList()
    {
    }
}
#endif
#if !UNITY_SERVER
#if !UNITY_CLIENT || UNITY_SERVER || UNITY_EDITOR 
#endif
[AlwaysUpdateSystem]
public class TickClientSimulationSystem : ComponentSystemGroup
{
    public override void SortSystemUpdateList()
    {
    }
}
#endif
#if !UNITY_SERVER
#if !UNITY_CLIENT || UNITY_SERVER || UNITY_EDITOR
#endif
[AlwaysUpdateSystem]
public class TickNetUpdateSystem : ComponentSystemGroup
{
    public override void SortSystemUpdateList()
    {
    }
}
#endif
