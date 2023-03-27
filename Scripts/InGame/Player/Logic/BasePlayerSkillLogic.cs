using Cysharp.Threading.Tasks;
using InGame.Player.Entity;
using InGame.Player.View;

namespace InGame.Player.Logic
{
    public abstract class BasePlayerSkillLogic
    {
        protected readonly PlayerInputEntity playerInputEntity;
        protected readonly PlayerCommonInStageEntity playerCommonInStageEntity;
        protected readonly PlayerConstEntity playerConstEntity;
        protected readonly BasePlayerView playerView;
        protected readonly PlayerAnimatorView playerAnimatorView;
        protected readonly WeaponView weaponView;

        protected BasePlayerSkillLogic(PlayerInputEntity playerInputEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity,PlayerConstEntity playerConstEntity,
            BasePlayerView playerView, PlayerAnimatorView playerAnimatorView,WeaponView weaponView)
        {
            this.playerInputEntity = playerInputEntity;
            this.playerConstEntity = playerConstEntity;
            this.playerCommonInStageEntity = playerCommonInStageEntity;
            this.playerView = playerView;
            this.weaponView = weaponView;
            this.playerAnimatorView = playerAnimatorView;
        }

        public abstract void UpdatePlayerSkill();

        public void UpdateStopping()
        {
            playerInputEntity.OffSkillFlag();
        }
        
        protected async void CheckSkillFlagByAnimator()
        {
            if (!playerAnimatorView.IsUsingSkill())
            {
                await UniTask.WaitUntil(() => playerAnimatorView.IsUsingSkill(),
                    cancellationToken: playerAnimatorView.thisToken);
            }
            await UniTask.WaitWhile(() => playerAnimatorView.IsUsingSkill(),
                cancellationToken: playerAnimatorView.thisToken);
            playerCommonInStageEntity.SetIsUsingSkill(false);
        }
    }
}