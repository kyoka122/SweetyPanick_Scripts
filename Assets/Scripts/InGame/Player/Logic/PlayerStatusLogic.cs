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
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly BasePlayerView _playerView;
        private readonly PlayerAnimatorView _playerAnimatorView;

        public PlayerStatusLogic(PlayerConstEntity playerConstEntity, PlayerCommonInStageEntity playerCommonInStageEntity, 
            PlayerCommonUpdateableEntity playerCommonUpdateableEntity,PlayerInputEntity playerInputEntity,BasePlayerView playerView,
            PlayerAnimatorView playerAnimatorView)
        {
            _playerConstEntity = playerConstEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerInputEntity=playerInputEntity;
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

        public void Pause()
        {
            _playerAnimatorView.Pause();
            Debug.Log("Pause");
        }

        public void Reset()
        {
            _playerAnimatorView.Rebind();
            _playerInputEntity.ResetAllFlag();
            Debug.Log("ResetFlag");
        }
        
        public PlayableCharacter GetCharacterType()
        {
            return _playerConstEntity.Type;
        }
    }
}