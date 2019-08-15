using Community.Server.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Community.Server.Systems
{ 
    public class RegionSystem : JobComponentSystem
    {
        private RegionsComponent regionsComponent;
        private PlayersInfoComponent PlayersInfo;
        private float timeOut = 0;
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            regionsComponent = ServerManager.manager.regionsComponent;
            PlayersInfo = ServerManager.manager.playersData;
            regionsComponent.CreateRegions();
        }
        [BurstCompile]
        public struct UpdateRegion : IJobForEach<Region> 
        { 
            public Vector3 positonPlayer;
            public ushort range;
            public NativeArray<ushort> id;

            public void Execute(ref Region region)
            {
               
            }
            private bool GetRange(float num, float target, float range)
            {
                if (num <= target + range && num >= target - range) return true;
                else return false;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle handle = new JobHandle();
            if (ServerCallBlack.isServerRun)
            {
                if ((Time.time - timeOut) > 2)
                {
                  
                        timeOut = Time.time;
                    for (int i = 0; i < PlayersInfo.players.Count; i++)
                    {
                        PlayerManager player = PlayersInfo.playersList[i];
                        NativeArray<ushort> ids = new NativeArray<ushort>(new ushort[1] { 2000 }, Allocator.TempJob);
                        UpdateRegion regionJob = new UpdateRegion() { positonPlayer = player.transform.position, range = regionsComponent.RangeRegion, id = ids };

                        handle = regionJob.Schedule(this,inputDeps);
                        handle.Complete();
                        if (handle.IsCompleted)
                        {
                            EntityManager.SetComponentData(player.entity, new PlayerRegion(ids[0])); 
                            ids.Dispose();
                        }

                    }
                }
            }

            return handle;
        }
    }

}