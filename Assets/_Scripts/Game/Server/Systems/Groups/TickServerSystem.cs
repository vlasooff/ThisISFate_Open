using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

[AlwaysUpdateSystem]
public class TickServerInitializationSystem : ComponentSystemGroup
{
    public override void SortSystemUpdateList()
    {
    }
}   
[AlwaysUpdateSystem]
public class TickServerUpdateSystem : ComponentSystemGroup
{
    public override void SortSystemUpdateList()
    {
    }
}
[AlwaysUpdateSystem]
public class TickServerSimulationSystem : ComponentSystemGroup
{
    public override void SortSystemUpdateList()
    {
    }
}
