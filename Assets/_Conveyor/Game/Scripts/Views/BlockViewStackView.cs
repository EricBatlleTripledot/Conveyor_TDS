using System;
using System.Collections.Generic;
using System.Linq;
using Game.BlockAnimation;
using TMPro;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// View for the stack of tiles that wait for a time to launch onto the belt
    /// </summary>
    public class BlockViewStackView : MonoBehaviour
    {
        private static readonly Vector3 GizmoCubeSize = new (1, 0.3f, 1.06f);

        private readonly List<ConveyorBlockView> stack = new();

        private Vector3 conveyorBeltTargetPoint;
        private float conveyorBeltSplineTime;
        
        [SerializeField]
        private BlockStackAnimator stackAnimator;
        [SerializeField]
        private TMP_Text stackLabel;
        
        private int capacity;
        
        public Vector3 StackPoint => stackAnimator.StackTopPoint;
        public int CurrentStackCount => stack.Count;
        public int CapacityLeft => capacity - CurrentStackCount;

        public ConveyorManager ConveyorManager;

        private void Awake()
        {
            ConveyorManager.SocketEnteredLaunchWindow += (_) => LaunchBlock();
        }

        public void Initialize(Vector3 beltPoint, float beltTime, int capacity)
        {
            conveyorBeltTargetPoint = beltPoint;
            conveyorBeltSplineTime = beltTime;
            this.capacity = capacity;
            
            UpdateLabel();
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
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            stackLabel.text = $"{CurrentStackCount}/{capacity}";
        }

        private void LaunchBlock()
        {
            if (!stack.Any()) {
                return;
            }
            var launchingView = stack[0];
            // todo: when removing this view from the stack, re-parent it to the GameGrid,
            // if that's what we want it to do - Canvas
            stackAnimator.RemoveFromStack(launchingView.transform);
            stack.RemoveAt(0);

            launchingView.Launch(conveyorBeltTargetPoint, conveyorBeltSplineTime);
            stackAnimator.DoStackJumpWithDelay();

            UpdateLabel();
        }
    }
}