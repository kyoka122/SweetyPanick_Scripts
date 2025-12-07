using System.Threading;
using Cysharp.Threading.Tasks;
using KanKikuchi.AudioManager;
using Loader.Entity;
using Loader.View;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Loader.Logic
{
    /// <summary>
    /// ゲームオーバーを表示して遷移する時のLogic
    /// </summary>
    public class GameOverLogic
    {
        public bool isFadeInState { get; private set; }
        public bool isPlayingFade { get; private set; }

        private readonly LoadEntity _loadEntity;
        private readonly GameOverView _gameOverView;
        private readonly LoadCameraView _loadCameraView;
        private CancellationTokenSource _tokenSource;

        public GameOverLogic(LoadEntity loadEntity, GameOverView gameOverView, LoadCameraView loadCameraView)
        {
            _loadEntity = loadEntity;
            _gameOverView = gameOverView;
            _loadCameraView = loadCameraView;
            InitViewData();
        }

        /// <summary>
        /// フェードイン演出を開始する
        /// </summary>
        public async UniTask PlayFadeIn(CancellationToken token)
        {
            isPlayingFade = true;
            _gameOverView.InitOnFadeIn();
            _gameOverView.SetActive(true);
            _loadCameraView.SetActive(true);
            BGMManager.Instance.FadeOut(_loadEntity.GameOverBGMFadeOutDuration);
            await _gameOverView.FadeIn(_loadEntity.GameOverFadeInDuration, token);
            SEManager.Instance.Play(SEPath.GAME_OVER);
            _tokenSource = new CancellationTokenSource();
            StartUpdateScrollAnimation();
            isFadeInState = true;
            isPlayingFade = false;
        }

        /// <summary>
        /// フェードアウト状態に一瞬で切り替える（別スクリーンで隠れている間に実行）
        /// </summary>
        public void SetFadeOut()
        {
            isPlayingFade = true;
            _gameOverView.SetFadeOut();
            _gameOverView.SetActive(false);
            isFadeInState = false;
            isPlayingFade = false;
        }

        /// <summary>
        /// 次に進むためのキー入力がされるまで待機する
        /// </summary>
        public async UniTask WaitOnNextKey()
        {
            _loadEntity.RegisterInputObserver();
            await _loadEntity.NextKeyAnyPlayer.ToUniTask(true, _gameOverView.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// ロード中のスクロールアニメーション
        /// </summary>
        private void StartUpdateScrollAnimation()
        {
            _gameOverView.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    UpdateLeftScroll(_gameOverView.BackgroundRectTransform, _gameOverView.backGroundXDefaultPosition,
                        _loadEntity.gameOverBackGroundMoveDistance, _loadEntity.BackgroundScrollSpeed);
                }).AddTo(_tokenSource.Token);
        }

        private void InitViewData()
        {
            _loadEntity.SetGameOverBackGroundMoveDistance(_gameOverView.BackgroundRectTransform.sizeDelta.x - _loadCameraView.cameraWidth);
        }

        /// <summary>
        /// 指定のゲームオブジェクトをスクロールする。
        /// Update関数で呼び出すこと。
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="defaultPosX"></param>
        /// <param name="width"></param>
        /// <param name="speed"></param>
        private void UpdateLeftScroll(RectTransform rectTransform, float defaultPosX, float width, float speed)
        {
            rectTransform.Translate(-speed * Time.deltaTime, 0, 0);
            if (defaultPosX - rectTransform.localPosition.x > width)
            {
                Vector3 prevPos = rectTransform.localPosition;
                rectTransform.localPosition = new Vector3(defaultPosX, prevPos.y, prevPos.z);
            }
        }
    }
}