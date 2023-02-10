using System;
using System.Collections.Generic;
using System.Threading;
using InGame.Common.Database;
using InGame.MyCamera.Controller;
using InGame.Database;
using InGame.Enemy;
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
        private readonly EnemyController _enemyController;
        private readonly MoveStageGimmickManager _stageGimmickGimmickManager;
        private readonly CameraController _cameraController;
        private readonly Action<string> _moveNextSceneEvent;
        private CancellationTokenSource _fadeOutTokenSource;

        public DebugStageManager(MoveStageGimmickInstaller moveStageGimmickInstaller,CameraController cameraController,
            InGameDatabase inGameDatabase, CommonDatabase commonDatabase,Action<string> moveNextSceneEvent)
        {
            _stageGimmickGimmickManager = moveStageGimmickInstaller.Install(SwitchStageEvent, inGameDatabase,commonDatabase);
            _cameraController = cameraController;
            _moveNextSceneEvent = moveNextSceneEvent;
            _controllers = new List<BasePlayerController>();
            _enemyController = inGameDatabase.GetEnemyData().EnemyInstaller.Install(inGameDatabase,commonDatabase);
        }
        

        public void AddController(BasePlayerController controller)
        {
            _controllers.Add(controller);
        }

        public void FixedUpdateEnemy()
        {
            _enemyController.FixedUpdate();
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
            _stageGimmickGimmickManager.FixedUpdate();
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
            await _stageGimmickGimmickManager.GetAllPlayerHealAnimationTask();
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
                case StageEvent.EnterFirstStageMiddleDoor:
                    MoveStage(StageArea.FirstHiddenStage);
                    break;
                case StageEvent.EnterFirstHiddenStageDoor:
                    MoveStage(StageArea.FirstStageMiddle);
                    break;
                case StageEvent.EnterFirstStageGoalDoor:
                    SetAllPlayerStop();
                    _moveNextSceneEvent.Invoke(SceneName.SecondStage);
                    break;
                case StageEvent.EnterSecondStageGoalDoor:
                    SetAllPlayerStop();
                    _moveNextSceneEvent.Invoke(SceneName.BossStage);
                    break;
                default:
                    Debug.LogError($"stageEvent: {stageEvent}");
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
        }
    }
}