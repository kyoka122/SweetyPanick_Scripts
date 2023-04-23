using System;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using InGame.OutGame.Score;
using InGame.SceneLoader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class ScoreSceneSequencer:BaseSceneSequencer
    {
        [SerializeField] private ScoreSceneBehaviour scoreSceneBehaviour;
        [SerializeField] private float sceneLoadDelay=5f;
        
        protected override void Init(InGameDatabase inGameDatabase, OutGameDatabase outGameDatabase, CommonDatabase commonDatabase)
        {
            scoreSceneBehaviour.Init(inGameDatabase, outGameDatabase, MoveTitle);
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
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
            scoreSceneBehaviour.StartTalkingShowScoreAnimation();
        }

        private void MoveTitle()
        {
            toNextSceneFlag.OnNext(SceneName.Title);
        }

        protected override async void Finish(string nextSceneName)
        {
            BGMManager.Instance.Stop(BGMPath.STAGE_BGM);
            try
            {
                await LoadManager.Instance.TryPlayBlackFadeIn();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }

            await UniTask.Delay(TimeSpan.FromSeconds(sceneLoadDelay), cancellationToken: this.GetCancellationTokenOnDestroy());
            SceneManager.LoadScene(nextSceneName);
        }
    }
}