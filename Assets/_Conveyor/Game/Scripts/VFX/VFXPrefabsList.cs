using UnityEngine;
using UnityEngine.Serialization;

namespace _Conveyor.Scripts.Gameplay.VFX
{
    [CreateAssetMenu(
        fileName = "VFXPrefabsList", 
        menuName = "Color Block Arrow/VFX Prefabs List",
        order = 0)]
    public class VFXPrefabsList : ScriptableObject
    {
        [Header("Cascade VFX")]
        [SerializeField]
        private VfxReferences stackingFakeBlock;
        [SerializeField]
        private float heightPerStackedBlock = -0.212f;
        // in order to not clip the particle with the incoming Block,
        // I offset the index to get heightPerStackedBlock * (index + offset),
        // this first value is for the first cascade, so that the particle doesn't vanish under the floor
        [SerializeField]
        private float firstCascadeIndexOffset = 0.5f;
        // and this second value if for index 1+ onwards
        [SerializeField]
        private float indexOffset = 1.5f;
        [Space(10)]
        [SerializeField]
        private GameObject tileOnCascadeLanding;
        [SerializeField]
        private GameObject tileOnCascadeFastLanding;
        [SerializeField]
        private int thresholdForFasterVFX = 2;
        
        [Header("Tile Finish VFX")]
        [SerializeField]
        private GameObject tileOnHide;
        [SerializeField]
        private GameObject tileOnHideShortCascade;

        public VfxReferences StackingBlock => stackingFakeBlock;
        public float HeightPerStackedBlock => heightPerStackedBlock;
        public float FirstCascadeIndexOffset => firstCascadeIndexOffset;
        public float IndexOffset => indexOffset;

        public GameObject GetCascadeLandingVfx(int index) =>
            index >= thresholdForFasterVFX ? tileOnCascadeFastLanding : tileOnCascadeLanding;

        public GameObject TileOnHide => tileOnHide;
        public GameObject TileOnHideShortCascade => tileOnHideShortCascade;
    }
}