using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using InGame.Enemy.Interface;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public sealed class DriftState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.Drift;

        public DriftState(ColateEntity colateEntity, ColateView colateView, ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent)
            : base(colateEntity, colateView, colateStatusView, spawnEnemyEvent)
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
            Drift();
            if (nextStateInstance!=null)
            {
                stage = Event.Exit;
                return;
            }
            base.Update();
        }

        private void RegisterObserver()
        {
            disposables.Add(
                colateView.Attacked.Subscribe(_ =>
                    {
                        nextStateInstance =
                            new DroppingState(colateEntity, colateView, colateStatusView, spawnEnemyEvent);
                    }
                ).AddTo(colateView));
        }
    }
}