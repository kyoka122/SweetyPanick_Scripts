using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Player.Logic;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Controller
{
    public abstract class BasePlayerController:IDisposable
    {
        public bool isMoving { get; private set; } = true;
        public bool isInStage { get; private set; } = true;
        
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
        protected readonly ActionKeyLogic actionKeyLogic;
        protected readonly PlayerReviveLogic playerReviveLogic;
        protected readonly List<IDisposable> disposables;
        private readonly ReactiveProperty<bool> _moveStateChanged;
        
        /// <summary>
        /// Logicから使用中かのデータを受け取る
        /// </summary>
        public readonly IObservable<bool> onChangedInStageData;
        public readonly IObservable<bool> onChangedRevivingData;
        
        private readonly bool _initUsed;

        private CancellationToken _cancellationToken;

        protected BasePlayerController(int playerNum,PlayerMoveLogic playerMoveLogic, PlayerJumpLogic playerJumpLogic,
            PlayerPunchLogic playerPunchLogic, BasePlayerSkillLogic playerSkillLogic, PlayerReShapeLogic playerReShapeLogic,
            PlayerHealLogic playerHealLogic, PlayerStatusLogic playerStatusLogic, PlayerParticleLogic playerParticleLogic,
            PlayerFixSweetsLogic playerFixSweetsLogic, PlayerEnterDoorLogic playerEnterDoorLogic,
            PlayableCharacterSelectLogic playableCharacterSelectLogic, PlayerTalkLogic playerTalkLogic, 
            PlayerGetKeyLogic playerGetKeyLogic, ActionKeyLogic actionKeyLogic,PlayerReviveLogic playerReviveLogic,
            List<IDisposable> disposables, bool initUsed,bool isInStage,IObservable<bool> onChangedInStageData,IObservable<bool> onChangedRevivingData)
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
            this.actionKeyLogic = actionKeyLogic;
            this.playerReviveLogic = playerReviveLogic;
            this.disposables = disposables;
            this.onChangedInStageData = onChangedInStageData;
            this.onChangedRevivingData = onChangedRevivingData;
            _initUsed = initUsed;
            this.isInStage = isInStage;
            RegisterObserver();
            playerStatusLogic.SetInstalled();
        }

        public void InitHealAndRevive()
        {
            Debug.Log($"InitHealAndRevive");
            playerHealLogic.TryReStartPlayHealTask();
            playerReviveLogic.TrySetRevivedParameterStoppedBySceneMove();
        }

        private void RegisterObserver()
        {
            onChangedInStageData.Subscribe(on =>
                {
                    if (on)
                    {
                        Debug.Log($"onChangedInStageData. true {GetCharacterType()}");
                        isInStage = true;
                        return;
                    }

                    Debug.Log($"onChangedInStageData, false");
                    SetNoOperationPlayer();
                    isInStage = false;
                });

            onChangedRevivingData.Where(on => !on)
                .Subscribe(_ =>
                {
                    OnHadMovedStageInScene();
                    Debug.Log($"RevivePlayer");
                    isInStage = true;
                });
        }

        protected abstract void FixedUpdateEachPlayer();

        public void LateInit()
        {
            playerMoveLogic.LateInit();
            playerReShapeLogic.LateInit();
            if (_initUsed)
            {
                actionKeyLogic.RegisterActionKeyFlagObserver();
            }
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
                actionKeyLogic.UpdateActionKey();
                FixedUpdateEachPlayer();
                return;
            }

            playerMoveLogic.UpdateStopping();
            playerJumpLogic.UpdateStopping();
            playerPunchLogic.UpdateStopping();
            playerFixSweetsLogic.UpdateStopping();
            playerSkillLogic.UpdateStopping();
        }
        
        public void SetNoOperationPlayer()
        {
            Debug.Log($"PlayerStop!");
            playerFixSweetsLogic.Stop();
            playerEnterDoorLogic.Stop();
            isMoving = false;
        }
        
        public void OnMoveScene()
        {
            Debug.Log($"PlayerStop!");
            playerHealLogic.TryKillHealTask();
            playerReviveLogic.TryKillRevive();
            isMoving = false;
        }
        
        public void OnMoveStageInScene(StageArea stageArea)
        {
            Debug.Log($"MoveStage");
            playerMoveLogic.SetPosition(stageArea);
            playerHealLogic.TryKillHealTask();
            playerReviveLogic.TryKillRevive();
            isMoving = false;
        }

        /// <summary>
        /// 同一シーン内でステージ移動後
        /// </summary>
        public void OnHadMovedStageInScene()
        {
            Debug.Log($"OnHadMovedStageInScene()");
            playerHealLogic.TryReStartPlayHealTask();
            playerReviveLogic.TrySetRevivedParameterStoppedByStageMove();
            playerStatusLogic.Reset();
            isMoving = true;
        }

        public PlayableCharacter GetCharacterType()
        {
            return playerStatusLogic.GetCharacterType();
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
            playerStatusLogic.SetActivePlayerView(true);
        }

        public void FinishTalk()
        {
            playerTalkLogic.FinishTalk();
            playerStatusLogic.TrySetActivePlayer();
        }

        public void OnGameOver()
        {
            playerHealLogic.TryKillHealTask();
            playerReviveLogic.TryKillRevive();
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
        
        /*public void StopPlayer()
        {
            Debug.Log($"PlayerStop!");
            playerFixSweetsLogic.Stop();
            playerEnterDoorLogic.Stop();
            playerStatusLogic.Pause();
            playerReviveLogic.Stop();
            isMoving = false;
        }*/
        /*public void ReSetAndStartPlayer()
        {
            Debug.Log($"PlayerReStart!");
            playerHealLogic.TryReStartPlayHealTask();
            isMoving = true;
        }*/


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