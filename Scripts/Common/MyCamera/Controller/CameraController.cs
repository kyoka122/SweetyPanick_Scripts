using InGame.MyCamera.Logic;
using MyApplication;

namespace InGame.MyCamera.Controller
{
    public class CameraController
    {
        private readonly PlayingInStageLogic _playingInStageLogic;
        private readonly MoveStageLogic _moveStageLogic;

        public CameraController(PlayingInStageLogic playingInStageLogic,MoveStageLogic moveStageLogic)
        {
            _playingInStageLogic = playingInStageLogic;
            _moveStageLogic = moveStageLogic;
        }

        public void SetCameraMoveState(StageArea area)
        {
            _moveStageLogic.MoveStage(area);
        }
    }
}