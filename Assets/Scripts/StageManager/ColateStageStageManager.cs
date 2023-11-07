using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Colate.Installer;
using InGame.Database;
using InGame.Enemy;
using InGame.Colate.Manager;
using InGame.Colate.View;
using InGame.Common.Database;
using Common.Database.ScriptableData;
using InGame.Enemy.Installer;
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
    public class ColateStageStageManager:IStageManager,IDisposable
    {
        public IReadOnlyList<BasePlayerController> Controllers=>_controllers;
        private readonly List<BasePlayerController> _controllers;
        private readonly EnemyManager _enemyManager;
        private ColateController _colateController;
        private readonly ColateStageGimmickManager _stageGimmickManager;
        private readonly EnemyInstaller _enemyInstaller;
        private readonly CameraData _battleCameraData;
        private ColateInstaller _colateInstaller;
        private readonly CameraController _cameraController;
        private readonly Action<string> _moveNextSceneEvent;

        public ColateStageStageManager(ColateStageGimmickInstaller colateStageGimmickInstaller,CameraController cameraController,
            CameraData battleCameraData, InGameDatabase inGameDatabase, CommonDatabase commonDatabase,Action<string> moveNextSceneEvent)
        {
            _stageGimmickManager = colateStageGimmickInstaller.Install(inGameDatabase);
            _cameraController = cameraController;
            _moveNextSceneEvent = moveNextSceneEvent;
            _battleCameraData = battleCameraData;
            _controllers = new List<BasePlayerController>();
            _enemyInstaller = inGameDatabase.GetEnemyData().EnemyInstaller;
            _enemyManager = _enemyInstaller.Install();
        }

        public void InstallColate(DefaultSweetsLiftView[] sweetsLifts, InGameDatabase inGameDatabase)
        {
            _colateInstaller = inGameDatabase.GetColateData().Installer;
            _colateController = _colateInstaller.Install(inGameDatabase, SpawnEnemyEvent,sweetsLifts);
            RegisterColateObserver();
        }

        private void RegisterColateObserver()
        {
            _colateController.IsColateDead.Subscribe(_ =>
            {
                _moveNextSceneEvent.Invoke(SceneName.Epilogue);
            });
        }

        public void AddController(BasePlayerController controller)
        {
            _controllers.Add(controller);
        }
        
        public void LateInit()
        {
            _stageGimmickManager.LateInit();
            foreach (var playerController in _controllers)
            {
                playerController.LateInit();
            }
            _cameraController.LateInit();
        }

        public void StartTalk()
        {
            foreach (var playerController in _controllers)
            {
                playerController.StartTalk();
            }

            _colateController.StartTalk();
        }
        
        public void StartBattle()
        {
            _cameraController.SetCameraData(_battleCameraData);
            foreach (var playerController in _controllers)
            {
                playerController.FinishTalk();
            }
            _colateController.StartBattle();
        }

        public void FixedUpdatePlayableCharacter(int currentMovePlayer)
        {
            foreach (var controller in _controllers)
            {
                if (controller.GetPlayerNum()==currentMovePlayer)
                {
                    controller.FixedUpdate();
                    break;
                }
            }
        }

        public void FixedUpdateEnemy()
        {
            _enemyManager.FixedUpdate();
        }
        
        public void FixedUpdateStage()
        {
            _stageGimmickManager.FixedUpdate();
        }

        public void FixedUpdateCamera()
        {
            _cameraController.FixedUpdate();
        }
        
        public void FixedUpdateColate()
        {
            _colateController.FixedUpdate();
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
                        LoadManager.Instance.TryPlayGameOverFadeIn();
                    }
                });
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

        private void OnStageEvent()
        {
            
        }

        #endregion

        #region ColateEvent

        private IColateOrderAble SpawnEnemyEvent(Vector2 spawnPos)
        {
            //MEMO: 今はとりあえずdefaultのエネミーのみ生成できる。
            return _enemyInstaller.InstallDefaultEnemyByColate(_enemyManager,spawnPos);
        }
        

        #endregion

        public void Dispose()
        {
            _colateController?.Dispose();
        }
    }
}