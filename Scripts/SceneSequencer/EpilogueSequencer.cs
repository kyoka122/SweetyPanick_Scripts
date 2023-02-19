using InGame.Common.Database;
using InGame.Database;
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
        [SerializeField] private EpilogueBehaviour epilogueBehaviour;

        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            BGMManager.Instance.Stop();
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
            //MEMO: ↓のリストから入力情報を得られる。Joyconでストーリーを先に進めたい場合はこっち。
            //_inputCaseUnknownController = new InputCaseUnknownController();
            epilogueBehaviour.Init(MoveToNextScene);
        }

        protected override void ProcessInOrder()
        {
        }

        private void MoveToNextScene()
        {
            toNextSceneFlag.OnNext(SceneName.Score);
        }

        protected override void Finish(string nextSceneName)
        {
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE, 2, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            SceneManager.LoadScene(nextSceneName);
        }
    }
}