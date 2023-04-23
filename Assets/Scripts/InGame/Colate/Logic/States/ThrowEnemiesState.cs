using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using InGame.Colate.Entity;
using InGame.Colate.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public class ThrowEnemiesState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.ThrowEnemies;
        private float _passedInterval;
        private readonly List<IColateOrderAble> _enemies;

        public ThrowEnemiesState(ColateEntity colateEntity, ColateView colateView, ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent,DefaultSweetsLiftView[] sweetsLiftViews)
            : base(colateEntity, colateView, colateStatusView,spawnEnemyEvent,sweetsLiftViews)
        {
            _enemies = new List<IColateOrderAble>();
        }

        protected override void Enter()
        {
            colateView.SetSprite(ColateSpriteType.RideChocolate);
            //colateView.SetActiveGroundColliderWhenRidingBoardState(true);
            colateView.FreezeConstrainsY();
            RegisterObserver();
            base.Enter();
        }

        protected override void Update()
        {
            _passedInterval += Time.deltaTime;
            Drift();
            if (_passedInterval>colateEntity.ThrowEnemyInterval)
            {
                ThrowEnemy();
                _passedInterval = 0;
            }

            if (nextStateInstance.state!=ColateState.ThrowEnemies)
            {
                stage = Event.Exit;
                return;
            }
            base.Update();
        }

        protected override void Exit()
        {
            colateView.FreePositionConstrain();
            DestroyEnemies();
        }

        private void RegisterObserver()
        {
            RegisterAttackedByEnemyObserver();
        }

        private void RegisterAttackedByEnemyObserver()
        {
            disposables.Add(
                colateView.OnCollisionEnterEvent
                    .Subscribe(collision =>
                    {
                        if (collision.gameObject.TryGetComponent(out ICollideAbleToColate colateOrderAble))
                        {
                            if (colateOrderAble.state!=EnemyState.Fly&&colateOrderAble.state!=EnemyState.Death)
                            {
                                return;
                            }

                            colateView.AddVelocity(colateView.AdjustModelDirectionX(colateEntity.NockBackPower));
                            nextStateInstance = new DroppingState(colateEntity, colateView, colateStatusView, 
                                spawnEnemyEvent,sweetsLiftViews);
                        }
                    }).AddTo(colateView));
        }
        
        private void ThrowEnemy()
        {
            colateView.OnExplosionEffect();
            var colateOrderAble=spawnEnemyEvent.Invoke(
                colateView.GetPosition()+colateView.AdjustModelDirectionX(colateEntity.ThrowEnemyPivot));
            float xVelocity=colateView.GetDirectionX() * colateEntity.ThrowEnemyPower.x;
            colateOrderAble.AddVelocity(new Vector2(xVelocity, colateEntity.ThrowEnemyPower.y));
            _enemies.Add(colateOrderAble);
        }

        private void DestroyEnemies()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy==null||enemy.state is EnemyState.Fly or EnemyState.Death)
                {
                    continue;
                }
                ParticleSystem particle =
                    colateEntity.smallMiscParticle.GetObject(enemy.CenterPos);
                particle.Play();
                particle.GetAsyncParticleSystemStoppedTrigger()
                    .ToObservable()
                    .FirstOrDefault()
                    .Subscribe(_ =>
                    {
                        Debug.Log($"particle:{particle}",particle);
                        colateEntity.smallMiscParticle.ReleaseObject(particle);                        
                    }).AddTo(particle);
                enemy.Destroy();
            }
        }
    }
}