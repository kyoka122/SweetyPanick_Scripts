using System;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using InGame.SceneLoader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using OutGame.Prologue;
using OutGame.Prologue.MyInput;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class PrologueSequencer:BaseSceneSequencer
    {
        [SerializeField] private PrologueBehaviour prologueBehaviour;

        private InputCaseUnknownController _inputCaseUnknownController;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            //MEMO: ↓のリストから入力情報を得られる。Joyconでストーリーを先に進めたい場合はこっち。
            //_inputCaseUnknownController = new InputCaseUnknownController();
            prologueBehaviour.Init(OnToNextSceneFlag);
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
            
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE, 2, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            SceneManager.LoadScene(nextSceneName);
        }
    }
}