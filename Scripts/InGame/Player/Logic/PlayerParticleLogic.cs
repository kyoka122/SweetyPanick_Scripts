using InGame.Player.Entity;
using InGame.Player.View;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerParticleLogic
    {
        private readonly BasePlayerView _playerView;
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;

        public PlayerParticleLogic(PlayerConstEntity playerConstEntity, BasePlayerView playerView,
            PlayerCommonInStageEntity playerCommonInStageEntity)
        {
            _playerConstEntity = playerConstEntity;
            _playerView = playerView;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            RegisterEffects();
            RegisterObserver();
        }

        private void RegisterEffects()
        {
            ParticleSystem runningParticle = _playerView.InstanceParticle(_playerConstEntity.RunParticle);
            _playerView.SetChild(runningParticle.transform);
            _playerCommonInStageEntity.SetRunningParticle(runningParticle);
            
            ParticleSystem onJumpParticle = _playerView.InstanceParticle(_playerConstEntity.OnJumpParticle);
            _playerView.SetChild(onJumpParticle.transform);
            _playerCommonInStageEntity.SetOnJumpParticle(onJumpParticle);
            
            ParticleSystem offJumpParticle = _playerView.InstanceParticle(_playerConstEntity.OffJumpParticle);
            _playerCommonInStageEntity.SetOffJumpParticle(offJumpParticle);
            
            ParticleSystem onPunchParticle = _playerView.InstanceParticle(_playerConstEntity.PunchParticle);
            _playerCommonInStageEntity.SetOnPunchParticle(onPunchParticle);
            
            ParticleSystem onSkillParticle = _playerView.InstanceParticle(_playerConstEntity.SkillParticle);
            _playerCommonInStageEntity.SetOnSkillParticle(onSkillParticle);
        }

        private void RegisterObserver()
        {
            _playerCommonInStageEntity.IsRunning
                .Subscribe(isRunning =>
                {
                    if (isRunning)
                    {
                        _playerView.PlayParticle(_playerCommonInStageEntity.runningParticle);
                    }
                    else
                    {
                        _playerView.StopParticle(_playerCommonInStageEntity.runningParticle);
                    }
                }).AddTo(_playerView);
            
            _playerCommonInStageEntity.OnJump
                .Where(onJump => onJump)
                .Subscribe(_ =>
                {
                    _playerView.PlayParticle(_playerCommonInStageEntity.onJumpParticle);
                }).AddTo(_playerView);
            
            _playerCommonInStageEntity.IsJumping
                .Where(isJumping => !isJumping)
                .Subscribe(_ =>
                {
                    _playerView.PlayParticleAtPlayerPos(_playerCommonInStageEntity.offJumpParticle);
                }).AddTo(_playerView);
            
            _playerCommonInStageEntity.OnPunch
                .Where(onPunch => onPunch)
                .Subscribe(_ =>
                {
                    _playerView.PlayParticleAtPlayerWeaponPos(_playerCommonInStageEntity.onPunchParticle);
                }).AddTo(_playerView);
            
            _playerCommonInStageEntity.OnSkill
                .Where(onSkill => onSkill)
                .Subscribe(_ =>
                {
                    _playerView.PlayParticleAtPlayerPos(_playerCommonInStageEntity.onSkillParticle);
                }).AddTo(_playerView);
        }

    }
}