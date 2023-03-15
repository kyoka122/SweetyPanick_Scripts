using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Enemy.Entity;
using InGame.Enemy.View;
using InGame.Stage.View;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace InGame.Enemy.Logic
{
    //TODO: Logicの分離
    public abstract class BaseEnemyLogic
    {
        protected readonly BaseEnemyEntity enemyEntity;
        protected readonly BaseEnemyView enemyView;

        protected BaseEnemyLogic(BaseEnemyEntity enemyEntity,BaseEnemyView enemyView)
        {
            this.enemyEntity = enemyEntity;
            this.enemyView = enemyView;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            enemyView.OnPlayerPunch
                .Subscribe(HadPunched)
                .AddTo(enemyView);

            enemyView.OnRolled
                .Subscribe(_=>HadRolled())
                .AddTo(enemyView);

            enemyView.OnBindByPlayer
                .Subscribe(SetBindSettings)
                .AddTo(enemyView);
                
            enemyView.OnDecoyByPlayer
                .Subscribe(SetDecoySettings)
                .AddTo(enemyView);
            
            enemyView.OnPullByPlayer
                .Subscribe(SetPullSettings)
                .AddTo(enemyView);
                
            enemyView.SearchedCollisionObject
                .Subscribe(otherEnemy =>
                {
                    if (enemyView.state != EnemyState.Fly)
                    {
                        return;
                    }
                    TryCheckHitDuringFlying(otherEnemy);

                }).AddTo(enemyView);

            enemyView.OnHitFlyingCollider
                .Subscribe(otherEnemy =>
                {
                    if (enemyView.state != EnemyState.Fly)
                    {
                        return;
                    }
                    Debug.Log($"Searched!!!Destroy!!:{otherEnemy}",otherEnemy.gameObject);
                    DestroyWithEffect(otherEnemy.GetContact(0).point);
   
                }).AddTo(enemyView);
        }
        
        public abstract void UpdateEnemies();

        public bool IsDeath()
        {
            return enemyView.state == EnemyState.Death;
        }
        
        protected void Move()
        {
            enemyView.SetVelocity(new Vector2(enemyEntity.moveSpeed * enemyView.enemyDirectionX, enemyView.GetVelocity().y));
        }
        
        protected void BindMove()
        {
            enemyView.SetVelocity(Vector2.zero);
        }

        protected void DecoyMove(Vector2 decoyPos)
        {
            enemyView.SetDirection((int) (decoyPos.x - enemyView.GetPosition().x));
            enemyView.SetVelocity(new Vector2(enemyEntity.moveSpeed * enemyView.enemyDirectionX, enemyView.GetVelocity().y));
        }

        protected void PullMove(Vector2 pullingWeaponPosition)
        {
            enemyView.SetPosition(pullingWeaponPosition);
        }

        protected void CheckMoveLimit()
        {
            bool outOfMoveLimit = enemyView.OutOfMoveLimit(enemyView.GetLocalPosition(), enemyView.enemyDirectionX);

            bool isFacedWall = IsFacedWall();
            
            if(outOfMoveLimit||isFacedWall)
            {
                ChangeState(EnemyState.ChangeDirection);
                enemyView.SetVelocity(Vector2.zero);

                enemyView.SetChangeDirectionTokenSource(new CancellationTokenSource());
                
                CancellationToken changeDirectionToken =
                    CancellationTokenSource.CreateLinkedTokenSource(enemyView.thisToken,
                       enemyView.ChangeDirectionToken).Token;
                ChangeDirectionTaskAsync(changeDirectionToken).Forget();
            }
        }

        protected async UniTask ChangeDirectionTaskAsync(CancellationToken token)
        {
            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Idle,true);
            float waitTime = enemyEntity.changeDirectionTime / 2;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Canceled DirectionChange");
                return;
            }
            enemyView.SwitchDirection(token);
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Canceled DirectionChange");
                return;
            }

            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Idle,false);
            ChangeState(EnemyState.Walk);
        }

        protected void CheckOutOfScreen()
        {
            Vector2 viewPortPos = enemyEntity.WorldToViewPortPoint(enemyView.GetPosition());
            bool inScreenX = SquareRangeCalculator.InXRange(viewPortPos, enemyEntity.ObjectInScreenRange);
            bool inScreen = SquareRangeCalculator.InSquareRange(viewPortPos, enemyEntity.ObjectInScreenRange);
            
            enemyView.SetInScreenX(inScreenX);
            enemyView.SetInScreen(inScreen);
        }
        
        protected void CheckHadFallen()
        {
            bool hadFallen = enemyView.GetPosition().y< enemyEntity.StageBottom;
            if (hadFallen)
            {
                Destroy();
            }
        }

        protected void TryUpdateEatState()
        {
            ISweets[] facedSweets = GetFacedSweets();
            
            if (enemyView.eatingSweets!=null)
            {
                if (facedSweets.Contains(enemyView.eatingSweets))
                {
                    return;
                }
                Reset();
                return;
            }

            ISweets[] eatableSweets = GetEatableSweets(facedSweets);
            if (eatableSweets.Length>0)
            {
                ISweets eatableSweet = eatableSweets[0];
                OnEatSweets(eatableSweet);
            }

        }
        
        private ISweets[] GetFacedSweets()
        {
            var direction = new Vector2(enemyView.enemyDirectionX, 0);
            RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(enemyView.ToSweetsRayPos, direction,
                enemyEntity.toSweetsDistance, LayerInfo.SweetsMask);
            var sweets = raycastHit2D
                .Select(hit => hit.collider.gameObject.GetComponent<ISweets>())
                .Where(sweetsComponent => sweetsComponent != null)
                .ToArray();
        
#if UNITY_EDITOR
            if (enemyView.OnDrawRay)
            {
                DrawRay(enemyView.ToSweetsRayPos,direction*enemyEntity.toSweetsDistance,Color.green);
            }
#endif
            return sweets;
        }
        
        protected bool IsFacedWall()
        {
            var direction = new Vector2(enemyView.enemyDirectionX, 0);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(enemyView.CenterPos, direction,
                enemyEntity.toWallDistance, LayerInfo.SideWallMask);

#if UNITY_EDITOR
            if (enemyView.OnDrawRay)
            {
                DrawRay(enemyView.CenterPos, direction * enemyEntity.toWallDistance, Color.cyan);
            }
#endif
            return raycastHit2D.collider != null;
        }

        /// <summary>
        /// スイーツを食べ始める → 食べ終わる まで
        /// </summary>
        /// <param name="sweets"></param>
        private async void OnEatSweets(ISweets sweets)
        {
            ChangeState(EnemyState.Eat);
            enemyView.SetVelocity(Vector2.zero);
            enemyView.SetEatingSweets(sweets);
            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Eat,true);
            enemyView.SetEatSweetsTokenSource(new CancellationTokenSource());
            CancellationToken token = CancellationTokenSource.CreateLinkedTokenSource(
                enemyView.EatSweetsToken, sweets.cancellationToken).Token;
            SEManager.Instance.Play(SEPath.EAT_SWEETS,pitch:1.2f,isLoop:true);
            await sweets.BreakSweets(enemyEntity.eatUpTime,token);
            SEManager.Instance.Stop(SEPath.EAT_SWEETS);
            Reset();
        }

        protected void Reset()
        {
            if (!enemyView.EatSweetsToken.IsCancellationRequested)
            {
                enemyView.CancelEatSweetsTokenSource();
            }
            enemyView.SetEatingSweets(null);
            enemyView.SetCurrentEatingTime(0);
            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Eat,false);
            ChangeState(EnemyState.Walk);
        }

        private void HadPunched(Vector2 playerPos)
        {
            enemyView.SetTakeOffCollider();
            enemyView.SetCollidersCaseFlying();
            enemyView.SetGravity(0);
            enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Cry,true);
            
            //MEMO: ↓Set FlyawayDirection
            float toPlayerDirection = enemyView.GetPosition().x - playerPos.x;
            float flyAngle=Random.Range(enemyEntity.flyMinAngle,enemyEntity.flyMaxAngle);
            Vector2 flyDirection = new Vector2((float) Math.Cos(flyAngle * Mathf.Deg2Rad) * toPlayerDirection,
                (float) Math.Sin(flyAngle * Mathf.Deg2Rad)).normalized;
            enemyView.SetFlyAwayDirection(flyDirection);
            
            ChangeState(EnemyState.Punched);
        }
        
        private void HadRolled()
        {
            ChangeState(EnemyState.Death);
            enemyView.SetTakeOffCollider();
            //SpriteEffectView shockSpriteView = _enemyView.InstanceDestroyEffect(_enemyEntity.shockSprite, hitPos);
            //shockSpriteView.Play(_enemyEntity.shockSpriteDuration);
            SEManager.Instance.Play(SEPath.HIT_WALL);
            Destroy();
        }
        
        protected void TryFly()
        {
            if (enemyView.state != EnemyState.Punched)
            {
                return;
            }
            enemyView.SetState(EnemyState.Fly);
            //enemyView.SetRotation(Quaternion.Euler(0,0,enemyEntity.flyRotateAnimationAngle) * enemyView.GetRotation());
            //enemyView.SetPosition(enemyView.GetPosition() + enemyView.flyAwayDirection * enemyEntity.flyAwaySpeed);
            enemyView.SetFreezeRotation(false);
            enemyView.SetAngularVelocity(enemyEntity.flyRotateAnimationAngle);
            enemyView.SetVelocity(enemyView.flyAwayDirection * enemyEntity.flyAwaySpeed);
        }

        private async void SetBindSettings(bool on)
        {
            Debug.Log($"onBind!!!!!!!!!!!!!!!!!!!");
            if (on)
            {
                ChangeState(EnemyState.Bind);
                enemyView.OffAllAnimationParameter();
                enemyView.SetGravity(0);
            }
            else
            {
                await TimeReleaseReaction();
                enemyView.SetGravity(enemyEntity.originGravityScele);//TODO: いらない？
                ChangeState(EnemyState.Walk);
            }
        }
        
        private async UniTask TimeReleaseReaction()
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(enemyEntity.gumReleaseReactionTime),
                    cancellationToken: enemyView.ReleaseGumReactionToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Destroy Until Reaction");
            }
        }

        private void SetDecoySettings(Transform decoyTransform)
        {
            if (decoyTransform!=null)
            {
                ChangeState(EnemyState.isDecoyed);
                enemyView.OffAllAnimationParameter();
            }
            else
            {
                ChangeState(EnemyState.Walk);
            }
            enemyView.SetDecoyTransform(decoyTransform);
        }

        private void SetPullSettings(Transform weaponTransform)
        {
            if (weaponTransform!=null)
            {
                ChangeState(EnemyState.isPulled);
                enemyView.OffAllAnimationParameter();
                
            }
            else
            {
                ChangeState(EnemyState.Walk);
            }
            enemyView.SetPullingWeaponTransform(weaponTransform);
        }

        private ISweets[] GetEatableSweets(ISweets[] facedSweets)
        {
            return facedSweets
                .Where(sweetComponent=>sweetComponent.CanBreakSweets())
                .Where(sweetComponent=>sweetComponent.type!=SweetsType.GimmickSweets)
                .ToArray();
        }
        
        protected bool TryUpdateActionByPlayer()
        {
            switch (enemyView.state)
            {
                case EnemyState.Bind:
                    BindMove();
                    return true;
                case EnemyState.isDecoyed:
                    DecoyMove(enemyView.decoyTransform.position);
                    return true;
                case EnemyState.isPulled:
                    PullMove(enemyView.pullingWeaponTransform.position);
                    return true;
            }
            return false;
        }
        
        protected bool TryUpdateFlyState()
        {
            if (enemyView.state != EnemyState.Fly)
            {
                return false;
            }

            if (!enemyView.inScreen)
            {
                Debug.Log($"Death");
                Destroy();
            }

            return true;
        }
        
        //MEMO: Enemyが攻撃された時に接触していた地面との当たり判定か識別する　→　違ったら消滅処理
        private void TryCheckHitDuringFlying(Collision2D enemyCollision)
        {
            var contactsToEnemy=new List<ContactPoint2D>();
            enemyCollision.GetContacts(contactsToEnemy);
            foreach (var contactToEnemy in contactsToEnemy)
            {
                //MEMO: Enemyが攻撃された時に接触していた地面かどうか → 違ったら横の壁とぶつかったと見なせる
                bool isHitSideWall = enemyView.takeOffGroundColliders.All(collider => collider != contactToEnemy.collider);
                
                if (isHitSideWall)
                {
                    DestroyWithEffect(contactToEnemy.point);
                    return;
                }
            }
        }
        
        protected void DestroyWithEffect(Vector2 hitPos)
        {
            ChangeState(EnemyState.Death);
            enemyView.SetVelocity(Vector2.zero);
            enemyView.SetAngularVelocity(0);
            SpriteEffectView shockSpriteView = enemyView.InstanceDestroyEffect(enemyEntity.shockSprite, hitPos);
            SEManager.Instance.Play(SEPath.HIT_WALL);
            enemyView.SetGravity(0);
            shockSpriteView.Play(enemyEntity.shockSpriteDuration);
            Destroy();
        }
        
        protected void Destroy()
        {
            ChangeState(EnemyState.Death);
            enemyView.Destroy();
        }
        
        protected void ChangeState(EnemyState state)
        {
            enemyView.CancelChangeDirectionTokenSource();
            enemyView.CancelEatSweetsTokenSource();
            enemyView.SetState(state);
        }

        [Conditional("UNITY_EDITOR")]
        protected void DrawRay(Vector2 rayStartPos,Vector2 vector,Color rayColor)
        {
            Debug.DrawRay(rayStartPos, vector, rayColor, 0.5f);
        }
                
    }
}