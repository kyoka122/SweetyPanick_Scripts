using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using InGame.Enemy.Interface;
using InGame.Enemy.Logic;
using MyApplication;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Colate.Logic
{
    public class SurfaceState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.Surface;
        
        public SurfaceState(ColateEntity colateEntity, ColateView colateView,ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent) : base(colateEntity, colateView,colateStatusView,spawnEnemyEvent)
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
                nextStateInstance=GetRandomColateState();
                stage = Event.Exit;
                return;
            }
            base.Update();
        }

        private BaseColateStateLogic GetRandomColateState()
        {
            int randomIndex=Random.Range(0, 2);
            if (randomIndex==0)
            {
                //TODO: Driftを実装したらコメントアウト解除
                //return new DriftState(colateEntity, colateView,colateStatusView,spawnEnemyEvent);
            }

            return new ThrowEnemiesState(colateEntity, colateView,colateStatusView,spawnEnemyEvent);
        }
    }
}