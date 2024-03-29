﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.SceneLoader.Entity;
using InGame.SceneLoader.View;
using KanKikuchi.AudioManager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.SceneLoader.Logic
{
    public class GameOverLogic
    {
        public bool isFadeIn { get; private set; }
        public bool isFading { get; private set; }
        
        private readonly LoadEntity _loadEntity;
        private readonly GameOverView _gameOverView;
        private readonly LoadCameraView _loadCameraView;
        private CancellationTokenSource _tokenSource;

        public GameOverLogic(LoadEntity loadEntity,GameOverView gameOverView,LoadCameraView loadCameraView)
        {
            _loadEntity = loadEntity;
            _gameOverView = gameOverView;
            _loadCameraView = loadCameraView;
            SetInitViewData();
        }

        /// <summary>
        /// フェードイン演出を開始する
        /// </summary>
        public async UniTask PlayFadeIn(CancellationToken token)
        {
            isFading = true;
            _gameOverView.InitOnFadeIn();
            _gameOverView.SetActive(true);
            _loadCameraView.SetActive(true);
            BGMManager.Instance.FadeOut(_loadEntity.GameOverBGMFadeOutDuration);
            await _gameOverView.FadeIn(_loadEntity.GameOverFadeInDuration,token);
            SEManager.Instance.Play(SEPath.GAME_OVER);
            _tokenSource = new CancellationTokenSource();
            StartUpdateScrollAnimation();
            isFadeIn = true;
            isFading = false;
        }

        /// <summary>
        /// フェードアウト状態に切り替える（別スクリーンで隠れている間に実行）
        /// </summary>
        public void TrySetFadeOut()
        {
            isFading = true;
            _gameOverView.SetFadeOut();
            _gameOverView.SetActive(false);
            //_loadCameraView.SetActive(false);//MEMO: カメラは別スクリーンでfalseにする
            isFadeIn = false;
            isFading = false;
        }
        
        public async UniTask WaitOnNextKey()
        {
            _loadEntity.RegisterInputObserver();
            await _loadEntity.NextKeyAnyPlayer.ToUniTask(true,cancellationToken:_gameOverView.GetCancellationTokenOnDestroy());
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

        private void SetInitViewData()
        {
            _loadEntity.SetGameOverBackGroundMoveDistance(_gameOverView.BackgroundRectTransform.sizeDelta.x -
                                                          _loadCameraView.cameraWidth);
        }
        
        /// <summary>
        /// ゲームオブジェクトをスクロールする。Update関数で呼び出すこと
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="defaultPosX"></param>
        /// <param name="width"></param>
        /// <param name="speed"></param>
        private void UpdateLeftScroll(RectTransform rectTransform,float defaultPosX,float width,float speed)
        {
            rectTransform.Translate(-speed * Time.deltaTime, 0, 0);
            if (defaultPosX - rectTransform.localPosition.x > width)
            {
                rectTransform.localPosition = new Vector3(defaultPosX, rectTransform.localPosition.y,
                    rectTransform.localPosition.z);
            }
        }
    }
}