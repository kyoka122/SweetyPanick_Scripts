using System;
using UnityEngine;

namespace InGame.Enemy.View
{
    public class EnemyChildComponents:MonoBehaviour,IDisposable
    {
        public Vector2 ToSweetsRayPos => toSweetsRayTransform.position;
        public Vector2 CenterPos => centerTransform.position;
        public Collider2D  UseDuringFlyingCollider =>  useDuringFlyingCollider;
        public Animator Animator =>  animator;

        public EnemyAnimatorCallBackBehaviour EnemyAnimatorCallBackBehaviour=>enemyAnimatorCallBackBehaviour;
        
        [SerializeField] private Transform toSweetsRayTransform;
        [SerializeField] private Transform centerTransform;
        [SerializeField] private Collider2D useDuringFlyingCollider;
        [SerializeField] private Animator animator;
        [SerializeField] private EnemyAnimatorCallBackBehaviour enemyAnimatorCallBackBehaviour;

        public void Dispose()
        {
            enemyAnimatorCallBackBehaviour.Dispose();
        }
    }
}