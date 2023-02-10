using System;
using InGame.Enemy.View;
using MyApplication;
using UnityEngine;

namespace InGame.Stage.View
{
    public class CrepeGimmickView:DefaultGimmickSweetsView
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject brokenPart;
        [SerializeField] private Collider2D rotatePart;
        protected override void EachSweetsEvent()
        {
            brokenPart.SetActive(false);
            animator.SetTrigger(StageAnimatorParameter.OnRoll);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.GetContact(0).otherCollider == rotatePart)
            {
                return;
            }
            if (other.collider.gameObject.TryGetComponent<IEnemyDamageAble>(out var damageable))
            {
                damageable.OnCrepeRolled();
            }
        }
    }
}