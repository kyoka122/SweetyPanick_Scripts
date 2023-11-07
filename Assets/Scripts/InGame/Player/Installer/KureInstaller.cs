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
    public class KureInstaller:BasePlayerInstaller
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
            bool isUsed = inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Kure).isUsed;
            
            var kureView = viewGenerator.GenerateKure(inGameDatabase.GetKureConstData().Prefab);
            kureView.Init();
            kureView.transform.position = inGameDatabase.GetPlayerInstanceData(stageArea)
                .GetPosition(PlayableCharacter.Kure);
            var weaponView = kureView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var actionKeyView = kureView.GetFirstActionKeyObject().GetComponent<ActionKeyView>();
            if (isUsed)
            {
                actionKeyView.Init(playerInputEntity.DeviceType);
            }
            var playerAnimatorView = kureView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init(PlayableCharacter.Kure);
            StageUIData stageUIData = inGameDatabase.GetUIData();
            
            var playerStatusView = viewGenerator.GeneratePlayerStatusView(stageUIData.PlayerStatusView,
                stageUIData.Canvas.transform,stageUIData.PlayerStatusDataPos[playerNum-1]);
            
            var playerUpdateableData = inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Kure);
            var playerCommonStatusData = inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Kure);
            playerStatusView.Init(PlayableCharacterIndex.Kure,playerCommonStatusData.MaxHp,
                playerUpdateableData.currentHp,playerCommonStatusData.HealHpToRevive, isUsed, playerUpdateableData.isDead);
            
            
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Kure);
            var kureConstEntity = new KureConstEntity(inGameDatabase);
            var playerCommonInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Kure,inGameDatabase);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, commonDatabase,
                PlayableCharacter.Kure,playerNum,kureView.GetTransform());//MEMO: PlayerUpdateableDataが存在する事を確認してから実行
            var playerTalkEntity = new PlayerTalkEntity(outGameDatabase);
            
            var playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, kureView, playerAnimatorView);
            var playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, kureView,playerAnimatorView);
            var playerPunchLogic = new PlayerPunchLogic(playerInputEntity, playerConstEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, kureView,playerAnimatorView, weaponView);
            var kureSkillLogic = new KureSkillLogic(playerInputEntity, playerConstEntity, kureConstEntity,playerCommonInStageEntity,
                kureView,playerAnimatorView,weaponView);
            var playerReShapeLogic = new PlayerReShapeLogic(kureView, actionKeyView, playerInputEntity,playerCommonInStageEntity);
            var playerHealLogic = new PlayerHealLogic(playerConstEntity, playerCommonUpdateableEntity,
                playerCommonInStageEntity, playerStatusView);
            var playerDamageLogic = new PlayerDamageLogic(kureView,playerAnimatorView, playerConstEntity,playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView,actionKeyView);
            var playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, playerInputEntity, kureView, playerAnimatorView);
            var playerParticleLogic = new PlayerParticleLogic(playerConstEntity, kureView, playerCommonInStageEntity,
                particleGeneratorView, healHpBarParticleGeneratorView);
            var playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerCommonInStageEntity,playerCommonUpdateableEntity, kureView,playerAnimatorView);
            var playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerCommonInStageEntity, kureView);
            var playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Kure);
            var playerLogic = new PlayerTalkLogic(playerTalkEntity,playerCommonUpdateableEntity, playerAnimatorView, 
                kureView,playerStatusView);
            var playerGetKeyLogic = new PlayerGetKeyLogic(playerCommonUpdateableEntity,playerConstEntity,
                playerCommonInStageEntity,kureView, inGameDatabase.GetUIData().Key);
            var firstActionKeyLogic = new ActionKeyLogic(playerCommonInStageEntity, playerCommonUpdateableEntity,
                playerConstEntity,playerInputEntity, kureView, weaponView, actionKeyView);
            var playerReviveLogic = new PlayerReviveLogic(playerCommonUpdateableEntity, playerCommonInStageEntity,
                kureView, playerStatusView, reviveCharacterCallbackAnimatorView,actionKeyView);
            
            var disposables = new List<IDisposable>
            {
                playerInputEntity, playerCommonUpdateableEntity,playerCommonInStageEntity,playerAnimatorView
            };
            
            return new KureController(playerNum,playerMoveLogic, playerJumpLogic, playerPunchLogic, kureSkillLogic,
                playerReShapeLogic, playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic,
                playerEnterDoorLogic, playableCharacterSelectLogic,playerLogic,playerGetKeyLogic,firstActionKeyLogic,
                playerReviveLogic, disposables, isUsed, playerCommonUpdateableEntity.IsInStage, playerCommonUpdateableEntity.OnIsInStage, playerCommonUpdateableEntity.OnReviving);
        }
    }
}