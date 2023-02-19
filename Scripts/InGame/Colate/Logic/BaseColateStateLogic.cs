using System;
using System.Collections.Generic;
using System.Diagnostics;
using InGame.Colate.Entity;
using InGame.Colate.View;
using InGame.Enemy.Interface;
using MyApplication;
using UniRx;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        protected bool isTalking { get; private set; }
        
        protected Event stage;

        protected BaseColateStateLogic(ColateEntity colateEntity, ColateView colateView,ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent)
        {
            this.colateEntity = colateEntity;
            this.colateView = colateView;
            this.colateStatusView = colateStatusView;
            this.spawnEnemyEvent = spawnEnemyEvent;
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
        }

        /// <summary>
        /// このステートを抜けたい場合はnextStateInstanceに新しいステートを入れてstageにExitを代入する
        /// </summary>
        protected virtual void Update()
        {
            stage = Event.Update;
        }

        protected virtual void Exit()
        {
            stage = Event.Exit;
        }
        
        
        #region ステート間共通メソッド
        
        protected void Drift()
        {
            colateView.SetVelocity(new Vector2(colateEntity.MoveSpeed * colateView.GetDirectionX(), colateView.GetVelocity().y));
            if (FacedSideWall())
            {
                TurnAround();
            }
        }

        private void TurnAround()
        {
            colateView.TurnAround();
        }

        protected bool FacedSideWall()
        {
            var direction = new Vector2(colateView.GetDirectionX(), 0);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(colateView.GetToSweetsRayPos(), direction,
                colateEntity.ToSideWallDistance, LayerInfo.SideWallNum);
            
#if UNITY_EDITOR
            if (colateView.OnDrawRay)
            {
                DrawSweetsRay(direction * colateEntity.ToSideWallDistance);
            }
#endif
            return raycastHit2D.collider!=null;
        }

        public bool CanDamage()
        {
            return state == ColateState.IsGround;
        }


        #endregion
        
        
#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        private void DrawSweetsRay(Vector2 vector)
        {
            Debug.DrawRay(colateView.GetToSweetsRayPos(), vector, Color.green, 0.5f);
        }
#endif
        
    }
}