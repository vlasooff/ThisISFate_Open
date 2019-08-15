using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Community.Client.Components.Managers
{
    public class ResourceManager : MonoBehaviour
    {
        public List<Chunk> chunks = new List<Chunk>();
        public static ResourceManager manager;

        private void Awake()
        {
            manager = this;
        }

        [EasyButtons.Button]
        public void CreateChunks()
        {
            RegionComponents regions = FindObjectOfType<RegionComponents>();
            for (int i = 0; i < regions.regions.Count; i++)
            {
                chunks.Add(new Chunk(regions.regions[i].id));
            }
            LevelObject[] objects = FindObjectsOfType<LevelObject>();
            foreach (var  obj in objects)
            {
                if (obj.mask == EObjectLevel.small)
                {
                    chunks[obj.idRegion - 1].small.Add(new ChunkObject(obj._transform.position, obj._transform.rotation, obj.id));
                }
                else
                {
                    if (obj.mask == EObjectLevel.medium)
                    { 
                        chunks[obj.idRegion - 1].medium.Add(new ChunkObject(obj._transform.position, obj._transform.rotation, obj.id));
                    }
                    else
                    {
                        if (obj.mask == EObjectLevel.large)
                        {
                            chunks[obj.idRegion - 1].large.Add(new ChunkObject(obj._transform.position, obj._transform.rotation, obj.id));
                        }
                        else Debug.LogWarning("OBJ NOT MASK FIND");
                    }
                }

            }
        }

      
    

    }
    [System.Serializable]
    public struct Chunk
    {
        public ushort regionId;
        public List<ChunkObject> small;
        public List<ChunkObject> medium;
        public List<ChunkObject> large;

        public Chunk(ushort idRegion)
        {
            regionId = idRegion;
            small = new List<ChunkObject>();
            medium = new List<ChunkObject>();
            large = new List<ChunkObject>();
        }
    }
    [System.Serializable]
    public struct ChunkObject
    {
        public Vector3 position;
        public Quaternion rotation;
        public ushort id;

        public ChunkObject(Vector3 pos,Quaternion rot,ushort idobj)
        {
            position = pos;
            rotation = rot;
            id = idobj;

        }
    }
}
