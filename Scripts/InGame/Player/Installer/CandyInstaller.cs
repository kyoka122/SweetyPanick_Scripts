using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using InGame.Player.Entity;
using InGame.Player.Logic;
using InGame.Player.Controller;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Player.Installer
{
    /// <summary>
    /// presentation層のインスタンスを返す+初期化処理
    /// </summary>
    public class CandyInstaller:BasePlayerInstaller
    {
        private PlayerMoveLogic _playerMoveLogic;
        private PlayerJumpLogic _playerJumpLogic;
        private PlayerPunchLogic _playerPunchLogic;
        private CandySkillLogic _candySkillLogic;
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
                uiData.Canvas.transform,uiData.StatusDataPos[playerNum-1]);
            playerStatusView.Init(PlayableCharacterIndex.Candy,inGameDatabase.GetCharacterCommonStatus(PlayableCharacter.Candy).maxHp);
            
            
            var playerInputEntity = new PlayerInputEntity(playerNum,inGameDatabase,commonDatabase);
            var playerConstEntity = new PlayerConstEntity(inGameDatabase,commonDatabase,PlayableCharacter.Candy);
            var playerCommonInStageEntity = new PlayerCommonInStageEntity(PlayableCharacter.Candy);
            var playerCommonUpdateableEntity = new PlayerCommonUpdateableEntity(inGameDatabase, 
                PlayableCharacter.Candy,playerNum,candyView.GetTransform());
            
            
            _playerMoveLogic = new PlayerMoveLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,candyView,playerAnimatorView);
            _playerJumpLogic = new PlayerJumpLogic(playerConstEntity, playerInputEntity, playerCommonInStageEntity,
                playerCommonUpdateableEntity,candyView,playerAnimatorView);
            _playerPunchLogic = new PlayerPunchLogic(playerInputEntity,playerConstEntity,playerCommonInStageEntity,
                playerCommonUpdateableEntity, candyView,playerAnimatorView,weaponView);
            _playerReShapeLogic = new PlayerReShapeLogic(candyView, playerInputEntity,playerCommonInStageEntity);
            _playerHealLogic = new PlayerHealLogic(playerConstEntity,playerCommonUpdateableEntity);
            _playerDamageLogic = new PlayerDamageLogic(candyView,playerAnimatorView,playerConstEntity,playerCommonInStageEntity,
                playerCommonUpdateableEntity,playerStatusView);
            _candySkillLogic = new CandySkillLogic(playerInputEntity,playerConstEntity,playerCommonInStageEntity,
                candyView,playerAnimatorView,weaponView);
            _playerStatusLogic = new PlayerStatusLogic(playerConstEntity, playerCommonInStageEntity, candyView,
                playerAnimatorView);
            _playerParticleLogic = new PlayerParticleLogic(playerConstEntity, candyView,playerCommonInStageEntity);
            _playerFixSweetsLogic = new PlayerFixSweetsLogic(playerInputEntity, playerConstEntity,
                playerCommonInStageEntity, playerCommonUpdateableEntity,candyView,playerAnimatorView,particleGeneratorView);
            _playerEnterDoorLogic = new PlayerEnterDoorLogic(playerConstEntity, playerInputEntity,
                playerCommonInStageEntity, candyView);
            _playableCharacterSelectLogic = new PlayableCharacterSelectLogic(playerInputEntity, playerCommonInStageEntity
                , playerCommonUpdateableEntity,playerStatusView, PlayableCharacterIndex.Candy);

            var disposables = new List<IDisposable> {playerInputEntity, playerCommonUpdateableEntity};
            return new CandyController(playerNum,_playerMoveLogic, _playerJumpLogic, _playerPunchLogic, _candySkillLogic,
                _playerReShapeLogic, _playerHealLogic, _playerStatusLogic, _playerParticleLogic, _playerFixSweetsLogic,
                _playerEnterDoorLogic,_playableCharacterSelectLogic,disposables,playerCommonUpdateableEntity.OnDead);
        }
    }
}