using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using InGame.SceneLoader.Entity;
using InGame.SceneLoader.Logic;
using InGame.SceneLoader.View;
using MyApplication;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace InGame.SceneLoader
{
    public class LoadManager:SingletonMonoBehaviour<LoadManager>
    {
        protected override bool IsDontDestroyOnLoad => true;

        [SerializeField] private LoadScreenView loadScreenView;
        [SerializeField] private BlackFadeView blackFadeView;
        [SerializeField] private GameOverView gameOverView;
        [SerializeField] private LoadCameraView loadCameraView;
        
        private LoadEntity _loadEntity;
        private BlackFadeLogic _blackFadeLogic;
        private LoadLogic _loadLogic;
        private GameOverLogic _gameOverLogic;

        public void Init(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            loadScreenView.Init();
            blackFadeView.Init();
            gameOverView.Init();
            loadCameraView.Init();
            _loadEntity = new LoadEntity(inGameDatabase,commonDatabase);
            _loadLogic = new LoadLogic(_loadEntity, loadScreenView,blackFadeView,loadCameraView);
            _blackFadeLogic = new BlackFadeLogic(_loadEntity, blackFadeView,loadCameraView);
            _gameOverLogic = new GameOverLogic(_loadEntity, gameOverView, loadCameraView);
        }

        protected override void Awake()
        {
            base.Awake();
            if (this != Instance)
            {
                foreach (Transform tmp in gameObject.transform)
                {
                    Destroy(tmp.gameObject);
                }
                Destroy(gameObject);
                return;
            }

            if (IsDontDestroyOnLoad)
            {
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// ブラックスクリーンによるフェードインをしていなければフェードイン演出を開始する
        /// </summary>
        public async UniTask TryPlayBlackFadeIn()
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(() => _blackFadeLogic.isFading||_loadLogic.isFading||_gameOverLogic.isFading,
                cancellationToken: thisToken);
            
            //MEMO: 他のLoadが実行中であれば実行しない
            if (_blackFadeLogic.isFadeIn||_loadLogic.isLoading||_gameOverLogic.isFadeIn)
            {
                Debug.Log(
                    $"Already Load. Load:{_loadLogic.isLoading}, BlackFade:{_blackFadeLogic.isFadeIn}, gameOver:{_gameOverLogic.isFadeIn}");
                return;
            }
            await _blackFadeLogic.PlayFadeIn(thisToken);
        }
        
        /// <summary>
        /// ブラックスクリーンによるフェードインをしていなければフェードイン演出を開始する
        /// </summary>
        public async UniTask TryPlayBlackFadeIn(Vector2 fadeCenter)
        {
            //TODO: Fade位置の変更処理
            await TryPlayBlackFadeIn();
        }

        /// <summary>
        /// ロード中でなければロード演出を開始する
        /// </summary>
        public async UniTask TryPlayLoadScreen(float thisTaskDuration,float toFadeOutDurationMin)
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(() => _blackFadeLogic.isFading||_loadLogic.isFading||
                                          _gameOverLogic.isFading, cancellationToken: thisToken);
            
            //MEMO: 他のLoadが実行中であれば実行しない
            if (_blackFadeLogic.isFadeIn||_loadLogic.isLoading||_gameOverLogic.isFadeIn)
            {
                Debug.Log(
                    $"Already Load. Load:{_loadLogic.isLoading}, BlackFade:{_blackFadeLogic.isFadeIn}, gameOver:{_gameOverLogic.isFadeIn}");
                return;
            }
            await _loadLogic.PlayLoadScreen(toFadeOutDurationMin,thisToken);
            await UniTask.Delay(TimeSpan.FromSeconds(thisTaskDuration), cancellationToken: thisToken);
            
        }

        
        /// <summary>
        /// 現在のフェード、Load状態に合わせてフェードアウトする
        /// </summary>
        public async UniTask TryPlayFadeOut()
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(() => _blackFadeLogic.isFading||_loadLogic.isFading, cancellationToken: thisToken);

            if (_blackFadeLogic.isFadeIn)
            {
                Debug.Log($"BlackFadeOut!");
                _gameOverLogic.TrySetFadeOut();
                await _blackFadeLogic.TryPlayBlackFadeOut(thisToken);
                return;
            }
            if (_loadLogic.isLoading)
            {
                Debug.Log($"LoadFadeOut");
                await UniTask.WaitUntil(() => _loadLogic.canFinishLoading, cancellationToken: thisToken);
                await _loadLogic.TryPlayLoadScreenFadeOut(thisToken);
                return;
            }
            
            Debug.Log($"Couldn`t FadeOut!");
        }
        
        public async void TryPlayGameOverFadeIn()
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(() => _blackFadeLogic.isFading||_loadLogic.isFading, cancellationToken: thisToken);

            //MEMO: 他のLoadが実行中であれば実行しない
            if (_blackFadeLogic.isFadeIn||_loadLogic.isLoading||_gameOverLogic.isFadeIn)
            {
                Debug.Log(
                    $"Already Load. Load:{_loadLogic.isLoading}, BlackFade:{_blackFadeLogic.isFadeIn}, gameOver:{_gameOverLogic.isFadeIn}");
                return;
            }
            await _gameOverLogic.PlayFadeIn(thisToken);
            await _gameOverLogic.WaitOnNextKey();
            await _blackFadeLogic.PlayFadeIn(thisToken);
            SceneManager.LoadScene(SceneName.Title);
        }

        private void OnDestroy()
        {
            _loadEntity?.Dispose();
        }
    }
}