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
using OutGame.PlayerTalks;
using UnityEngine;

namespace InGame.Player.Installer
{
    /// <summary>
    /// presentation層のインスタンスを返す+初期化処理
    /// </summary>
    public class CandyInstaller:BasePlayerInstaller
    {
        public override BasePlayerController Install(int playerNum,InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            var candyStatus = inGameDatabase.GetCandyStatus();
            inGameDatabase.SetCandyStatus(candyStatus);
            var candyView = viewGenerator.GenerateCandy(inGameDatabase.GetCandyConstData().Prefab);
            candyView.Init();
            candyView.transform.position = inGameDatabase.GetPlayerInstancePositions(StageArea.FirstStageFirst)
                .CandyInstancePos;
            var weaponView = candyView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var playerAnimatorView = candyView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init();
            UIData uiData = inGameDatabase.GetUIData();
            
            
            var playerStatusView = viewGenerator.GeneratePlayerStatusView(uiData.PlayerStatusView,
                uiData.Canvas.transform,uiData.PlayerStatusDataPos[playerNum-1]);
            playerStatusView.Init(PlayableCharacterIndex.Candy,inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Candy).maxHp);
            
            
            var playerInputEntity = new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Candy);
            var playerCommonInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Candy);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, 
                PlayableCharacter.Candy,playerNum,candyView.GetTransform());
            var playerTalkEntity = new PlayerTalkEntity(outGameDatabase);
            
            
            var playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,candyView,playerAnimatorView);
            var playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,candyView,playerAnimatorView);
            var playerPunchLogic = new PlayerPunchLogic(playerInputEntity,playerConstEntity,playerCommonInStageEntity,
                playerCommonUpdateableEntity, candyView,playerAnimatorView,weaponView);
            var playerReShapeLogic = new PlayerReShapeLogic(candyView, playerInputEntity,playerCommonInStageEntity);
            var playerHealLogic = new PlayerHealLogic(playerConstEntity,playerCommonUpdateableEntity);
            var playerDamageLogic = new PlayerDamageLogic(candyView,playerAnimatorView,playerConstEntity,playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView);
            var candySkillLogic = new CandySkillLogic(playerInputEntity,playerConstEntity,playerCommonInStageEntity,
                candyView,playerAnimatorView,weaponView);
            var playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerCommonInStageEntity, candyView,
                playerAnimatorView);
            var playerParticleLogic = new PlayerParticleLogic(playerConstEntity, candyView,playerCommonInStageEntity);
            var playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerCommonInStageEntity, playerCommonUpdateableEntity,candyView,playerAnimatorView,particleGeneratorView);
            var playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerCommonInStageEntity, candyView);
            var playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerCommonInStageEntity
                , playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Candy);
            var playerTalkLogic = new PlayerTalkLogic(playerTalkEntity, playerAnimatorView, candyView);

            var disposables = new List<IDisposable> {playerInputEntity, playerCommonUpdateableEntity};
            return new CandyController(playerNum,playerMoveLogic, playerJumpLogic, playerPunchLogic, candySkillLogic,
                playerReShapeLogic, playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic,
                playerEnterDoorLogic,playableCharacterSelectLogic,playerTalkLogic,disposables,playerCommonUpdateableEntity.OnDead);
        }

    }
}