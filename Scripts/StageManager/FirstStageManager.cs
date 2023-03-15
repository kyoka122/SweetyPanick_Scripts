using System;
using System.Collections.Generic;
using System.Linq;
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
using UniRx;
using UnityEngine;

namespace StageManager
{
    public class FirstStageManager:IDisposable
    {
        public IReadOnlyList<BasePlayerController> Controllers=>_controllers;
        
        private readonly List<BasePlayerController> _controllers;
        private readonly EnemyManager _enemyManager;
        private readonly MoveStageGimmickManager _stageGimmickManager;
        private readonly CameraController _cameraController;
        private readonly Action<string> _moveNextSceneEvent;
        private CancellationTokenSource _blackFadeInTokenSource;
        
        public FirstStageManager(MoveStageGimmickInstaller moveStageGimmickInstaller,CameraController cameraController,
            InGameDatabase inGameDatabase,CommonDatabase commonDatabase,Action<string> moveNextSceneEvent)
        {
            _stageGimmickManager = moveStageGimmickInstaller.Install(SwitchStageEvent, inGameDatabase,commonDatabase);

            _cameraController = cameraController;
            _moveNextSceneEvent = moveNextSceneEvent;
            _controllers = new List<BasePlayerController>();
            _enemyManager = inGameDatabase.GetEnemyData().EnemyInstaller.InstallWithStageEnemies();
            InitAtStageMove(StageArea.FirstStageFirst);
        }
        

        public void AddController(BasePlayerController controller)
        {
            _controllers.Add(controller);
            RegisterUsePlayerObserver(controller);
        }
        
        public void FixedUpdateEnemy()
        {
            _enemyManager.FixedUpdate();
        }

        public void LateInit()
        {
            foreach (var playerController in _controllers)
            {
                playerController.LateInit();
            }
        }
        
        public void FixedUpdatePlayableCharacter(int currentMovePlayer)
        {
            var playerController=_controllers
                .FirstOrDefault(controller => controller.GetPlayerNum() == currentMovePlayer);
            playerController?.FixedUpdate();
        }

        public void FixedUpdateStage()
        {
            _stageGimmickManager.FixedUpdate();
        }
        
        public void LateUpdateBackGround()
        {
            _stageGimmickManager.LateUpdateBackGround();
        }

        public void FixedUpdateCamera()
        {
            _cameraController.FixedUpdate();
        }

        #region PlayerEvent

        private void RegisterUsePlayerObserver(BasePlayerController playerController)
        {
            playerController.onChangedUseDataUse.Subscribe(_ =>
            {
                if (_controllers.Count(data=>data.isUsed)==0)
                {
                    LoadManager.Instance.TryPlayGameOverFadeIn();
                }
            });
        }
        
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
            try
            {
               await LoadManager.Instance.TryPlayBlackFadeIn();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }

            InitAtStageMove(nextStageArea);
            
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

        private void InitAtStageMove(StageArea nextStageArea)
        {
            _cameraController.SetCameraMoveState(nextStageArea);
            foreach (var controller in _controllers)
            {
                controller.MoveStage(nextStageArea);
            }
            _stageGimmickManager.InitAtStageMove(nextStageArea);
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

        private void SetPlayerPosition()
        {
            
        }
        
        private void OnStageEvent()
        {
            
        }

        #endregion


        public void Dispose()
        {
            _blackFadeInTokenSource?.Dispose();
            _stageGimmickManager.Dispose();
        }
    }
}