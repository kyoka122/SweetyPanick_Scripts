using System;
using InGame.Common.Database;
using InGame.Database;
using UniRx;
using UnityEngine;

namespace InGame.SceneLoader.Entity
{
    public class LoadEntity:IDisposable
    {
        public float BlackScreenFadeInDuration => _commonDatabase.GetSceneLoadData().BlackScreenFadeInDuration;
        public float BlackScreenFadeOutDuration => _commonDatabase.GetSceneLoadData().BlackScreenFadeOutDuration;
        public float LoadFadeInDuration => _commonDatabase.GetSceneLoadData().LoadFadeInDuration;
        public float LoadFadeOutDuration => _commonDatabase.GetSceneLoadData().LoadFadeOutDuration;
        public float LoadBackgroundFadeOutDuration => _commonDatabase.GetSceneLoadData().LoadBackgroundFadeOutDuration;
        public float BackgroundScrollSpeed => _commonDatabase.GetSceneLoadData().BackgroundScrollSpeed;
        public float WaveScrollSpeed => _commonDatabase.GetSceneLoadData().WaveScrollSpeed;
        public Vector2 MainCameraPos => _commonDatabase.GetReadOnlyCameraController().GetPosition();

        public IObservable<bool> NextKeyAnyPlayer => _onNextKeyAnyPlayer;
        
        public float loadBackGroundMoveDistance { get; private set; }
        public float waveObjMoveDistance { get; private set; }
        
        
        public float gameOverBackGroundMoveDistance { get; private set; }
        
        public float GameOverFadeInDuration => _commonDatabase.GetSceneLoadData().GameOverFadeInDuration;
        public float GameOverBGMFadeOutDuration => _commonDatabase.GetSceneLoadData().GameOverBGMFadeOutDuration;
        public float PlayingInfoFadeInDuration => _commonDatabase.GetSceneLoadData().PlayingInfoFadeInDuration;
        public float PlayingInfoFadeOutDuration => _commonDatabase.GetSceneLoadData().PlayingInfoFadeOutDuration;

        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;
        
        private readonly Subject<bool> _onNextKeyAnyPlayer;
        
        public LoadEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            _onNextKeyAnyPlayer = new Subject<bool>();
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }

        public void RegisterInputObserver()
        {
            foreach (var controllerData in _commonDatabase.GetAllControllerData())
            {
                controllerData.playerInput.Next
                    .Subscribe(_ =>
                    {
                        _onNextKeyAnyPlayer.OnNext(true);
                    });
            }
        }

        public void SetLoadBackGroundMoveDistance(float distance)
        {
            loadBackGroundMoveDistance = distance;
        }

        public void SetWaveObjMoveDistance(float distance)
        {
            waveObjMoveDistance = distance;
        }

        public void Dispose()
        {
            _onNextKeyAnyPlayer?.Dispose();
        }

        public void SetGameOverBackGroundMoveDistance(float distance)
        {
            gameOverBackGroundMoveDistance = distance;
        }
    }
}