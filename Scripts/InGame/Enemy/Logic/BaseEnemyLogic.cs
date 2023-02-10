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
        protected BaseEnemyEntity _enemyEntity;
        protected BaseEnemyView _enemyView;

        protected BaseEnemyLogic(BaseEnemyEntity enemyEntity,BaseEnemyView enemyView)
        {
            _enemyEntity = enemyEntity;
            _enemyView = enemyView;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _enemyView.OnPlayerPunch
                .Subscribe(HadPunched)
                .AddTo(_enemyView);

            _enemyView.OnRolled
                .Subscribe(_=>HadRolled())
                .AddTo(_enemyView);

            _enemyView.OnBindByPlayer
                .Subscribe(SetBindSettings)
                .AddTo(_enemyView);
                
            _enemyView.OnDecoyByPlayer
                .Subscribe(SetDecoySettings)
                .AddTo(_enemyView);
            
            _enemyView.OnPullByPlayer
                .Subscribe(SetPullSettings)
                .AddTo(_enemyView);
                
            _enemyView.SearchedCollisionObject
                .Subscribe(otherEnemy =>
                {
                    if (_enemyView.state != EnemyState.Fly)
                    {
                        return;
                    }
                    TryCheckHitDuringFlying(otherEnemy);

                }).AddTo(_enemyView);

            _enemyView.OnHitFlyingCollider
                .Subscribe(otherEnemy =>
                {
                    if (_enemyView.state != EnemyState.Fly)
                    {
                        return;
                    }
                    Debug.Log($"Searched!!!Destroy!!:{otherEnemy}",otherEnemy.gameObject);
                    DestroyWithEffect(otherEnemy.GetContact(0).point);
   
                }).AddTo(_enemyView);
        }
        
        public abstract void UpdateEnemies();
        
        protected void Move()
        {
            _enemyView.SetVelocity(new Vector2(_enemyEntity.moveSpeed * _enemyView.enemyDirectionX, _enemyView.GetVelocity().y));
        }
        
        protected void BindMove()
        {
            _enemyView.SetVelocity(Vector2.zero);
        }

        protected void DecoyMove(Vector2 decoyPos)
        {
            _enemyView.SetDirection((int) (decoyPos.x - _enemyView.GetPosition().x));
            _enemyView.SetVelocity(new Vector2(_enemyEntity.moveSpeed * _enemyView.enemyDirectionX, _enemyView.GetVelocity().y));
        }

        protected void PullMove(Vector2 pullingWeaponPosition)
        {
            _enemyView.SetPosition(pullingWeaponPosition);
        }

        protected void CheckMoveLimit()
        {
            bool outOfMoveLimit = _enemyView.OutOfMoveLimit(_enemyView.GetPosition(), _enemyView.enemyDirectionX);
            bool isFacedWall = IsFacedWall();
            
            if(outOfMoveLimit||isFacedWall)
            {
                ChangeState(EnemyState.ChangeDirection);
                _enemyView.SetVelocity(Vector2.zero);

                _enemyView.SetChangeDirectionTokenSource(new CancellationTokenSource());
                
                CancellationToken changeDirectionToken =
                    CancellationTokenSource.CreateLinkedTokenSource(_enemyView.thisToken,
                       _enemyView.ChangeDirectionToken).Token;
                ChangeDirectionTaskAsync(changeDirectionToken).Forget();
            }
        }

        private async UniTask ChangeDirectionTaskAsync(CancellationToken token)
        {
            _enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Idle,true);
            float waitTime = _enemyEntity.changeDirectionTime / 2;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Canceled DirectionChange");
                return;
            }
            _enemyView.SwitchDirection(token);
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Canceled DirectionChange");
                return;
            }

            _enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Idle,false);
            ChangeState(EnemyState.Walk);
        }

        protected void CheckOutOfScreen()
        {
            Vector2 viewPortPos = _enemyEntity.WorldToViewPortPoint(_enemyView.GetPosition());
            bool inScreenX = SquareRangeCalculator.InXRange(viewPortPos, _enemyEntity.ObjectInScreenRange);
            bool inScreen = SquareRangeCalculator.InSquareRange(viewPortPos, _enemyEntity.ObjectInScreenRange);
            
            _enemyView.SetInScreenX(inScreenX);
            _enemyView.SetInScreen(inScreen);
        }
        
        protected void CheckHadFallen()
        {
            bool hadFallen = _enemyView.GetPosition().y< _enemyEntity.StageBottom;
            if (hadFallen)
            {
                Destroy();
            }
        }

        protected void TryUpdateEatState()
        {
            ISweets[] facedSweets = GetFacedSweets();
            ISweets[] eatableSweets = GetEatableSweets(facedSweets);
            
            if (_enemyView.eatingSweets!=null)
            {
                if (eatableSweets.Contains(_enemyView.eatingSweets))
                {
                    return;
                }
                Reset();
            }
            
            
            if (eatableSweets.Length!=0)
            {
                ISweets eatableSweet = eatableSweets[0];
                Debug.Log($"eatableSweet:{eatableSweet}");
                OnEatSweets(eatableSweet);
            }

        }

        private ISweets[] GetFacedSweets()
        {
            var direction = new Vector2(_enemyView.enemyDirectionX, 0);
            RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(_enemyView.ToSweetsRayPos, direction,
                _enemyEntity.toSweetsDistance, LayerInfo.SweetsMask);
            var sweets = raycastHit2D
                .Select(hit => hit.collider.gameObject.GetComponent<ISweets>())
                .Where(sweetsComponent => sweetsComponent != null)
                .ToArray();
        
#if UNITY_EDITOR
            if (_enemyView.OnDrawRay)
            {
                DrawRay(_enemyView.ToSweetsRayPos,direction*_enemyEntity.toSweetsDistance,Color.green);
            }
#endif
            return sweets;
        }
        
        private bool IsFacedWall()
        {
            var direction = new Vector2(_enemyView.enemyDirectionX, 0);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(_enemyView.CenterPos, direction,
                _enemyEntity.toWallDistance, LayerInfo.GroundMask);

#if UNITY_EDITOR
            if (_enemyView.OnDrawRay)
            {
                DrawRay(_enemyView.CenterPos, direction * _enemyEntity.toWallDistance, Color.cyan);
            }
#endif
            return raycastHit2D.collider != null;
        }

        private async void OnEatSweets(ISweets sweets)
        {
            _enemyView.SetVelocity(Vector2.zero);
            _enemyView.SetEatingSweets(sweets);
            _enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Eat,false);
            ChangeState(EnemyState.Eat);
            CancellationToken token = CancellationTokenSource.CreateLinkedTokenSource(
                _enemyView.EatSweetsToken, sweets.cancellationToken).Token;
            SEManager.Instance.Play(SEPath.EAT_SWEETS,pitch:1.2f,isLoop:true);
            await sweets.BreakSweets(_enemyEntity.eatUpTime,token);
            SEManager.Instance.Stop(SEPath.EAT_SWEETS);
            Reset();
        }

        protected void Reset()
        {
            if (!_enemyView.EatSweetsToken.IsCancellationRequested)
            {
                _enemyView.CancelEatSweetsTokenSource();
            }
            _enemyView.SetEatingSweets(null);
            _enemyView.SetCurrentEatingTime(0);
            _enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Eat,false);
            ChangeState(EnemyState.Walk);
        }

        private void HadPunched(Vector2 playerPos)
        {
            _enemyView.SetTakeOffCollider();
            _enemyView.SetCollidersCaseFlying();
            _enemyView.SetGravity(0);
            _enemyView.PlayBoolAnimation(EnemyAnimatorStateName.Cry,true);
            
            //MEMO: ↓Set FlyawayDirection
            float toPlayerDirection = _enemyView.GetPosition().x - playerPos.x;
            float flyAngle=Random.Range(_enemyEntity.flyMinAngle,_enemyEntity.flyMaxAngle);
            Vector2 flyDirection = new Vector2((float) Math.Cos(flyAngle * Mathf.Deg2Rad) * toPlayerDirection,
                (float) Math.Sin(flyAngle * Mathf.Deg2Rad)).normalized;
            _enemyView.SetFlyAwayDirection(flyDirection);
            
            ChangeState(EnemyState.Punched);
        }
        
        private void HadRolled()
        {
            ChangeState(EnemyState.Death);
            _enemyView.SetTakeOffCollider();
            //SpriteEffectView shockSpriteView = _enemyView.InstanceDestroyEffect(_enemyEntity.shockSprite, hitPos);
            //shockSpriteView.Play(_enemyEntity.shockSpriteDuration);
            SEManager.Instance.Play(SEPath.HIT_WALL);
            Destroy();
        }
        
        protected void Fly()
        {
            //enemyView.SetRotation(Quaternion.Euler(0,0,enemyEntity.flyRotateAnimationAngle) * enemyView.GetRotation());
            //enemyView.SetPosition(enemyView.GetPosition() + enemyView.flyAwayDirection * enemyEntity.flyAwaySpeed);
            _enemyView.SetFreezeRotation(false);
            _enemyView.SetAngularVelocity(_enemyEntity.flyRotateAnimationAngle);
            _enemyView.SetVelocity(_enemyView.flyAwayDirection * _enemyEntity.flyAwaySpeed);
        }

        private async void SetBindSettings(bool on)
        {
            Debug.Log($"onBind!!!!!!!!!!!!!!!!!!!");
            if (on)
            {
                ChangeState(EnemyState.Bind);
                _enemyView.OffAllAnimationParameter();
                _enemyView.SetGravity(0);
            }
            else
            {
                await TimeReleaseReaction();
                _enemyView.SetGravity(_enemyEntity.originGravityScele);//TODO: いらない？
                ChangeState(EnemyState.Walk);
            }
        }
        
        private async UniTask TimeReleaseReaction()
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_enemyEntity.gumReleaseReactionTime),
                    cancellationToken: _enemyView.ReleaseGumReactionToken);
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
                _enemyView.OffAllAnimationParameter();
            }
            else
            {
                ChangeState(EnemyState.Walk);
            }
            _enemyView.SetDecoyTransform(decoyTransform);
        }

        private void SetPullSettings(Transform weaponTransform)
        {
            if (weaponTransform!=null)
            {
                ChangeState(EnemyState.isPulled);
                _enemyView.OffAllAnimationParameter();
                
            }
            else
            {
                ChangeState(EnemyState.Walk);
            }
            _enemyView.SetPullingWeaponTransform(weaponTransform);
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
            switch (_enemyView.state)
            {
                case EnemyState.Bind:
                    BindMove();
                    return true;
                case EnemyState.isDecoyed:
                    DecoyMove(_enemyView.decoyTransform.position);
                    return true;
                case EnemyState.isPulled:
                    PullMove(_enemyView.pullingWeaponTransform.position);
                    return true;
            }
            return false;
        }
        
        protected bool TryUpdateFlyState()
        {
            if (_enemyView.state != EnemyState.Fly)
            {
                return false;
            }

            if (!_enemyView.inScreen)
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
                bool isHitSideWall = _enemyView.takeOffGroundColliders.All(collider => collider != contactToEnemy.collider);
                
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
            _enemyView.SetVelocity(Vector2.zero);
            _enemyView.SetAngularVelocity(0);
            SpriteEffectView shockSpriteView = _enemyView.InstanceDestroyEffect(_enemyEntity.shockSprite, hitPos);
            SEManager.Instance.Play(SEPath.HIT_WALL);
            _enemyView.SetGravity(0);
            shockSpriteView.Play(_enemyEntity.shockSpriteDuration);
            Destroy();
        }
        
        protected void Destroy()
        {
            ChangeState(EnemyState.Death);
            _enemyView.Destroy();
        }
        
        private void ChangeState(EnemyState state)
        {
            _enemyView.CancelChangeDirectionTokenSource();
            _enemyView.CancelEatSweetsTokenSource();
            _enemyView.SetState(state);
        }

        [Conditional("UNITY_EDITOR")]
        protected void DrawRay(Vector2 rayStartPos,Vector2 vector,Color rayColor)
        {
            Debug.DrawRay(rayStartPos, vector, rayColor, 0.5f);
        }
                
    }
}