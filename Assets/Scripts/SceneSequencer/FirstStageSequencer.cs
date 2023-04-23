using System;
using Cinemachine;
using InGame.Common.Database;
using InGame.Database;
using InGame.Database.Installer;
using InGame.Stage.Installer;
using KanKikuchi.AudioManager;
using MyApplication;
using InGame.Database.ScriptableData;
using InGame.SceneLoader;
using OutGame.Database;
using StageManager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class FirstStageSequencer:BaseSceneSequencer
    {
        [SerializeField] private int currentMovePlayer;
        
        [SerializeField] private StageUIScriptableData stageUIScriptableData;
        [SerializeField] private PlayerInstanceData firstPlayerInstanceData;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private MoveStageGimmickInstaller moveStageGimmickInstaller;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
        [SerializeField] private CameraInitData firstStageCameraData;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera camera;

        private FirstStageStageManager _firstStageStageManager;
        private CommonInGameDatabaseInstaller _commonInGameDatabaseInstaller;
        private InGameDatabase _inGameDatabase;
        private OutGameDatabase _outGameDatabase;
        private CommonDatabase _commonDatabase;
        
        private bool _isChangingScene;
        private int _playerCount;

        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            if (inGameDatabase==null)
            {
                Debug.LogError($"Not Found InGameDatabase");
                return;
            }
            
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _outGameDatabase = outGameDatabase;
            
            _commonInGameDatabaseInstaller = new CommonInGameDatabaseInstaller(_inGameDatabase,_outGameDatabase,_commonDatabase);
            SetDatabase();
            
            var cameraController = _inGameDatabase.GetStageSettings().CameraInstallerPrefab.InstallMoveCamera(inGameDatabase, commonDatabase, targetGroup,
                cinemachineImpulseSource, camera);
            _firstStageStageManager=new FirstStageStageManager(moveStageGimmickInstaller,cameraController,_inGameDatabase,_commonDatabase,
                MoveNextScene);
            
            _commonInGameDatabaseInstaller.InstallAllPlayer(_firstStageStageManager,StageArea.FirstStageFirst);
            _firstStageStageManager.LateInit();
            _playerCount = _commonDatabase.GetUseCharacterData().Count;
        }

        private void SetDatabase()
        {
            var uiData = new StageUIData(stageUIScriptableData, canvas, null,null);
            _commonInGameDatabaseInstaller.SetInGameDatabase(uiData, stageSettingsScriptableData,
                new[] { new EachStagePlayerInstanceData(StageArea.FirstStageFirst, firstPlayerInstanceData)},
                new[]{firstStageCameraData});
        }

        protected override async void ProcessInOrder()
        {
            BGMManager.Instance.Stop();
            this.LateUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _firstStageStageManager.LateUpdateBackGround();
                }).AddTo(this);
            
            try
            {
                Debug.Log($"FadeOut!!");
                await LoadManager.Instance.TryPlayFadeOut();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            
            BGMManager.Instance.Play(BGMPath.STAGE_BGM);
            
            
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (_isChangingScene)
                    {
                        return;
                    }
                    _firstStageStageManager.FixedUpdateEnemy();
                    _firstStageStageManager.FixedUpdateStage();
                    _firstStageStageManager.FixedUpdateCamera();
                    for (int i = 1; i <= _playerCount; i++)
                    {
                        _firstStageStageManager.FixedUpdatePlayableCharacter(i);
                    }
                })
                .AddTo(this);
            
        }

        protected override async void Finish(string nextSceneName)
        {
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
        
        private void MoveNextScene(string sceneName)
        {
            _isChangingScene = true;
            toNextSceneFlag.OnNext(sceneName);
        }

        private void OnDestroy()
        {
            _firstStageStageManager?.Dispose();
        }
    }
}