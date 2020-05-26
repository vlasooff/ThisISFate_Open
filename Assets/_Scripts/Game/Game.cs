using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Game : ComponentSystem
{
    World clientWorld;
    // Start is called before the first frame update
    protected override void OnCreate()
    {
        base.OnCreate();
       
            Debug.Log("[Client] Client create world"); 
            clientWorld = CreateClientWorld(EntityManager.World, "Client");
  
            Debug.Log("[Client] Server create world"); 
            clientWorld = CreateServerWorld(EntityManager.World, "Server"); 
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        
    }
    public static World CreateClientWorld(World defaultWorld, string name)
    {
#if UNITY_SERVER
            throw new NotImplementedException();
#else
        var world = new World(name);
        var initializationGroup = world.GetOrCreateSystem<ClientInitializationSystemGroup>();
        var simulationGroup = world.GetOrCreateSystem<ClientSimulationSystemGroup>();
        //var presentationGroup = world.GetOrCreateSystem<NetWorkClientSystemGroup>();
        initializationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<NetWorkClientSystem>());
        initializationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<NetEntitySystem>());
        initializationGroup .AddSystemToUpdateList(world.GetOrCreateSystem<ChatSystem>());
        //foreach (var systemType in s_State.ClientInitializationSystems)
        //{
        //    var system = world.GetOrCreateSystem(systemType);
        //    initializationGroup.AddSystemToUpdateList(system);
        //}
        //foreach (var systemType in s_State.ClientSimulationSystems)
        //{
        //    var system = world.GetOrCreateSystem(systemType);
        //    simulationGroup.AddSystemToUpdateList(system);
        //}
        //foreach (var systemType in s_State.ClientPresentationSystems)
        //{
        //    var system = world.GetOrCreateSystem(systemType);
        //    presentationGroup.AddSystemToUpdateList(system);
        //}
        //foreach (var systemParentType in s_State.ClientChildSystems)
        //{
        //    var system = world.GetOrCreateSystem(systemParentType.Item1);
        //    var group = world.GetOrCreateSystem(systemParentType.Item2) as ComponentSystemGroup;
        //    group.AddSystemToUpdateList(system);
        //}
        initializationGroup.SortSystemUpdateList();
        simulationGroup.SortSystemUpdateList();
       // presentationGroup.SortSystemUpdateList();
        defaultWorld.GetOrCreateSystem<TickClientInitializationSystem>().AddSystemToUpdateList(initializationGroup);
        defaultWorld.GetOrCreateSystem<TickClientSimulationSystem>().AddSystemToUpdateList(simulationGroup);
       // defaultWorld.GetOrCreateSystem<TickNetUpdateSystem>().AddSystemToUpdateList(presentationGroup);
        return world;
#endif
    }

    public static World CreateServerWorld(World defaultWorld, string name)
    {
#if UNITY_SERVER
            throw new NotImplementedException();
#else
        var world = new World(name);
        var initializationGroup = world.GetOrCreateSystem<ServerInitializationSystemGroup>();
        var simulationGroup = world.GetOrCreateSystem<ServerSimulationSystemGroup>(); 
        initializationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<NetServerSystem>());
        initializationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<EntityNetSystem>());
        initializationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ChatSystem>());
        initializationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SpawnSystem>());
        //foreach (var systemType in s_State.ClientInitializationSystems)
        //{
        //    var system = world.GetOrCreateSystem(systemType);
        //    initializationGroup.AddSystemToUpdateList(system);
        //}
        //foreach (var systemType in s_State.ClientSimulationSystems)
        //{
        //    var system = world.GetOrCreateSystem(systemType);
        //    simulationGroup.AddSystemToUpdateList(system);
        //}
        //foreach (var systemType in s_State.ClientPresentationSystems)
        //{
        //    var system = world.GetOrCreateSystem(systemType);
        //    presentationGroup.AddSystemToUpdateList(system);
        //}
        //foreach (var systemParentType in s_State.ClientChildSystems)
        //{
        //    var system = world.GetOrCreateSystem(systemParentType.Item1);
        //    var group = world.GetOrCreateSystem(systemParentType.Item2) as ComponentSystemGroup;
        //    group.AddSystemToUpdateList(system);
        //}
        initializationGroup.SortSystemUpdateList();
        simulationGroup.SortSystemUpdateList();
        //presentationGroup.SortSystemUpdateList();
        defaultWorld.GetOrCreateSystem<TickServerInitializationSystem>().AddSystemToUpdateList(initializationGroup);
        defaultWorld.GetOrCreateSystem<TickServerSimulationSystem>().AddSystemToUpdateList(simulationGroup);
       // defaultWorld.GetOrCreateSystem<TickServerUpdateSystem>().AddSystemToUpdateList(presentationGroup);
        return world;
#endif
    }
}
