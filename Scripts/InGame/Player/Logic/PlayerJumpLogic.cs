using System.Diagnostics;
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
            float ySpeed = _playerView.GetVelocity().y;
            UpdateJumpAnimation(ySpeed);
        }

        public void UpdateStopping()
        {
            _playerView.SetYVelocity(0);
        }
        
        private void SetGroundType()
        {
            float rayDistance = _playerConstEntity.ToGroundDistance;
            RaycastHit2D downRaycastHit2D = Physics2D.Raycast(_playerView.GetToGroundRayPos(), Vector2.down,
                rayDistance, LayerInfo.GroundMask);
            RaycastHit2D upRaycastHit2D = Physics2D.Raycast(_playerView.GetToGroundRayPos(), Vector2.up,
                rayDistance, LayerInfo.GroundMask);
#if UNITY_EDITOR
            if (_playerView.OnDrawRay)
            {
                DrawGroundRay(rayDistance);
            }
#endif
            //TODO: トランポリン修正
            if (IsTriggerGroundHit(upRaycastHit2D,downRaycastHit2D))
            {
                _playerCommonInStageEntity.SetPrevStandPos(downRaycastHit2D.point);
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
                Debug.Log($"IsIgnoreCheckGround");
                _playerCommonInStageEntity.SetGroundType(GroundType.Default);
                return;
            }
#endif
            _playerCommonInStageEntity.SetGroundType(GroundType.None);
        }

        private bool IsTriggerGroundHit(RaycastHit2D upRaycastHit2D,RaycastHit2D downRaycastHit2D)
        {
            GameObject groundObj = null;
            if (upRaycastHit2D.collider != null)
            {
                groundObj = upRaycastHit2D.collider.gameObject;
            }
            else if (downRaycastHit2D.collider != null)
            {
                groundObj = downRaycastHit2D.collider.gameObject;
            }
            
            if (groundObj!=null)
            {
                _playerCommonInStageEntity.SetHighJumpAbleStand(groundObj.GetComponent<IHighJumpAbleStand>());
                if (_playerCommonInStageEntity.highJumpAbleStand != null && _playerCommonInStageEntity.highJumpAbleStand.HighJumpAble)
                {
                    return true;
                }
            }
            return false;
        }
        
        private bool IsGroundHit(RaycastHit2D upRaycastHit2D,RaycastHit2D downRaycastHit2D)
        {
            return upRaycastHit2D.collider == null && downRaycastHit2D.collider != null;
        }

        private void SetIsJumping()
        {
            if (_playerCommonInStageEntity.IsGround)
            {
                if (_playerCommonInStageEntity.IsJumping.Value)
                {
                    SEManager.Instance.Play(SEPath.JUMP_LANDING);
                }
                _playerCommonInStageEntity.SetIsJumping(false);
                return;
            }
            _playerCommonInStageEntity.SetIsJumping(true);
        }
        
        private void TryOnJump()
        {
            if (!_playerInputEntity.jumpFlag)
            {
                return;
            }
            if (!_playerCommonInStageEntity.IsGround||_playerView.GetVelocity().y>0)
            {
                _playerInputEntity.OffJumpFlag();
                return;
            }

            float ySpeed=0;
            if (_playerCommonInStageEntity.playerOnGroundType==GroundType.Trampoline)
            {
                ySpeed = _playerConstEntity.HighJumpValue * DefaultJumpValue;
                SEManager.Instance.Play(SEPath.TRAMPOLINE);
            }
            else if(_playerCommonInStageEntity.playerOnGroundType==GroundType.Default)
            {
                ySpeed = _playerConstEntity.JumpValue * DefaultJumpValue;
                SEManager.Instance.Play(SEPath.JUMP);
            }
            
            _playerView.AddYVelocity(ySpeed);
            PlayOnJumpAnimation(ySpeed);
            if (_playerCommonInStageEntity.highJumpAbleStand!=null)
            {
                _playerCommonInStageEntity.highJumpAbleStand.PlayPressAnimation();
            }
            
            _playerCommonInStageEntity.OnJumpTrigger();
            _playerInputEntity.OffJumpFlag();
        }

        private void UpdateJumpAnimation(float yParamValue)
        {
            _playerAnimatorView.PlayFloatAnimation(PlayerAnimatorParameter.VerticalMove,Mathf.Abs(yParamValue));
            if (yParamValue==0)
            {
                _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsVerticalMove,false);
                return;
            }
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsVerticalMove,true);
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
            Debug.DrawRay(_playerView.GetToGroundRayPos(),Vector3.down*rayDistance,Color.blue,0.5f);
            Debug.DrawRay(_playerView.GetToGroundRayPos(),Vector3.up*rayDistance,Color.red,0.5f);
        }

    }
}