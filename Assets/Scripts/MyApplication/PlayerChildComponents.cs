using UnityEngine;

namespace MyApplication
{
    public class PlayerChildComponents:MonoBehaviour
    {
        public GameObject Animator => animator;
        public Transform ModelTransform => modelTransform;
        public Vector2 UpGroundRayPos => upGroundRayTransform.position;
        public Vector2 DownGroundRayPos => downGroundRayTransform.position;
        public Vector2 ToSweetsRayPos => toSweetsRayTransform.position;
        public Vector2 ToSlopeRayPos => toSlopeRayTransform.position;
        public Vector2 AttackParticlePos => attackParticleTransform.position;
        public GameObject WeaponColliderObject => weaponColliderObject;
        public GameObject FirstActionKeyObject => firstActionKeyObject;
        public SpriteRenderer PlayerIcon => playerIcon;
        public Vector2 ModelAppearEffectPivot=>modelAppearEffectTransform.position;
        public Vector2 ModelAppearEffectScale=>modelAppearEffectTransform.lossyScale;
        


        [SerializeField] private GameObject animator;
        [SerializeField] private Transform modelTransform;
        [SerializeField] private Transform upGroundRayTransform;
        [SerializeField] private Transform downGroundRayTransform;
        [SerializeField] private Transform toSweetsRayTransform;
        [SerializeField] private Transform toSlopeRayTransform;
        [SerializeField] private Transform attackParticleTransform;
        [SerializeField] private GameObject weaponColliderObject;
        [SerializeField] private GameObject firstActionKeyObject;
        [SerializeField] private SpriteRenderer playerIcon;
        [SerializeField] private Transform modelAppearEffectTransform;
        
    }
}