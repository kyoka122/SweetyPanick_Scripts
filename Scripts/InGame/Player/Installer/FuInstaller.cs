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
    public class FuInstaller : BasePlayerInstaller
    {
        private PlayerMoveLogic _playerMoveLogic;
        private PlayerJumpLogic _playerJumpLogic;
        private PlayerPunchLogic _playerPunchLogic;
        private FuSkillLogic _fuSkillLogic;
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
            var fuStatus = inGameDatabase.GetFuStatus();
            inGameDatabase.SetFuStatus(fuStatus);//TODO: FuConstEntityへ移動
            var fuView = viewGenerator.GenerateFu(inGameDatabase.GetFuConstData().Prefab);
            fuView.Init();
            fuView.transform.position = inGameDatabase.GetPlayerInstancePositions(StageArea.FirstStageFirst)
                .FuInstancePos;
            var weaponView = fuView.GetWeaponObject().GetComponent<WeaponView>();
            weaponView.Init();
            var playerAnimatorView = fuView.GetAnimatorObject().GetComponent<PlayerAnimatorView>();
            playerAnimatorView.Init();
            UIData uiData = inGameDatabase.GetUIData();
            
            
            var playerStatusView =
                viewGenerator.GeneratePlayerStatusView(uiData.PlayerStatusView, uiData.Canvas.transform,uiData.StatusDataPos[playerNum-1]);
            playerStatusView.Init(PlayableCharacterIndex.Fu,inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Fu).maxHp);
            var playerInputEntity = new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Fu);
            var fuConstEntity = new FuConstEntity(inGameDatabase);
            var playerInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Fu);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, 
                PlayableCharacter.Fu,playerNum,fuView.GetTransform());
                
            
            _playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerInStageEntity,
                playerCommonUpdateableEntity,fuView,playerAnimatorView);
            _playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerInStageEntity,
                playerCommonUpdateableEntity, fuView,playerAnimatorView);
            _playerPunchLogic = new PlayerPunchLogic(playerInputEntity, playerConstEntity, playerInStageEntity,
                playerCommonUpdateableEntity,fuView,playerAnimatorView,weaponView);
            _fuSkillLogic = new FuSkillLogic(playerInputEntity, playerConstEntity,fuConstEntity, playerInStageEntity,
                fuView,playerAnimatorView,weaponView);
            _playerReShapeLogic = new PlayerReShapeLogic(fuView, playerInputEntity, playerInStageEntity);
            _playerHealLogic = new PlayerHealLogic(playerConstEntity,playerCommonUpdateableEntity);
            _playerDamageLogic = new PlayerDamageLogic(fuView,playerAnimatorView, playerConstEntity,playerInStageEntity
                ,playerCommonUpdateableEntity,playerStatusView);
            _playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerInStageEntity, fuView,playerAnimatorView);
            _playerParticleLogic = new PlayerParticleLogic(playerConstEntity, fuView, playerInStageEntity);
            _playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerInStageEntity,playerCommonUpdateableEntity, fuView,playerAnimatorView,particleGeneratorView);
            _playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerInStageEntity, fuView);
            _playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerInStageEntity, 
                playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Fu);
            
            var disposables = new List<IDisposable> {playerInputEntity, playerCommonUpdateableEntity};

            return new FuController(playerNum, _playerMoveLogic, _playerJumpLogic, _playerPunchLogic, _fuSkillLogic,
                _playerReShapeLogic, _playerHealLogic, _playerStatusLogic, _playerParticleLogic, _playerFixSweetsLogic,
                _playerEnterDoorLogic, _playableCharacterSelectLogic, disposables, playerCommonUpdateableEntity.OnDead);
        }
    }
}