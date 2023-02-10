using InGame.Player.Entity;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerStatusLogic
    {
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly BasePlayerView _playerView;
        private readonly PlayerAnimatorView _playerAnimatorView;

        public PlayerStatusLogic(PlayerConstEntity playerConstEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, BasePlayerView playerView,
            PlayerAnimatorView playerAnimatorView)
        {
            _playerConstEntity = playerConstEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerView = playerView;
            _playerAnimatorView = playerAnimatorView;
        }

        public void UpdateAnimationStatus()
        {
            string animationName=_playerAnimatorView.GetCurrentAnimationName();
            _playerCommonInStageEntity.SetCurrentAnimation(animationName,_playerCommonInStageEntity.CharacterType);
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