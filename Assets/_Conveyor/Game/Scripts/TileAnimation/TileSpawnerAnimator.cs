using System;
using System.Collections;
using System.Threading.Tasks;
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

        [Header("VFX")]
        [SerializeField]
        private ParticleSystem onSpawnParticles;

        // todo: remove this + Update
        [Header("Testing")]
        public bool trigger;

        public MeshRenderer tileClone;
        public Material tileMaterialToSpawn;

        private MaterialPropertyBlock propertyBlock;
        
        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (trigger)
            {
                trigger = false;
                
                tileClone.material = new Material(tileMaterialToSpawn);
                animator.Play(actClipName);
                onSpawnParticles.Play();
                
                animator.PlayQueued(idleClipName);
            }
        }

        public Task DoTileAnimation(Color color)
        {
            propertyBlock.SetColor("_Color", color);
            tileClone.SetPropertyBlock(propertyBlock);
            
            animator.Play(actClipName);
            onSpawnParticles.Play();
                
            animator.PlayQueued(idleClipName);
            
            // todo: replace this with a UniTask.Delay approach
            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(WaitForAnimation(() => tcs.TrySetResult(true)));
            
            return tcs.Task;
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