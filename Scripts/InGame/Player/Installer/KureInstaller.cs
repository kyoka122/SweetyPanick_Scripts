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

namespace InGame.Player.Installer
{
    public class KureInstaller:BasePlayerInstaller
    {
        private PlayerMoveLogic _playerMoveLogic;
        private PlayerJumpLogic _playerJumpLogic;
        private PlayerPunchLogic _playerPunchLogic;
        private KureSkillLogic _kureSkillLogic;
        private PlayerReShapeLogic _playerReShapeLogic;
        private PlayerHealLogic _playerHealLogic;
        private PlayerDamageLogic _playerDamageLogic;
        private PlayerStatusLogic _playerStatusLogic;
        private PlayerParticleLogic _playerParticleLogic;
        private PlayerFixSweetsLogic _playerFixSweetsLogic;
        private PlayerEnterDoorLogic _playerEnterDoorLogic;
        private PlayableCharacterSelectLogic _playableCharacterSelectLogic;

        public override BasePlayerController Install(int playerNum,InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            var kureStatus = inGameDatabase.GetKureStatus();
            inGameDatabase.SetKureStatus(kureStatus);
            var kureView = viewGenerator.GenerateKure(inGameDatabase.GetKureConstData().Prefab);
            kureView.Init();
            kureView.transform.position = inGameDatabase.GetPlayerInstancePositions(StageArea.FirstStageFirst)
                .KureInstancePos;
            var weaponView = kureView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var playerAnimatorView = kureView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init();
            UIData uiData = inGameDatabase.GetUIData();
            
            
            var playerStatusView = viewGenerator.GeneratePlayerStatusView(uiData.PlayerStatusView,uiData.Canvas.transform,uiData.StatusDataPos[playerNum-1]);
            playerStatusView.Init(PlayableCharacterIndex.Kure,inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Kure).maxHp);
            var playerInputEntity = new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Kure);
            var kureConstEntity = new KureConstEntity(inGameDatabase);
            var playerInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Kure);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, 
                PlayableCharacter.Kure,playerNum,kureView.GetTransform());
            
            
            _playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerInStageEntity,
                playerCommonUpdateableEntity, kureView, playerAnimatorView);
            _playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerInStageEntity,
                playerCommonUpdateableEntity, kureView,playerAnimatorView);
            _playerPunchLogic = new PlayerPunchLogic(playerInputEntity, playerConstEntity, playerInStageEntity,
                playerCommonUpdateableEntity, kureView,playerAnimatorView, weaponView);
            _kureSkillLogic = new KureSkillLogic(playerInputEntity, playerConstEntity, kureConstEntity,playerInStageEntity,
                kureView,playerAnimatorView,weaponView);
            _playerReShapeLogic = new PlayerReShapeLogic(kureView, playerInputEntity,playerInStageEntity);
            _playerHealLogic = new PlayerHealLogic(playerConstEntity,playerCommonUpdateableEntity);
            _playerDamageLogic = new PlayerDamageLogic(kureView,playerAnimatorView, playerConstEntity,playerInStageEntity,
                playerCommonUpdateableEntity,playerStatusView);
            _playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerInStageEntity, kureView,playerAnimatorView);
            _playerParticleLogic = new PlayerParticleLogic(playerConstEntity, kureView,playerInStageEntity);
            _playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerInStageEntity,playerCommonUpdateableEntity, kureView,playerAnimatorView,particleGeneratorView);
            _playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerInStageEntity, kureView);
            _playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerInStageEntity,
                playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Kure);
            
            var disposables = new List<IDisposable> {playerInputEntity, playerCommonUpdateableEntity};
            
            return new KureController(playerNum,_playerMoveLogic, _playerJumpLogic, _playerPunchLogic, _kureSkillLogic,
                _playerReShapeLogic, _playerHealLogic, _playerStatusLogic, _playerParticleLogic, _playerFixSweetsLogic,
                _playerEnterDoorLogic, _playableCharacterSelectLogic,disposables,playerCommonUpdateableEntity.OnDead);
        }
    }
}