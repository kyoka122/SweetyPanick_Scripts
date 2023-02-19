using System;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using InGame.MyCamera.Installer;
using InGame.SceneLoader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSequencer
{
    public class TitleSequencer:BaseSceneSequencer
    {
        [SerializeField] private Camera camera;
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            new CameraInstaller().InstallConstCamera(inGameDatabase,commonDatabase,camera);
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
        }

        protected override void ProcessInOrder()
        {
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        toNextSceneFlag.OnNext(SceneName.Prologue);
                    }
                }).AddTo(this);
                
            /*foreach (var joycon in JoyconManager.Instance.j)
            {
                if (joycon.GetButtonDown(Joycon.Button.DPAD_RIGHT)&&!joycon.isLeft)
                {
                    toNextSceneFlag.OnNext(SceneName.Prologue);
                }
            }*/
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
            
            await UniTask.Delay(TimeSpan.FromSeconds(1f),cancellationToken:this.GetCancellationTokenOnDestroy());
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE, 2, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            SceneManager.LoadScene(nextSceneName);
        }
    }
}