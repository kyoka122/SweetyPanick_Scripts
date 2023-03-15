using InGame.Common.Database;
using InGame.Database;
using MyApplication;
using UnityEngine;

namespace InGame.Stage.Entity
{
    /// <summary>
    /// ステージの基本データ
    /// </summary>
    public class StageBaseEntity
    {
        public Vector2 CameraPos => _commonDatabase.GetReadOnlyCameraController().GetPosition();
        public Vector2 CameraSize => _commonDatabase.GetReadOnlyCameraController().GetSize();
        public Transform CameraTransform => _commonDatabase.GetCameraEvent().GetCameraTransform();
        public float BackGroundMoveRateX => _inGameDatabase.GetStageSettings().BackGroundMoveRateX;
        public float BackGroundMoveRateY => _inGameDatabase.GetStageSettings().BackGroundMoveRateY;
        public BoxCollider2D StageAreaCollider2D(StageArea area) => _commonDatabase.GetCameraInitData(area)
            .StageAreaCollider;

        public CameraInitData GetCameraData(StageArea newStageArea) => _commonDatabase.GetCameraInitData(newStageArea);
        
        public BoxCollider2D stageRangeCollider2D { get; private set; }
        
        public Vector2 prevCameraPos { get; private set; }
        public Vector2 cameraInitPos { get; private set; }
        public Rect stageRangeRect{ get; private set; }
        public Rect backGroundRangeRect{ get; private set; }
        public Rect cameraRangeRect { get; private set; }
        
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

        public void SetStageRangeRect(Rect range)
        {
            stageRangeRect = range;
        }

        public void SetBackGroundRangeRect(Rect range)
        {
            backGroundRangeRect = range;
        }

        public void SetCameraRangeRect(Rect cameraRect)
        {
            cameraRangeRect = cameraRect;
        }

        public void SetBackGroundRangeCollider(BoxCollider2D backGroundRangeCollider2D)
        {
            stageRangeCollider2D = backGroundRangeCollider2D;
        }
    }
}