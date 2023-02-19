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
        }
        
        public void LateUpdate()
        {
            Vector2 moveVec = _stageBaseEntity.CameraPos - _stageBaseEntity.prevCameraPos;
            
            //Debug中…
            float x=MyMathf.InRange(moveVec.x, -0.3f,0.3f);
            float y = MyMathf.InRange(moveVec.y, -0.4f, 0.4f);
            
            Vector2 clampedVelocity = new(x, y);
            Vector2 backGroundVelocity =
                new Vector2(clampedVelocity.x * 20f, clampedVelocity.y)*2f * Time.deltaTime;
            _backgroundView.SetVelocity(backGroundVelocity);
            _stageBaseEntity.SetPrevCameraPos(_stageBaseEntity.CameraPos);
        }
    }
}