using System;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using Loader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using OutGame.Prologue;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class PrologueSequencer:BaseSceneSequencer
    {
        private const float ToFadeInTime = 2;
        private const float FadeInBGMTime = 3;
        
        [SerializeField] private PrologueBehaviour prologueBehaviour;

        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            prologueBehaviour.Init(OnToNextSceneFlag,outGameDatabase);
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
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
            prologueBehaviour.CallStartNarration();
        }
        

        private void OnToNextSceneFlag()
        {
            toNextSceneFlag.OnNext(SceneName.PlayerCustom);
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
            prologueBehaviour.Dispose();
        }
    }
}