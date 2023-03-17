using System;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using InGame.SceneLoader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using OutGame.Title;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class TitleSequencer:BaseSceneSequencer
    {
        private const float ToFadeInTime = 1;
        private const float FadeInBGMTime = 2;
        
        [SerializeField] private Camera camera;
        [SerializeField] private TitleBehaviour titleBehaviour;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            inGameDatabase.GetStageSettings().CameraInstallerPrefab.InstallConstCamera(inGameDatabase,commonDatabase,camera);
            titleBehaviour.Init(OnToNextSceneFlag,outGameDatabase);
        }

        protected override async void ProcessInOrder()
        {
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
            try
            {
                await LoadManager.Instance.TryPlayFadeOut();
                Debug.Log($"FadeOut");
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }

            titleBehaviour.StartWaitingInput();
        }
        
        private void OnToNextSceneFlag()
        {
            toNextSceneFlag.OnNext(SceneName.Prologue);
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
            
            await UniTask.Delay(TimeSpan.FromSeconds(ToFadeInTime),cancellationToken:this.GetCancellationTokenOnDestroy());
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE, FadeInBGMTime, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            SceneManager.LoadScene(nextSceneName);
        }

        private void OnDestroy()
        {
            titleBehaviour.Dispose();
        }
    }
}