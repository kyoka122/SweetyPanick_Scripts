using System;
using System.Linq;
using Cinemachine;
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
        [SerializeField] private PlayerScriptableData playerScriptableData;
        [SerializeField] private EnemyScriptableData enemyScriptableData;
        [SerializeField] private StageGimmickScriptableData stageGimmickScriptableData;
        [SerializeField] private int currentMovePlayer;
        
        [SerializeField] private StageUIScriptableData stageUIScriptableData;
        [SerializeField] private PlayerInstancePositions firstPlayerInstancePositions;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private MoveStageGimmickInstaller moveStageGimmickInstaller;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
        [SerializeField] private Canvas canvas;
        
        [SerializeField] private Camera camera;

        private FirstStageManager _firstStageManager;
        private InGameDatabase _inGameDatabase;
        private OutGameDatabase _outGameDatabase;
        private CommonDatabase _commonDatabase;
        private bool _startSceneChange;

        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            BGMManager.Instance.Stop();
            BGMManager.Instance.Play(BGMPath.STAGE_BGM);
            if (inGameDatabase==null)
            {
                Debug.LogError($"Not Found InGameDatabase");
            }
            else
            {
                _inGameDatabase = inGameDatabase;
            }
            
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _outGameDatabase = outGameDatabase;
            SetInGameDatabase();
            var cameraController = new CameraInstaller().InstallMoveCamera(inGameDatabase, commonDatabase, targetGroup,
                cinemachineImpulseSource, camera);
            //_debugCharacterChanger = new DebugCharacterChanger();
            _firstStageManager=new FirstStageManager(moveStageGimmickInstaller,cameraController,_inGameDatabase,_commonDatabase,
                MoveNextScene);
            InstallAllPlayer();
            _firstStageManager.LateInit();
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
            
            BGMManager.Instance.Play(BGMPath.STAGE_BGM);
            
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (_startSceneChange)
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
            _inGameDatabase.SetEnemyData(enemyScriptableData);
            _inGameDatabase.SetStageGimmickData(stageGimmickScriptableData);
            _inGameDatabase.SetStageSettings(stageSettingsScriptableData);
            SetCharacterDatabase();
        }

        //TODO: 委譲する
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

            _inGameDatabase.AddPlayerInstancePositions(StageArea.FirstStageFirst,firstPlayerInstancePositions);
        }

        private void InstallAllPlayer()
        {
            CharacterCommonConstData[] allCharacterConstData = _inGameDatabase.GetAllCharacterConstData();
            var characterData=_commonDatabase.GetUseCharacterData();

            for (int i = 1; i <= characterData.Count; i++)
            {
                var oneCharacterData = characterData.FirstOrDefault(data => data.playerNum == i);
                if (oneCharacterData==null)
                {
                    Debug.LogError($"Couldn`t Find OneCharacterData");
                    continue;
                }
                PlayableCharacter character =
                    oneCharacterData.playableCharacter;
                var installer = allCharacterConstData.FirstOrDefault(data => data.CharacterType == character)
                    ?.Installer;
                if (installer==null)
                {
                    continue;
                }
                var playerController = installer.Install(i, _inGameDatabase,_outGameDatabase, _commonDatabase);
                _firstStageManager.AddController(playerController);
                //targetGroup.AddMember(controller.GetPlayerPrefabTransform(),cinemaChineWeight,cinemaChineRadius);
                _firstStageManager.RegisterPlayerEvent(playerController);
            }
        }

        private void MoveNextScene(string sceneName)
        {
            _startSceneChange = true;
            toNextSceneFlag.OnNext(sceneName);
        }

        private void OnDestroy()
        {
            _firstStageManager.Dispose();
        }
    }
}