using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
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
            Func<Vector2, IColateOrderAble> spawnEnemyEvent,DefaultSweetsLiftView[] sweetsLiftViews) 
            : base(colateEntity,colateView,colateStatusView,spawnEnemyEvent,sweetsLiftViews)
        {
        }

        protected override void Enter()
        {
            disposables.Add(
                colateView.Attacked.Subscribe(_ =>
                    {
                        colateEntity.DamageDefault();
                        colateView.Rumble(colateEntity.DamagedRumbleDuration, colateEntity.DamagedRumbleStrength,
                            colateEntity.DamagedRumbleVibrato);
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
                nextStateInstance = new DamagedSurface(colateEntity, colateView, colateStatusView, spawnEnemyEvent,sweetsLiftViews);
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