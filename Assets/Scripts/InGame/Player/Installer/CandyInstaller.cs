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
        public override BasePlayerController Install(int playerNum,StageArea stageArea,InGameDatabase inGameDatabase,
            OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            PlayerInputEntity playerInputEntity=new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
            return Install(playerNum, stageArea, inGameDatabase, outGameDatabase, commonDatabase, playerInputEntity);
        }
        
        private BasePlayerController Install(int playerNum,StageArea stageArea,InGameDatabase inGameDatabase,
            OutGameDatabase outGameDatabase,CommonDatabase commonDatabase,PlayerInputEntity playerInputEntity)
        {
            bool isUsed = inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Candy).isUsed;
            
            var candyView = viewGenerator.GenerateCandy(inGameDatabase.GetCandyConstData().Prefab);
            candyView.Init();
            candyView.transform.position = inGameDatabase.GetPlayerInstanceData(stageArea)
                .GetPosition(PlayableCharacter.Candy);
            var weaponView = candyView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var playerAnimatorView = candyView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init(PlayableCharacter.Candy);
            StageUIData stageUIData = inGameDatabase.GetUIData();
            var playerStatusView = viewGenerator.GeneratePlayerStatusView(stageUIData.PlayerStatusView,
                stageUIData.Canvas.transform,stageUIData.PlayerStatusDataPos[playerNum-1]);
            playerStatusView.Init(PlayableCharacterIndex.Candy,inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Candy).MaxHp,
                inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Candy).currentHp,isUsed);
            
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Candy);
            var playerCommonInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Candy,inGameDatabase);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, commonDatabase,
                PlayableCharacter.Candy,playerNum,candyView.GetTransform());//MEMO: PlayerUpdateableDataが存在する事を確認してから実行
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
            var playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity, candyView, playerAnimatorView);
            var playerParticleLogic = new PlayerParticleLogic(playerConstEntity, candyView,playerCommonInStageEntity,
                particleGeneratorView);
            var playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerCommonInStageEntity, playerCommonUpdateableEntity,candyView,playerAnimatorView);
            var playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerCommonInStageEntity, candyView);
            var playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerCommonInStageEntity
                , playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Candy);
            var playerTalkLogic = new PlayerTalkLogic(playerTalkEntity,playerCommonUpdateableEntity, playerAnimatorView, 
                candyView,playerStatusView);
            var playerGetKeyLogic = new PlayerGetKeyLogic(playerCommonUpdateableEntity,playerConstEntity,
                playerCommonInStageEntity,candyView, inGameDatabase.GetUIData().Key);
            
            var disposables = new List<IDisposable>
            {
                playerInputEntity, playerCommonUpdateableEntity,playerCommonInStageEntity,playerAnimatorView
            };
            
            return new CandyController(playerNum,playerMoveLogic, playerJumpLogic, playerPunchLogic, candySkillLogic,
                playerReShapeLogic, playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic,
                playerEnterDoorLogic,playableCharacterSelectLogic,playerTalkLogic,playerGetKeyLogic,disposables,
                playerCommonUpdateableEntity.OnUse);
        }

        

    }
}