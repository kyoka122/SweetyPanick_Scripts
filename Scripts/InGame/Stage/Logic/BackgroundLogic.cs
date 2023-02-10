using InGame.Stage.Entity;
using InGame.Stage.View;
using UnityEngine;

namespace InGame.Stage.Logic
{
    public class BackgroundLogic
    {
        private readonly StageBaseEntity _stageBaseEntity;
        private readonly BackgroundView _backgroundView;

        public BackgroundLogic(StageBaseEntity stageBaseEntity, BackgroundView backgroundView)
        {
            _stageBaseEntity = stageBaseEntity;
            _backgroundView = backgroundView;
        }

        public void LateInit()
        {
            _stageBaseEntity.SetCameraInitPos(_stageBaseEntity.CameraPos);
            Debug.Log($"_stageBaseEntity.CameraPos:{_stageBaseEntity.CameraPos}");
            LateUpdate();
        }
        
        public void LateUpdate()
        {
            float newBackGroundPosX =
                (_stageBaseEntity.CameraPos.x - _stageBaseEntity.cameraInitPos.x) * _stageBaseEntity.BackGroundMoveRateX 
                + _backgroundView.initPos.x;
            //Debug.Log($"_stageBaseEntity.CameraPos.y:{_stageBaseEntity.CameraPos.y}");
            Debug.Log($"_stageBaseEntity.cameraInitPos.y:{_stageBaseEntity.cameraInitPos.y}");
            Debug.Log($"vec{(_stageBaseEntity.CameraPos.y - _stageBaseEntity.cameraInitPos.y) * _stageBaseEntity.BackGroundMoveRateY}");
            //Debug.Log($"_backgroundView.initPos.y:{_backgroundView.initPos.y}");
            float newBackGroundPosY =
                (_stageBaseEntity.CameraPos.y - _stageBaseEntity.cameraInitPos.y) * _stageBaseEntity.BackGroundMoveRateY
                + _backgroundView.initPos.y;
            _backgroundView.SetPosition(new Vector2(newBackGroundPosX,newBackGroundPosY));
        }
    }
}