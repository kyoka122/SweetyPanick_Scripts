using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using InGame.Colate.View;
using InGame.Common.Database;
using Common.Database.ScriptableData;
using InGame.Database;
using InGame.Database.Installer;
using InGame.Database.ScriptableData;
using Loader;
using InGame.Stage.Installer;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.ColateStage;
using OutGame.Database;
using StageManager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class ColateStageSequencer:BaseSceneSequencer
    {
        private const float ToFadeInTime = 2;
        private const float FadeInBGMDuration = 3;
        
        [SerializeField] private PlayableCharacter debugCharacter = PlayableCharacter.Candy;

        [SerializeField] private ColateStageTalkPartBehaviour colateStageTalkPartBehaviour;
        [SerializeField] private ColateStageGimmickInstaller colateStageGimmickInstaller;
        [SerializeField] private StageUIScriptableData stageUiScriptableData;
        [SerializeField] private EnemyScriptableData enemyScriptableData;
        [SerializeField] private PlayerInstanceData playerInstanceData;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private CameraData battleCameraData;
        [SerializeField] private DefaultSweetsLiftView[] sweetsLifts;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera camera;
        
        private ColateStageStageManager _colateStageStageManager;
        private CommonInGameDatabaseInstaller _commonInGameDatabaseInstaller;
        private InGameDatabase _inGameDatabase;
        private OutGameDatabase _outGameDatabase;
        private CommonDatabase _commonDatabase;
        
        private bool _startSceneChange;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _outGameDatabase = outGameDatabase;
            _commonDatabase = commonDatabase;

            _commonInGameDatabaseInstaller = new CommonInGameDatabaseInstaller(_inGameDatabase,_outGameDatabase,_commonDatabase,debugCharacter);
            SetDatabase();
            
            var cameraController = inGameDatabase.GetStageSettings().CameraInstallerPrefab.InstallMoveCamera(inGameDatabase, 
                commonDatabase,camera);
            _colateStageStageManager=new ColateStageStageManager(colateStageGimmickInstaller,cameraController,battleCameraData,
                _inGameDatabase,_commonDatabase, MoveNextScene);
            
            _commonInGameDatabaseInstaller.InstallAllPlayer(_colateStageStageManager,StageArea.ColateStageFirst);
            _colateStageStageManager.InstallColate(sweetsLifts, _inGameDatabase);
            
            colateStageTalkPartBehaviour.Init(StartBattle,outGameDatabase);
            _colateStageStageManager.LateInit();
        }

        protected override async void ProcessInOrder()
        {
            try
            {
                await LoadManager.Instance.TryPlayFadeOut();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            _colateStageStageManager.StartTalk();
            colateStageTalkPartBehaviour.StartTalkScene();
        }

        private void StartBattle()
        {
            colateStageTalkPartBehaviour.Dispose();
            BGMManager.Instance.Stop();
            BGMManager.Instance.Play(BGMPath.BOSS_BGM);
            _colateStageStageManager.StartBattle();
            foreach (var playerController in _colateStageStageManager.Controllers)
            {
                playerController.InitHealAndRevive();
            }
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (_startSceneChange)
                    {
                        return;
                    }
                    _colateStageStageManager.FixedUpdateEnemy();
                    _colateStageStageManager.FixedUpdateStage();
                    _colateStageStageManager.FixedUpdateColate();
                    for (int i = 1; i <=  _commonDatabase.GetUseCharacterData().Count; i++)
                    {
                        _colateStageStageManager.FixedUpdatePlayableCharacter(i);
                    }
                }).AddTo(this);
        }
        
        private void SetDatabase()
        {
            var uiData = new StageUIData(stageUiScriptableData, canvas, null,null);
            _commonInGameDatabaseInstaller.SetInGameDatabase(uiData, stageSettingsScriptableData,
                new[] { new EachStagePlayerInstanceData(StageArea.ColateStageFirst,playerInstanceData)},
                null);
            _inGameDatabase.SetStageSettings(stageSettingsScriptableData);
            _inGameDatabase.SetEnemyData(enemyScriptableData);
        }
        
        private void MoveNextScene(string sceneName)
        {
            _startSceneChange = true;
            toNextSceneFlag.OnNext(sceneName);
        }

        private void DisposeInGameController()
        {
            IReadOnlyList<ControllerNumData> controllerData = _commonDatabase.GetAllControllerData();
            foreach (var numData in controllerData)
            {
                numData.playerInput.Dispose();
            }
        }
        
        protected override async void Finish(string nextSceneName)
        {
            DisposeInGameController();
            await UniTask.Delay(TimeSpan.FromSeconds(ToFadeInTime), cancellationToken: this.GetCancellationTokenOnDestroy());
            
            BGMManager.Instance.FadeOut(BGMPath.STAGE_BGM, FadeInBGMDuration, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            
            try
            {
                await LoadManager.Instance.TryPlayBlackFadeIn();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            SceneManager.LoadScene(nextSceneName);
        }

        private void OnDestroy()
        {
            _colateStageStageManager?.Dispose();
        }
    }
}