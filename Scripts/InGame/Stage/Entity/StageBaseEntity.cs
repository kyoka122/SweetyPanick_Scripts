using InGame.Common.Database;
using InGame.Database;
using UnityEngine;

namespace InGame.Stage.Entity
{
    /// <summary>
    /// ステージの基本データ
    /// </summary>
    public class StageBaseEntity
    {
        public Vector2 CameraPos => _commonDatabase.GetReadOnlyCameraController().GetPosition();
        public Vector2 prevCameraPos { get; private set; }
        public Vector2 cameraInitPos { get; private set; }
        public float BackGroundMoveRateX => _inGameDatabase.GetStageSettings().BackGroundMoveRateX;
        public float BackGroundMoveRateY => _inGameDatabase.GetStageSettings().BackGroundMoveRateY;
        
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public StageBaseEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }

        public void SetCameraInitPos(Vector2 pos)
        {
            cameraInitPos = pos;
        }

        public void SetPrevCameraPos(Vector2 newPrevCameraPos)
        {
            prevCameraPos = newPrevCameraPos;
        }
    }
}