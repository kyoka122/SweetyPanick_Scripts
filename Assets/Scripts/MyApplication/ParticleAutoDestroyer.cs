using UnityEngine;

namespace MyApplication
{
    public class ParticleAutoDestroyer:MonoBehaviour
    {
        private void OnParticleSystemStopped()
        {
            Destroy(gameObject);
        }
    }
}