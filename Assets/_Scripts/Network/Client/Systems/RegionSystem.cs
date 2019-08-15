using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Entities;
using Community.Client.Components;
using Unity.Burst;
using Unity.Collections;

namespace Community.Client.Systems
{
    public delegate void OnUpdateRegion(ushort idRegion);
    public class RegionsCS : JobComponentClient
    {
        public static OnUpdateRegion onUpdateRegion;
        private RegionComponents regionsComponent;
        private PlayersManager players; 
        private float timeOut = 0;
        
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            regionsComponent = ClientManager.manager.GetComponent<RegionComponents>();
            players = ClientManager.manager.playersManager;
            regionsComponent.CreateRegions(); 
        }
        [BurstCompile]
        public struct UpdateRegion : IJobForEach<Region>
        {
            public Vector3 positonPlayer;
            public ushort range;
            public NativeArray<ushort> id; 

          
            public void Execute( ref Region region)
            {
                if (GetRange(region.position, positonPlayer, range / 2))
                {
                    id[0] = region.id;
                    Debug.Log("[C] Region local player: " + region.id);
                    region.Active = true; 
                    return;
                  }
            }

            private bool GetRange(Vector3 current, Vector3 target, int range)
            {
                if (GetRange(current.x, target.x, range) && GetRange((int)current.z, target.z, range)) return true;
                else return false;
            }
            private bool GetRange(float num, float target, float range)
            {
                if (num <= target + range && num >= target - range) return true;
                else return false;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        { 
            if (ClientCallBlack.isConnected)
            {
                if ((Time.time - timeOut) > 2)
                {
                    timeOut = Time.time;
                    if (players._clientPlayer.transform == null) return inputDeps;
                    NativeArray<ushort> ids = new NativeArray<ushort>(new ushort[1] { 2000 }, Allocator.TempJob); 
                    UpdateRegion regionJob = new UpdateRegion() { positonPlayer = players._clientPlayer.transform.position, range = regionsComponent.RangeRegion, id = ids };

                    inputDeps = regionJob.Schedule(this, inputDeps); 
                    inputDeps.Complete();
                    if (inputDeps.IsCompleted)
                    { 
                        if(players._clientPlayer.idRegion != ids[0])
                        {
                            players._clientPlayer.idRegion = ids[0];
                            OnUpdateRegion(ids[0]);
                        }
                        ids.Dispose();
                    }

                }
            }

            return inputDeps;
        }
        public void OnUpdateRegion(ushort idRegion)
        {
            onUpdateRegion?.Invoke(idRegion);
        }
    }

}
