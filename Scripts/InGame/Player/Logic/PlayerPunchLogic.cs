using System.Collections.Generic;
using InGame.Enemy.View;
using MyApplication;
using InGame.Player.Entity;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerPunchLogic
    {
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly BasePlayerView _playerView;
        private readonly PlayerAnimatorView _playerAnimatorView;
        private readonly WeaponView _weaponView;

        public PlayerPunchLogic(PlayerInputEntity playerInputEntity, PlayerConstEntity playerConstEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, PlayerCommonUpdateableEntity playerCommonUpdateableEntity,
            BasePlayerView playerView, PlayerAnimatorView playerAnimatorView, WeaponView weaponView)
        {
            _playerInputEntity = playerInputEntity;
            _playerConstEntity = playerConstEntity;
            _playerView = playerView;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerAnimatorView = playerAnimatorView;
            _weaponView = weaponView;

            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _playerAnimatorView.OnAnimationEvent
                .Subscribe(animationClipName =>
                {
                    if (animationClipName==PlayerAnimationEventName.Punch)
                    {
                        Punch();
                    }
                    
                })
                .AddTo(_playerView);
        }

        public void UpdateStopping()
        {
            _playerInputEntity.OffPunchFlag();
        }

        public void UpdatePlayerPunch()
        {
            if (_playerCommonUpdateableEntity.IsDead)
            {
                return;
            }
            if (_playerCommonInStageEntity.IsPunching)//MEMO: Punchアニメーション中
            {
                _playerView.SetLayer(LayerInfo.NotCollideEnemyPlayerNum);//MEMO: 敵との当たり判定を無視
                _playerInputEntity.OffPunchFlag();//MEMO:PunchキーのInputを無視
                return;
            }
            
            _playerView.SetLayer(LayerInfo.PlayerNum);
            if (!_playerInputEntity.punchFlag)
            {
                return;
            }
            _playerView.SetLayer(LayerInfo.NotCollideEnemyPlayerNum);
            _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnPunch);
            _playerCommonInStageEntity.OnPunchTrigger();
            _playerInputEntity.OffPunchFlag();
        }

        private void Punch()
        {
            IReadOnlyList<Collider2D> triggerStayColliders = _weaponView.TriggerStayColliders;
            foreach (var collider in triggerStayColliders)
            {
                if (collider.TryGetComponent(out IEnemyDamageAble damageable))
                {
                    damageable.OnDamaged(new Struct.DamagedInfo(Attacker.Player,_playerView.transform.position));
                    _playerInputEntity.rumble?.Invoke();
                    SEManager.Instance.Play(AudioName.GetAttackPath(_playerView.type));
                }
            }
        }
    }
}