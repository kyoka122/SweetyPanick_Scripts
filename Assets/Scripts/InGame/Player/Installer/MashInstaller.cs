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
    public class MashInstaller:BasePlayerInstaller
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
            bool isUsed = inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Mash).isUsed;
            
            var mashView = viewGenerator.GenerateMash(inGameDatabase.GetMashConstData().Prefab);
            mashView.Init();
            mashView.transform.position = inGameDatabase.GetPlayerInstanceData(stageArea)
                .GetPosition(PlayableCharacter.Mash);
            var weaponView = mashView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var actionKeyView = mashView.GetFirstActionKeyObject().GetComponent<ActionKeyView>();
            if (isUsed)
            {
                actionKeyView.Init(playerInputEntity.DeviceType);
            }
            var playerAnimatorView = mashView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init(PlayableCharacter.Mash);
            StageUIData stageUIData = inGameDatabase.GetUIData();
            
            var playerStatusView =
                viewGenerator.GeneratePlayerStatusView(stageUIData.PlayerStatusView, stageUIData.Canvas.transform,
                    stageUIData.PlayerStatusDataPos[playerNum-1]);
            
            var playerUpdateableData = inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Mash);
            var playerCommonStatusData = inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Mash);
            playerStatusView.Init(PlayableCharacterIndex.Mash,playerCommonStatusData.MaxHp,
                playerUpdateableData.currentHp,playerCommonStatusData.HealHpToRevive, isUsed, playerUpdateableData.isDead);
            
            
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Mash);
            var mashConstEntity = new MashConstEntity(inGameDatabase);
            var playerCommonInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Mash,inGameDatabase);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, commonDatabase,
                PlayableCharacter.Mash,playerNum,mashView.GetTransform());//MEMO: PlayerUpdateableDataが存在する事を確認してから実行
            var playerTalkEntity = new PlayerTalkEntity(outGameDatabase);
            
            
            var playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, mashView,playerAnimatorView);
            var playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, mashView,playerAnimatorView);
            var playerPunchLogic = new PlayerPunchLogic(playerInputEntity, playerConstEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, mashView,playerAnimatorView,weaponView);
            var mashSkillLogic = new MashSkillLogic(playerInputEntity, playerConstEntity, mashConstEntity,playerCommonInStageEntity,
                mashView,playerAnimatorView,weaponView);
            var playerReShapeLogic = new PlayerReShapeLogic(mashView, actionKeyView, playerInputEntity, playerCommonInStageEntity);
            var playerHealLogic = new PlayerHealLogic(playerConstEntity, playerCommonUpdateableEntity,
                playerCommonInStageEntity, playerStatusView);
            var playerDamageLogic = new PlayerDamageLogic(mashView,playerAnimatorView, playerConstEntity,playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView,actionKeyView);
            var playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, playerInputEntity, mashView, playerAnimatorView);
            var playerParticleLogic = new PlayerParticleLogic(playerConstEntity, mashView, playerCommonInStageEntity,
                particleGeneratorView, healHpBarParticleGeneratorView);
            var playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerCommonInStageEntity,playerCommonUpdateableEntity,  mashView,playerAnimatorView);
            var playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerCommonInStageEntity, mashView);
            var playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Mash);
            var playerTalkLogic = new PlayerTalkLogic(playerTalkEntity,playerCommonUpdateableEntity, playerAnimatorView, 
                mashView,playerStatusView);
            var playerGetKeyLogic = new PlayerGetKeyLogic(playerCommonUpdateableEntity,playerConstEntity,
                playerCommonInStageEntity,mashView, inGameDatabase.GetUIData().Key);
            var firstActionKeyLogic = new ActionKeyLogic(playerCommonInStageEntity, playerCommonUpdateableEntity,
                playerConstEntity,playerInputEntity, mashView, weaponView, actionKeyView);
            var playerReviveLogic = new PlayerReviveLogic(playerCommonUpdateableEntity, playerCommonInStageEntity,
                mashView, playerStatusView, reviveCharacterCallbackAnimatorView,actionKeyView);
            
            var disposables = new List<IDisposable>
            {
                playerInputEntity, playerCommonUpdateableEntity,playerCommonInStageEntity,playerAnimatorView
            };
            
            return new MashController(playerNum,playerMoveLogic, playerJumpLogic, playerPunchLogic, mashSkillLogic,
                playerReShapeLogic, playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic,
                playerEnterDoorLogic, playableCharacterSelectLogic,playerTalkLogic,playerGetKeyLogic, firstActionKeyLogic,
                playerReviveLogic, disposables, isUsed, playerCommonUpdateableEntity.IsInStage, playerCommonUpdateableEntity.OnIsInStage, playerCommonUpdateableEntity.OnReviving);
        }
    }
}