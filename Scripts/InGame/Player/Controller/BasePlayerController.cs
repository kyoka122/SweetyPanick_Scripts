using System;
using System.Collections.Generic;
using InGame.Database;
using InGame.Player.Installer;
using InGame.Player.Logic;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Controller
{
    public abstract class BasePlayerController
    {
        public bool isMoving { get; private set; } = true;
        private int _playerNum = -1;
        
        protected readonly PlayerMoveLogic playerMoveLogic;
        protected readonly PlayerJumpLogic playerJumpLogic;
        protected readonly PlayerPunchLogic playerPunchLogic;
        protected readonly BasePlayerSkillLogic playerSkillLogic;
        protected readonly PlayerReShapeLogic playerReShapeLogic;
        protected readonly PlayerHealLogic playerHealLogic;
        protected readonly PlayerStatusLogic playerStatusLogic;
        protected readonly PlayerParticleLogic playerParticleLogic;
        protected readonly PlayerFixSweetsLogic playerFixSweetsLogic;
        protected readonly PlayerEnterDoorLogic playerEnterDoorLogic;
        protected readonly PlayableCharacterSelectLogic playableCharacterSelectLogic;
        protected readonly List<IDisposable> disposables;
        protected readonly IObservable<bool> onDead;
        

        protected BasePlayerController(int playerNum,PlayerMoveLogic playerMoveLogic, PlayerJumpLogic playerJumpLogic,
            PlayerPunchLogic playerPunchLogic,
            BasePlayerSkillLogic playerSkillLogic, PlayerReShapeLogic playerReShapeLogic,
            PlayerHealLogic playerHealLogic, PlayerStatusLogic playerStatusLogic,
            PlayerParticleLogic playerParticleLogic, PlayerFixSweetsLogic playerFixSweetsLogic,
            PlayerEnterDoorLogic playerEnterDoorLogic, PlayableCharacterSelectLogic playableCharacterSelectLogic,
            List<IDisposable> disposables,IObservable<bool> onDead)
        {
            _playerNum = playerNum;
            this.playerMoveLogic = playerMoveLogic;
            this.playerJumpLogic = playerJumpLogic;
            this.playerPunchLogic = playerPunchLogic;
            this.playerSkillLogic = playerSkillLogic;
            this.playerReShapeLogic = playerReShapeLogic;
            this.playerHealLogic = playerHealLogic;
            this.playerStatusLogic = playerStatusLogic;
            this.playerParticleLogic = playerParticleLogic;
            this.playerFixSweetsLogic = playerFixSweetsLogic;
            this.playerEnterDoorLogic = playerEnterDoorLogic;
            this.playableCharacterSelectLogic = playableCharacterSelectLogic;
            this.disposables = disposables;
            this.onDead = onDead;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            onDead.Where(dead => dead)
                .Subscribe(_ =>
                {
                    foreach (var disposable in disposables)
                    {
                        disposable.Dispose();
                    }
                });
        }

        protected abstract void FixedUpdateEachPlayer();

        public void FixedUpdateMoving()
        {
            Debug.Log($"FixedUpdate!");
            playerMoveLogic.UpdatePlayerMove();
            playerJumpLogic.UpdatePlayerJump();
            playerReShapeLogic.UpdatePlayerDirection();
            playerPunchLogic.UpdatePlayerPunch();
            playerSkillLogic.UpdatePlayerSkill();
            playerStatusLogic.UpdateAnimationStatus();
            playerFixSweetsLogic.UpdatePlayerFixSweets();
            playerEnterDoorLogic.UpdatePlayerEnterDoor();//MEMO: UpdatePlayerFixSweetsより前に実行しない
            FixedUpdateEachPlayer();
        }

        public void StopPlayer()
        {
            isMoving = false;
            playerFixSweetsLogic.Stop();
            playerEnterDoorLogic.Stop();
        }

        public void ReStartPlayer()
        {
            isMoving = true;
        }

        public void FixedUpdateStopping()
        {
            playerMoveLogic.UpdateStopping();
            playerJumpLogic.UpdateStopping();
            playerPunchLogic.UpdateStopping();
            playerFixSweetsLogic.UpdateStopping();
            playerSkillLogic.UpdateStopping();
        }
        
        public PlayableCharacter GetCharacterType()
        {
            return playerStatusLogic.GetCharacterType();
        }
        
        public void MoveStage(StageArea stageArea)
        {
            playerMoveLogic.SetPosition(stageArea);
        }

        public void HealHp()
        {
            playerHealLogic.HealHp();
        }
        
        public int GetPlayerNum()
        {
            return _playerNum;
        }

        public abstract void RegisterPlayerEvent(Action<FromPlayerEvent> playerEvent);
        
        public void OnPlayerEvent(ToPlayerEvent toPlayerEvent)
        {
            switch (toPlayerEvent)
            {
                case ToPlayerEvent.ConsumeKureHealPower:
                    TryConsumeHealPower();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(toPlayerEvent), toPlayerEvent, null);
            }
        }

        protected virtual void TryConsumeHealPower()
        {
            //MEMO: クレー以外何もしない
            return;
        }

    }
}