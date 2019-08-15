using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Client.Components;
using Community.Core;
using Community.Core.Serializables;
using Network;

namespace Community.Client.Systems
{
    public class CharaterCustomSystem : ComponentClient
    {
        private InventoryCManager InventoryCManager;
        private CustomManager customManager;
        PlayersManager playersManager;

        protected override void OnStartClient()
        {
            base.OnStartClient();
             
            ClientData._packetProcessor.SubscribeReusable<CharacterDefaultPacket>(OnDefaultCharacter); 
            InventoryCManager = ClientManager.manager.inventoryManager;
            customManager = ClientManager.manager.CustomManager;
            playersManager = ClientManager.manager.playersManager;
            PlayersClient.OnSpawnCurrentPlayer += OnSpawn ;
        }

        private void OnSpawn(CPlayerManager playerManager)
        {
            UpdateCustom();
        }

        private void OnDefaultCharacter(CharacterDefaultPacket obj)
        {
            customManager.characterDefault = obj;
        } 
        public void UpdateCustom()
        {
            Entities.ForEach((ref PlayerID playerID, ref CustomCharacter custom, ref CharacterCustomHead head, ref CharacterCustomBody body) =>
            {
                CharacterCustom models = playersManager._clientPlayer.custom;
                models.mesh_hat.materials[0].SetColor("_Color_body", head.Color_Hair);
                models.mesh_head.materials[0].SetColor("_Color_lips", head.Color_lips);
                models.mesh_head.materials[0].SetColor("_Color_eyes", head.Color_eyes);
                models.mesh_head.materials[0].SetColor("_Color_beard", head.Color_beard);
                models.mesh_head.materials[0].SetColor("_Color_body", body.Color_body);
                models.mesh_head.materials[0].SetColor("_Color_Hair", head.Color_Hair);
                models.mesh_pants.materials[0].SetColor("_Color_body", body.Color_body);
                models.mesh_body.materials[1].SetColor("_Color_body", body.Color_body);
                models.mesh_beard.materials[0].SetColor("_Color_body", head.Color_beard);

                if (custom.time < 100)
                {
                    models.mesh_beard.SetBlendShapeWeight(0, custom.time);
                    models.mesh_hat.SetBlendShapeWeight(0, custom.time);
                    models.mesh_beard.SetBlendShapeWeight(1, 0);
                    models.mesh_hat.SetBlendShapeWeight(1, 0);
                }
                else
                {
                    models.mesh_beard.SetBlendShapeWeight(0, 100);
                    models.mesh_hat.SetBlendShapeWeight(0, 100);
                    models.mesh_beard.SetBlendShapeWeight(1, custom.time - 100);
                    models.mesh_hat.SetBlendShapeWeight(1, custom.time - 100);
                }
                models.mesh_head.SetBlendShapeWeight(0, head.beardSize);
                models.mesh_beard.SetBlendShapeWeight(3, head.beardSize);
                models.mesh_beard.SetBlendShapeWeight(2, custom.massa);
                models.mesh_head.SetBlendShapeWeight(2, custom.massa);
                models.mesh_hat.SetBlendShapeWeight(2, custom.massa);
                models.mesh_body.SetBlendShapeWeight(0, custom.massa);
                models.mesh_pants.SetBlendShapeWeight(0, custom.massa);
            });

                /*

            InstanceMesh();*/

            }
        private void InstanceMesh()
        {
            /*
            CustomItem body = charaters.GetModels(charater.data.id_body);
            CustomItem pants = charaters.GetModels(charater.data.id_pants);
            mesh_hat.sharedMesh = charaters.GetModels(charater.data.id_hair).mesh;
            mesh_beard.sharedMesh = charaters.GetModels(charater.data.id_beard).mesh;
            mesh_head.sharedMesh = charaters.GetModels(charater.data.id_head).mesh;
            mesh_body.sharedMesh = body.mesh;
            mesh_body.materials[0].SetTexture("_MainTex", body.texture);
            mesh_pants.materials[1].SetTexture("_MainTex", pants.texture);
            mesh_pants.sharedMesh = pants.mesh;
            */
        }
        protected override void OnUpdate()
        { 

        }
    }
}
