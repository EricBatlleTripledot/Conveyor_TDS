using System;
using UnityEngine;

namespace _2025.ColourBlockArrowProto.Scripts
{
    /// <summary>
    /// Helper struct to bundle a duration curve and animation trigger name together
    /// </summary>
    [Serializable]
    public struct TileMotionConfig
    {
        public float duration;
        public AnimationCurve curve;
        public string clipName;

        public bool HasClip() => !string.IsNullOrEmpty(clipName);
        public int ClipHash() => Animator.StringToHash(clipName);
    }
}