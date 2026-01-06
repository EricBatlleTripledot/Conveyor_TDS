using System;
using System.Collections.Generic;
using _2025.ColourBlockArrowProto.Scripts;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// View for the stack of tiles that wait for a time to launch onto the belt
    /// </summary>
    public class BlockViewStackView : MonoBehaviour
    {
        private readonly List<ConveyorBlockView> stack = new();

        private Vector3 conveyorBeltTargetPoint;
        private float conveyorBeltSplineTime;
        
        [SerializeField]
        private TileStackAnimator stackAnimator;

        public Vector3 StackPoint => stackAnimator.StackTopPoint;
        
        // todo: for now launch with an interval, replace with pod detection on belt later - Canvas
        [SerializeField]
        private float launchInterval = 1;

        private float ticker;

        private static readonly Vector3 GizmoCubeSize = new (1, 0.25f, 1);

        public void Initialize(Vector3 beltPoint, float beltTime)
        {
            conveyorBeltTargetPoint = beltPoint;
            conveyorBeltSplineTime = beltTime;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.DrawLine(stackAnimator.StackTopPoint, conveyorBeltTargetPoint);
                Gizmos.DrawWireCube(conveyorBeltTargetPoint, GizmoCubeSize);
            }
        }
        
        public void AddBlockToStack(ConveyorBlockView conveyorBlockView)
        {
            stack.Add(conveyorBlockView);
            stackAnimator.AddToStack(conveyorBlockView.transform);
            
            stackAnimator.DoStackJump();
        }
        
        private void Update()
        {
            if (ticker <= 0)
            {
                if (stack.Count > 0 && !stackAnimator.AnyTweensActive())
                {
                    var launchingView = stack[0];
                    stackAnimator.RemoveFromStack(launchingView.transform);
                    stack.RemoveAt(0);

                    launchingView.Launch(conveyorBeltTargetPoint, conveyorBeltSplineTime);
                    stackAnimator.DoStackJump();

                    ticker = launchInterval;
                }
            }
            else
            {
                ticker -= Time.deltaTime;
            }
        }
    }
}