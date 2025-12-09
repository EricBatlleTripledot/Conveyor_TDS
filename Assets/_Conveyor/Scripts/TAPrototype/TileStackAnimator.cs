using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _2025.ColourBlockArrowProto.Scripts
{
    public class TileStackAnimator : MonoBehaviour
    {

        [Header("Stacking")]
        [SerializeField]
        private Transform stackContainer;
        [SerializeField]
        private Transform stackOrigin;
        [SerializeField]
        private Transform spawnOrigin;
        [SerializeField]
        private Vector3 spacingPerTile = new(0, 0.225f, 0);

        [Header("Add-to-Stack Animation")]
        [SerializeField]
        private Vector3 jumpLocalDirection = Vector3.up;
        [SerializeField]
        private AnimationCurve jumpCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField]
        private float[] jumpHeightsPerStackedTile;
        [SerializeField]
        private float[] jumpDurationsPerStackedTile;

        private List<Transform> currentStack = new();
        
        [Header("Temp: Spawning")]
        public bool trigger;
        public GameObject tilePrefab;
        public Material tileMaterialToSpawn;

        private void Update()
        {
            if (trigger)
            {
                trigger = false;

                var clone = Instantiate(tilePrefab, spawnOrigin.position, spawnOrigin.rotation, stackContainer);
                clone.GetComponentInChildren<MeshRenderer>().material = new Material(tileMaterialToSpawn);

                RefreshStackList();
                DoStackJump();
            }
        }

        private void RefreshStackList()
        {
            currentStack.Clear();
            for (int i = 0; i < stackContainer.childCount; i++)
            {
                currentStack.Add(stackContainer.GetChild(i));
            }
            
            currentStack.Reverse();
        }

        private void DoStackJump()
        {
            var durationsCount = jumpDurationsPerStackedTile.Length;
            var distancesCount = jumpHeightsPerStackedTile.Length;
            
            for (var i = 0; i < currentStack.Count; i++)
            {
                var stackTf = currentStack[i];
                var rootLocalPoint = stackOrigin.localPosition + (spacingPerTile * i);
                
                var duration = jumpDurationsPerStackedTile[Mathf.Clamp(i, 0, durationsCount - 1)];
                var jumpToLocalPoint = rootLocalPoint + jumpLocalDirection * jumpHeightsPerStackedTile[Mathf.Clamp(i, 0, distancesCount - 1)];

                stackTf.DOKill();
                
                stackTf.localPosition = rootLocalPoint;
                stackTf.DOLocalMove(jumpToLocalPoint, duration)
                    .SetEase(jumpCurve);
            }
        }
    }
}