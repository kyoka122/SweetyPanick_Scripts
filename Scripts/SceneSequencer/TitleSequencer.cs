using System;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using InGame.MyCamera.Installer;
using InGame.SceneLoader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
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
            foreach (var joycon in JoyconManager.Instance.j)
            {
                if (joycon.GetButtonDown(Joycon.Button.DPAD_RIGHT)&&!joycon.isLeft)
                {
                    toNextSceneFlag.OnNext(SceneName.Prologue);
                }
            }
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