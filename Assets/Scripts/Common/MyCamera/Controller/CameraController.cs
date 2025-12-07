using Common.Database.ScriptableData;
using Common.MyCamera.Logic;
using MyApplication;
using UnityEngine;

namespace Common.MyCamera.Controller
{
    public class CameraController
    {
        private readonly PlayingInStageLogic _playingInStageLogic;
        private readonly MoveStageLogic _moveStageLogic;
        private readonly SwitchCameraLogic _switchCameraLogic;

        public CameraController(PlayingInStageLogic playingInStageLogic,MoveStageLogic moveStageLogic,
            SwitchCameraLogic switchCameraLogic)
        {
            _playingInStageLogic = playingInStageLogic;
            _moveStageLogic = moveStageLogic;
            _switchCameraLogic = switchCameraLogic;
        }
        
        public CameraController(PlayingInStageLogic playingInStageLogic)
        {
            _playingInStageLogic = playingInStageLogic;
        }
        
        public CameraController()
        {
        }

        public void LateInit()
        {
            _playingInStageLogic.LateInit();
        }
        
        public void FixedUpdate()
        {
            _switchCameraLogic.UpdateCameraPriority();
            _playingInStageLogic.FixedUpdate();
        }

        public void SetCameraMoveState(StageArea area)
        {
            if (_moveStageLogic == null||_playingInStageLogic==null)
            {
                Debug.LogError($"Miss Instance CameraController");
                return;
            }
            _moveStageLogic.MoveStage(area);
        }

        public void SetCameraData(CameraData battleCameraData)
        {
            _playingInStageLogic.SetCameraData(battleCameraData);
        }
    }
}