using System;
using UnityEngine;

namespace Game.BlockAnimation
{
    /// <summary>
    /// Helper struct to bundle a duration curve and animation trigger name together
    /// </summary>
    [Serializable]
    public struct BlockMotionConfig
    {
        public float duration;
        public AnimationCurve curve;
        public string clipName;

        public bool HasClip() => !string.IsNullOrEmpty(clipName);
        public int ClipHash() => Animator.StringToHash(clipName);
    }
}