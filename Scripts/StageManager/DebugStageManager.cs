using System;
using System.Collections.Generic;
using System.Threading;
using InGame.Common.Database;
using InGame.Database;
using InGame.Enemy;
using InGame.MyCamera.Controller;
using InGame.Player.Controller;
using InGame.SceneLoader;
using InGame.Stage.Installer;
using InGame.Stage.Manager;
using MyApplication;
using UnityEngine;

namespace StageManager
{
    public class DebugStageManager:IDisposable
    {
        public IReadOnlyList<BasePlayerController> Controllers=>_controllers;
        
        private readonly List<BasePlayerController> _controllers;
        private readonly EnemyManager _enemyManager;
        private readonly MoveStageGimmickManager _stageGimmickManager;
        private readonly CameraController _cameraController;
        private readonly Action<string> _moveNextSceneEvent;
        private CancellationTokenSource _fadeOutTokenSource;


        public DebugStageManager(MoveStageGimmickInstaller moveStageGimmickInstaller,CameraController cameraController,
            InGameDatabase inGameDatabase, CommonDatabase commonDatabase,Action<string> moveNextSceneEvent)
        {
            _stageGimmickManager = moveStageGimmickInstaller.Install(SwitchStageEvent, inGameDatabase,commonDatabase);
            _cameraController = cameraController;
            _moveNextSceneEvent = moveNextSceneEvent;
            _controllers = new List<BasePlayerController>();
            _enemyManager = inGameDatabase.GetEnemyData().EnemyInstaller.InstallWithStageEnemies();
        }
        

        public void AddController(BasePlayerController controller)
        {
            _controllers.Add(controller);
        }

        public void FixedUpdateEnemy()
        {
            _enemyManager.FixedUpdate();
        }
        
        public void LateInit()
        {
            _stageGimmickManager.LateInit();
            foreach (var playerController in _controllers)
            {
                playerController.LateInit();
            }
        }
        
        public void FixedUpdatePlayableCharacter(int currentMovePlayer)
        {
            foreach (var controller in _controllers)
            {
                if (controller.GetPlayerNum()==currentMovePlayer)
                {
                    controller.FixedUpdateMoving();
                    break;
                }

                //Debug.LogWarning($"StopPlayer!!!!!!!!!{controller.GetCharacterType()}");
                //controller.StopPlayer();
            }
        }

        public void FixedUpdateStage()
        {
            _stageGimmickManager.FixedUpdate();
        }

        #region PlayerEvent

        public void RegisterPlayerEvent(BasePlayerController controller)
        {
            controller.RegisterPlayerEvent(SwitchPlayerActionEvent);
        }
        
        private void SwitchPlayerActionEvent(FromPlayerEvent fromPlayerEvent)
        {
            switch (fromPlayerEvent)
            {
                case FromPlayerEvent.AllPlayerHeal:
                    OnAllPlayerHealEvent();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fromPlayerEvent), fromPlayerEvent, null);
            }
        }
        
        private async void OnAllPlayerHealEvent()
        {
            await _stageGimmickManager.GetAllPlayerHealAnimationTask();
            foreach (var controller in _controllers)
            {
                controller.HealHp();
            }

            foreach (var controller in _controllers)
            {
                controller.OnPlayerEvent(ToPlayerEvent.ConsumeKureHealPower);
            }
        }

        #endregion

        #region StageEvent

        private void SwitchStageEvent(StageEvent stageEvent)
        {
            switch (stageEvent)
            {
                case StageEvent.EnterFirstStageGoalDoor:
                    SetAllPlayerStop();
                    _moveNextSceneEvent.Invoke(SceneName.SecondStage);
                    break;
                case StageEvent.EnterSecondStageMiddleDoor:
                    MoveStage(StageArea.SecondHiddenStage);
                    break;
                case StageEvent.EnterSecondHiddenStageDoor:
                    MoveStage(StageArea.SecondStageMiddle);
                    break;
                case StageEvent.EnterSecondStageGoalDoor:
                    SetAllPlayerStop();
                    _moveNextSceneEvent.Invoke(SceneName.ColateStage);
                    break;
                default:
                    Debug.LogError($"Could Not Found stageEvent: {stageEvent}");
                    return;
            }
        }
        
        private async void MoveStage(StageArea nextStageArea)
        {
            SetAllPlayerStop();
            _cameraController.SetCameraMoveState(nextStageArea);
            foreach (var controller in _controllers)
            {
                controller.MoveStage(nextStageArea);
            }
            try
            {
                await LoadManager.Instance.TryPlayFadeOut();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            SetAllPlayerReStart();
        }

        private void SetAllPlayerStop()
        {
            foreach (var controller in _controllers)
            {
                controller.StopPlayer();
            }
        }
        
        private void SetAllPlayerReStart()
        {
            foreach (var controller in _controllers)
            {
                controller.ReStartPlayer();
            }
        }

        #endregion


        public void Dispose()
        {
            _fadeOutTokenSource.Dispose();
            _stageGimmickManager.Dispose();
        }
    }
}