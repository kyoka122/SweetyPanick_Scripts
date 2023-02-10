using System;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DebugInput;
using InGame.Common.Database;
using InGame.Database;
using InGame.Stage.Installer;
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
    public class DebugMoveSceneSequencer:BaseSceneSequencer
    {
        private const float ToNextStageDelay = 1.0f;
        private const float ToMoveSceneFadeOutDurationMin = 5.0f;
        
        [SerializeField] private int currentMovePlayer;
        
        [SerializeField] private PlayerScriptableData playerScriptableData;
        [SerializeField] private EnemyScriptableData enemyScriptableData;
        [SerializeField] private StageUIScriptableData stageUIScriptableData;
        [SerializeField] private PlayerInstancePositions playerInstancePositions;
        
        [SerializeField] private MoveStageGimmickInstaller moveStageGimmickInstaller;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private Canvas canvas;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private Camera camera;

        private DebugStageManager _debugStageManager;
        private DebugCharacterChanger _debugCharacterChanger;
        private InGameDatabase _inGameDatabase;
        private CommonDatabase _commonDatabase;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            SetDatabase();
            var cameraController = new CameraInstaller().InstallMoveCamera(inGameDatabase,commonDatabase, targetGroup, camera);
            _debugCharacterChanger = new DebugCharacterChanger();
            _debugStageManager=new DebugStageManager(moveStageGimmickInstaller,cameraController,_inGameDatabase,commonDatabase,
                MoveNextScene);
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
            _inGameDatabase.AddPlayerInstancePositions(StageArea.FirstStageFirst,playerInstancePositions);
            _inGameDatabase.SetUIData(new UIData(stageUIScriptableData,canvas));
            _inGameDatabase.SetStageSettings(stageSettingsScriptableData);
            _inGameDatabase.SetEnemyData(enemyScriptableData);
            SetCharacterDatabase();
        }

        private void SetCharacterDatabase()
        {
     
            var candyCommonParameter = playerScriptableData.GetCommonParameter(PlayableCharacter.Candy);
            var mashCommonParameter = playerScriptableData.GetCommonParameter(PlayableCharacter.Mash);
            var fuCommonParameter = playerScriptableData.GetCommonParameter(PlayableCharacter.Fu);
            var kureCommonParameter = playerScriptableData.GetCommonParameter(PlayableCharacter.Kure);

            var candyParameter = playerScriptableData.GetCandyParameter();
            var mashParameter = playerScriptableData.GetMashParameter();
            var fuParameter = playerScriptableData.GetFuParameter();
            var kureParameter = playerScriptableData.GetKureParameter();

            if (candyCommonParameter != null)
            {
                _inGameDatabase.SetCandyStatus(new CandyStatus(candyCommonParameter,
                    playerScriptableData.GetCandyParameter(),
                    0));
                _inGameDatabase.SetCommonCandyConstData(new CharacterCommonConstData(candyCommonParameter));
                _inGameDatabase.SetCandyConstData(new CandyConstData(candyParameter));
            }

            if (mashCommonParameter != null)
            {
                _inGameDatabase.SetMashStatus(new MashStatus(mashCommonParameter,
                    playerScriptableData.GetMashParameter()));
                _inGameDatabase.SetCommonMashConstData(new CharacterCommonConstData(mashCommonParameter));
                _inGameDatabase.SetMashConstData(new MashConstData(mashParameter));
            }

            if (fuCommonParameter != null)
            {
                _inGameDatabase.SetFuStatus(new FuStatus(fuCommonParameter, playerScriptableData.GetFuParameter()));
                _inGameDatabase.SetCommonFuConstData(new CharacterCommonConstData(fuCommonParameter));
                _inGameDatabase.SetFuConstData(new FuConstData(fuParameter));
            }

            if (kureCommonParameter != null)
            {
                _inGameDatabase.SetKureStatus(new KureStatus(kureCommonParameter,
                    playerScriptableData.GetKureParameter()));
                _inGameDatabase.SetCommonKureConstData(new CharacterCommonConstData(kureCommonParameter));
                _inGameDatabase.SetKureConstData(new KureConstData(kureParameter));
            }
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
                var controller = allCharacterConstData[characterIndex].Installer.Install(currentMovePlayer,_inGameDatabase,_commonDatabase);
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
            _debugStageManager.Dispose();
        }
    }
}