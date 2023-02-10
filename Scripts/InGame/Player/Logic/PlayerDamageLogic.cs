using System;
using Cysharp.Threading.Tasks;
using InGame.Player.Entity;
using InGame.Player.View;
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
            PlayerConstEntity playerConstEntity ,PlayerCommonInStageEntity playerCommonInStageEntity,PlayerCommonUpdateableEntity playerCommonUpdateableEntity,PlayerStatusView playerStatusView)
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
            
            _playerCommonUpdateableEntity.OnDead
                .Where(onDefeated=>onDefeated)
                .Subscribe(_ =>
                {
                    Defeat();
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
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.OnDamaged, true);
            Damage();
            float knockBackDirection = (_playerView.transform.position.x - enemyPos.x) >= 0 ? 1 : -1;
            _playerView.AddXVelocity(_playerConstEntity.KnockBackValue * knockBackDirection -
                                     _playerView.GetVelocity().x,ForceMode2D.Impulse);
            WaitDamageAnimation();
        }

        private void TryDamageByFallen()
        {
            if (_playerCommonUpdateableEntity.IsDead)
            {
                return;
            }
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.OnDamaged, true);
            Damage();
            WaitDamageAnimation();
        }
        
        private void Damage()
        {
            _playerCommonUpdateableEntity.DamageDefault();
            _playerStatusView.Damage(_playerCommonUpdateableEntity.CurrentHp);
        }

        private async void WaitDamageAnimation()
        {
            _playerCommonUpdateableEntity.SetCanDamageFlag(false);
            _playerView.SetLayer(LayerInfo.NotCollideEnemyPlayerNum);
            try
            {
                await UniTask.WaitWhile(
                    () => _playerAnimatorView.GetCurrentAnimationName() ==
                          PlayerAnimationName.GetEachName(_playerView.type, PlayerAnimationName.OnDamaged),
                    cancellationToken: _playerAnimatorView.thisToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"NockBack Canceled");
            }
            _playerView.SetLayer(LayerInfo.PlayerNum);
            _playerCommonUpdateableEntity.SetCanDamageFlag(true);
        }

        private async void Defeat()
        {
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.OnDeath, true);
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken:_playerView.thisToken);
            _playerView.gameObject.SetActive(false);
            //_playerConstEntity.ObjectInScreenRange
        }
    }
}