using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Common.Database.ScriptableData;
using InGame.Database;
using InGame.Database.ScriptableData;
using InGame.Player.Installer;
using InGame.SceneLoader;
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
        private const float FadeInBGMTime = 3;
        
        [SerializeField] private ColateStageTalkPartBehaviour colateStageTalkPartBehaviour;
        [SerializeField] private ColateStageGimmickInstaller colateStageGimmickInstaller;
        [SerializeField] private StageUIScriptableData stageUiScriptableData;
        [SerializeField] private EnemyScriptableData enemyScriptableData;
        [SerializeField] private PlayerInstancePositions playerInstancePositions;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private CameraData battleCameraData;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera camera;
        
        
        private ColateStageManager _colateStageManager;
        private InGameDatabase _inGameDatabase;
        private OutGameDatabase _outGameDatabase;
        private CommonDatabase _commonDatabase;
        private bool _startSceneChange;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _outGameDatabase = outGameDatabase;
            _commonDatabase = commonDatabase;

            SetInGameDatabase();
            var cameraController = inGameDatabase.GetStageSettings().CameraInstallerPrefab.InstallMoveCamera(inGameDatabase, 
                commonDatabase,camera);
            
            _colateStageManager=new ColateStageManager(colateStageGimmickInstaller,cameraController,battleCameraData,_inGameDatabase,_commonDatabase,
                MoveNextScene);
            
            //MEMO: Playerのインスタンス
            InstallAllPlayer();
            colateStageTalkPartBehaviour.Init(StartBattle,outGameDatabase);
            _colateStageManager.LateInit();
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
            _colateStageManager.StartTalk();
            colateStageTalkPartBehaviour.StartTalkScene();
        }

        private void StartBattle()
        {
            colateStageTalkPartBehaviour.Dispose();
            BGMManager.Instance.Stop();
            BGMManager.Instance.Play(BGMPath.STAGE_BGM);
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
            _inGameDatabase.SetStageSettings(stageSettingsScriptableData);
            _inGameDatabase.SetEnemyData(enemyScriptableData);
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
                var playerController = installer.Install(i,StageArea.ColateStageFirst, _inGameDatabase,_outGameDatabase, _commonDatabase);
                _colateStageManager.AddController(playerController);
                _colateStageManager.RegisterPlayerEvent(playerController);
            }
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
            
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE, FadeInBGMTime, () => {
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
            _colateStageManager?.Dispose();
        }
    }
}