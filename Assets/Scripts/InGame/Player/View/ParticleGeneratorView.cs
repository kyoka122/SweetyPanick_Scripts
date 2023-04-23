using UnityEngine;
using Utility;

namespace InGame.Player.View
{
    public class ParticleGeneratorView : MonoBehaviour, IPoolObjectGenerator<ParticleSystem>
    {
        public ParticleSystem Prefab => prefab;
        
        [SerializeField] private ParticleSystem prefab;

        public ParticleSystem Generate()
        {
            return Instantiate(prefab);
        }
    }
}