using Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Conveyor.Scripts.Gameplay.VFX
{
    public class GameVFXSpawner : MonoBehaviour
    {
        [SerializeField]
        private VFXPrefabsList prefabsList;

        private Quaternion rotation = Quaternion.Euler(90, 0, 0);
        
        public void SpawnStackedBlock(Transform blockView, int cascadeIndex, Color blockColor)
        {
            var clone = Instantiate(prefabsList.StackingBlock, blockView);
            clone.transform.localRotation = rotation;

            var system = clone.GetComponent<ParticleSystem>();
            var systemMainModule = system.main;
            
            systemMainModule.simulationSpeed = prefabsList.CascadeIndexOverSimSpeed.Evaluate(cascadeIndex);
            
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", blockColor);
            clone.Renderer.SetPropertyBlock(propertyBlock);

            var offset = cascadeIndex == 0 ? prefabsList.FirstCascadeIndexOffset : prefabsList.IndexOffset;
            for (int i = 0; i <= cascadeIndex; i++)
            {
                var blockEmit = new ParticleSystem.EmitParams
                {
                    position = prefabsList.HeightPerStackedBlock * (i + offset) * Vector3.back,
                };

                clone.System.Emit(blockEmit, 1);
            }
            
            AwaitVfxAndReturn(clone.gameObject);
        }
        
        public void SpawnVanishingStackedBlock(Vector3 point, int cascadeIndex, Color blockColor)
        {
            var clone = Instantiate(prefabsList.VanishingStackingFakeBlock, point, Quaternion.identity);
            
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", blockColor);
            clone.Renderer.SetPropertyBlock(propertyBlock);

            for (int i = 0; i <= cascadeIndex; i++)
            {
                var blockEmit = new ParticleSystem.EmitParams
                {
                    position = prefabsList.HeightPerStackedBlock * (i + prefabsList.IndexOffset) * Vector3.up,
                    startLifetime = 0.6f + (i * 0.1f)
                };

                clone.System.Emit(blockEmit, 1);
            }
            
            AwaitVfxAndReturn(clone.gameObject);
        }
        
        public void SpawnCascadeLanding(Vector3 point, int cascadeIndex)
        {
            var prefab = prefabsList.GetCascadeLandingVfx(cascadeIndex);
            AwaitVfxAndReturn(Instantiate(prefab, point, Quaternion.identity));
        }

        public void SpawnTileFinish(Vector3 point)
        {
            AwaitVfxAndReturn(Instantiate(prefabsList.TileOnHide, point, Quaternion.identity));
        }
        
        public void SpawnTileFinishShort(Vector3 point)
        {
            AwaitVfxAndReturn(Instantiate(prefabsList.TileOnHideShortCascade, point, Quaternion.identity));
        }

        void AwaitVfxAndReturn(GameObject clone)
        {
            if (clone.TryGetComponent<WaitForVfxFinish>(out var waitComponent))
            {
                waitComponent.ParticleSystemFinished += o =>
                {
                    // todo: pooling?
                    Destroy(o.gameObject);
                };
            }
            else
            {
                Debug.LogWarning($"GameVFXSpawner: spawned a VFX prefab without a WaitForVfxFinish component: '{clone.name}'", clone);
            }
        }
    }
}