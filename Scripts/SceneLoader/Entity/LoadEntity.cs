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
        
        public float LoadBackGroundMoveDistance { get; private set; }
        public float WaveObjMoveDistance { get; private set; }
        public float GameOverFadeInDuration => _commonDatabase.GetSceneLoadData().GameOverFadeInDuration;

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
            LoadBackGroundMoveDistance = distance;
        }

        public void SetWaveObjMoveDistance(float distance)
        {
            WaveObjMoveDistance = distance;
        }

        public void Dispose()
        {
            _onNextKeyAnyPlayer?.Dispose();
        }
    }
}