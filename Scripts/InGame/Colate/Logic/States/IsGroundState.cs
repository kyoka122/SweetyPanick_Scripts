using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using InGame.Enemy.Interface;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public class IsGroundState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.IsGround;
        
        private float _passedInterval;
        
        public IsGroundState(ColateEntity colateEntity,ColateView colateView,ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent) : base(colateEntity,colateView,colateStatusView,spawnEnemyEvent)
        {
        }

        protected override void Enter()
        {
            disposables.Add(
                colateView.Attacked.Subscribe(_ =>
                    {
                        colateEntity.DamageDefault();
                    }
                ).AddTo(colateView));
            colateView.SetSprite(ColateSpriteType.Confuse);
            base.Enter();
        }

        protected override void Update()
        {
            _passedInterval += Time.deltaTime;
            if (_passedInterval>colateEntity.ConfuseDuration)
            {
                nextStateInstance = new SurfaceState(colateEntity, colateView, colateStatusView, spawnEnemyEvent);
                stage = Event.Exit;
                return;
            }
            base.Update();
        }

        protected override void Exit()
        {
            colateView.SetActiveGravity(false);
            base.Exit();
        }
        
    }
}