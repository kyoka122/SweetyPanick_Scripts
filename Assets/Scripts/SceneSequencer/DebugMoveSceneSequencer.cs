using System;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DebugInput;
using InGame.Common.Database;
using InGame.Database;
using InGame.Database.Installer;
using InGame.Stage.Installer;
using MyApplication;
using InGame.Database.ScriptableData;
using Common.MyCamera.Installer;
using InGame.SceneLoader;
using OutGame.Database;
using StageManager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class DebugMoveSceneSequencer:BaseSceneSequencer
    {
        private const float ToNextStageDelay = 1.0f;
        private const float ToMoveSceneFadeOutDurationMin = 5.0f;
        
        [SerializeField] private int currentMovePlayer;
        [SerializeField] private PlayableCharacter debugCharacter = PlayableCharacter.Candy;

        [SerializeField] private StageUIScriptableData stageUIScriptableData;
        [SerializeField] private PlayerInstanceData playerInstanceData;
        [SerializeField] private MoveStageGimmickInstaller moveStageGimmickInstaller;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
        [SerializeField] private Canvas canvas;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private CameraInitData cameraData;
        [SerializeField] private Camera camera;

        private DebugStageManager _debugStageManager;
        private DebugCharacterChanger _debugCharacterChanger;
        private CommonInGameDatabaseInstaller _commonInGameDatabaseInstaller;
        private InGameDatabase _inGameDatabase;
        private OutGameDatabase _outGameDatabase;
        private CommonDatabase _commonDatabase;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _outGameDatabase = outGameDatabase;
            
            _commonInGameDatabaseInstaller = new CommonInGameDatabaseInstaller(_inGameDatabase,_outGameDatabase,_commonDatabase,debugCharacter);
            SetDatabase();

            var cameraController = inGameDatabase.GetStageSettings().CameraInstallerPrefab.InstallMoveCamera(inGameDatabase, commonDatabase, targetGroup,
                cinemachineImpulseSource, camera);
            _debugCharacterChanger = new DebugCharacterChanger();
            _debugStageManager=new DebugStageManager(moveStageGimmickInstaller,cameraController,_inGameDatabase,commonDatabase,
                MoveNextScene);
            _debugStageManager.LateInit();
        }

        protected override void ProcessInOrder()
        {
            _debugCharacterChanger.PlayerNum
                .Subscribe(num => currentMovePlayer = num)
                .AddTo(this);

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    TryInstancePlayer();
                    _debugStageManager.FixedUpdateEnemy();
                    _debugStageManager.FixedUpdateStage();
                    _debugStageManager.FixedUpdateCamera();
                    _debugStageManager.FixedUpdatePlayableCharacter(currentMovePlayer);
                })
                .AddTo(this);
        }

        protected override void Finish(string nextSceneName)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        
        private void SetDatabase()
        {
            _commonInGameDatabaseInstaller = new CommonInGameDatabaseInstaller(_inGameDatabase,_outGameDatabase,_commonDatabase,debugCharacter);
            var uiData = new StageUIData(stageUIScriptableData, canvas, null,null);
            _commonInGameDatabaseInstaller.SetInGameDatabase(uiData, stageSettingsScriptableData,
                new[] { new EachStagePlayerInstanceData(StageArea.FirstStageFirst, playerInstanceData)},
                new[]{cameraData});
        }

        private void TryInstancePlayer()
        {
            if (currentMovePlayer < 1 || 4 < currentMovePlayer)
            {
                return;
            }

            int playerCount =
                _debugStageManager.Controllers.Count(installer => installer.GetPlayerNum() == currentMovePlayer);
            if (playerCount==0)
            {
                CharacterCommonConstData[] allCharacterConstData = _inGameDatabase.GetAllCharacterConstData();
                int characterIndex = _debugStageManager.Controllers.Count % allCharacterConstData.Length;
                var controller = allCharacterConstData[characterIndex].Installer.Install(currentMovePlayer,
                    StageArea.FirstStageFirst,_inGameDatabase, _outGameDatabase,_commonDatabase);
                _debugStageManager.AddController(controller);
                //targetGroup.AddMember(controller.GetPlayerPrefabTransform(),cinemaChineWeight,cinemaChineRadius);
                _debugStageManager.RegisterPlayerEvent(controller);
            }
        }

        private async void MoveNextScene(string sceneName)
        {
            try
            {
                await LoadManager.Instance.TryPlayLoadScreen(ToNextStageDelay,ToMoveSceneFadeOutDurationMin);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            
            toNextSceneFlag.OnNext(sceneName);
        }

        private void OnDestroy()
        {
            _debugStageManager?.Dispose();
        }
    }
}