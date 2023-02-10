using System;
using System.Linq;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DebugInput;
using InGame.Common.Database;
using InGame.Database;
using InGame.Stage.Installer;
using KanKikuchi.AudioManager;
using MyApplication;
using InGame.Database.ScriptableData;
using InGame.MyCamera.Installer;
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
        [SerializeField] private PlayerInstancePositions firstPlayerInstancePositions;
        [SerializeField] private PlayerInstancePositions firstStageMiddle;
        [SerializeField] private PlayerInstancePositions firstHiddenStage;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;

        [SerializeField] private MoveStageGimmickInstaller moveStageGimmickInstaller;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private Canvas canvas;
        
        [SerializeField] private Camera camera;

        private FirstStageManager _firstStageManager;
        private DebugCharacterChanger _debugCharacterChanger;
        private InGameDatabase _inGameDatabase;
        private CommonDatabase _commonDatabase;
        private bool startSceneChange;

        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            SetInGameDatabase();
            var cameraController = new CameraInstaller().InstallMoveCamera(inGameDatabase,commonDatabase, targetGroup, camera);
            _debugCharacterChanger = new DebugCharacterChanger();
            _firstStageManager=new FirstStageManager(moveStageGimmickInstaller,cameraController,_inGameDatabase,_commonDatabase,
                MoveNextScene);
            inGameDatabase.SetStageSettings(stageSettingsScriptableData);
            InstallAllPlayer();
        }

        protected override async void ProcessInOrder()
        {
            BGMManager.Instance.Stop();
            try
            {
                await LoadManager.Instance.TryPlayFadeOut();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            _firstStageManager.LateInit();
            BGMManager.Instance.Play(BGMPath.STAGE_BGM);
            
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (startSceneChange)
                    {
                        return;
                    }
                    _firstStageManager.FixedUpdateEnemy();
                    _firstStageManager.FixedUpdateStage();
                    for (int i = 1; i <=  _commonDatabase.GetUseCharacterData().Count; i++)
                    {
                        _firstStageManager.FixedUpdatePlayableCharacter(i);
                    }
                })
                .AddTo(this);
            this.LateUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _firstStageManager.LateUpdate();
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
            _inGameDatabase.AddPlayerInstancePositions(StageArea.FirstStageFirst,firstPlayerInstancePositions);
            _inGameDatabase.AddPlayerInstancePositions(StageArea.FirstStageMiddle,firstStageMiddle);
            _inGameDatabase.AddPlayerInstancePositions(StageArea.FirstHiddenStage,firstHiddenStage);
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
                    ?.Installer.Install(i, _inGameDatabase, _commonDatabase);
               
                _firstStageManager.AddController(playerController);
                //targetGroup.AddMember(controller.GetPlayerPrefabTransform(),cinemaChineWeight,cinemaChineRadius);
                _firstStageManager.RegisterPlayerEvent(playerController);
            }
        }

        private void MoveNextScene(string sceneName)
        {
            startSceneChange = true;
            toNextSceneFlag.OnNext(sceneName);
        }

        private void OnDestroy()
        {
            _firstStageManager.Dispose();
        }
    }
}