using System;
using InGame.Common.Database;
using InGame.Database;
using InGame.SceneLoader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using OutGame.Epilogue;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class EpilogueSequencer:BaseSceneSequencer
    {
        private const float ToNextStageDelay = 1.0f;
        private const float ToMoveSceneFadeOutDurationMin = 5.0f;
        
        [SerializeField] private EpilogueBehaviour epilogueBehaviour;

        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            BGMManager.Instance.Stop();
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
            epilogueBehaviour.Init(MoveToNextScene,outGameDatabase);
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
            epilogueBehaviour.CallStartTalk();
        }

        private void MoveToNextScene()
        {
            toNextSceneFlag.OnNext(SceneName.Score);
        }

        protected override async void Finish(string nextSceneName)
        {
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE, 2, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            try
            {
                await LoadManager.Instance.TryPlayLoadScreen(ToNextStageDelay,ToMoveSceneFadeOutDurationMin);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            SceneManager.LoadScene(nextSceneName);
        }
        
        private void OnDestroy()
        {
            epilogueBehaviour.Dispose();
        }
    }
}