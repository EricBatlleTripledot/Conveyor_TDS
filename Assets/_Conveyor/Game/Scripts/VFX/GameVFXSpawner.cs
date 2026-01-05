using UnityEngine;
using UnityEngine.Serialization;

namespace _Conveyor.Scripts.Gameplay.VFX
{
    public class GameVFXSpawner : MonoBehaviour
    {
        [SerializeField]
        private VFXPrefabsList prefabsList;

        public void SpawnCascadeLanding(Vector3 point, int cascadeIndex)
        {
            var prefab = prefabsList.GetCascadeLandingVfx(cascadeIndex);
            Instantiate(prefab, point, Quaternion.identity);
            // todo: await vfx to finish then cleanup back into a pool
        }

        public void SpawnTileFinish(Vector3 point)
        {
            Instantiate(prefabsList.TileOnHide, point, Quaternion.identity);
        }
    }
}