using System;
using System.Collections.Generic;
using System.Diagnostics;
using InGame.Colate.Entity;
using InGame.Colate.View;
using MyApplication;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace InGame.Colate.Logic
{
    /// <summary>
    /// ボスの挙動の基底クラス（ステートパターン）。 継承ののち、Enter,Update,Exitを必要に応じて実装する。
    /// nextStateInstanceに新しいステートのインスタンスを代入し、stageにEvent.Exitを代入することで次のステートに移行できる。
    /// </summary>
    public abstract class BaseColateStateLogic
    {
        protected enum Event
        {
            Enter, Update, Exit
        }

        public abstract ColateState state { get; }
        
        
        protected readonly ColateEntity colateEntity;
        protected readonly ColateView colateView;
        protected readonly ColateStatusView colateStatusView;
        protected BaseColateStateLogic nextStateInstance;
        protected readonly Func<Vector2, IColateOrderAble> spawnEnemyEvent;
        protected readonly List<IDisposable> disposables;
        protected readonly DefaultSweetsLiftView[] sweetsLiftViews;
        protected bool isTalking { get; private set; }

        protected Event stage;

        protected BaseColateStateLogic(ColateEntity colateEntity, ColateView colateView,ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent,DefaultSweetsLiftView[] sweetsLiftViews)
        {
            this.colateEntity = colateEntity;
            this.colateView = colateView;
            this.colateStatusView = colateStatusView;
            this.spawnEnemyEvent = spawnEnemyEvent;
            this.sweetsLiftViews = sweetsLiftViews;
            nextStateInstance = this;
            stage = Event.Enter;
            disposables = new List<IDisposable>();
        }

        /// <summary>
        /// Update等で更新すると自動でステートが進む
        /// </summary>
        /// <returns></returns>
        public BaseColateStateLogic Process()
        {
            switch (stage)
            {
                case Event.Enter:
                    Enter();
                    break;
                case Event.Update:
                    Update();
                    break;
                case Event.Exit:
                    Exit();
                    return nextStateInstance;
                default:
                    Debug.LogError($"Not Found State:{stage}");
                    break;
            }
            return this;
        }
        
        public void SetIsTalking(bool on)
        {
            isTalking = on;
        }
        
        protected virtual void Enter()
        {
            stage = Event.Update;
            Debug.Log($"Enter:{state}");
        }

        /// <summary>
        /// このステートを抜けたい場合はnextStateInstanceに新しいステートを入れてstageにExitを代入する
        /// </summary>
        protected virtual void Update()
        {
            stage = Event.Update;
            ForceCheckIsDead();
        }

        protected virtual void Exit()
        {
            stage = Event.Exit;
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
        
        
        #region ステート間共通メソッド
        
        /// <summary>
        /// 左右に浮遊する
        /// </summary>
        protected virtual void Drift()
        {
            colateView.SetVelocity(new Vector2(colateEntity.MoveSpeed * colateView.GetDirectionX(), 0));
            if (FacedSideWall())
            {
                TurnAround();
            }
        }

        protected void TurnAround()
        {
            Debug.Log($"TurnAround!!");
            colateView.TurnAround();
        }

        protected virtual bool FacedSideWall()
        {
            var direction = new Vector2(colateView.GetDirectionX(), 0);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(colateView.GetToSideWallRayPos(), direction,
                colateEntity.ToSideWallDistance, LayerInfo.SideWallMask);
            
#if UNITY_EDITOR
            if (colateView.OnDrawRay)
            {
                DrawSideWallRay(direction * colateEntity.ToSideWallDistance);
            }
#endif
            return raycastHit2D.collider!=null;
        }

        private void ForceCheckIsDead()
        {
            if (colateEntity.CurrentColateHp==0&&state!=ColateState.Dead)
            {
                nextStateInstance = new DeadState(colateEntity, colateView, colateStatusView, spawnEnemyEvent,sweetsLiftViews);
                stage = Event.Exit;
            }
        }
        
        protected BaseColateStateLogic GetNextAttackColateState()
        {
            BaseColateStateLogic nextState;
            if (colateEntity.prevAttackState==ColateState.SweetsLift)
            {
                nextState= new ThrowEnemiesState(colateEntity, colateView,colateStatusView,spawnEnemyEvent,sweetsLiftViews);
            }
            else
            {
                nextState= new SweetsLiftState(colateEntity, colateView,colateStatusView,spawnEnemyEvent,sweetsLiftViews);
            }
            colateEntity.SetPrevAttackState(nextState.state);
            return nextState;
        }
        
        protected BaseColateStateLogic GetRandomAttackColateState()
        {
            BaseColateStateLogic nextState;
            int randomIndex=Random.Range(0, 2);
            if (randomIndex == 0)
            {
                nextState = new ThrowEnemiesState(colateEntity, colateView, colateStatusView, spawnEnemyEvent,
                    sweetsLiftViews);
            }
            else
            {
                nextState = new SweetsLiftState(colateEntity, colateView,colateStatusView,spawnEnemyEvent,sweetsLiftViews);
            }
            colateEntity.SetPrevAttackState(nextState.state);
            return nextState;
        }

        public bool CanDamage()
        {
            return state == ColateState.IsGround;
        }


        #endregion
        
        
#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        protected void DrawSideWallRay(Vector2 vector)
        {
            Debug.DrawRay(colateView.GetToSideWallRayPos(), vector, Color.green, 0.5f);
        }
#endif
    }
}