using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using Common.MyCamera.Installer;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.PlayerCustom.Installer;
using OutGame.PlayerCustom.Manager;
using InGame.SceneLoader;
using OutGame.Database;
using OutGame.Database.ScriptableData;
using OutGame.PlayerCustom.View;
using TalkSystem;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class PlayerCustomSceneSequencer:BaseSceneSequencer
    {
        private const float ToNextStageDelay = 0.3f;
        private const float ToMoveSceneToFadeOutDurationMin = 5.0f;
        private const float PlayingInfoToFadeOutDurationMin = 4f;
        
        [SerializeField] private PlayerCountView playerCountView;
        [SerializeField] private ControllersPanelView controllersPanelView;
        [SerializeField] private CharacterSelectPanelView characterSelectPanelView;
        [SerializeField] private FromMessageWindowRecieverView fromMessageWindowRecieverView;
        [SerializeField] private ToMessageWindowSenderView toMessageWindowSenderView;
        [SerializeField] private CharacterIconView[] characterIconView;
        
        [SerializeField] private Camera camera;
        
        [SerializeField] private Dialogs playerCountDialog;
        [SerializeField] private Dialogs controllerDialog;
        [SerializeField] private Dialogs characterDialog;
        [SerializeField] private Dialogs cheerDialog;
        
        private PlayerCustomManager _manager;
        private PlayerCustomInstaller _playerCustomInstaller;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            inGameDatabase.GetStageSettings().CameraInstallerPrefab.InstallConstCamera(inGameDatabase,commonDatabase,camera);
            var onMoveScene = new Subject<bool>();
            _playerCustomInstaller = new PlayerCustomInstaller();
            _manager=_playerCustomInstaller.Install(playerCountView, controllersPanelView, characterSelectPanelView,
                fromMessageWindowRecieverView, toMessageWindowSenderView,characterIconView, playerCountDialog,
                controllerDialog,characterDialog,cheerDialog, outGameDatabase,commonDatabase, onMoveScene);
            
            onMoveScene
                .Subscribe(_=> toNextSceneFlag.OnNext(SceneName.FirstStage)).AddTo(this);
        }

        protected override async void ProcessInOrder()
        {
            BGMManager.Instance.Stop();
            await LoadManager.Instance.TryPlayFadeOut();
            BGMManager.Instance.Play(BGMPath.PLAYER_CUSTOM);
            _manager.Start();
        }

        protected override async void Finish(string nextSceneName)
        {
            BGMManager.Instance.FadeOut(BGMPath.PLAYER_CUSTOM, 1, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            _manager.Dispose();
            try
            {
                await LoadManager.Instance.TryPlayLoadScreen(ToNextStageDelay,ToMoveSceneToFadeOutDurationMin);
                await LoadManager.Instance.TryFadeInPlayingInfo(PlayingInfoType.Fix,PlayingInfoToFadeOutDurationMin);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            
            SceneManager.LoadScene(nextSceneName);
        }

        private void OnDestroy()
        {
            _manager?.Dispose();
        }
    }
}