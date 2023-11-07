using System;
using Cysharp.Threading.Tasks;
using Common.Interface;
using InGame.Enemy.View;
using MyApplication;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Stage.View
{
    public class CrepeGimmickView:DefaultGimmickSweetsView,IAnimationCallback,IAnimationCallbackSender
    {
        public IObservable<string> OnAnimationEvent=>_animationEventSubject;
        
        [SerializeField] private Collider2D parentCollider2D;
        [SerializeField] private Collider2D rotatePartCollider2D;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject brokenPart;
        [SerializeField] private float animationDelay;
        
        private Subject<string> _animationEventSubject;

        public override void Init()
        {
            base.Init();
            _animationEventSubject = new Subject<string>();
        }
        
        protected override async void EachSweetsEvent()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(animationDelay), cancellationToken: this.GetCancellationTokenOnDestroy());
            brokenPart.SetActive(false);
            animator.SetTrigger(StageAnimatorParameter.OnRoll);
            parentCollider2D.enabled = false;
            rotatePartCollider2D.enabled = true;
            
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(other =>
                {
                    if (other.gameObject.TryGetComponent<IEnemyDamageAble>(out var damageable))
                    {
                        if (!damageable.canDamage)
                        {
                            return;
                        }
                        damageable.OnDamaged(new Struct.DamagedInfo(Attacker.Crepe,transform.position));
                    }
                }).AddTo(this);
        }
        
        public void CallbackAnimation(string animationClipName)
        {
            _animationEventSubject.OnNext(animationClipName);
        }

        protected override void OnDestroy()
        {
            _animationEventSubject?.Dispose();
        }
    }
}