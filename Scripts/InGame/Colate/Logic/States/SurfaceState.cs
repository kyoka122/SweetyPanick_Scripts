using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using MyApplication;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public class SurfaceState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.Surface;
        
        public SurfaceState(ColateEntity colateEntity, ColateView colateView,ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent,DefaultSweetsLiftView[] sweetsLiftViews)
            : base(colateEntity, colateView,colateStatusView,spawnEnemyEvent,sweetsLiftViews)
        {
        }

        protected override void Enter()
        {
            colateView.SetSprite(ColateSpriteType.RideChocolate);
            colateView.SetVelocity(new Vector2(0,colateEntity.SurfaceSpeed));
            colateView.SetActiveGravity(false);
            base.Enter();
        }

        protected override void Update()
        {
            //TODO: 帽子を被るモーション入れる？
            
            if (colateView.GetPosition().y>colateEntity.ColateFlyHeight)
            {
                colateView.SetVelocity(Vector2.zero);
                nextStateInstance=GetRandomAttackColateState();
                stage = Event.Exit;
                return;
            }
            base.Update();
        }
    }
}