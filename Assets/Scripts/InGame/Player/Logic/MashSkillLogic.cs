using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using InGame.Enemy.View;
using InGame.Player.Entity;
using InGame.Player.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class MashSkillLogic:BasePlayerSkillLogic
    {
        private MashNekoView _mashNekoView;
        private readonly MashConstEntity _mashConstEntity;
        private readonly MashView _mashView;

        public MashSkillLogic(PlayerInputEntity playerInputEntity, PlayerConstEntity playerConstEntity,MashConstEntity mashConstEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, MashView mashView, PlayerAnimatorView playerAnimatorView,
            WeaponView weaponView)
            : base(playerInputEntity, playerCommonInStageEntity, playerConstEntity, mashView, playerAnimatorView, weaponView)
        {
            _mashConstEntity = mashConstEntity;
            _mashView = mashView;
        }

        public override void UpdatePlayerSkill()
        {
            if (_mashNekoView == null)
            {
                TryOnSkill();
                return;
            }

            CheckEnemyDecoyAble();
        }

        private void TryOnSkill()
        {
            if (!playerInputEntity.skillFlag)
            {
                return;
            }
     
            playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnSkill);
            playerCommonInStageEntity.SetIsUsingSkill(true);
            CheckSkillFlagByAnimator();
            InstallMashNeko();
            RegisterMashNekoObserver();
            
            TimeBindEnemy(_mashNekoView).Forget();
            
            playerCommonInStageEntity.OnSkillTrigger();
            playerInputEntity.OffSkillFlag();
        }

        private void InstallMashNeko()
        {
            Vector2 instancePos = (Vector2) _mashView.transform.position + new Vector2(
                _mashConstEntity.toMashNekoInstanceVec.x * playerCommonInStageEntity.playerDirection,
                _mashConstEntity.toMashNekoInstanceVec.y);
            
            _mashNekoView = _mashView.GenerateMashNeko(_mashConstEntity.mashNekoPrefab,instancePos);
            _mashNekoView.Init(playerCommonInStageEntity.playerDirection);
        }

        private void RegisterMashNekoObserver()
        {
            _mashNekoView.OnTriggerEnter
                .Subscribe(collider2D =>
                {
                    if (!collider2D.TryGetComponent(out IEnemyDecoyAble decoyAbleEnemy))
                    {
                        return;
                    }
                    if (!decoyAbleEnemy.isDecoyAble)
                    {
                        return;
                    }

                    decoyAbleEnemy.OnDecoy(_mashNekoView.GetTransform());
                    _mashNekoView.RegisterDecoyEnemy(decoyAbleEnemy);
                    
                }).AddTo(_mashNekoView);

        }

        private async UniTask TimeBindEnemy(MashNekoView mashNekoViewCache)
        {
            var token = mashNekoViewCache.thisToken;
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_mashConstEntity.mashNekoAliveTime), cancellationToken: token);
                //アニメーション
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"MashNeko Destroyed");
                return;
            }

            DestroyMashNeko(mashNekoViewCache);
        }

        private void CheckEnemyDecoyAble()
        {
            var decoyAbleEnemies = new List<IEnemyDecoyAble>(_mashNekoView.decoyAbleEnemies);
            foreach (var isNotDecoyAble in decoyAbleEnemies.Where(x => !x.isDecoyAble))
            {
                isNotDecoyAble.OffDecoy();
                _mashNekoView.ReleaseEnemyFromList(isNotDecoyAble);
            }
        } 

        private void DestroyMashNeko(MashNekoView mashNekoViewCache)
        {
            //TODO: エフェクト?
            _mashNekoView = null;
            ReleaseEnemies();
            mashNekoViewCache.Destroy();
        }

        private void ReleaseEnemies()
        {
            var decoyAbleEnemies = new List<IEnemyDecoyAble>(_mashNekoView.decoyAbleEnemies);
            foreach (var decoyAble in decoyAbleEnemies)
            {
                decoyAble.OffDecoy();
            }
        }
    }
}