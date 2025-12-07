using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using Loader.Entity;
using Loader.Logic;
using Loader.View;
using MyApplication;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Loader
{
    /// <summary>
    /// 画面遷移時の情報の全体管理を行うクラス
    /// </summary>
    public class LoadManager : SingletonMonoBehaviour<LoadManager>
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

        public void Init(InGameDatabase inGameDatabase, CommonDatabase commonDatabase)
        {
            loadScreenView.Init();
            blackFadeView.Init();
            gameOverView.Init();
            loadCameraView.Init();
            playingInfoView.Init();
            _loadEntity = new LoadEntity(inGameDatabase, commonDatabase);
            _loadLogic = new LoadLogic(_loadEntity, loadScreenView, blackFadeView, loadCameraView);
            _blackFadeLogic = new BlackFadeLogic(_loadEntity, blackFadeView, loadCameraView);
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
        /// スクリーンが出ていなければブラックスクリーンのフェードイン演出を開始する
        /// </summary>
        public async UniTask<bool> TryPlayBlackFadeIn()
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(IsPlayingAnyScreenFading, cancellationToken: thisToken);

            //MEMO: 他のスクリーンが表示中であれば実行しない
            if (IsAnyScreenFadeState())
            {
                Debug.Log($"Already Load. Load:{_loadLogic.isLoadingState}, BlackFade:{_blackFadeLogic.isFadeInState}, gameOver:{_gameOverLogic.isFadeInState}");
                return false;
            }

            await _blackFadeLogic.PlayFadeIn(thisToken);
            return true;
        }

        /// <summary>
        /// ロード中でなければロード演出を開始する
        /// </summary>
        public async UniTask<bool> TryPlayLoadScreen(float thisTaskDelay, float toFadeOutDurationMin)
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(IsPlayingAnyScreenFading, cancellationToken: thisToken);

            //MEMO: 他のスクリーンが表示中であれば実行しない
            if (IsAnyScreenFadeState())
            {
                Debug.Log($"Already Load. Load:{_loadLogic.isLoadingState}, BlackFade:{_blackFadeLogic.isFadeInState}, gameOver:{_gameOverLogic.isFadeInState}");
                return false;
            }

            await _loadLogic.PlayLoadScreen(toFadeOutDurationMin, thisToken);
            await UniTask.Delay(TimeSpan.FromSeconds(thisTaskDelay), cancellationToken: thisToken);
            return true;
        }

        /// <summary>
        /// 指定の操作説明画面をフェードインさせる
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toFadeOutDurationMin"></param>
        public async UniTask<bool> TryFadeInPlayingInfo(PlayingInfoType type, float toFadeOutDurationMin)
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(() => IsPlayingAnyScreenFading() || _playingInfoLogic.isPlayingFade, cancellationToken: thisToken);

            //MEMO: LoadScreen以外のLoadが表示されていたら実行しない
            if (_blackFadeLogic.isFadeInState || _gameOverLogic.isFadeInState)
            {
                Debug.Log($"Already Load. Load: BlackFade:{_blackFadeLogic.isFadeInState}, gameOver:{_gameOverLogic.isFadeInState}");
                return false;
            }

            //MEMO: LoadScreenが出ていなければ実行しない
            if (!_loadLogic.isLoadingState)
            {
                Debug.Log($"Not Active LoadScreen.");
                return false;
            }

            await _playingInfoLogic.PlayPlayingInfoFadeIn(type, toFadeOutDurationMin, thisToken);
            return true;
        }

        /// <summary>
        /// ゲームオーバー画面をフェードインさせる
        /// </summary>
        /// <returns></returns>
        public async UniTask<bool> TryPlayGameOverFadeIn()
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(IsPlayingAnyScreenFading, cancellationToken: thisToken);

            //MEMO: 他のスクリーンが表示中であれば実行しない
            if (IsAnyScreenFadeState())
            {
                Debug.Log($"Already Load. Load:{_loadLogic.isLoadingState}, BlackFade:{_blackFadeLogic.isFadeInState}, gameOver:{_gameOverLogic.isFadeInState}");
                return false;
            }

            await _gameOverLogic.PlayFadeIn(thisToken);
            await _gameOverLogic.WaitOnNextKey();
            await _blackFadeLogic.PlayFadeIn(thisToken);
            SceneManager.LoadScene(SceneName.Title);
            return true;
        }

        /// <summary>
        /// 現在のフェード、Load状態に合わせてフェードアウトする
        /// </summary>
        public async UniTask<bool> TryPlayFadeOut()
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(IsPlayingAnyScreenFading, cancellationToken: thisToken);

            if (_blackFadeLogic.isFadeInState)
            {
                Debug.Log($"BlackFadeOut!");
                _gameOverLogic.SetFadeOut();
                await _blackFadeLogic.PlayBlackFadeOut(thisToken);
                return true;
            }

            if (_loadLogic.isLoadingState)
            {
                Debug.Log($"LoadFadeOut");
                await UniTask.WaitUntil(() => _loadLogic.canFinishLoading, cancellationToken: thisToken);
                await _loadLogic.PlayLoadScreenFadeOut(thisToken);
                return true;
            }
            //MEMO: ゲームオーバーはフェードアウトしないので実装無し

            Debug.Log($"Couldn`t FadeOut!");
            return false;
        }


        /// <summary>
        /// 指定の操作説明画面をフェードアウトさせる
        /// </summary>
        /// <param name="playingInfoType"></param>
        public async UniTask<bool> TryFadeOutPlayingInfo(PlayingInfoType playingInfoType)
        {
            CancellationToken thisToken = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitWhile(() => _blackFadeLogic.isPlayingFade || 
                                          _loadLogic.isPlayingFade || 
                                          !_playingInfoLogic.canFadeOut,
                cancellationToken: thisToken);

            if (_playingInfoLogic.currentPlayingInfoType == playingInfoType)
            {
                Debug.Log($"PlayingInfoFadeOut!");
                await _playingInfoLogic.TryPlayPlayingInfoFadeOut(thisToken);
                return true;
            }

            Debug.Log($"Couldn`t FadeOut!");
            return false;
        }

        /// <summary>
        /// フェード用スクリーンがどれか1つでも処理中かどうか
        /// (ゲームの操作説明画面はスクリーンではないので判定無し)
        /// </summary>
        /// <returns></returns>
        private bool IsPlayingAnyScreenFading()
        {
            return _blackFadeLogic.isPlayingFade ||
                   _loadLogic.isPlayingFade ||
                   _gameOverLogic.isPlayingFade;
        }
        
        /// <summary>
        /// フェード用スクリーンがどれか1つでも表示中かどうか
        /// (ゲームの操作説明画面はスクリーンではないので判定無し)
        /// </summary>
        /// <returns></returns>
        private bool IsAnyScreenFadeState()
        {
            return _blackFadeLogic.isFadeInState ||
                   _loadLogic.isLoadingState ||
                   _gameOverLogic.isFadeInState;
        }

        private void OnDestroy()
        {
            _loadEntity?.Dispose();
        }
    }
}