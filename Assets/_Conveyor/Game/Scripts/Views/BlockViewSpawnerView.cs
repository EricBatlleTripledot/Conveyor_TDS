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
        private BlockViewStackView stackView;

        // todo: move into a controller
        private readonly Queue<QueueEntry> spawnQueue = new();
        private Task activeLaunchTask;
        
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

        public void AddBlockToSpawnQueue(Color color, ConveyorBlockView createdView)
        {
            // hide view until needed
            createdView.transform.position = Vector3.down * 100;
            
            spawnQueue.Enqueue(new QueueEntry(color, createdView));

            if (activeLaunchTask == null)
            {
                activeLaunchTask = SpawnAndAddToStack();
            }
            // else there is already an active launch task acting, so let it handle Dequeuing this block 
        }

        private async Task SpawnAndAddToStack()
        {
            var entry = spawnQueue.Dequeue();
            var color = entry.color;
            var createdView = entry.createdView;
            
            await spawnerAnimator.DoTileAnimation(color);
            
            stackView.AddBlockToStack(createdView);

            if (spawnQueue.Count > 0)
            {
                activeLaunchTask = SpawnAndAddToStack();
            }
            else
            {
                activeLaunchTask = null;
            }
        }
    }
}