using System;
using System.Collections.Generic;
using System.Linq;
using MyApplication;
using InGame.Player.Entity;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using UnityEngine;
using Utility;

namespace InGame.Player.Logic
{
    public class PlayerMoveLogic
    {
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly BasePlayerView _playerView;
        private readonly PlayerAnimatorView _playerAnimatorView;
        private readonly WeaponView _weaponView;

        public PlayerMoveLogic(PlayerConstEntity playerConstEntity, PlayerInputEntity playerInputEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, PlayerCommonUpdateableEntity playerCommonUpdateableEntity,
            BasePlayerView playerView, PlayerAnimatorView playerAnimatorView)
        {
            _playerConstEntity = playerConstEntity;
            _playerInputEntity = playerInputEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerView = playerView;
            _playerAnimatorView = playerAnimatorView;
        }

        public Transform GetPlayerViewTransform()
        {
            return _playerView.transform;
        }

        public void LateInit()
        {
            _playerCommonInStageEntity.SetPlayerDirection(1);
        }

        public void UpdatePlayerMove()
        {
            if (_playerInputEntity.IsOnPlayerSelector)
            {
                Debug.Log($"Selecting");
                return;
            }

            if (_playerCommonUpdateableEntity.IsWarping)
            {
                Debug.Log($"Warping");
                return;
            }

            if (_playerCommonUpdateableEntity.IsDead)
            {
                XMove();
                return;
            }

            XMove();
            TryAddUpSlopePower();
            SetDirection();
            CheckHadFallen();
            CheckOutOfScreen();
            CheckOutOfGroupScreen();
            float newColliderSizeX = 1 + Mathf.Abs(_playerView.GetVelocity().x) / _playerConstEntity.MaxSpeed*(_playerConstEntity.MaxColliderSizeX-1);
            _playerView.ChangeWeaponColliderSize(new Vector2(newColliderSizeX,1));
        }

        public void UpdateStopping()
        {
            _playerView.SetXVelocity(0);
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsHorizontalMove,false);
        }

        /// <summary>
        /// ステージの移動に伴う位置の移動
        /// </summary>
        public void SetPosition(StageArea stageArea)
        {
            _playerView.SetPosition(_playerConstEntity.GetInstancePositionCaseMoveStage(stageArea,
                _playerCommonInStageEntity.CharacterType));
        }
        
        private void XMove()
        {
            float newXSpeed = _playerView.GetVelocity().x;

            newXSpeed = AddedInputValue(newXSpeed);
            newXSpeed = AddFriction(newXSpeed);
            newXSpeed = LimitWithMaxSpeed(newXSpeed);
            _playerView.SetXVelocity(newXSpeed);
            PlayMoveAnimation(_playerInputEntity.xMoveValue);
        }

        private void TryAddUpSlopePower()
        {
            if (_playerAnimatorView.IsJumping() || Mathf.Abs(_playerInputEntity.xMoveValue) <= 0.001)
            {
                return;
            }

            var direction = new Vector2(_playerCommonInStageEntity.playerDirection, 0);
            RaycastHit2D underRaycastHit2D = Physics2D.Raycast(_playerView.GetToSlopeRayPos(), direction,
                _playerConstEntity.UnderToSlopeDistance, LayerInfo.GroundMask);
            RaycastHit2D upRaycastHit2D = Physics2D.Raycast(_playerView.GetToSlopeRayPos()+new Vector2(0,0.1f),
                direction, _playerConstEntity.ToSlopeDistance, LayerInfo.GroundMask);
            
            if (underRaycastHit2D.collider==null||upRaycastHit2D.collider==null)
            {
                return;
            }
            
            Vector2 slopeVec=upRaycastHit2D.point - underRaycastHit2D.point;
            if(Mathf.Abs(slopeVec.x)<=0.001)
            {
                return;
            }

            _playerView.AddYVelocity(slopeVec.y*_playerConstEntity.SlopeHelpPower);
        }
        
        private float AddedInputValue(float originXSpeed)
        {
            if (!(_playerAnimatorView.IsUsingSkill()||_playerAnimatorView.IsFixingGimmickSweets()||_playerCommonUpdateableEntity.IsDead))
            {
                originXSpeed += _playerInputEntity.xMoveValue * _playerConstEntity.AccelerateRateX;
            }

            return originXSpeed;
        }
        
        private float AddFriction(float originXSpeed)
        {
            if (_playerAnimatorView.IsPunching())
            {
                originXSpeed *= _playerConstEntity.DecelerateRateXWhilePunching;
            }
            else if(_playerCommonInStageEntity.isNormalSweetsFixing)
            {
                originXSpeed *= _playerConstEntity.DecelerateRateXWhileFixing;
            }
            else
            {
                originXSpeed *= _playerConstEntity.DecelerateRateX;
            }

            return originXSpeed;
        }

        private void SetDirection()
        {
            if (_playerInputEntity.xMoveValue==0)
            {
                return;
            }

            if (_playerCommonInStageEntity.isUsingSkill|| _playerAnimatorView.IsFixingGimmickSweets()||
                _playerCommonUpdateableEntity.IsDead)
            {
                return;
            }
            _playerCommonInStageEntity.SetPlayerDirection(_playerInputEntity.xMoveValue);
        }

        private float LimitWithMaxSpeed(float xSpeed)
        {
            if (xSpeed>0)
            {
                xSpeed = Mathf.Min(xSpeed, _playerConstEntity.MaxSpeed);
            }
            else
            {
                xSpeed = Mathf.Max(xSpeed, -_playerConstEntity.MaxSpeed);
            }

            return xSpeed;
        }

        private void PlayMoveAnimation(float xParamValue)
        {
            _playerAnimatorView.PlayFloatAnimation(PlayerAnimatorParameter.HorizontalMove, Mathf.Abs(xParamValue));
            if (xParamValue==0)
            {
                _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsHorizontalMove,false);
                return;
            }

            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsHorizontalMove,true);
        }
        
        private void CheckHadFallen()
        {
            bool hadFallen = _playerView.GetPosition().y< _playerConstEntity.StageBottom;
            if (hadFallen)
            {
                _playerCommonInStageEntity.OnFallTrigger();
                WarpLastStandPos();
            }
        }

        private void WarpLastStandPos()
        {
            int fallDirection = (_playerView.GetPosition().x - _playerCommonInStageEntity.prevStandPos.x)>0 ? 1 : -1;
            var warpPos = new Vector2(_playerCommonInStageEntity.prevStandPos.x - fallDirection * 
                _playerConstEntity.WarpPosOffset.x, _playerCommonInStageEntity.prevStandPos.y+_playerConstEntity.WarpPosOffset.y);
            Warp(warpPos,_playerConstEntity.WarpDuration);
        }
        
        private void CheckOutOfScreen()
        {
            Vector2 viewPortPos=_playerConstEntity.WorldToViewPortPoint(_playerView.GetPosition());
            bool inScreen = SquareRangeCalculator.InRangeWithOutBottom(viewPortPos, _playerConstEntity.ObjectInScreenRange); 
            if (!inScreen&&!_playerCommonUpdateableEntity.IsOtherPlayerWarping)
            {
                Warp(GetNearestPlayerPosition(), _playerConstEntity.WarpDuration);
            }
        }
        
        private void CheckOutOfGroupScreen()
        {
            Vector2 viewPortPos=_playerConstEntity.WorldToViewPortPoint(_playerView.GetPosition());
            float nearRate = SquareRangeCalculator.GetNearRateOfTargetView(viewPortPos, 
                _playerConstEntity.InPlayerGroupRange);
            _playerCommonUpdateableEntity.SetInNearnessFromTargetView(nearRate);
        }
        
        private async void Warp(Vector2 endPos,float duration)
        {
            Debug.Log($"Warp! endPos:{endPos}");
            _playerCommonUpdateableEntity.SetWarping(true);
            _playerView.SetYVelocity(0);
            _playerView.OffCollider();
            _playerView.OffSprite();
            _playerView.SetPlayerIcon(true);
            int audioId=SEManager.Instance.Play(SEPath.WARP,isLoop:true);

            await _playerView.Warp(endPos, duration, _playerView.thisToken);

            _playerView.SetYVelocity(0);
            _playerView.OnCollider();
            _playerView.OnSprite();
            _playerView.SetPlayerIcon(false);
            _playerCommonUpdateableEntity.SetWarping(false);
            SEManager.Instance.Stop(audioId);
        }

        private Vector2 GetNearestPlayerPosition()
        {
            int maxValue = Enum.GetValues(typeof(PlayableCharacter)).Length;
            var positions = new List<Vector2>();
            for (int i = 1; i < maxValue; i++)
            {
                PlayableCharacter type=(PlayableCharacter) i;
                //自分は対象外
                if (type == _playerView.type)
                {
                    continue;
                }
                //使用していないプレイヤーは対象外
                if (!_playerCommonUpdateableEntity.IsUsedEachPlayer(type))
                {
                    continue;
                }
                Transform eachTransform = _playerCommonUpdateableEntity.GetEachTransform(type);
                if (eachTransform == null)
                {
                    continue;
                }
                Debug.Log($"type:{type}, eachTransform.position:{eachTransform.position}");
                positions.Add(eachTransform.position);
            }

            Debug.Log($"distances.Count:{positions.Count}");
            return positions
                .OrderBy(position =>
                    Vector2.Distance(_playerConstEntity.ViewPortCenter, 
                        _playerConstEntity.WorldToViewPortPoint(position)))
                .FirstOrDefault();
        }
    }
}