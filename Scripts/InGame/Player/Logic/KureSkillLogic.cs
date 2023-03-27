using System;
using InGame.Player.Entity;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class KureSkillLogic:BasePlayerSkillLogic
    {
        private Action<FromPlayerEvent> _playerEvent;

        public KureSkillLogic(PlayerInputEntity playerInputEntity, PlayerConstEntity playerConstEntity,KureConstEntity kureConstEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, KureView kureView, PlayerAnimatorView playerAnimatorView,
            WeaponView weaponView)
            : base(playerInputEntity, playerCommonInStageEntity, playerConstEntity, kureView, playerAnimatorView, weaponView)
        {
        }

        public void RegisterPlayerEvent(Action<FromPlayerEvent> playerEvent)
        {
            _playerEvent = playerEvent;
        }
        
        public override void UpdatePlayerSkill()
        {
            if (!playerInputEntity.skillFlag)
            {
                return;
            }
            playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnSkill);
            playerCommonInStageEntity.SetIsUsingSkill(true);
            CheckSkillFlagByAnimator();
            _playerEvent.Invoke(FromPlayerEvent.AllPlayerHeal);
            Debug.Log($"HealEventInvoke!");
            playerInputEntity.OffSkillFlag();
        }

        public void ConsumeHealPower()
        {
            
        }
    }
}