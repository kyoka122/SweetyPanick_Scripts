using System;
using System.Collections.Generic;
using InGame.Colate.Installer;
using InGame.Database;
using InGame.Enemy;
using InGame.Colate.Manager;
using InGame.Colate.View;
using InGame.Common.Database;
using InGame.Common.Database.ScriptableData;
using InGame.Enemy.Installer;
using InGame.MyCamera.Controller;
using InGame.Player.Controller;
using InGame.Stage.Installer;
using InGame.Stage.Manager;
using MyApplication;
using UniRx;
using UnityEngine;

namespace StageManager
{
    public class ColateStageManager:IManagerInitAble,IDisposable
    {
        public IReadOnlyList<BasePlayerController> Controllers=>_controllers;
        private readonly List<BasePlayerController> _controllers;
        private readonly EnemyManager _enemyManager;
        private readonly ColateController _colateController;
        private readonly ColateStageGimmickManager _stageGimmickManager;
        private readonly EnemyInstaller _enemyInstaller;
        private readonly CameraData _battleCameraData;
        private readonly ColateInstaller _colateInstaller;
        private readonly CameraController _cameraController;
        private readonly Action<string> _moveNextSceneEvent;

        public ColateStageManager(ColateStageGimmickInstaller colateStageGimmickInstaller,CameraController cameraController,
            CameraData battleCameraData,DefaultSweetsLiftView[] sweetsLifts, InGameDatabase inGameDatabase, CommonDatabase commonDatabase,Action<string> moveNextSceneEvent)
        {
            _stageGimmickManager = colateStageGimmickInstaller.Install(inGameDatabase);
            _cameraController = cameraController;
            _moveNextSceneEvent = moveNextSceneEvent;
            _battleCameraData = battleCameraData;
            _controllers = new List<BasePlayerController>();
            _enemyInstaller = inGameDatabase.GetEnemyData().EnemyInstaller;
            _enemyManager = _enemyInstaller.Install();
            _colateInstaller = inGameDatabase.GetColateData().Installer;
            _colateController = _colateInstaller.Install(inGameDatabase, SpawnEnemyEvent,sweetsLifts);
            RegisterObserver();
        }

        private void RegisterObserver()
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