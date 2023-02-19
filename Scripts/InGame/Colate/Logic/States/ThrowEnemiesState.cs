using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using InGame.Enemy.Interface;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public class ThrowEnemiesState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.ThrowEnemies;
        private float _passedInterval;
        

        public ThrowEnemiesState(ColateEntity colateEntity, ColateView colateView, ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent)
            : base(colateEntity, colateView, colateStatusView,spawnEnemyEvent)
        {
        }

        protected override void Enter()
        {
            colateView.SetSprite(ColateSpriteType.RideChocolate);
            RegisterObserver();
            base.Enter();
        }

        protected override void Update()
        {
            _passedInterval += Time.deltaTime;
            Drift();
            if (_passedInterval<colateEntity.ThrowEnemyInterval)
            {
                ThrowEnemy();
                _passedInterval = 0;
            }

            if (nextStateInstance!=null)
            {
                stage = Event.Exit;
                return;
            }
            base.Update();
        }
        
        protected override void Exit()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
            base.Exit();
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
                        if (collision.gameObject.TryGetComponent(out IColateOrderAble colateOrderAble))
                        {
                            if (colateOrderAble.state!=EnemyState.Fly)
                            {
                                return;
                            }
                            colateView.AddVelocity(new Vector2(colateEntity.NockBackPower.x * colateView.GetDirectionX(),
                                colateEntity.NockBackPower.y));
                            nextStateInstance = new DroppingState(colateEntity, colateView, colateStatusView, spawnEnemyEvent);
                        }
                    }).AddTo(colateView));
        }
        
        private void ThrowEnemy()
        {
            var colateOrderAble=spawnEnemyEvent.Invoke(colateView.GetPosition());
            float xVelocity=colateView.GetDirectionX() * colateEntity.ThrowEnemyPower.x;
            colateOrderAble.AddVelocity(new Vector2(xVelocity, colateEntity.ThrowEnemyPower.y));
        }
    }
}