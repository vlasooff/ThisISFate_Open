using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Community.Client.Components
{

    public class RegionComponents : MonoBehaviour
    {
        public ushort RangeRegion = 100;
        public List<Region> regions = new List<Region>();
        public Vector2Int size = new Vector2Int(5, 5);

        [EasyButtons.Button]
        public void CreateRegions()
        {
            for (int y = 1; y < size.y; y++)
            {
                for (int x = 1; x < size.x; x++)
                {
                    Region region = new Region(x, y, RangeRegion, (ushort)(regions.Count + 1));

                    Entity entity = World.Active.EntityManager.CreateEntity();
                    World.Active.EntityManager.AddComponentData(entity, region);
                    regions.Add(region);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < regions.Count; i++)
            {
                Gizmos.DrawWireCube(regions[i].position, new Vector3(RangeRegion, 500, RangeRegion));
            }
        }
    }
    public struct RegionActive : IComponentData
    { }
   [System.Serializable]
    public struct Region : IComponentData
    {
        public ushort id;
        public bool Active;
        public Vector3 position;


        public Region(int x, int y, ushort range, ushort idRegion)
        {
            position = new Vector3(x * range, 0, y * range); 
            id = idRegion;
            Active = false;

        }
    }
}