using UnityEngine;

namespace InGame.Enemy.View
{
    public class EnemyChildComponents:MonoBehaviour
    {
        public Vector2 ToSweetsRayPos => toSweetsRayTransform.position;
        public Vector2 CenterPos => centerTransform.position;
        public Collider2D  UseDuringFlyingCollider =>  useDuringFlyingCollider;
        public Animator Animator =>  animator;
        
        [SerializeField] private Transform toSweetsRayTransform;
        [SerializeField] private Transform centerTransform;
        [SerializeField] private Collider2D useDuringFlyingCollider;
        [SerializeField] private Animator animator;
        
    }
}