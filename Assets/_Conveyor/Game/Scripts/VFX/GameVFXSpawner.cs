using Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Conveyor.Scripts.Gameplay.VFX
{
    public class GameVFXSpawner : MonoBehaviour
    {
        [SerializeField]
        private VFXPrefabsList prefabsList;

        public void SpawnStackedBlock(Transform blockView, int cascadeIndex, Color blockColor)
        {
            var clone = Instantiate(prefabsList.StackingBlock, blockView);
            
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", blockColor);
            clone.Renderer.SetPropertyBlock(propertyBlock);

            var offset = cascadeIndex == 0 ? prefabsList.FirstCascadeIndexOffset : prefabsList.IndexOffset;
            for (int i = 0; i <= cascadeIndex; i++)
            {
                var blockEmit = new ParticleSystem.EmitParams
                {
                    position = prefabsList.HeightPerStackedBlock * (i + offset) * Vector3.up,
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