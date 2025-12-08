using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace _2025.ColourBlockArrowProto.Scripts
{
    public class ArrowSceneSequences : MonoBehaviour
    {
        public ArrowTileMotions stackOntoBeltTile;
        public Transform ontoStackPoint;
        public string motionName; 
        
        [Header("Cascade 1")]
        public ArrowTileMotions beltOntoBoardTile;
        [Space(10)]
        public ArrowTileMotions[] cascadingRightTiles;

        [Header("Cascade 2")]
        public ArrowTileMotions beltOntoBoardTile2;
        [Space(10)]
        public ArrowTileMotions[] cascadingRightTiles2;
        
        [Header("Cascade 3")]
        public ArrowTileMotions beltOntoBoardTile3;
        [Space(10)]
        public ArrowTileMotions[] cascadingRightTiles3;
        
        [Header("Cascade 4")]
        public ArrowTileMotions beltOntoBoardTile4;
        [Space(10)]
        public ArrowTileMotions[] cascadingRightTiles4;
        
        private Vector3 stackOrigin;

        [Header("VFX Prototypes")]
        public GameObject vfxOnTileCascadeLanding;
        public GameObject vfxOnTileCascadeFastLanding;
        public int thresholdForFasterVFX = 2;
        
        [Header("Controls")]
        public bool triggerStack1;
        public bool resetStack1;
        public bool triggerCascade;
        public bool triggerCascade2;
        public bool triggerCascade3;
        public bool triggerCascade4;

        private void Awake()
        {
            stackOrigin = stackOntoBeltTile.transform.position;
        }

        private void Update()
        {
            if (triggerStack1)
            {
                triggerStack1 = false;
                var tween = stackOntoBeltTile.DoMoveOntoBelt(ontoStackPoint.position);
                tween.OnComplete(() =>
                {
                    stackOntoBeltTile.GetComponent<Animation>().PlayQueued(motionName);
                });
            }
            if (resetStack1)
            {
                resetStack1 = false;
                stackOntoBeltTile.transform.position = stackOrigin;
                stackOntoBeltTile.GetComponent<Animation>().Stop();
                stackOntoBeltTile.gameObject.SetActive(true);
            }
            if (triggerCascade)
            {
                triggerCascade = false;
                stackOntoBeltTile.gameObject.SetActive(false);
                
                var tween = beltOntoBoardTile.DoMoveOntoBoard(cascadingRightTiles[0].transform.position);
                tween.OnComplete(() => DoCascade(beltOntoBoardTile, cascadingRightTiles, 0));
            }
            if (triggerCascade2)
            {
                triggerCascade2 = false;
                
                var tween = beltOntoBoardTile2.DoMoveOntoBoard(cascadingRightTiles2[0].transform.position);
                tween.OnComplete(() => DoCascade(beltOntoBoardTile2, cascadingRightTiles2, 0));
            }
            if (triggerCascade3)
            {
                triggerCascade3 = false;
                
                var tween = beltOntoBoardTile3.DoMoveOntoBoard(cascadingRightTiles3[0].transform.position);
                tween.OnComplete(() => DoCascade(beltOntoBoardTile3, cascadingRightTiles3, 0));
            }
            if (triggerCascade4)
            {
                triggerCascade4 = false;
                
                var tween = beltOntoBoardTile4.DoMoveOntoBoard(cascadingRightTiles4[0].transform.position);
                tween.OnComplete(() => DoCascade(beltOntoBoardTile4, cascadingRightTiles4, 0));
            }
        }

        private IEnumerator WaitForAnimation(Animation target, Action onFinish)
        {
            while (target.isPlaying)
            {
                yield return null;
            }
            onFinish.Invoke();
        }

        private void DoCascade(ArrowTileMotions initiator, ArrowTileMotions[] tiles, int cascade)
        {
            initiator.gameObject.SetActive(false);

            if (cascade >= tiles.Length)
                return;

            // for the final tile in the cascade, it has it's own anim + VFX, so don't spawn a landing VFX
            var vfxPrefab = cascade >= thresholdForFasterVFX ? vfxOnTileCascadeFastLanding : vfxOnTileCascadeLanding; 
            Instantiate(vfxPrefab, initiator.transform.position, Quaternion.identity);
            
            Vector3 nextPos;
            if (cascade + 1 < tiles.Length)
            {
                nextPos = tiles[cascade + 1].transform.position;
            }
            else
            {
                var pos = initiator.transform.position;
                var dir = (pos - tiles[cascade - 2].transform.position).normalized;
                nextPos = pos + dir * 0.5f;

            }

            var tween = tiles[cascade].DoCascade(nextPos, cascade, cascade + 1 == tiles.Length);
            tween.OnComplete(() =>
            {
                DoCascade(tiles[cascade], tiles, cascade + 1);
            });
        }
    }
}