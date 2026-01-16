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

        public GameObject GetCascadeLandingVfx(int index) =>
            index >= thresholdForFasterVFX ? tileOnCascadeFastLanding : tileOnCascadeLanding;

        public GameObject TileOnHide => tileOnHide;
        public GameObject TileOnHideShortCascade => tileOnHideShortCascade;
    }
}