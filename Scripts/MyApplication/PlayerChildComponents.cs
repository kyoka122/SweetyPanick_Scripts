using UnityEngine;

namespace MyApplication
{
    public class PlayerChildComponents:MonoBehaviour
    {
        public GameObject Animator => animator;
        public Transform ModelTransform => modelTransform;
        public Vector2 ToGroundRayPos => toGroundRayTransform.position;
        public Vector2 ToSweetsRayPos => toSweetsRayTransform.position;
        public Vector2 ToSlopeRayPos => toSlopeRayTransform.position;
        public Vector2 AttackParticlePos => attackParticleTransform.position;
        public GameObject WeaponColliderObject => weaponColliderObject;
        public SpriteRenderer PlayerIcon => playerIcon;
        
        
        [SerializeField] private GameObject animator;
        [SerializeField] private Transform modelTransform;
        [SerializeField] private Transform toGroundRayTransform;
        [SerializeField] private Transform toSweetsRayTransform;
        [SerializeField] private Transform toSlopeRayTransform;
        [SerializeField] private Transform attackParticleTransform;
        [SerializeField] private GameObject weaponColliderObject;
        [SerializeField] private SpriteRenderer playerIcon;
        
    }
}