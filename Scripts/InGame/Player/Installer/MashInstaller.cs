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
using UnityEngine;

namespace InGame.Player.Installer
{
    public class MashInstaller:BasePlayerInstaller
    {
        public override BasePlayerController Install(int playerNum,StageArea stageArea, InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            if (inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Mash)==null)
            {
                inGameDatabase.SetPlayerUpdateableData(PlayableCharacter.Mash, new PlayerUpdateableData(playerNum,
                    inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Mash).MaxHp,true,false));
            }
            
            var mashView = viewGenerator.GenerateMash(inGameDatabase.GetMashConstData().Prefab);
            mashView.Init();
            mashView.transform.position = inGameDatabase.GetPlayerInstanceData(stageArea)
                .GetPosition(PlayableCharacter.Mash);
            var weaponView = mashView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var playerAnimatorView = mashView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init(PlayableCharacter.Mash);
            StageUIData stageUIData = inGameDatabase.GetUIData();
            
            var playerStatusView =
                viewGenerator.GeneratePlayerStatusView(stageUIData.PlayerStatusView, stageUIData.Canvas.transform,stageUIData.PlayerStatusDataPos[playerNum-1]);
            playerStatusView.Init(PlayableCharacterIndex.Mash,inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Mash).MaxHp,
                inGameDatabase.GetPlayerUpdateableData(PlayableCharacter.Mash).currentHp);
            
            
            var playerInputEntity = new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
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
            var playerReShapeLogic = new PlayerReShapeLogic(mashView, playerInputEntity, playerCommonInStageEntity);
            var playerHealLogic = new PlayerHealLogic(playerConstEntity,playerCommonUpdateableEntity);
            var playerDamageLogic = new PlayerDamageLogic(mashView,playerAnimatorView, playerConstEntity,playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView);
            var playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerCommonInStageEntity, 
                playerCommonUpdateableEntity,mashView,playerAnimatorView);
            var playerParticleLogic = new PlayerParticleLogic(playerConstEntity, mashView, playerCommonInStageEntity);
            var playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerCommonInStageEntity,playerCommonUpdateableEntity,  mashView,playerAnimatorView,particleGeneratorView);
            var playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerCommonInStageEntity, mashView);
            var playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Mash);
            var playerTalkLogic = new PlayerTalkLogic(playerTalkEntity, playerAnimatorView, mashView,playerStatusView);
            var playerGetKeyLogic = new PlayerGetKeyLogic(playerCommonUpdateableEntity,playerConstEntity,
                playerCommonInStageEntity,mashView, inGameDatabase.GetUIData().Key);
            
            var disposables = new List<IDisposable>
            {
                playerInputEntity, playerCommonUpdateableEntity,playerCommonInStageEntity,playerAnimatorView
            };
            
            return new MashController(playerNum,playerMoveLogic, playerJumpLogic, playerPunchLogic, mashSkillLogic,
                playerReShapeLogic, playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic,
                playerEnterDoorLogic, playableCharacterSelectLogic,playerTalkLogic,playerGetKeyLogic, disposables,
                playerCommonUpdateableEntity.OnUse);
        }
    }
}