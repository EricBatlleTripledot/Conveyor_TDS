using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _2025.ColourBlockArrowProto.Scripts
{
    public class ArrowSceneTileSpawnSequence : MonoBehaviour
    {
        [Header("References")]
        public TileSpawnerAnimator furnace;
        public TileStackAnimator stack;

        [Header("Test")]
        public Material[] materialsToSpawn;
        public bool triggerNewTile;


        private void Update()
        {
            if (triggerNewTile)
            {
                triggerNewTile = false;

                var i = Random.Range(0, materialsToSpawn.Length);
                
                furnace.tileMaterialToSpawn = materialsToSpawn[i];
                furnace.trigger = true;

                StartCoroutine(furnace.WaitForAnimation(() =>
                {
                    stack.tileMaterialToSpawn = materialsToSpawn[i];
                    stack.trigger = true;
                }));
            }
        }
    }
}