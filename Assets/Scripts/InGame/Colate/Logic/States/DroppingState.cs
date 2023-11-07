using System;
using System.Diagnostics;
using InGame.Colate.Entity;
using InGame.Colate.View;
using MyApplication;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InGame.Colate.Logic
{
    public class DroppingState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.Dropping;
        
        public DroppingState(ColateEntity colateEntity, ColateView colateView,ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent,DefaultSweetsLiftView[] sweetsLiftViews) 
            : base(colateEntity, colateView,colateStatusView,spawnEnemyEvent,sweetsLiftViews)
        {

        }
        
        protected override void Enter()
        {
            colateView.SetActiveGravity(true);
            colateView.SetSprite(ColateSpriteType.Falling);
            colateView.SetBoolAnimation(ColateAnimatorParameter.IsDropBlinking,true);
            base.Enter();
        }
        
        protected override void Update()
        {
            if (IsGround())
            {
                nextStateInstance = new IsGroundState(colateEntity, colateView, colateStatusView, spawnEnemyEvent,
                    sweetsLiftViews);
                stage = Event.Exit;
                return;
            }
            base.Update();
        }
        
        private bool IsGround()
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(colateView.GetToGroundRayPos(), Vector2.down,
                colateEntity.ToGroundDistance, LayerInfo.GroundMask);
#if UNITY_EDITOR
            if (colateView.OnDrawRay)
            {
                DrawGroundRay(colateEntity.ToGroundDistance);
            }
#endif
            if (raycastHit2D.collider!=null)
            {
                return true;
            }
            return false;
        }
        
        [Conditional("UNITY_EDITOR")]
        private void DrawGroundRay(float rayDistance)
        {
            Debug.DrawRay(colateView.GetToGroundRayPos(),Vector3.down*rayDistance,Color.blue,0.5f);
            Debug.DrawRay(colateView.GetToGroundRayPos(),Vector3.up*rayDistance,Color.red,0.5f);
        }

        protected override void Exit()
        {
            colateView.SetBoolAnimation(ColateAnimatorParameter.IsDropBlinking,false);
            base.Exit();
        }

    }
}