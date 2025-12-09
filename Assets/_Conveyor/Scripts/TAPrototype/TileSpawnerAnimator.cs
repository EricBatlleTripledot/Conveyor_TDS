using System;
using System.Collections;
using UnityEngine;

namespace _2025.ColourBlockArrowProto.Scripts
{
    public class TileSpawnerAnimator : MonoBehaviour
    {
        [Header("Furnace")]
        [SerializeField]
        private Animation animator;
        [SerializeField]
        private string idleClipName;
        [SerializeField]
        private string actClipName;

        [Header("Testing")]
        public bool trigger;

        public MeshRenderer tileClone;
        public Material tileMaterialToSpawn;

        private void Update()
        {
            if (trigger)
            {
                trigger = false;
                
                tileClone.material = new Material(tileMaterialToSpawn);
                animator.Play(actClipName);
                animator.PlayQueued(idleClipName);
            }
        }
        
        public IEnumerator WaitForAnimation(Action onFinish)
        {
            yield return new WaitForEndOfFrame();
            
            while (animator.isPlaying)
            {
                yield return null;
            }
            onFinish.Invoke();
        }
    }
}