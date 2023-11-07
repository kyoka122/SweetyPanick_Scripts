using System;
using Cysharp.Threading.Tasks;
using InGame.Colate.Logic;
using InGame.Enemy.Entity;
using InGame.Enemy.View;
using InGame.Stage.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace InGame.Enemy.Logic
{
    public class HavingKeyEnemyLogic:BaseEnemyLogic
    {
        private readonly HavingKeyEnemyEntity _havingKeyEnemyEntity;
        private readonly HavingKeyEnemyView _havingKeyEnemyView;
        
        private const float DefaultJumpValue = 100f;
        
        public HavingKeyEnemyLogic(HavingKeyEnemyEntity enemyEntity, HavingKeyEnemyView enemyView) : base(enemyEntity, enemyView)
        {
            _havingKeyEnemyEntity = enemyEntity;
            _havingKeyEnemyView = enemyView;
            Init();
            RegisterObserver();
        }

        private void Init()
        {
            enemyView.SetActiveAnimator(false);
        }

        private void RegisterObserver()
        {
            _havingKeyEnemyView.OnAnimationEvent
                .Subscribe(eventName =>
                {
                    if (eventName==EnemyAnimationCallbackName.TriggerBarkVoice)
                    {
                        SEManager.Instance.Play(SEPath.DOG_BARK,SEVolume.DOG_BARK);
                    }
                }).AddTo(_havingKeyEnemyView);
        }

        public override void UpdateEnemies()
        {
            if (enemyView == null || enemyView.state == EnemyState.Death)
            {
                return;
            }

            CheckHadFallen();
            //CheckOutOfScreen();
            CheckSelfOutOfScreen();
            TryFly();
            if (TryUpdateFlyState())
            {
                return;
            }
            if (enemyView.state == EnemyState.Punched)
            {
                return;
            }
            RaycastHit2D hitGroundObj = GetGroundObject();

            if (IsStandDeadZone(hitGroundObj))
            {
                DestroyWithEffect(enemyView.GetPosition());
                _havingKeyEnemyView.SetActiveKey(false);
                return;
            }
            
            if (TryUpdateActionByPlayer())
            {
                return;
            }

            if (!enemyEntity.hadMoved)//MEMO: 一度動き出したら画面外でも止まらない
            {
                if (!_havingKeyEnemyEntity.hadInScreenX)
                {
                    if (!_havingKeyEnemyEntity.inScreenX)
                    {
                        return;
                    }
                    enemyView.SetActiveAnimator(true);
                    _havingKeyEnemyEntity.SetHadInScreenX(true);
                    return;
                }

                Vector2 viewPortPos = enemyEntity.WorldToViewPortPoint(enemyView.GetPosition());
                if (!SquareRangeCalculator.InXRange(viewPortPos, _havingKeyEnemyEntity.CanMoveRange))
                {
                    return;
                }
                    
                enemyEntity.SetHadMoved();//MEMO: 一度でも画面内に入ったかどうかを保持するフラグを設定
                //MEMO: 動き始めは吠える。
                enemyView.SetState(EnemyState.Bark);
                Bark();
            }

            //↓動き出してから
            if (CanTrampolineBound(hitGroundObj))
            {
                TryBound();
                return;
            }
            if (enemyView.state==EnemyState.ChangeDirection)
            {
                return;
            }
            if (!IsGround(hitGroundObj))
            {
                return;
            }
            
            TryUpdateEatState();
            if (enemyView.state==EnemyState.Eat||enemyView.state==EnemyState.Idle||enemyView.state==EnemyState.Bark)
            {
                enemyView.SetVelocity(Vector2.zero);
                return;
            }
            Move();
            CheckMoveLimit();
        }

        private bool IsGround(RaycastHit2D hitGroundObj)
        {
            return hitGroundObj.collider != null;
        }

        private RaycastHit2D GetGroundObject()
        {
            return Physics2D.Raycast(_havingKeyEnemyView.GetToGroundRayPos(), Vector2.down,
                _havingKeyEnemyEntity.ToGroundDistance,LayerInfo.EnemyGroundMask);
        }
        
        private bool CanTrampolineBound(RaycastHit2D raycastHit2D)
        {
            if (raycastHit2D.collider==null)
            {
                return false;
            }
            if (TryGetTrampolineHit(raycastHit2D,out IBoundAble highJumpAbleStand))
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// DeadZoneに落下したかどうか判別する
        /// </summary>
        /// <returns></returns>
        private bool IsStandDeadZone(RaycastHit2D raycastHit2D)
        {
            if (raycastHit2D.collider==null)
            {
                return false;
            }
            Collider2D raycastHitCollider = raycastHit2D.collider;
            if (raycastHitCollider!=null&&raycastHitCollider.gameObject.layer==LayerInfo.KeyEnemyDeadZoneNum)
            {
                Debug.Log($"downRaycastHit2D:{raycastHit2D}",raycastHit2D.collider);
                return true;
            }
            return false;
        }
        
        private bool TryGetTrampolineHit(RaycastHit2D raycastHit2D,out IBoundAble boundAble)
        {
            GameObject groundObj = null;
            if (raycastHit2D.collider != null)
            {
                groundObj = raycastHit2D.collider.gameObject;
            }
            if (groundObj!=null)
            {
                var stand = groundObj.GetComponent<IBoundAble>();
                if (stand is {BoundAble: true})
                {
                    boundAble = stand;
                    return true;
                }
            }

            boundAble = null;
            return false;
        }
        
        private bool TryBound()
        {
            _havingKeyEnemyEntity.AddCurrentBoundDelayCount();
            bool isMaxDelayCount = _havingKeyEnemyEntity.currentBoundDelayCount >_havingKeyEnemyEntity.TrampolineBoundDelayCount;
            
            if (isMaxDelayCount)
            {
                float ySpeed = _havingKeyEnemyEntity.BoundValue * DefaultJumpValue;
                SEManager.Instance.Play(SEPath.TRAMPOLINE);
                _havingKeyEnemyView.AddYVelocity(ySpeed);
                _havingKeyEnemyEntity.ClearCurrentDelayCount();
                return true;
            }
            return false;
        }

        protected override void HadPunched(Vector2 playerPos)
        {
            enemyView.SetTakeOffCollider();
            enemyView.SetCollidersCaseFlying();
            enemyView.SetGravity(0);
            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Cry,true);
            _havingKeyEnemyView.DropKey();
            
            //MEMO: ↓Set FlyawayDirection
            float toPlayerDirection = enemyView.GetPosition().x - playerPos.x;
            float flyAngle=Random.Range(enemyEntity.flyMinAngle,enemyEntity.flyMaxAngle);
            Vector2 flyDirection = new Vector2((float) Math.Cos(flyAngle * Mathf.Deg2Rad) * toPlayerDirection,
                (float) Math.Sin(flyAngle * Mathf.Deg2Rad)).normalized;
            enemyView.SetFlyAwayDirection(flyDirection);
            
            ChangeState(EnemyState.Punched);
        }
        
        private void CheckSelfOutOfScreen()
        {
            Vector2 viewPortPos = enemyEntity.WorldToViewPortPoint(enemyView.GetPosition());
            bool inScreenX = SquareRangeCalculator.InXRange(viewPortPos, _havingKeyEnemyEntity.ObjectInScreenRange);
            bool inScreen = SquareRangeCalculator.InSquareRange(viewPortPos, enemyEntity.ObjectInScreenRange);
            enemyEntity.SetInScreenX(inScreenX);
            enemyEntity.SetInScreen(inScreen);
            
            
        }

        private async void Bark()
        {
            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Bark, true);
            Debug.Log("WaitStart");
            await UniTask.WaitUntil(HadFinishedBarkAnimation, cancellationToken: enemyView.thisToken);
            Debug.Log($"WaitFinish");
            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Bark, false);
            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Walk, true);
            enemyView.SetState(EnemyState.Walk);
        }

        private bool IsPlayingBarkAnimation()
        {
            return enemyView.GetCurrentAnimationName() == EnemyAnimationName.Bark;
        }
        
        private bool HadFinishedBarkAnimation()
        {
            var stateInfo = enemyView.GetCurrentAnimatorStateInfo();
            return stateInfo.normalizedTime >= 1 && stateInfo.IsName(EnemyAnimationName.Bark);
        }
    }
}