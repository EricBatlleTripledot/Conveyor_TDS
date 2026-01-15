using UnityEngine;

namespace _Conveyor.Scripts.Gameplay.VFX
{
    /// <summary>
    /// Component to store references to various Particle System components, to reduce Gets
    /// </summary>
    public class VfxReferences : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem system;
        [SerializeField]
        private ParticleSystemRenderer systemRenderer;

        public ParticleSystem System => system;
        public ParticleSystemRenderer Renderer => systemRenderer;
    }
}