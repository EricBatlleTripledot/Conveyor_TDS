using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _2025.ColourBlockArrowProto.Scripts
{
    public class ArrowTileMotions : MonoBehaviour
    {
        public Animation animator;
        public ArrowTileAnimationSettings animationSettings;

        
        public Tween DoMoveOntoBelt(Vector3 point)
        {
            return DoMotion(animationSettings.FromStackToBeltMotion, point);
        }
        
        public Tween DoMoveOntoBoard(Vector3 point)
        {
            return DoMotion(animationSettings.FromBeltToBoardMotion, point);
        }

        public Sequence DoRejectFromBoard(Vector3 point, Vector3 returnPoint)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(point, animationSettings.FromBeltToRejectDuration)
                .SetEase(animationSettings.FromBeltToRejectMoveCurve));
            sequence.Append(transform.DOMove(returnPoint, animationSettings.FromRejectToBeltDuration)
                .SetEase(animationSettings.FromRejectToBeltMoveCurve));
            sequence.AppendInterval(animationSettings.PostRejectIdle);
            
            if (!string.IsNullOrEmpty(animationSettings.FromBeltToRejectClipName))
                animator.Play(animationSettings.FromBeltToRejectClipName);

            return sequence;
        }

        public Tween DoRejectOnBoard()
        {
            var originalPosition = transform.position;
            return transform.DOShakePosition(animationSettings.RejectOnBoardDuration, animationSettings.RejectOnBoardStrength, animationSettings.RejectOnBoardVibrato)
                .SetDelay(animationSettings.RejectOnBoardDelay)
                .OnKill(() => transform.position = originalPosition)
                .OnComplete(() => transform.position = originalPosition);
        }
        
        public Tween DoCascade(Vector3 point, int cascadeIndex, bool isFinal)
        {
            var i = Mathf.Clamp(cascadeIndex, 0, animationSettings.CascadeMotions.Length - 1);
            if (isFinal)
            {
                if (animationSettings.ShouldDoShorterCascade(cascadeIndex))
                {
                    return DoMotion(animationSettings.FinalCascadeLowThresholdMotion, point);
                }

                return DoMotion(animationSettings.FinalCascadeMotion, point);
            }

            return DoMotion(animationSettings.CascadeMotions[i], point);
        }

        public void DoPreEmptCascade(int cascadeIndex)
        {
            StartCoroutine(DelayBeforeAnimation( animationSettings.PreEmptClipName, animationSettings.PreEmptDelayPerIndex * cascadeIndex));
        }

        // Utilities
        private Tween DoMotion( TileMotionConfig config, Vector3 point )
        {
            var tween = transform.DOMove(point, config.duration)
                .SetEase(config.curve);

            if (config.HasClip())
            {
                animator.Play(config.clipName);
            }

            return tween;
        }

        public Task WaitForAnimation()
        {
            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(WaitForAnimator(() => tcs.TrySetResult(true)));
            
            return tcs.Task;
        }

        private IEnumerator WaitForAnimator(Action onFinish)
        {
            while (animator.isPlaying)
            {
                yield return null;
            }
            
            onFinish.Invoke();
        }
        
        private IEnumerator DelayBeforeAnimation(string clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (!string.IsNullOrEmpty(clip))
            {
                animator.Play(clip);
            }
        }
    }
}