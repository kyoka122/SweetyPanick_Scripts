using System;
using Cinemachine;
using InGame.Common.Database;
using InGame.Database;
using InGame.Database.Installer;
using InGame.Database.ScriptableData;
using InGame.SceneLoader;
using InGame.Stage.Installer;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using StageManager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class SecondStageSequencer:BaseSceneSequencer
    {
        private const float ToNextStageDelay = 1.0f;
        private const float ToMoveSceneFadeOutDurationMin = 5.0f;
        
        [SerializeField] private int currentMovePlayer;
        
        [SerializeField] private StageUIScriptableData stageUIScriptableData;
        [SerializeField] private PlayerInstanceData secondStageStartInstanceData;
        [SerializeField] private PlayerInstanceData secondStageMiddleInstanceData;
        [SerializeField] private PlayerInstanceData secondHiddenStageInstanceData;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private CameraInitData secondStageCameraData;
        [SerializeField] private CameraInitData secondHiddenStageCameraData;
        [SerializeField] private CameraInitData secondStageMiddleCameraData;

        [SerializeField] private MoveStageGimmickInstaller moveStageGimmickInstaller;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
        [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject key;
        [SerializeField] private GameObject trailEffect;
        
        [SerializeField] private Camera camera;

        private SecondStageStageManager _secondStageStageManager;
        private CommonInGameDatabaseInstaller _commonInGameDatabaseInstaller;
        
        private InGameDatabase _inGameDatabase;
        private OutGameDatabase _outGameDatabase;
        private CommonDatabase _commonDatabase;
        private bool _startSceneChange;

        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _outGameDatabase = outGameDatabase;
            
            SetDatabase();
            var cameraController = _inGameDatabase.GetStageSettings().CameraInstallerPrefab.InstallMoveAndMultiStageCamera
            (_inGameDatabase, _commonDatabase, targetGroup,
                cinemachineImpulseSource, camera,cinemachineConfiner2D);
            _secondStageStageManager=new SecondStageStageManager(moveStageGimmickInstaller,cameraController,_inGameDatabase,
                _commonDatabase, MoveNextScene);

            _commonInGameDatabaseInstaller.InstallAllPlayer(_secondStageStageManager,StageArea.SecondStageFirst);
            _secondStageStageManager.LateInit();
        }

        protected override async void ProcessInOrder()
        {
            this.LateUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _secondStageStageManager.LateUpdate();
                }).AddTo(this);
            
            try
            {
                await LoadManager.Instance.TryPlayFadeOut();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            
            if (!BGMManager.Instance.IsPlaying())
            {
                BGMManager.Instance.Play(BGMPath.STAGE_BGM);
            }

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (_startSceneChange)
                    {
                        return;
                    }
                    _secondStageStageManager.FixedUpdateEnemy();
                    _secondStageStageManager.FixedUpdateStage();
                    _secondStageStageManager.FixedUpdateCamera();
                    for (int i = 1; i <=  _commonDatabase.GetUseCharacterData().Count; i++)
                    {
                        _secondStageStageManager.FixedUpdatePlayableCharacter(i);
                    }
                })
                .AddTo(this);
        }

        protected override async void Finish(string nextSceneName)
        {
            BGMManager.Instance.FadeOut(BGMPath.STAGE_BGM, 2, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            try
            {
                await LoadManager.Instance.TryPlayLoadScreen(ToNextStageDelay,ToMoveSceneFadeOutDurationMin);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            
            SceneManager.LoadScene(nextSceneName);
        }
        
        private void SetDatabase()
        {
            _commonInGameDatabaseInstaller = new CommonInGameDatabaseInstaller(_inGameDatabase,_outGameDatabase,_commonDatabase);
            
            EachStagePlayerInstanceData[] eachStagePlayerInstanceData =
            {
                new(StageArea.SecondStageFirst, secondStageStartInstanceData),
                new(StageArea.SecondHiddenStage,secondHiddenStageInstanceData),
                new(StageArea.SecondStageMiddle,secondStageMiddleInstanceData)
            };
            CameraInitData[] cameraInitData = 
                {secondStageCameraData,secondHiddenStageCameraData,secondStageMiddleCameraData};

            var uiData = new StageUIData(stageUIScriptableData, canvas, key,trailEffect);
            _commonInGameDatabaseInstaller.SetInGameDatabase(uiData,stageSettingsScriptableData,
                eachStagePlayerInstanceData,cameraInitData);
        }

        private void MoveNextScene(string sceneName)
        {
            _startSceneChange = true;
            toNextSceneFlag.OnNext(sceneName);
        }

        private void OnDestroy()
        {
            _secondStageStageManager?.Dispose();
        }
    }
}