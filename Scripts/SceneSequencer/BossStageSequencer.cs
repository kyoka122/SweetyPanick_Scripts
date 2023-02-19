using System;
using System.Linq;
using Cinemachine;
using InGame.Common.Database;
using InGame.Database;
using InGame.Database.ScriptableData;
using InGame.MyCamera.Installer;
using InGame.SceneLoader;
using InGame.Stage.Installer;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.ColateStage;
using OutGame.Database;
using OutGame.Prologue.MyInput;
using StageManager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class ColateStageSequencer:BaseSceneSequencer
    {
        [SerializeField] private ColateStageTalkPartBehaviour colateStageTalkPartBehaviour;
        [SerializeField] private ColateStageGimmickInstaller colateStageGimmickInstaller;
        [SerializeField] private PlayerScriptableData playerScriptableData;
        [SerializeField] private EnemyScriptableData enemyScriptableData;
        [SerializeField] private StageUIScriptableData stageUiScriptableData;
        [SerializeField] private StageGimmickScriptableData stageGimmickScriptableData;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private PlayerInstancePositions playerInstancePositions;

        [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
        [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;
        
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera camera;
        
        
        private ColateStageManager _colateStageManager;
        private InGameDatabase _inGameDatabase;
        private CommonDatabase _commonDatabase;
        private bool _startSceneChange;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            BGMManager.Instance.Stop();
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
            //MEMO: ↓のリストから入力情報を得られる。Joyconでストーリーを先に進めたい場合はこっち。
            //_inputCaseUnknownController = new InputCaseUnknownController();
            
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;

            SetInGameDatabase();
            var cameraController = new CameraInstaller().InstallMoveAndMultiStageCamera(inGameDatabase, commonDatabase, targetGroup,
                cinemachineImpulseSource, camera,cinemachineConfiner2D);
            
            _colateStageManager=new ColateStageManager(colateStageGimmickInstaller,cameraController,_inGameDatabase,_commonDatabase,
                MoveNextScene);
            
            //MEMO: Playerのインスタンス
            InstallAllPlayer();
            colateStageTalkPartBehaviour.Init(StartBattle);
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
            _colateStageManager.StartTalk();
            colateStageTalkPartBehaviour.StartTalkScene();
        }

        private void StartBattle()
        {
            _colateStageManager.StartBattle();
            
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (_startSceneChange)
                    {
                        return;
                    }
                    _colateStageManager.FixedUpdateEnemy();
                    _colateStageManager.FixedUpdateStage();
                    _colateStageManager.FixedUpdateColate();
                    for (int i = 1; i <=  _commonDatabase.GetUseCharacterData().Count; i++)
                    {
                        _colateStageManager.FixedUpdatePlayableCharacter(i);
                    }
                }).AddTo(this);
        }

        private void SetInGameDatabase()
        {
            _inGameDatabase.AddPlayerInstancePositions(StageArea.ColateStageFirst,playerInstancePositions);
            _inGameDatabase.SetUIData(new UIData(stageUiScriptableData,canvas));
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
                var playerController = installer.Install(i, _inGameDatabase, _commonDatabase);
                _colateStageManager.AddController(playerController);
                _colateStageManager.RegisterPlayerEvent(playerController);
            }
        }

        private void MoveNextScene(string sceneName)
        {
            _startSceneChange = true;
            toNextSceneFlag.OnNext(sceneName);
        }
        
        protected override void Finish(string nextSceneName)
        {
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE, 2, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            SceneManager.LoadScene(nextSceneName);
        }

        private void OnDestroy()
        {
            _colateStageManager.Dispose();
        }
    }
}