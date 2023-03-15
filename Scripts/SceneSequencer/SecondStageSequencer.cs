using System;
using System.Linq;
using Cinemachine;
using DebugInput;
using InGame.Common.Database;
using InGame.Database;
using InGame.Database.ScriptableData;
using InGame.Player.Installer;
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
        [SerializeField] private PlayerInstancePositions secondStageStartInstancePositions;
        [SerializeField] private PlayerInstancePositions secondStageMiddleInstancePositions;
        [SerializeField] private PlayerInstancePositions secondHiddenStageInstancePositions;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private CameraInitData secondStageCameraData;
        [SerializeField] private CameraInitData secondHiddenStageCameraData;
        [SerializeField] private CameraInitData secondStageMiddleCameraData;

        [SerializeField] private MoveStageGimmickInstaller moveStageGimmickInstaller;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
        [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera camera;

        private SecondStageManager _secondStageManager;
        private DebugCharacterChanger _debugCharacterChanger;
        private InGameDatabase _inGameDatabase;
        private OutGameDatabase _outGameDatabase;
        private CommonDatabase _commonDatabase;
        private bool _startSceneChange;

        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            Debug.Log($"Init SecondStage!!",gameObject);
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _outGameDatabase = outGameDatabase;
            SetInGameDatabase();
            commonDatabase.AddCameraInitData(secondStageCameraData);
            commonDatabase.AddCameraInitData(secondHiddenStageCameraData);
            commonDatabase.AddCameraInitData(secondStageMiddleCameraData);
            
            var cameraController = _inGameDatabase.GetStageSettings().CameraInstallerPrefab.InstallMoveAndMultiStageCamera(inGameDatabase, commonDatabase, targetGroup,
                cinemachineImpulseSource, camera,cinemachineConfiner2D);
            //_debugCharacterChanger = new DebugCharacterChanger();
            _secondStageManager=new SecondStageManager(moveStageGimmickInstaller,cameraController,_inGameDatabase,_commonDatabase,
                MoveNextScene);
            
            InstallAllPlayer();
            _secondStageManager.LateInit();
        }

        protected override async void ProcessInOrder()
        {
            this.LateUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _secondStageManager.LateUpdate();
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
                    _secondStageManager.FixedUpdateEnemy();
                    _secondStageManager.FixedUpdateStage();
                    _secondStageManager.FixedUpdateCamera();
                    for (int i = 1; i <=  _commonDatabase.GetUseCharacterData().Count; i++)
                    {
                        _secondStageManager.FixedUpdatePlayableCharacter(i);
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
        
        private void SetInGameDatabase()
        {
            _inGameDatabase.SetUIData(new UIData(stageUIScriptableData,canvas));
            _inGameDatabase.AddPlayerInstancePositions(StageArea.SecondStageFirst,secondStageStartInstancePositions);
            _inGameDatabase.AddPlayerInstancePositions(StageArea.SecondHiddenStage,secondHiddenStageInstancePositions);
            _inGameDatabase.AddPlayerInstancePositions(StageArea.SecondStageMiddle,secondStageMiddleInstancePositions);
            _inGameDatabase.SetStageSettings(stageSettingsScriptableData);
        }

        private void InstallAllPlayer()
        {
            CharacterCommonConstData[] allCharacterConstData = _inGameDatabase.GetAllCharacterConstData();
            var characterData=_commonDatabase.GetUseCharacterData();
            
            //MEMO: Player設定シーンを飛ばした場合、仮でキーボード、Candyのデータを追加する
            if (characterData == null)
            {
                new DebugInputInstaller().Install(_commonDatabase);
            }
            characterData=_commonDatabase.GetUseCharacterData();
            
            for (int i = 1; i <= characterData.Count; i++)
            {
                UseCharacterData useCharacterData = characterData.FirstOrDefault(data => data.playerNum == i);
                if (useCharacterData==null)
                {
                    Debug.LogError($"Coldn`t Find UseCharacterData");
                    return;
                }
                var playerController = allCharacterConstData
                        .FirstOrDefault(data => data.CharacterType == useCharacterData.playableCharacter)?
                        .Installer.Install(i,StageArea.SecondStageFirst, _inGameDatabase,_outGameDatabase, _commonDatabase);
               
                _secondStageManager.AddController(playerController);
                _secondStageManager.RegisterPlayerEvent(playerController);
            }
        }

        private void MoveNextScene(string sceneName)
        {
            _startSceneChange = true;
            toNextSceneFlag.OnNext(sceneName);
        }

        private void OnDestroy()
        {
            _secondStageManager?.Dispose();
        }

    }
}