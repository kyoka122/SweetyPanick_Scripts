using System;
using Cysharp.Threading.Tasks;
using InGame.Player.Entity;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerDamageLogic
    {
        private readonly BasePlayerView _playerView;
        private readonly PlayerAnimatorView _playerAnimatorView;
        private readonly PlayerStatusView _playerStatusView;
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        
        public PlayerDamageLogic(BasePlayerView playerView,PlayerAnimatorView playerAnimatorView,
            PlayerConstEntity playerConstEntity ,PlayerCommonInStageEntity playerCommonInStageEntity,
            PlayerCommonUpdateableEntity playerCommonUpdateableEntity,PlayerStatusView playerStatusView)
        {
            _playerView = playerView;
            _playerConstEntity = playerConstEntity;
            _playerAnimatorView = playerAnimatorView;
            _playerStatusView = playerStatusView;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _playerView.OnCollisionEnterObj
                .Subscribe(TryDamageByEnemy)
                .AddTo(_playerView);

            _playerCommonInStageEntity.OnFall.
                Subscribe(_=>
                {
                    TryDamageByFallen();
                }).AddTo(_playerView);
        }

        private void TryDamageByEnemy(Collision2D collision)
        {
            if (!_playerCommonUpdateableEntity.canAttackedByEnemy)
            {
                return;
            }
                    
            bool isCollided = collision.gameObject.GetComponent<ICollideAbleToPlayer>() != null &&
                              collision.otherCollider.gameObject.layer == LayerInfo.PlayerNum;
            if (isCollided)
            {
                DamageByEnemy(collision.transform.position);
            }
            
        }

        private void DamageByEnemy(Vector2 enemyPos)
        {
            _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnDamaged);
            float knockBackDirection = (_playerView.transform.position.x - enemyPos.x) >= 0 ? 1 : -1;
            _playerView.AddXVelocity(_playerConstEntity.KnockBackValue * knockBackDirection -
                                     _playerView.GetVelocity().x,ForceMode2D.Impulse);
            SEManager.Instance.Play(SEPath.DAMAGE);
            Damage();
            WaitDamageAnimation();
        }

        private void TryDamageByFallen()
        {
            if (_playerCommonUpdateableEntity.IsDead)
            {
                return;
            }

            _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnDamaged);
            Damage();
            WaitDamageAnimation();
        }
        
        private void Damage()
        {
            _playerCommonUpdateableEntity.DamageDefault();
            _playerStatusView.SetHpValue(_playerCommonUpdateableEntity.CurrentHp);
        }

        private async void WaitDamageAnimation()
        {
            _playerCommonUpdateableEntity.SetCanDamageFlag(false);
            _playerView.SetLayer(LayerInfo.NotCollideEnemyNum);
            string damaged=PlayerAnimationName.GetEachName(_playerView.type, PlayerAnimationName.Damaged);
            if (!IsPlayingThisAnimation(damaged))
            {
                await UniTask.WaitUntil(()=>IsPlayingThisAnimation(damaged), 
                    cancellationToken: _playerView.thisToken);
            }
            await UniTask.WaitWhile(()=>IsPlayingThisAnimation(damaged),
                cancellationToken: _playerView.thisToken);

            if (_playerCommonUpdateableEntity.IsDead)
            {
                Defeat();
                return;
            }
            _playerView.SetLayer(LayerInfo.PlayerNum);
            _playerCommonUpdateableEntity.SetCanDamageFlag(true);
        }

        private async void Defeat()
        {
            _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnDeath);
            string death=PlayerAnimationName.GetEachName(_playerView.type, PlayerAnimationName.Death);
            
            if (!IsPlayingThisAnimation(death))
            {
                await UniTask.WaitUntil(()=>IsPlayingThisAnimation(death), 
                    cancellationToken: _playerView.thisToken);
            }

            await UniTask.WaitUntil(()=>IsFinishedThisAnimation(death), cancellationToken: _playerView.thisToken);
            _playerCommonUpdateableEntity.SetCanDamageFlag(false);
            _playerCommonUpdateableEntity.SetOnUseCharacter(false);
            //MEMO: シーン上に他にプレイヤーがいたら
            if (_playerCommonUpdateableEntity.LivingPlayerCount>=1)
            {
                _playerView.gameObject.SetActive(false);
                _playerCommonUpdateableEntity.SetCanTarget(false);
            }
        }

        private bool IsPlayingThisAnimation(string animationName)
        {
            return _playerAnimatorView.GetCurrentAnimationName() == animationName;
        }
        
        private bool IsFinishedThisAnimation(string animationName)
        {
            AnimatorStateInfo stateInfo = _playerAnimatorView.GetCurrentAnimationStateInfo();
            return stateInfo.normalizedTime >= 1 && _playerAnimatorView.GetCurrentAnimationName() == animationName;
        }
    }
}