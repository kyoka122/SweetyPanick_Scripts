using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public sealed class DriftState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.Drift;

        public DriftState(ColateEntity colateEntity, ColateView colateView, ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent,DefaultSweetsLiftView[] sweetsLiftViews)
            : base(colateEntity, colateView, colateStatusView, spawnEnemyEvent,sweetsLiftViews)
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
            if (nextStateInstance.state!=ColateState.Drift)
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
                            new DroppingState(colateEntity, colateView, colateStatusView, spawnEnemyEvent,sweetsLiftViews);
                    }
                ).AddTo(colateView));
        }
    }
}