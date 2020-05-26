using System.Collections.Generic;
using Unity.Entities;
using Unity.Profiling;

[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ServerInitializationSystemGroup : InitializationSystemGroup
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
public class ServerSimulationSystemGroup : ComponentSystemGroup
{
    private BeginSimulationEntityCommandBufferSystem m_beginBarrier; 
    private uint m_previousServerTick;
    private float m_previousServerTickFraction;
    private ProfilerMarker m_fixedUpdateMarker;

    protected override void OnCreate()
    {
        m_beginBarrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_fixedUpdateMarker = new ProfilerMarker("ServerFixedUpdate");
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


        base.OnUpdate();


    }
}
