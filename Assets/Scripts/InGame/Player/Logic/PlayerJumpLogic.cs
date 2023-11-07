using System.Diagnostics;
using InGame.Database;
using InGame.Player.Entity;
using InGame.Stage.View;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InGame.Player.Logic
{
    public class PlayerJumpLogic
    {
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly BasePlayerView _playerView;
        private readonly PlayerAnimatorView _playerAnimatorView;

        private const float DefaultJumpValue = 100f;

        public PlayerJumpLogic(PlayerConstEntity playerConstEntity, PlayerInputEntity playerInputEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, PlayerCommonUpdateableEntity playerCommonUpdateableEntity, 
            BasePlayerView playerView,PlayerAnimatorView playerAnimatorView)
        {
            _playerConstEntity = playerConstEntity;
            _playerInputEntity = playerInputEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerView = playerView;
            _playerAnimatorView = playerAnimatorView;
        }
        
        public void UpdatePlayerJump()
        {
            if (_playerCommonUpdateableEntity.IsDead)
            {
                return;
            }
            SetGroundType();
            SetIsJumping();
            TryOnJump();
        }

        public void UpdateStopping()
        {
            _playerView.SetYVelocity(0);
        }
        
        private void SetGroundType()
        {
            float rayDistance = _playerConstEntity.ToGroundDistance;
            RaycastHit2D downRaycastHit2D = Physics2D.Raycast(_playerView.GetDownToGroundRayPos(), Vector2.down,
                rayDistance, LayerInfo.GroundMask);
            RaycastHit2D upRaycastHit2D = Physics2D.Raycast(_playerView.GetUpGroundRayPos(), Vector2.up,
                rayDistance, LayerInfo.GroundMask);
#if UNITY_EDITOR
            if (_playerView.OnDrawRay)
            {
                DrawGroundRay(rayDistance);
            }
#endif
            if (TryGetTrampolineHit(upRaycastHit2D,downRaycastHit2D,out IBoundAble highJumpAbleStand))
            {
                _playerCommonInStageEntity.SetPrevStandPos(downRaycastHit2D.point);
                _playerCommonInStageEntity.SetHighJumpAbleStand(highJumpAbleStand);
                _playerCommonInStageEntity.SetGroundType(GroundType.Trampoline);
                return;
            }

            if(IsGroundHit(upRaycastHit2D,downRaycastHit2D))
            {
                _playerCommonInStageEntity.SetPrevStandPos(downRaycastHit2D.point);
                _playerCommonInStageEntity.SetGroundType(GroundType.Default);
                return;   
            }

#if UNITY_EDITOR 
            if (IsIgnoreCheckGround())
            {
                _playerCommonInStageEntity.SetGroundType(GroundType.Default);
                return;
            }
#endif
            _playerCommonInStageEntity.SetGroundType(GroundType.None);
        }
        
        private bool TryGetTrampolineHit(RaycastHit2D upRaycastHit2D,RaycastHit2D downRaycastHit2D,out IBoundAble boundAble)
        {
            GameObject groundObj = null;
            if (upRaycastHit2D.collider != null)
            {
                groundObj = upRaycastHit2D.collider.gameObject;
            }
            else if (downRaycastHit2D.collider != null)
            {
                //Debug.Log($"groundObj:{downRaycastHit2D.collider.gameObject.name}",downRaycastHit2D.collider);
                groundObj = downRaycastHit2D.collider.gameObject;
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
        
        private bool IsGroundHit(RaycastHit2D upRaycastHit2D,RaycastHit2D downRaycastHit2D)
        {
            return upRaycastHit2D.collider == null && downRaycastHit2D.collider != null;
        }

        /// <summary>
        /// 床に地面があったらジャンプを終了する
        /// </summary>
        private void SetIsJumping()
        {
            if (_playerCommonUpdateableEntity.IsWarping)
            {
                return;
            }
            if (_playerCommonInStageEntity.IsGround)
            {
                if (_playerCommonInStageEntity.IsJumping.Value)
                {
                    SEManager.Instance.Play(SEPath.JUMP_LANDING);
                }
                _playerCommonInStageEntity.SetIsJumping(false);
                _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsVerticalMove,false);
                return;
            }
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsVerticalMove,true);
            _playerCommonInStageEntity.SetIsJumping(true);
        }

        private void TryOnJump()
        {
            if (!_playerCommonInStageEntity.IsGround)
            {
                _playerInputEntity.OffJumpFlag();
                return;
            }
            if (_playerInputEntity.jumpFlag)
            {
                if (TryJumpTrampoline())
                {
                    _playerCommonInStageEntity.ClearCurrentDelayCount();
                    return;
                }
                if (TryJumpNormal())
                {
                    _playerCommonInStageEntity.ClearCurrentDelayCount();
                    return;
                }
            }
            TryBound();
        }

        private bool TryBound()
        {
            _playerCommonInStageEntity.AddCurrentBoundDelayCount();
            bool isMaxDelayCount = _playerCommonInStageEntity.currentBoundDelayCount >_playerConstEntity.BoundDelayCount;
            
            if (_playerCommonInStageEntity.onGroundType==GroundType.Trampoline&&isMaxDelayCount)
            {
                //_playerCommonInStageEntity.highJumpAbleStand?.PlayPressAnimation(); //MEMO: バウンド時はアニメーションしないように変更
                SEManager.Instance.Play(SEPath.TRAMPOLINE,volumeRate:0.8f,pitch:1.7f);
                Jump(_playerConstEntity.BoundValue);
                _playerCommonInStageEntity.ClearCurrentDelayCount();
                return true;
            }
            return false;
        }

        private bool TryJumpTrampoline()
        {
            if (_playerCommonInStageEntity.onGroundType==GroundType.Trampoline)
            {
                float boundPower = _playerConstEntity.HighJumpValue;
                IBoundAble boundAble = _playerCommonInStageEntity.boundAble;
                if (boundAble!=null)
                {
                    boundPower *= boundAble.BoundPower;
                    boundAble.PlayPressAnimation();
                }
                SEManager.Instance.Play(SEPath.TRAMPOLINE);
                Jump(boundPower);
                return true;
            }
            return false;
        }
        
        private bool TryJumpNormal()
        {
            if(_playerCommonInStageEntity.onGroundType==GroundType.Default)
            {
                SEManager.Instance.Play(SEPath.JUMP);
                Jump(_playerConstEntity.JumpValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ジャンプ全種共通処理
        /// </summary>
        /// <param name="ySpeed"></param>
        private void Jump(float ySpeed)
        {
            _playerView.SetYVelocity(ySpeed);
            PlayOnJumpAnimation(ySpeed);
            _playerCommonInStageEntity.OnJumpTrigger();
            _playerInputEntity.OffJumpFlag();
        }
        
        //MEMO: ジャンプの予備動作があった場合に必要
        private void PlayOnJumpAnimation(float yParamValue)
        {
            _playerAnimatorView.PlayFloatAnimation(PlayerAnimatorParameter.VerticalMove, Mathf.Abs(yParamValue));
        }
        

#if UNITY_EDITOR
        private bool IsIgnoreCheckGround()
        {
            if (_playerView.IgnoreCheckGround)
            {
                return true;
            }
            return false;
        }
#endif
        
        [Conditional("UNITY_EDITOR")]
        private void DrawGroundRay(float rayDistance)
        {
            Debug.DrawRay(_playerView.GetDownToGroundRayPos(),Vector3.down*rayDistance,Color.blue,0.5f);
            Debug.DrawRay(_playerView.GetUpGroundRayPos(),Vector3.up*rayDistance,Color.red,0.5f);
        }

    }
}