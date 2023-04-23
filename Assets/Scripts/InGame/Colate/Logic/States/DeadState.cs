using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using MyApplication;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public class DeadState : BaseColateStateLogic
    {
        public override ColateState state => ColateState.Dead;

        public DeadState(ColateEntity colateEntity, ColateView colateView, ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent,DefaultSweetsLiftView[] sweetsLiftViews)
            : base(colateEntity, colateView, colateStatusView, spawnEnemyEvent,sweetsLiftViews)
        {

        }
        protected override void Enter()
        {
            colateEntity.OnFinishedDeadMotion();
            base.Enter();
        }
    }
}