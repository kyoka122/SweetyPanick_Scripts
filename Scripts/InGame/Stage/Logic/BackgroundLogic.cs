using InGame.Stage.Entity;
using InGame.Stage.View;
using UnityEngine;
using Utility;

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
            _stageBaseEntity.SetPrevCameraPos(_stageBaseEntity.CameraPos);
            Debug.Log($"_stageBaseEntity.CameraPos:{_stageBaseEntity.CameraPos}");
            //LateUpdate();
        }
        
        public void LateUpdate()
        {
            Vector2 moveVec = _stageBaseEntity.CameraPos - _stageBaseEntity.prevCameraPos;
            
            //Debug中…
            float x=MyMathf.InRange(moveVec.x, -0.5f,0.5f);
            float y = MyMathf.InRange(moveVec.y, -0.5f, 0.5f);
            Vector2 clampedVelocity = new(x, y);
            Debug.Log($"clampedVelocity:{clampedVelocity}");
            _backgroundView.SetVelocity(new Vector2(clampedVelocity.x * 20f,clampedVelocity.y));
            //Debug.Log($"moveVec:{moveVec}");
            // float newBackGroundPosX =
            //     (_stageBaseEntity.CameraPos.x - _stageBaseEntity.cameraInitPos.x) * _stageBaseEntity.BackGroundMoveRateX 
            //     + _backgroundView.initPos.x;
            // float newBackGroundPosY =
            //     (_stageBaseEntity.CameraPos.y - _stageBaseEntity.cameraInitPos.y) * _stageBaseEntity.BackGroundMoveRateY
            //     + _backgroundView.initPos.y;
            // _backgroundView.SetPosition(new Vector2(newBackGroundPosX,newBackGroundPosY));
            
            _stageBaseEntity.SetPrevCameraPos(_stageBaseEntity.CameraPos);
        }
    }
}