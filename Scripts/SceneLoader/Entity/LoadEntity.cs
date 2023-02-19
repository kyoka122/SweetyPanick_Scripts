using InGame.Common.Database;
using InGame.Database;
using UnityEngine;

namespace InGame.SceneLoader.Entity
{
    public class LoadEntity
    {
        public float BlackScreenFadeInDuration => _inGameDatabase.GetSceneLoadData().BlackScreenFadeInDuration;
        public float BlackScreenFadeOutDuration => _inGameDatabase.GetSceneLoadData().BlackScreenFadeOutDuration;
        public float LoadFadeInDuration => _inGameDatabase.GetSceneLoadData().LoadFadeInDuration;
        public float LoadFadeOutDuration => _inGameDatabase.GetSceneLoadData().LoadFadeOutDuration;
        public float LoadBackgroundFadeOutDuration => _inGameDatabase.GetSceneLoadData().LoadBackgroundFadeOutDuration;
        public float BackgroundScrollSpeed => _inGameDatabase.GetSceneLoadData().BackgroundScrollSpeed;
        public float WaveScrollSpeed => _inGameDatabase.GetSceneLoadData().WaveScrollSpeed;
        public Vector2 MainCameraPos => _commonDatabase.GetReadOnlyCameraController().GetPosition();
        
        public float LoadBackGroundMoveDistance { get; private set; }
        public float WaveObjMoveDistance { get; private set; }
        
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;
        
        public LoadEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }

        public void SetLoadBackGroundMoveDistance(float distance)
        {
            LoadBackGroundMoveDistance = distance;
        }

        public void SetWaveObjMoveDistance(float distance)
        {
            WaveObjMoveDistance = distance;
        }
    }
}