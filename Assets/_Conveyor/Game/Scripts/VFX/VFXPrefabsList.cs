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
        private GameObject slinky;
        [SerializeField]
        private int maxCycleCount = 10;
        // where X == loop index, Y == Simulation Speed
        [SerializeField]
        private AnimationCurve cascadeIndexOverSpeed;
        // where X == loop index, Y == Z Size (to thin the block mesh)
        [SerializeField]
        private AnimationCurve cascadeZSizeOverSpeed;
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

        public GameObject Slinky => slinky;

        public void ConfigureSlinkyForIndex(ParticleSystem system, int index)
        {
            var mainModule = system.main;

            var sizeZMinMax = mainModule.startSizeZ;
            sizeZMinMax.constant = cascadeZSizeOverSpeed.Evaluate(index);
            mainModule.startSizeZ = sizeZMinMax;
            
            var simulationSpeed = cascadeIndexOverSpeed.Evaluate(index);
            mainModule.simulationSpeed = simulationSpeed;

            var cycle = Mathf.Clamp(index, 1, maxCycleCount);
            var burst = new ParticleSystem.Burst(0, 1, 1, cycle, 0.1f);
            system.emission.SetBurst(0, burst);
        }

        public GameObject GetCascadeLandingVfx(int index) =>
            index >= thresholdForFasterVFX ? tileOnCascadeFastLanding : tileOnCascadeLanding;

        public GameObject TileOnHide => tileOnHide;
        public GameObject TileOnHideShortCascade => tileOnHideShortCascade;
    }
}