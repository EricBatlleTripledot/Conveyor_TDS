using System;
using UnityEngine;

namespace _Conveyor.Scripts.Gameplay.VFX
{
    public class WaitForVfxFinish : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem mainParticleSystem;
        [SerializeField]
        private bool checkChildrenAreAlive = true;

        public event Action<GameObject> ParticleSystemFinished; 
        
        private void Reset()
        {
            mainParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void Update()
        {
            if (mainParticleSystem.IsAlive(checkChildrenAreAlive))
            {
                return;
            }
            
            ParticleSystemFinished?.Invoke(gameObject);
            enabled = false;
        }
    }
}