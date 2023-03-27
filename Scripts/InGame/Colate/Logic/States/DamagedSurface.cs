using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using MyApplication;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public class DamagedSurface:BaseColateStateLogic
    {
        public override ColateState state => ColateState.Surface;
        
        public DamagedSurface(ColateEntity colateEntity, ColateView colateView,ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent,DefaultSweetsLiftView[] sweetsLiftViews) : 
            base(colateEntity, colateView,colateStatusView,spawnEnemyEvent,sweetsLiftViews)
        {
        }

        protected override void Enter()
        {
            colateView.SetSprite(ColateSpriteType.RideChocolate);
            colateView.SetVelocity(new Vector2(0,colateEntity.SurfaceSpeed));
            colateView.SetActiveGravity(false);
            colateView.SetBoolAnimation(ColateAnimatorParameter.IsSurfaceBlinking,true);
            base.Enter();
        }

        protected override void Update()
        {
            //TODO: 帽子を被るモーション入れる？
            
            if (colateView.GetPosition().y>colateEntity.ColateFlyHeight)
            {
                colateView.SetVelocity(Vector2.zero);
                nextStateInstance = GetNextAttackColateState();
                stage = Event.Exit;
                return;
            }
            base.Update();
        }

        protected override void Exit()
        {
            colateView.SetBoolAnimation(ColateAnimatorParameter.IsSurfaceBlinking,false);
            base.Exit();
        }
    }
}