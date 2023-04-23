using InGame.Player.Entity;
using InGame.Player.View;
using MyApplication;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerStatusLogic
    {
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly BasePlayerView _playerView;
        private readonly PlayerAnimatorView _playerAnimatorView;

        public PlayerStatusLogic(PlayerConstEntity playerConstEntity, PlayerCommonInStageEntity playerCommonInStageEntity, 
            PlayerCommonUpdateableEntity playerCommonUpdateableEntity,BasePlayerView playerView,
            PlayerAnimatorView playerAnimatorView)
        {
            _playerConstEntity = playerConstEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerView = playerView;
            _playerAnimatorView = playerAnimatorView;
        }

        public int GetPlayerNum()
        {
            return _playerCommonUpdateableEntity.PlayerNum;
        }
        
        public void SetInstalled()
        {
            //MEMO: カメラのターゲットに登録するため
            if (_playerCommonUpdateableEntity.IsUsed && !_playerCommonUpdateableEntity.IsDead)
            {
                _playerCommonUpdateableEntity.SetCanTarget(true);
            }
            else
            {
                _playerView.SetActive(false);
            }
        }

        public void SetActivePlayerView(bool active)
        {
            _playerView.SetActive(active);
        }
        
        public void TrySetActivePlayer()
        {
            if (!_playerCommonUpdateableEntity.IsUsed || _playerCommonUpdateableEntity.IsDead)
            {
                _playerView.SetActive(false);
            }
        }

        public void Stop()
        {
            _playerAnimatorView.ResetAllParameter();
            _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.ForceIdle);
        }
        
        public PlayableCharacter GetCharacterType()
        {
            return _playerConstEntity.Type;
        }
    }
}