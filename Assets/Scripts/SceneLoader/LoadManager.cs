using System;
using System.Threading;
using System.Threading.Tasks;
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
        [SerializeField] private PlayingInfoView playingInfoView;
        
        private LoadEntity _loadEntity;
        private BlackFadeLogic _blackFadeLogic;
        private LoadLogic _loadLogic;
        private GameOverLogic _gameOverLogic;
        private PlayingInfoLogic _playingInfoLogic;

        public void Init(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            loadScreenView.Init();
            blackFadeView.Init();
            gameOverView.Init();
            loadCameraView.Init();
            playingInfoView.Init();
            _loadEntity = new LoadEntity(inGameDatabase,commonDatabase);
            _loadLogic = new LoadLogic(_loadEntity, loadScreenView,blackFadeView,loadCameraView);
            _blackFadeLogic = new BlackFadeLogic(_loadEntity, blackFadeView,loadCameraView);
            _gameOverLogic = new GameOverLogic(_loadEntity, gameOverView, loadCameraView);
            _playingInfoLogic = new PlayingInfoLogic(_loadEntity, playingInfoView, loadCameraView);
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
        public async UniTask TryPlayLoadScreen(float thisTaskDelay,float toFadeOutDurationMin)
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
            await UniTask.Delay(TimeSpan.FromSeconds(thisTaskDelay), cancellationToken: thisToken);
            Debug.Log($"Load FadeIn Finish");
        }
        
        /// <summary>
        /// 指定の操作説明画面をフェードインさせる
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toFadeOutDurationMin"></param>
        public async UniTask TryFadeInPlayingInfo(PlayingInfoType type,float toFadeOutDurationMin)
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(() => _blackFadeLogic.isFading || _loadLogic.isFading ||
                                          _gameOverLogic.isFading|| _playingInfoLogic.isFading,cancellationToken: thisToken);

            Debug.Log($"WaitWhile:{_blackFadeLogic.isFading || _loadLogic.isFading || _gameOverLogic.isFading|| _playingInfoLogic.isFading}");
            //MEMO: 他のLoadが実行中であれば実行しない
            if (_blackFadeLogic.isFadeIn||_gameOverLogic.isFadeIn)
            {
                Debug.Log(
                    $"Already Load. Load: BlackFade:{_blackFadeLogic.isFadeIn}, gameOver:{_gameOverLogic.isFadeIn}");
                return;
            }
            //MEMO: LoadScreenが出ていなければ実行しない
            if (!_loadLogic.isLoading)
            {
                Debug.Log($"Not Active LoadScreen.");
                return;
            }
            await _playingInfoLogic.PlayPlayingInfoFadeIn(type,toFadeOutDurationMin,thisToken);
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
        
        
        /// <summary>
        /// 指定の操作説明画面をフェードアウトさせる
        /// </summary>
        /// <param name="playingInfoType"></param>
        public async UniTask TryFadeOutPlayingInfo(PlayingInfoType playingInfoType)
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(() => _blackFadeLogic.isFading||_loadLogic.isFading|| !_playingInfoLogic.canFadeOut,
                cancellationToken: thisToken);

            if (_playingInfoLogic.currentPlayingInfoType==playingInfoType)
            {
                Debug.Log($"PlayingInfoFadeOut!");
                await _playingInfoLogic.TryPlayPlayingInfoFadeOut(thisToken);
                return;
            }
            
            Debug.Log($"Couldn`t FadeOut!");
        }

        

        private void OnDestroy()
        {
            _loadEntity?.Dispose();
        }
    }
}