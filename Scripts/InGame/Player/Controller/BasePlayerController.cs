using System;
using System.Collections.Generic;
using InGame.Player.Logic;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Controller
{
    public abstract class BasePlayerController:IDisposable
    {
        public bool isMoving { get; private set; } = true;
        public bool isUsed { get; private set; } = true;
        
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
        protected readonly PlayerTalkLogic playerTalkLogic;
        protected readonly PlayerGetKeyLogic playerGetKeyLogic;
        protected readonly List<IDisposable> disposables;
        private readonly ReactiveProperty<bool> _moveStateChanged;
        
        /// <summary>
        /// Logicから使用中かのデータを受け取る
        /// </summary>
        public readonly IObservable<bool> onChangedUseData;

        protected BasePlayerController(int playerNum,PlayerMoveLogic playerMoveLogic, PlayerJumpLogic playerJumpLogic,
            PlayerPunchLogic playerPunchLogic, BasePlayerSkillLogic playerSkillLogic, PlayerReShapeLogic playerReShapeLogic,
            PlayerHealLogic playerHealLogic, PlayerStatusLogic playerStatusLogic, PlayerParticleLogic playerParticleLogic,
            PlayerFixSweetsLogic playerFixSweetsLogic, PlayerEnterDoorLogic playerEnterDoorLogic,
            PlayableCharacterSelectLogic playableCharacterSelectLogic, PlayerTalkLogic playerTalkLogic, 
            PlayerGetKeyLogic playerGetKeyLogic, List<IDisposable> disposables,IObservable<bool> onChangedUseData)
        {
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
            this.playerTalkLogic = playerTalkLogic;
            this.playerGetKeyLogic = playerGetKeyLogic;
            this.disposables = disposables;
            this.onChangedUseData = onChangedUseData;
            RegisterObserver();
            playerStatusLogic.SetInstalled();
        }

        private void RegisterObserver()
        {
            onChangedUseData.Subscribe(used =>
                {
                    if (used)
                    {
                        ReStartPlayer();
                        isUsed = true;
                        return;
                    }
                    StopPlayer();
                    isUsed = false;
                });
        }

        protected abstract void FixedUpdateEachPlayer();

        public void LateInit()
        {
            playerMoveLogic.LateInit();
            playerReShapeLogic.LateInit();
        }
        
        public void FixedUpdate()
        {
            if (isMoving)
            {
                playerMoveLogic.UpdatePlayerMove();
                playerJumpLogic.UpdatePlayerJump();
                playerReShapeLogic.UpdatePlayerDirection();
                playerPunchLogic.UpdatePlayerPunch();
                playerSkillLogic.UpdatePlayerSkill();
                playerFixSweetsLogic.UpdatePlayerFixSweets();
                playerEnterDoorLogic.UpdatePlayerEnterDoor();//MEMO: UpdatePlayerFixSweetsより前に実行しない
                FixedUpdateEachPlayer();
                return;
            }

            Debug.Log($"Stopping");
            playerMoveLogic.UpdateStopping();
            playerJumpLogic.UpdateStopping();
            playerPunchLogic.UpdateStopping();
            playerFixSweetsLogic.UpdateStopping();
            playerSkillLogic.UpdateStopping();
        }

        public void StopPlayer()
        {
            Debug.Log($"PlayerStop!");
            playerFixSweetsLogic.Stop();
            playerEnterDoorLogic.Stop();
            isMoving = false;
        }

        public void ReStartPlayer()
        {
            Debug.Log($"PlayerReStart!");
            isMoving = true;
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
            return playerStatusLogic.GetPlayerNum();
        }
        
        public void StartTalk()
        {
            playerTalkLogic.StartTalk();
        }

        public void FinishTalk()
        {
            playerTalkLogic.FinishTalk();
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
                    Debug.LogError($"ArgumentOutOfRangeException:{toPlayerEvent}");
                    return;
            }
        }

        protected virtual void TryConsumeHealPower()
        {
            //MEMO: クレー以外何もしない
        }

        public void Dispose()
        {
            _moveStateChanged?.Dispose();
            foreach (var disposable in disposables)
            {
                disposable?.Dispose();
            }
        }
    }
}