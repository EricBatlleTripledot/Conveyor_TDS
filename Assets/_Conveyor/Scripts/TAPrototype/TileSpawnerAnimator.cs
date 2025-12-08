using System;
using UnityEngine;

namespace _2025.ColourBlockArrowProto.Scripts
{
    public class TileSpawnerAnimator : MonoBehaviour
    {
        [Header("Furnace")]
        [SerializeField]
        private Animation animator;
        [SerializeField]
        private string idleClipName;
        [SerializeField]
        private string actClipName;

        [Header("Testing")]
        public bool trigger;

        private void Update()
        {
            if (trigger)
            {
                trigger = false;
                animator.Play(idleClipName);
                animator.PlayQueued(actClipName);
            }
        }
    }
}