using Community.Client.Components;
using Community.Client.Components.Managers;
using Network;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Community.Client.Systems
{

    public class ResourceSystem : ComponentClient
    {
        ResourceManager manager;
        RegionComponents regions;
        protected override void OnStartClient()
        {
            base.OnStartClient();
            RegionsCS.onUpdateRegion += OnUpdRegion;
            manager = ResourceManager.manager;
            regions = ClientManager.manager.GetComponent<RegionComponents>();

        }

        private void OnUpdRegion(ushort idRegion)
        {

        }
        private void LoadRegion(ushort idRegion)
        {

            //  manager.chunks[idRegion -1]
        }
        private void SpawnChunk(Chunk chunk)
        {
            for (int i = 0; i < chunk.small.Count; i++)
            {
                SpawnObject(chunk.small[i], EObjectLevel.small);
            }
            for (int i = 0; i < chunk.medium.Count; i++)
            {
                SpawnObject(chunk.medium[i], EObjectLevel.medium);

            }
            for (int i = 0; i < chunk.large.Count; i++)
            {
                SpawnObject(chunk.large[i], EObjectLevel.large);
            }
        }
        private void SpawnObject(ChunkObject obj, EObjectLevel level)
        {
            float3 fullPos = new float3(obj.position.x, obj.position.y, obj.position.z);
            float3 fullScale = new float3(1.0f, 1, 1.0f);
            Rotation rotation = new Rotation() { Value = obj.rotation };
            Translation translation = new Translation() { Value = fullPos };
            // TODO(cort): Use WriteGroups here instead
            LocalToWorld localToWorld = new LocalToWorld
            {
                Value = math.mul(float4x4.Translate(fullPos), float4x4.Scale(fullScale))
            };
            Entity entity = World.Active.EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, localToWorld);
            EntityManager.AddComponentData(entity, rotation);
            EntityManager.AddComponentData(entity, translation);
            ObjectAsset asset = Resources.Load<ObjectAsset>($"Prefabs/objects/{level.ToString()}/object_{obj.id}");
            if (asset == null) Debug.Log("Asset == null!");
            Debug.Log("[C] Spawn object 4 " + level.ToString());
            RenderMesh mesh = new RenderMesh() { mesh = asset.Mesh, subMesh = asset.Mesh.subMeshCount, material = asset.materials[0], castShadows = UnityEngine.Rendering.ShadowCastingMode.On, receiveShadows = true };
            World.Active.EntityManager.AddSharedComponentData(entity, mesh);

        }
        protected override void OnUpdate()
        {

        }
    }
}
