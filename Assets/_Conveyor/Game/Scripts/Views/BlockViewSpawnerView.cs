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

        public Vector3 StackPoint => stackAnimator.StackTopPoint;
        
        private Vector3 conveyorBeltTargetPoint;
        private float conveyorBeltSplineTime;

        private static readonly Vector3 GizmoCubeSize = new Vector3(1, 0.25f, 1);
        
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.DrawWireCube(conveyorBeltTargetPoint, GizmoCubeSize);
            }
        }

        public void Initialize(Vector3 beltPoint, float beltTime)
        {
            conveyorBeltTargetPoint = beltPoint;
            conveyorBeltSplineTime = beltTime;
        }

        /// <summary>
        /// Animates the given ColorBlock, adds it to the stack and then immediately launches it onto the spline 
        /// </summary>
        /// <returns>A task that awaits the animation, on end the ConveyorBlock is ready to begin spline animation</returns>
        public async Task SpawnAndLaunch(Color color, ConveyorBlockView createdView)
        {
            // hide view until needed
            createdView.transform.position = Vector3.down * 100;
            
            await spawnerAnimator.DoTileAnimation(color);
            createdView.transform.position = stackAnimator.StackTopPoint;
            
            // todo await stack animation
            await createdView.Launch(conveyorBeltTargetPoint, conveyorBeltSplineTime);
        }
    }
}