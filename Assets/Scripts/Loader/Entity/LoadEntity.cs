using System;
using InGame.Common.Database;
using InGame.Database;
using UniRx;
using UnityEngine;

namespace Loader.Entity
{
    /// <summary>
    /// 画面遷移時のパラメータ等、情報が集約されたクラス
    /// </summary>
    public class LoadEntity : IDisposable
    {
        public float BlackScreenFadeInDuration => _commonDatabase.GetSceneLoadData().BlackScreenFadeInDuration;
        public float BlackScreenFadeOutDuration => _commonDatabase.GetSceneLoadData().BlackScreenFadeOutDuration;
        public float LoadFadeInDuration => _commonDatabase.GetSceneLoadData().LoadFadeInDuration;
        public float LoadFadeOutDuration => _commonDatabase.GetSceneLoadData().LoadFadeOutDuration;
        public float LoadBackgroundFadeOutDuration => _commonDatabase.GetSceneLoadData().LoadBackgroundFadeOutDuration;
        public float BackgroundScrollSpeed => _commonDatabase.GetSceneLoadData().BackgroundScrollSpeed;
        public float WaveScrollSpeed => _commonDatabase.GetSceneLoadData().WaveScrollSpeed;
        public Vector2 MainCameraPos => _commonDatabase.GetReadOnlyCameraController().GetPosition();

        public float GameOverFadeInDuration => _commonDatabase.GetSceneLoadData().GameOverFadeInDuration;
        public float GameOverBGMFadeOutDuration => _commonDatabase.GetSceneLoadData().GameOverBGMFadeOutDuration;
        public float PlayingInfoFadeInDuration => _commonDatabase.GetSceneLoadData().PlayingInfoFadeInDuration;
        public float PlayingInfoFadeOutDuration => _commonDatabase.GetSceneLoadData().PlayingInfoFadeOutDuration;
        public IObservable<bool> NextKeyAnyPlayer => _onNextKeyAnyPlayer;

        public float loadBackGroundMoveDistance { get; private set; }
        public float waveObjMoveDistance { get; private set; }
        public float gameOverBackGroundMoveDistance { get; private set; }

        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        private readonly Subject<bool> _onNextKeyAnyPlayer;

        public LoadEntity(InGameDatabase inGameDatabase, CommonDatabase commonDatabase)
        {
            _onNextKeyAnyPlayer = new Subject<bool>();
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }

        /// <summary>
        /// プレイヤーのボタン入力を待つための購読処理登録
        /// </summary>
        public void RegisterInputObserver()
        {
            foreach (var controllerData in _commonDatabase.GetAllControllerData())
            {
                controllerData.playerInput.Next.Subscribe(_ => _onNextKeyAnyPlayer.OnNext(true));
            }
        }

        /// <summary>
        /// Load画面の背景スクロールを初期位置に戻すときの位置設定
        /// </summary>
        /// <param name="distance"></param>
        public void SetLoadBackGroundMoveDistance(float distance)
        {
            loadBackGroundMoveDistance = distance;
        }

        /// <summary>
        /// Load画面の部品スクロールを初期位置に戻すときの位置設定
        /// </summary>
        /// <param name="distance"></param>
        public void SetWaveObjMoveDistance(float distance)
        {
            waveObjMoveDistance = distance;
        }

        /// <summary>
        /// ゲームオーバー画面のスクロールを初期位置に戻す時の位置設定
        /// </summary>
        /// <param name="distance"></param>
        public void SetGameOverBackGroundMoveDistance(float distance)
        {
            gameOverBackGroundMoveDistance = distance;
        }

        public void Dispose()
        {
            _onNextKeyAnyPlayer?.Dispose();
        }
    }
}