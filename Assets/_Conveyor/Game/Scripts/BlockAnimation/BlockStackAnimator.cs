using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.BlockAnimation
{
    /// <summary>
    /// Handles the animation and stacking of transforms for the 'Stack' gameObject in the GameScene
    /// </summary>
    public class BlockStackAnimator : MonoBehaviour
    {
        [Header("Stacking")]
        [SerializeField]
        private Transform stackContainer;
        [SerializeField]
        private Transform stackOrigin;
        [SerializeField]
        private Vector3 spacingPerTile = new(0, 0.225f, 0);

        public Vector3 StackTopPoint => stackOrigin.position;
        
        [Header("Add-to-Stack Animation")]
        [SerializeField]
        private Vector3 jumpLocalDirection = Vector3.up;
        [SerializeField]
        private AnimationCurve jumpCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField]
        private float[] jumpHeightsPerStackedTile;
        [SerializeField]
        private float[] jumpDurationsPerStackedTile;
        [Space(10)]
        [SerializeField]
        private float randomRotationOffset = 5;
        [SerializeField]
        private AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Delay for Launching Tile")]
        [SerializeField]
        private float delayForLaunch = 0.25f;
        
        private readonly List<Transform> currentStack = new();
        private readonly List<Sequence> activeSequences = new();
        private int activeTweens;

        public void AddToStack(Transform t)
        {
            currentStack.Insert(0, t);
            // atm this animator is written around localPositions, so change parent
            t.SetParent(stackContainer);
        }

        public void RemoveFromStack(Transform t)
        {
            t.SetParent(null);
            currentStack.Remove(t);
        }

        public bool AnyTweensActive() => activeTweens > 0;

        // Adds a delay for the jump to let a Launching ConveyorBlockView animate first,
        // like a act-response thing
        public void DoStackJumpWithDelay()
        {
            DoStackJump(delayForLaunch);
        }
        
        public void DoStackJump(float delay = 0)
        {
            if (currentStack.Count == 0)
            {
                return;
            }
            
            activeTweens = 0;

            foreach (var sequence in activeSequences)
            {
                sequence.Kill();
            }
            activeSequences.Clear();
            
            var durationsCount = jumpDurationsPerStackedTile.Length;
            var distancesCount = jumpHeightsPerStackedTile.Length;
            var endRotation = stackOrigin.rotation;
            
            for (var i = 0; i < currentStack.Count; i++)
            {
                var stackTf = currentStack[i];
                var rootLocalPoint = stackOrigin.localPosition + (spacingPerTile * i);
                
                var duration = jumpDurationsPerStackedTile[Mathf.Clamp(i, 0, durationsCount - 1)];
                var jumpToLocalPoint = rootLocalPoint + jumpLocalDirection * jumpHeightsPerStackedTile[Mathf.Clamp(i, 0, distancesCount - 1)];

                var randomRotation = Quaternion.AngleAxis(Random.Range(-randomRotationOffset, randomRotationOffset), Vector3.up);
                randomRotation *= Quaternion.AngleAxis(Random.Range(-randomRotationOffset, randomRotationOffset), Vector3.forward);
                randomRotation *= Quaternion.AngleAxis(Random.Range(-randomRotationOffset, randomRotationOffset), Vector3.right);
                
                stackTf.localPosition = rootLocalPoint;

                var sequence = DOTween.Sequence();

                sequence.AppendInterval(delay);
                sequence.AppendCallback(() =>
                {
                    stackTf.localRotation = randomRotation * stackTf.localRotation;
                });
                sequence.Append(stackTf.DOLocalMove(jumpToLocalPoint, duration)
                    .SetEase(jumpCurve));
                sequence.Join(stackTf.DORotateQuaternion(endRotation, duration)
                    .SetEase(rotationCurve));
                sequence.OnComplete(() => activeTweens--);

                activeSequences.Add(sequence);
                activeTweens++;
            }
        }
    }
}