using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using InGame.Player.Entity;
using InGame.Player.Logic;
using InGame.Player.Controller;
using InGame.Player.View;
using MyApplication;
using InGame.Database.ScriptableData;
using OutGame.Database;

namespace InGame.Player.Installer
{
    public class KureInstaller:BasePlayerInstaller
    {
        public override BasePlayerController Install(int playerNum,StageArea stageArea,InGameDatabase inGameDatabase,
            OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            var kureStatus = inGameDatabase.GetKureStatus();
            inGameDatabase.SetKureStatus(kureStatus);
            var kureView = viewGenerator.GenerateKure(inGameDatabase.GetKureConstData().Prefab);
            kureView.Init();
            kureView.transform.position = inGameDatabase.GetPlayerInstancePositions(stageArea)
                .KureInstancePos;
            var weaponView = kureView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var playerAnimatorView = kureView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init();
            UIData uiData = inGameDatabase.GetUIData();
            
            
            var playerStatusView = viewGenerator.GeneratePlayerStatusView(uiData.PlayerStatusView,uiData.Canvas.transform,uiData.PlayerStatusDataPos[playerNum-1]);
            playerStatusView.Init(PlayableCharacterIndex.Kure,inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Kure).MaxHp);
            var playerInputEntity = new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Kure);
            var kureConstEntity = new KureConstEntity(inGameDatabase);
            var playerCommonInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Kure,inGameDatabase);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, commonDatabase,
                PlayableCharacter.Kure,playerNum,kureView.GetTransform());
            var playerTalkEntity = new PlayerTalkEntity(outGameDatabase);
            
            var playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, kureView, playerAnimatorView);
            var playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, kureView,playerAnimatorView);
            var playerPunchLogic = new PlayerPunchLogic(playerInputEntity, playerConstEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, kureView,playerAnimatorView, weaponView);
            var kureSkillLogic = new KureSkillLogic(playerInputEntity, playerConstEntity, kureConstEntity,playerCommonInStageEntity,
                kureView,playerAnimatorView,weaponView);
            var playerReShapeLogic = new PlayerReShapeLogic(kureView, playerInputEntity,playerCommonInStageEntity);
            var playerHealLogic = new PlayerHealLogic(playerConstEntity,playerCommonUpdateableEntity);
            var playerDamageLogic = new PlayerDamageLogic(kureView,playerAnimatorView, playerConstEntity,playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView);
            var playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerCommonInStageEntity, 
                playerCommonUpdateableEntity,kureView,playerAnimatorView);
            var playerParticleLogic = new PlayerParticleLogic(playerConstEntity, kureView,playerCommonInStageEntity);
            var playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerCommonInStageEntity,playerCommonUpdateableEntity, kureView,playerAnimatorView,particleGeneratorView);
            var playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerCommonInStageEntity, kureView);
            var playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Kure);
            var playerLogic = new PlayerTalkLogic(playerTalkEntity, playerAnimatorView, kureView,playerStatusView);
            
            var disposables = new List<IDisposable>
            {
                playerInputEntity, playerCommonUpdateableEntity,playerCommonInStageEntity,playerAnimatorView
            };
            
            return new KureController(playerNum,playerMoveLogic, playerJumpLogic, playerPunchLogic, kureSkillLogic,
                playerReShapeLogic, playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic,
                playerEnterDoorLogic, playableCharacterSelectLogic,playerLogic,disposables,playerCommonUpdateableEntity.OnUse);
        }
    }
}