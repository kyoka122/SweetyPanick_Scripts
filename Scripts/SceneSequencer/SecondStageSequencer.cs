using System;
using System.Linq;
using Cinemachine;
using DebugInput;
using InGame.Common.Database;
using InGame.Database;
using InGame.Database.ScriptableData;
using InGame.MyCamera.Installer;
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
        [SerializeField] private int currentMovePlayer;
        
        [SerializeField] private StageUIScriptableData stageUIScriptableData;
        [SerializeField] private PlayerInstancePositions secondStageStartInstancePositions;
        [SerializeField] private PlayerInstancePositions secondStageMiddleInstancePositions;
        [SerializeField] private PlayerInstancePositions secondHiddenStageInstancePositions;
        [SerializeField] private CameraData secondStageCameraData;
        [SerializeField] private CameraData secondHiddenStageCameraData;
        [SerializeField] private CameraData secondStageMiddleCameraData;

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
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _outGameDatabase = outGameDatabase;
            SetInGameDatabase();
            var cameraController = new CameraInstaller().InstallMoveAndMultiStageCamera(inGameDatabase, commonDatabase, targetGroup,
                cinemachineImpulseSource, camera,cinemachineConfiner2D);
            _debugCharacterChanger = new DebugCharacterChanger();
            _secondStageManager=new SecondStageManager(moveStageGimmickInstaller,cameraController,_inGameDatabase,_commonDatabase,
                MoveNextScene);
            
            commonDatabase.AddCameraData(secondStageCameraData);
            commonDatabase.AddCameraData(secondHiddenStageCameraData);
            commonDatabase.AddCameraData(secondStageMiddleCameraData);
            InstallAllPlayer();
            _secondStageManager.LateInit();
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

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (_startSceneChange)
                    {
                        return;
                    }
                    _secondStageManager.FixedUpdateEnemy();
                    _secondStageManager.FixedUpdateStage();
                    for (int i = 1; i <=  _commonDatabase.GetUseCharacterData().Count; i++)
                    {
                        _secondStageManager.FixedUpdatePlayableCharacter(i);
                    }
                })
                .AddTo(this);
            this.LateUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _secondStageManager.LateUpdate();
                }).AddTo(this);
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
        
        private void SetInGameDatabase()
        {
            _inGameDatabase.SetUIData(new UIData(stageUIScriptableData,canvas));
            _inGameDatabase.AddPlayerInstancePositions(StageArea.SecondStageFirst,secondStageStartInstancePositions);
            _inGameDatabase.AddPlayerInstancePositions(StageArea.SecondHiddenStage,secondHiddenStageInstancePositions);
            _inGameDatabase.AddPlayerInstancePositions(StageArea.SecondStageMiddle,secondStageMiddleInstancePositions);
        }

        private void InstallAllPlayer()
        {
            CharacterCommonConstData[] allCharacterConstData = _inGameDatabase.GetAllCharacterConstData();
            var characterData=_commonDatabase.GetUseCharacterData();

            for (int i = 1; i <= characterData.Count; i++)
            {
                PlayableCharacter character =
                    characterData.FirstOrDefault(data => data.playerNum == i).playableCharacter;
                var playerController = allCharacterConstData.FirstOrDefault(data => data.CharacterType == character)
                    ?.Installer.Install(i, _inGameDatabase,_outGameDatabase, _commonDatabase);
               
                _secondStageManager.AddController(playerController);
                //targetGroup.AddMember(controller.GetPlayerPrefabTransform(),cinemaChineWeight,cinemaChineRadius);
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
            _secondStageManager.Dispose();
        }

    }
}