using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Enemy.View;
using InGame.Player.Entity;
using InGame.Player.View;
using InGame.Stage.View;
using MyApplication;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class ActionKeyLogic
    {
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly BasePlayerView _playerView;
        private readonly WeaponView _weaponView;
        private readonly ActionKeyView _actionKeyView;

        private bool _hadFinishedCountDown;

        public ActionKeyLogic(PlayerCommonInStageEntity playerCommonInStageEntity,
            PlayerCommonUpdateableEntity playerCommonUpdateableEntity, PlayerConstEntity playerConstEntity,
            PlayerInputEntity playerInputEntity, BasePlayerView playerView, WeaponView weaponView, 
            ActionKeyView actionKeyView)
        {
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerConstEntity = playerConstEntity;
            _playerInputEntity = playerInputEntity;
            _playerView = playerView;
            _weaponView = weaponView;
            _actionKeyView = actionKeyView;
        }

        public void RegisterActionKeyFlagObserver()
        {
            CancellationToken viewCancellationToken = _actionKeyView.GetCancellationTokenOnDestroy();
            var observables = new Dictionary<Key, IObservable<bool>>
            {
                {Key.Jump, _playerCommonInStageEntity.OnJump},
                {Key.Punch, _playerCommonInStageEntity.OnPunch}
            };
            foreach (var valuePair in observables)
            {
                if (!_playerCommonUpdateableEntity.HadUsedFirstActionKey(valuePair.Key))
                {
                    CancellationToken token=new CancellationToken();
                    CancellationTokenSource tokenSource=CancellationTokenSource.CreateLinkedTokenSource(viewCancellationToken,token);
                    valuePair.Value
                        .Subscribe(_ =>
                        {
                            if (_playerCommonInStageEntity.currentFirstActionKey==valuePair.Key)
                            {
                                _playerCommonUpdateableEntity.AddUsedFirstActionKey(valuePair.Key);
                                tokenSource.Cancel();
                            }
                        }).AddTo(tokenSource.Token);
                }
            }
            
            Dictionary<Key, SweetsType> sweetsTypes = new Dictionary<Key, SweetsType>
            {
                {Key.Fix, SweetsType.Sweets},
                {Key.GimmickFix, SweetsType.GimmickSweets}
            };
            foreach (var sweetsType in sweetsTypes)
            {
                if (!_playerCommonUpdateableEntity.HadUsedFirstActionKey(sweetsType.Key))
                {
                    CancellationToken token=new CancellationToken();
                    CancellationTokenSource tokenSource=CancellationTokenSource.CreateLinkedTokenSource(viewCancellationToken,token);
                    _playerView.FixedUpdateAsObservable()
                        .Subscribe(_ =>
                        {
                            if (_playerCommonInStageEntity.currentFixingSweets != null &&
                                _playerCommonInStageEntity.currentFixingSweets.type == sweetsType.Value)
                            {
                                _playerCommonUpdateableEntity.AddUsedFirstActionKey(sweetsType.Key);
                                tokenSource.Cancel();
                            }
                        }).AddTo(tokenSource.Token);
                }
            }
            
            
            if (!_playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.Move))
            {
                DelayOnMoveKey();
                CancellationToken token=new CancellationToken();
                CancellationTokenSource tokenSource=CancellationTokenSource.CreateLinkedTokenSource(viewCancellationToken,token);
                _playerView.FixedUpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        if (Mathf.Abs(_playerView.GetVelocity().x)>0&&Mathf.Abs(_playerInputEntity.xMoveValue)>0)
                        {
                            _playerCommonUpdateableEntity.AddUsedFirstActionKey(Key.Move);
                            tokenSource.Cancel();
                        }
                           
                    }).AddTo(tokenSource.Token);
            }
        }

        public void UpdateActionKey()
        {
            //MEMO: Moveは時差表示
            if (_hadFinishedCountDown&&!_playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.Move))
            {
                SetKeySprite(Key.Move);
                return;
            }

            Key key=GetKeyJudgedByVerticalRay();
            if (key != Key.None)
            {
                SetKeySprite(key);
                return;
            }
            key=GetKeyJudgedByHorizontalRay();
            if (key != Key.None)
            {
                SetKeySprite(key);
                return;
            }

            SetKeySprite(Key.None);
        }
        
        private async void DelayOnMoveKey()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_playerConstEntity.ToActiveMoveKeyDuration));
            _hadFinishedCountDown = true;
        }
        
        private Key GetKeyJudgedByHorizontalRay()
        {
            /*bool hadAllKeyUsed=_playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.Jump) &&
                _playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.EnterDoor) &&
                _playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.Fix)&&
                _playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.Punch);
            
            if (hadAllKeyUsed)
            {
                return Key.None;
            }*/
            
            RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(_playerView.GetToSweetsRayPos(),
                new Vector2(_playerCommonInStageEntity.playerDirection,0) , _playerConstEntity.ToFacedObjectsMaxDistance);

            foreach (var hit2D in raycastHit2D)
            {
                GameObject obj = hit2D.collider.gameObject;
                int objLayer = obj.layer;
                float toObjDistance = hit2D.distance;
                
                if (IsMatchedAnyConditionsByRayObj(Key.EnterDoor,LayerInfo.DoorNum,objLayer,
                    _playerConstEntity.ToDoorDistance,toObjDistance))
                {
                    return Key.EnterDoor;
                }
                if (IsMatchedAnyConditionsByRayObj(Key.Jump,LayerInfo.GroundNum,objLayer,
                    _playerConstEntity.ToUpStairsDistance,toObjDistance))
                {
                    return Key.Jump;
                }
                if (objLayer==LayerInfo.SweetsNum&& _playerConstEntity.ToSweetsDistance>=toObjDistance)
                {
                    Key fixKey = GetFixKey(obj);
                    if (fixKey!=Key.None)
                    {
                        return fixKey;
                    }
                }
                if (!_playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.Punch)&&
                    obj.GetComponent<IEnemyDamageAble>()!=null&&
                    _playerConstEntity.ToEnemyDistance>=toObjDistance)
                {
                    return Key.Punch;
                }
            }

            return Key.None;
        }

        private Key GetKeyJudgedByVerticalRay()
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(_playerView.GetDownToGroundRayPos(),
                Vector2.down , _playerConstEntity.ToGroundDistance,LayerInfo.MarshmallowMask);

            if (raycastHit2D.collider!=null)
            {
                return Key.MarshmallowJump;
            }

            return Key.None;
        }
        

        private bool IsMatchedAnyConditionsByRayObj(Key key,int keyLayer,int gameObjectLayer,float toKeyObjDistance,float toObjDistance)
        {
            return !_playerCommonUpdateableEntity.HadUsedFirstActionKey(key)&&
                   gameObjectLayer==keyLayer&&
                   toKeyObjDistance>=toObjDistance;
        }
        
        private void SetKeySprite(Key key)
        {
            if (_playerCommonInStageEntity.currentFirstActionKey == key)
            {
                return;
            }
            if (_playerCommonInStageEntity.currentFirstActionKey == Key.None)
            {
                _actionKeyView.SetBoolAnimation(PlayerKeyAnimationName.GetKeyName(key),true);
                _playerCommonInStageEntity.SetCurrentFirstActionKey(key);
                return;
            }
            if (key==Key.None)
            {
                _actionKeyView.SetBoolAnimation(
                    PlayerKeyAnimationName.GetKeyName(_playerCommonInStageEntity.currentFirstActionKey), false);
                _playerCommonInStageEntity.SetCurrentFirstActionKey(key);
                return;
            }
            _actionKeyView.SetBoolAnimation(
                PlayerKeyAnimationName.GetKeyName(_playerCommonInStageEntity.currentFirstActionKey), false);
            _actionKeyView.SetBoolAnimation(PlayerKeyAnimationName.GetKeyName(key),true);
            _playerCommonInStageEntity.SetCurrentFirstActionKey(key);

        }

        private Key GetFixKey(GameObject obj)
        {
            ISweets sweets = obj.GetComponent<ISweets>();
            if (sweets == null || !sweets.CanFixSweets(_playerView.type))
            {
                return Key.None;
            }

            switch (sweets.type)
            {
                case SweetsType.Sweets:
                    if (!_playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.Fix))
                    {
                        return Key.Fix;
                    }
                    break;
                case SweetsType.GimmickSweets:
                    if (!_playerCommonUpdateableEntity.HadUsedFirstActionKey(Key.GimmickFix))
                    {
                        return Key.GimmickFix;
                    }
                    break;
                default:
                    return Key.None;
            }
            return Key.None;
        }
    }
}