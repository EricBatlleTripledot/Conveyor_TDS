using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2025.ColourBlockArrowProto.Scripts
{
    // todo: if the animations stay similar, turn this in to a general struct for the tile's motions
    [Serializable]
    public struct TileMotionConfig
    {
        public float duration;
        public AnimationCurve curve;
        public string clipName;
    }
    
    public class ArrowTileMotions : MonoBehaviour
    {
        public Animation animator;

        [Header("From Stack onto Belt Animation")]
        public float fromStackToBeltDuration = 1;
        public AnimationCurve fromStackToBeltMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public string fromStackToBeltClipName;
        
        [Header("From Belt onto the Board Animation")]
        public float fromBeltToBoardDuration = 1;
        public AnimationCurve fromBeltToBoardMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public string fromBeltToBoardClipName;
        
        [Header("From Belt but Rejected from Board Animation")]
        // one animation across both tweens
        public float fromBeltToRejectDuration = 1;
        public float fromRejectToBeltDuration = 0.5f;
        public AnimationCurve fromBeltToRejectMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve fromRejectToBeltMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public string fromBeltToRejectClipName;

        [Header("Reject on Board")]
        public float rejectOnBoardDelay = 0.28f;
        public float rejectOnBoardDuration = 0.5f;
        public float rejectOnBoardStrength = 1f;
        public int rejectOnBoardVibrato = 10;
        
        [Header("Cascade Animation")]
        // treated as an array of options to play through and repeat the last one until done,
        // if the cascade ends before this array is done, we always play the finish motion instead
        public TileMotionConfig[] cascadeMotions;
        public TileMotionConfig finalCascadeMotion;

        [Header("Cascade Pre-empt Animation")]
        public float preEmptDelayPerIndex = 0.05f;
        public string preEmptClipName;

        
        public Tween DoMoveOntoBelt(Vector3 point)
        {
            var tween = transform.DOMove(point, fromStackToBeltDuration)
                .SetEase(fromStackToBeltMoveCurve);

            if (!string.IsNullOrEmpty(fromStackToBeltClipName))
                animator.Play(fromStackToBeltClipName);

            return tween;
        }
        
        public Tween DoMoveOntoBoard(Vector3 point)
        {
            var tween = transform.DOMove(point, fromBeltToBoardDuration)
                .SetEase(fromBeltToBoardMoveCurve);

            if (!string.IsNullOrEmpty(fromBeltToBoardClipName))
                animator.Play(fromBeltToBoardClipName);

            return tween;
        }

        public Sequence DoRejectFromBoard(Vector3 point, Vector3 returnPoint)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(point, fromBeltToRejectDuration).SetEase(fromBeltToRejectMoveCurve));
            sequence.Append(transform.DOMove(returnPoint, fromRejectToBeltDuration).SetEase(fromRejectToBeltMoveCurve));
            
            if (!string.IsNullOrEmpty(fromBeltToRejectClipName))
                animator.Play(fromBeltToRejectClipName);

            return sequence;
        }

        public Tween DoRejectOnBoard()
        {
            return transform.DOShakePosition(rejectOnBoardDuration, rejectOnBoardStrength, rejectOnBoardVibrato)
                .SetDelay(rejectOnBoardDelay);
        }
        
        public Tween DoCascade(Vector3 point, int cascadeIndex, bool isFinal)
        {
            var i = Mathf.Clamp(cascadeIndex, 0, cascadeMotions.Length - 1);
            var motion = isFinal ? finalCascadeMotion : cascadeMotions[i];

            var tween = transform.DOMove(point, motion.duration)
                .SetEase(motion.curve);

            if (!string.IsNullOrEmpty(motion.clipName))
            {
                animator.Play(motion.clipName);
            }

            return tween;
        }

        public void DoPreEmptCascade(int cascadeIndex)
        {
            StartCoroutine(DelayBeforeAnimation( preEmptClipName, preEmptDelayPerIndex * cascadeIndex));
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