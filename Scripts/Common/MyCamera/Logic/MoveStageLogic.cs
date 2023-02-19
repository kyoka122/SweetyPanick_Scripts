using Cinemachine;
using InGame.MyCamera.Entity;
using InGame.MyCamera.View;
using MyApplication;

namespace InGame.MyCamera.Logic
{
    public class MoveStageLogic
    {
        private readonly CameraEntity _cameraEntity;
        private readonly MainCameraView _mainCameraView;
        private readonly CinemachineConfiner2D _cinemachineConfiner2D;

        public MoveStageLogic(CameraEntity cameraEntity, MainCameraView mainCameraView)
        {
            _cameraEntity = cameraEntity;
            _mainCameraView = mainCameraView;
        }
        public MoveStageLogic(CameraEntity cameraEntity, MainCameraView mainCameraView,CinemachineConfiner2D cinemachineConfiner2D)
        {
            _cameraEntity = cameraEntity;
            _mainCameraView = mainCameraView;
            _cinemachineConfiner2D = cinemachineConfiner2D;
        }

        public void MoveStage(StageArea area)
        {
            var cameraData = _cameraEntity.GetCameraSettingsData(area);
            if (cameraData==null)
            {
                return;
            }

            _cinemachineConfiner2D.m_BoundingShape2D = cameraData.StageAreaCollider;
            _mainCameraView.SetPosition(cameraData.InitPosition);
            //_cameraView.SetSize();

            
            //MEMO: 現在、既にシーン上に配置してあるカメラ自体に設定がなされているので、変更する必要なし。途中変更などが必要になればコメントアウトを解除する
            // switch (cameraData.CameraMode)
            // {
            //     case CameraMode.None:
            //         break;
            //     case CameraMode.Chase:
            //         break;
            //     case CameraMode.Freeze:
            //         break;
            //     default:
            //         Debug.LogError($"ArgumentOutOfRangeException. cameraMode:{cameraData.CameraMode}");
            //         break;
            // }
        }
    }
}