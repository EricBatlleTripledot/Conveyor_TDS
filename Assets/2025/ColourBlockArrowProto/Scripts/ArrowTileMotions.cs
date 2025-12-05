using System;
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

        [Header("From Stack onto Conveyor Animation")]
        public float fromStackToBeltDuration = 1;
        public AnimationCurve fromStackToBeltMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public string fromStackToBeltClipName;
        
        [Header("From Conveyor onto the Board Animation")]
        public float fromBeltToBoardDuration = 1;
        public AnimationCurve fromBeltToBoardMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public string fromBeltToBoardClipName;

        [Header("Cascade Animation")]
        // treated as an array of options to play through and repeat the last one until done,
        // if the cascade ends before this array is done, we always play the finish motion instead
        public TileMotionConfig[] cascadeMotions;
        public TileMotionConfig finalCascadeMotion;


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
    }
}