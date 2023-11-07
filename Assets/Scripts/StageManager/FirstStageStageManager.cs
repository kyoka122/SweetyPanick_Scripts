using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using InGame.Enemy;
using Common.MyCamera.Controller;
using InGame.Player.Controller;
using InGame.SceneLoader;
using InGame.Stage.Installer;
using InGame.Stage.Manager;
using MyApplication;
using UniRx;
using UnityEngine;

namespace StageManager
{
    public class FirstStageStageManager:IStageManager,IDisposable
    {
        public IReadOnlyList<BasePlayerController> Controllers=>_controllers;
        
        private const float MoveStageDuration = 1f;
        private const float ToMoveStageTime = 1.5f;
        private readonly List<BasePlayerController> _controllers;
        private readonly EnemyManager _enemyManager;
        private readonly MoveStageGimmickManager _stageGimmickManager;
        private readonly CameraController _cameraController;
        private readonly Action<string> _moveNextSceneEvent;
        private readonly CancellationTokenSource _tokenSource;
        
        public FirstStageStageManager(MoveStageGimmickInstaller moveStageGimmickInstaller,CameraController cameraController,
            InGameDatabase inGameDatabase,CommonDatabase commonDatabase,Action<string> moveNextSceneEvent)
        {
            _cameraController = cameraController;
            _moveNextSceneEvent = moveNextSceneEvent;
            _controllers = new List<BasePlayerController>();
            _tokenSource = new CancellationTokenSource();
            
            _stageGimmickManager = moveStageGimmickInstaller.Install(SwitchStageEvent, StageArea.FirstStageFirst,
                inGameDatabase,commonDatabase);
            _enemyManager = inGameDatabase.GetEnemyData().EnemyInstaller.InstallWithStageEnemies();
            InitByStageMove(StageArea.FirstStageFirst);
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
            foreach (var playerController in _controllers)
            {
                playerController.LateInit();
            }
            _cameraController.LateInit();
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

        public void RegisterGameOverObserver()
        {
            foreach (var controller in _controllers)
            {
                controller.onChangedInStageData.Subscribe(_ =>
                {
                    if (_controllers.Count(data=>data.isInStage)==0)
                    {
                        OnGameOver();
                        LoadManager.Instance.TryPlayGameOverFadeIn();
                    }
                });
            }
        }

        private void OnGameOver()
        {
            foreach (var controller in _controllers)
            {
                controller.OnGameOver();
            }
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
                    MoveScene(SceneName.SecondStage);
                    break;
                case StageEvent.EnterSecondStageMiddleDoor:
                    MoveStage(StageArea.SecondHiddenStage);
                    break;
                case StageEvent.EnterSecondHiddenStageDoor:
                    MoveStage(StageArea.SecondStageMiddle);
                    break;
                case StageEvent.EnterSecondStageGoalDoor:
                    MoveScene(SceneName.ColateStage);
                    break;
                default:
                    Debug.LogError($"Could Not Found stageEvent: {stageEvent}");
                    return;
            }
        }

        private async void MoveStage(StageArea nextStageArea)
        {
            foreach (var controller in _controllers)
            {
                controller.SetNoOperationPlayer();
                controller.OnMoveScene();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(ToMoveStageTime),cancellationToken:_tokenSource.Token);
            try
            {
               await LoadManager.Instance.TryPlayBlackFadeIn();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }

            InitByStageMove(nextStageArea);
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(MoveStageDuration),cancellationToken:_tokenSource.Token);
                await LoadManager.Instance.TryPlayFadeOut();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }

            SetAllPlayerResetAndStart();
        }
        
        private async void MoveScene(string sceneName)
        {
            foreach (var controller in _controllers)
            {
                controller.SetNoOperationPlayer();
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(ToMoveStageTime),cancellationToken:_tokenSource.Token);
            _moveNextSceneEvent.Invoke(sceneName);
        }

        private void InitByStageMove(StageArea nextStageArea)
        {
            _stageGimmickManager.UnsetBackGround();
            
            foreach (var controller in _controllers)
            {
                controller.OnMoveStageInScene(nextStageArea);
            }
            _cameraController.SetCameraMoveState(nextStageArea);
            _stageGimmickManager.InitAtStageMove(nextStageArea);
        }

        private void SetAllPlayerStop()
        {
            foreach (var controller in _controllers)
            {
                controller.SetNoOperationPlayer();
            }
        }
        
        private void SetAllPlayerResetAndStart()
        {
            foreach (var controller in _controllers)
            {
                controller.OnHadMovedStageInScene();
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
            _tokenSource?.Dispose();
            _stageGimmickManager.Dispose();
        }
    }
}