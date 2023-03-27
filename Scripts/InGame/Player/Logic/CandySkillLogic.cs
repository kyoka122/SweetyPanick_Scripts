using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Enemy.View;
using InGame.Player.Entity;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class CandySkillLogic:BasePlayerSkillLogic
    {
        private readonly List<IEnemyPullAble> _pullAbleEnemies;
        private readonly CandyView _candyView;
        private bool _isCatching;
        
        public CandySkillLogic(PlayerInputEntity playerInputEntity, PlayerConstEntity playerConstEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, CandyView candyView, PlayerAnimatorView playerAnimatorView,
            WeaponView weaponView)
            : base(playerInputEntity, playerCommonInStageEntity, playerConstEntity, candyView, playerAnimatorView, weaponView)
        {
            _candyView = candyView;
            _pullAbleEnemies = new List<IEnemyPullAble>();
            RegisterObserver();
        }

        public override void UpdatePlayerSkill()
        {
            TryOnSkill();
            CheckEnemyBindable();
        }

        private void RegisterObserver()
        {
            playerCommonInStageEntity.OnSkill
                .Where(on => on)
                .Subscribe(_ =>
                {
                    playerCommonInStageEntity.SetIsUsingSkill(true);
                    CheckSkillFlagByAnimator();
                    TimePullEnemy(weaponView.cancellationToken).Forget();

                }).AddTo(playerView);
            
            playerCommonInStageEntity.OnSkill
                .Where(on => !on)
                .Subscribe(_ =>
                {
                    _pullAbleEnemies.Clear();
                    _isCatching = false;
                }).AddTo(playerView);
            
            playerAnimatorView.OnAnimationEvent
                .Subscribe(animationClipName =>
                {
                    if (animationClipName==PlayerAnimationEventName.Skill)
                    {
                        CatchEnemy();
                    }
                }).AddTo(playerView);
        }
        
        private void TryOnSkill()
        {
            if (!playerInputEntity.skillFlag)
            {
                return;
            }
     
            playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnSkill);
            playerCommonInStageEntity.OnSkillTrigger();
            playerInputEntity.OffSkillFlag();
        }

        private void CatchEnemy()
        {
            foreach (var collider in weaponView.TriggerStayColliders)
            {
                if (collider.TryGetComponent(out IEnemyPullAble pullAbleEnemy))
                {
                    pullAbleEnemy.OnPull(weaponView.WeaponCenterTransform);
                    _pullAbleEnemies.Add(pullAbleEnemy);
                    SEManager.Instance.Play(SEPath.CANDY_SKILL);
                }
            }
            _isCatching = true;
        }

        private async UniTask TimePullEnemy(CancellationToken token)
        {
            CheckSkillFlagByAnimator();
            try
            {
                await UniTask.WaitWhile(
                    () => playerAnimatorView.GetCurrentAnimationName() ==
                          PlayerAnimationName.GetEachName(PlayableCharacter.Candy, PlayerAnimationName.Skill),
                    cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Weapon Destroyed During PullingTime");
            }
            
            ReleaseEnemies();
        }

        private void CheckEnemyBindable()
        {
            foreach (var enemy in _pullAbleEnemies.Where(enemy => !enemy.isPullAble))
            {
                enemy.OffPull();
                //SEManager.Instance.Stop(SEPath.CANDY_SKILL);
            }
        }

        private void ReleaseEnemies()
        {
            foreach (var enemy in _pullAbleEnemies)
            {
                enemy.OffPull();
            }
            playerCommonInStageEntity.OffSkillTrigger();
        }
    }
}