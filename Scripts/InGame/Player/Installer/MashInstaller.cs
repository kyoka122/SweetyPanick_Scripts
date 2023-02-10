using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using InGame.Player.Entity;
using InGame.Player.Logic;
using InGame.Player.Controller;
using InGame.Player.View;
using MyApplication;

namespace InGame.Player.Installer
{
    public class MashInstaller:BasePlayerInstaller
    {
        private PlayerMoveLogic _playerMoveLogic;
        private PlayerJumpLogic _playerJumpLogic;
        private PlayerPunchLogic _playerPunchLogic;
        private MashSkillLogic _mashSkillLogic;
        private PlayerReShapeLogic _playerReShapeLogic;
        private PlayerHealLogic _playerHealLogic;
        private PlayerDamageLogic _playerDamageLogic;
        private PlayerStatusLogic _playerStatusLogic;
        private PlayerParticleLogic _playerParticleLogic;
        private PlayerFixSweetsLogic _playerFixSweetsLogic;
        private PlayerEnterDoorLogic _playerEnterDoorLogic;
        private PlayableCharacterSelectLogic _playableCharacterSelectLogic;

        public override BasePlayerController Install(int playerNum, InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            var mashStatus = inGameDatabase.GetMashStatus();
            inGameDatabase.SetMashStatus(mashStatus);
            var mashView = viewGenerator.GenerateMash(inGameDatabase.GetMashConstData().Prefab);
            mashView.Init();
            mashView.transform.position = inGameDatabase.GetPlayerInstancePositions(StageArea.FirstStageFirst)
                .MashInstancePos;
            var weaponView = mashView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var playerAnimatorView = mashView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init();
            UIData uiData = inGameDatabase.GetUIData();
            
            var playerStatusView =
                viewGenerator.GeneratePlayerStatusView(uiData.PlayerStatusView, uiData.Canvas.transform,uiData.StatusDataPos[playerNum-1]);
            playerStatusView.Init(PlayableCharacterIndex.Mash,inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Mash).maxHp);
            
            
            var playerInputEntity = new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Mash);
            var mashConstEntity = new MashConstEntity(inGameDatabase);
            var playerInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Mash);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, 
                PlayableCharacter.Mash,playerNum,mashView.GetTransform());
            _playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerInStageEntity,
                playerCommonUpdateableEntity,
                
                
                mashView,playerAnimatorView);
            _playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerInStageEntity,
                playerCommonUpdateableEntity, mashView,playerAnimatorView);
            _playerPunchLogic = new PlayerPunchLogic(playerInputEntity, playerConstEntity, playerInStageEntity,
                playerCommonUpdateableEntity, mashView,playerAnimatorView,weaponView);
            _mashSkillLogic = new MashSkillLogic(playerInputEntity, playerConstEntity, mashConstEntity,playerInStageEntity,
                mashView,playerAnimatorView,weaponView);
            _playerReShapeLogic = new PlayerReShapeLogic(mashView, playerInputEntity, playerInStageEntity);
            _playerHealLogic = new PlayerHealLogic(playerConstEntity,playerCommonUpdateableEntity);
            _playerDamageLogic = new PlayerDamageLogic(mashView,playerAnimatorView, playerConstEntity,playerInStageEntity,
                playerCommonUpdateableEntity,playerStatusView);
            _playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerInStageEntity, mashView,playerAnimatorView);
            _playerParticleLogic = new PlayerParticleLogic(playerConstEntity, mashView, playerInStageEntity);
            _playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerInStageEntity,playerCommonUpdateableEntity,  mashView,playerAnimatorView,particleGeneratorView);
            _playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerInStageEntity, mashView);
            _playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerInStageEntity,
                playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Mash);
            
            var disposables = new List<IDisposable> {playerInputEntity, playerCommonUpdateableEntity};
            
            return new MashController(playerNum,_playerMoveLogic, _playerJumpLogic, _playerPunchLogic, _mashSkillLogic,
                _playerReShapeLogic, _playerHealLogic, _playerStatusLogic, _playerParticleLogic, _playerFixSweetsLogic,
                _playerEnterDoorLogic, _playableCharacterSelectLogic,disposables,playerCommonUpdateableEntity.OnDead);
        }
    }
}