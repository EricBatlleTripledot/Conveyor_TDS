using System.Collections.Generic;
using System.Threading.Tasks;
using _2025.ColourBlockArrowProto.Scripts;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// View for the animation elements that spawn a ConveyorBlockView into the scene
    /// </summary>
    public class BlockViewSpawnerView : MonoBehaviour
    {
        [SerializeField]
        private TileSpawnerAnimator spawnerAnimator;
        [SerializeField]
        private TileStackAnimator stackAnimator;

        private readonly Queue<QueueEntry> spawnQueue = new();
        private Task activeLaunchTask;
        
        public Vector3 StackPoint => stackAnimator.StackTopPoint;
        
        private Vector3 conveyorBeltTargetPoint;
        private float conveyorBeltSplineTime;

        private static readonly Vector3 GizmoCubeSize = new Vector3(1, 0.25f, 1);

        struct QueueEntry
        {
            public readonly Color color;
            public readonly ConveyorBlockView createdView;

            public QueueEntry(Color color, ConveyorBlockView createdView)
            {
                this.color = color;
                this.createdView = createdView;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.DrawLine(StackPoint, conveyorBeltTargetPoint);
                Gizmos.DrawWireCube(conveyorBeltTargetPoint, GizmoCubeSize);
            }
        }

        public void Initialize(Vector3 beltPoint, float beltTime)
        {
            conveyorBeltTargetPoint = beltPoint;
            conveyorBeltSplineTime = beltTime;
        }

        public void AddBlockToSpawnQueue(Color color, ConveyorBlockView createdView)
        {
            // hide view until needed
            createdView.transform.position = Vector3.down * 100;
            
            spawnQueue.Enqueue(new QueueEntry(color, createdView));

            if (activeLaunchTask == null)
            {
                activeLaunchTask = SpawnAndLaunch();
            }
            // else there is already an active launch task acting, so let it handle Dequeuing this block 
        }

        /// <summary>
        /// Animates the given ColorBlock, adds it to the stack and then immediately launches it onto the spline 
        /// </summary>
        /// <returns>A task that awaits the animation, on end the ConveyorBlock is ready to begin spline animation</returns>
        private async Task SpawnAndLaunch()
        {
            var entry = spawnQueue.Dequeue();
            var color = entry.color;
            var createdView = entry.createdView;
            
            await spawnerAnimator.DoTileAnimation(color);
            // todo: add view to stack animation
            createdView.transform.position = stackAnimator.StackTopPoint;
            
            // todo await stack animation
            await createdView.Launch(conveyorBeltTargetPoint, conveyorBeltSplineTime);

            if (spawnQueue.Count > 0)
            {
                activeLaunchTask = SpawnAndLaunch();
            }
            else
            {
                activeLaunchTask = null;
            }
        }
    }
}