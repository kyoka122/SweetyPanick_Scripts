using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using InGame.Player.Entity;
using InGame.Player.Logic;
using InGame.Player.Controller;
using InGame.Player.View;
using MyApplication;
using OutGame.Database;

namespace InGame.Player.Installer
{
    public class FuInstaller : BasePlayerInstaller
    {
        public override BasePlayerController Install(int playerNum,StageArea stageArea,InGameDatabase inGameDatabase,
            OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            PlayerInputEntity playerInputEntity=new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
            return Install(playerNum, stageArea, inGameDatabase, outGameDatabase, commonDatabase, playerInputEntity);
        }
        
        private BasePlayerController Install(int playerNum,StageArea stageArea,InGameDatabase inGameDatabase,
            OutGameDatabase outGameDatabase,CommonDatabase commonDatabase,PlayerInputEntity playerInputEntity)
        {
            bool isUsed = inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Fu).isUsed;
            
            var fuView = viewGenerator.GenerateFu(inGameDatabase.GetFuConstData().Prefab);
            fuView.Init();
            fuView.transform.position = inGameDatabase.GetPlayerInstanceData(stageArea)
                .GetPosition(PlayableCharacter.Fu);
            var weaponView = fuView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var actionKeyView = fuView.GetFirstActionKeyObject().GetComponent<ActionKeyView>();
            if (isUsed)
            {
                actionKeyView.Init(playerInputEntity.DeviceType);
            }
            var playerAnimatorView = fuView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init(PlayableCharacter.Fu);
            StageUIData stageUIData = inGameDatabase.GetUIData();
            
            var playerStatusView = viewGenerator.GeneratePlayerStatusView(stageUIData.PlayerStatusView, 
                stageUIData.Canvas.transform,stageUIData.PlayerStatusDataPos[playerNum-1]);
            
            var playerUpdateableData = inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Fu);
            var playerCommonStatusData = inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Fu);
            playerStatusView.Init(PlayableCharacterIndex.Fu,playerCommonStatusData.MaxHp,
                playerUpdateableData.currentHp,playerCommonStatusData.HealHpToRevive, isUsed, playerUpdateableData.isDead);
            
            
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Fu);
            var fuConstEntity = new FuConstEntity(inGameDatabase);
            var playerCommonInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Fu,inGameDatabase);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, commonDatabase,
                PlayableCharacter.Fu,playerNum,fuView.GetTransform());//MEMO: PlayerUpdateableDataが存在する事を確認してから実行
            var playerTalkEntity = new PlayerTalkEntity(outGameDatabase);
            
            
            var playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,fuView,playerAnimatorView);
            var playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, fuView,playerAnimatorView);
            var playerPunchLogic = new PlayerPunchLogic(playerInputEntity, playerConstEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,fuView,playerAnimatorView,weaponView);
            var fuSkillLogic = new FuSkillLogic(playerInputEntity, playerConstEntity,fuConstEntity, playerCommonInStageEntity,
                fuView,playerAnimatorView,weaponView);
            var playerReShapeLogic = new PlayerReShapeLogic(fuView, actionKeyView, playerInputEntity, playerCommonInStageEntity);
            var playerHealLogic = new PlayerHealLogic(playerConstEntity, playerCommonUpdateableEntity,
                playerCommonInStageEntity, playerStatusView);
            var playerDamageLogic = new PlayerDamageLogic(fuView,playerAnimatorView, playerConstEntity,playerCommonInStageEntity
                ,playerCommonUpdateableEntity,playerStatusView,actionKeyView);
            var playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, playerInputEntity, fuView,playerAnimatorView);
            var playerParticleLogic = new PlayerParticleLogic(playerConstEntity, fuView, playerCommonInStageEntity,
                particleGeneratorView, healHpBarParticleGeneratorView);
            var playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerCommonInStageEntity,playerCommonUpdateableEntity, fuView,playerAnimatorView);
            var playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerCommonInStageEntity, fuView);
            var playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerCommonInStageEntity, 
                playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Fu);
            var playerTalkLogic = new PlayerTalkLogic(playerTalkEntity,playerCommonUpdateableEntity, playerAnimatorView, 
                fuView,playerStatusView);
            var playerGetKeyLogic = new PlayerGetKeyLogic(playerCommonUpdateableEntity,playerConstEntity,
                playerCommonInStageEntity,fuView, inGameDatabase.GetUIData().Key);
            var firstActionKeyLogic = new ActionKeyLogic(playerCommonInStageEntity, playerCommonUpdateableEntity,
                playerConstEntity,playerInputEntity, fuView, weaponView, actionKeyView);
            var playerReviveLogic = new PlayerReviveLogic(playerCommonUpdateableEntity, playerCommonInStageEntity,
                fuView, playerStatusView, reviveCharacterCallbackAnimatorView,actionKeyView);
            
            var disposables = new List<IDisposable>
            {
                playerInputEntity, playerCommonUpdateableEntity,playerCommonInStageEntity,playerAnimatorView
                
            };

            return new FuController(playerNum, playerMoveLogic, playerJumpLogic, playerPunchLogic, fuSkillLogic,
                playerReShapeLogic, playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic,
                playerEnterDoorLogic, playableCharacterSelectLogic, playerTalkLogic,playerGetKeyLogic,firstActionKeyLogic,
                playerReviveLogic, disposables, isUsed, playerCommonUpdateableEntity.IsInStage,playerCommonUpdateableEntity.OnIsInStage, playerCommonUpdateableEntity.OnReviving);
        }
    }
}