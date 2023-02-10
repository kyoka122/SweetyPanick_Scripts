using InGame.MyCamera.Entity;
using InGame.MyCamera.View;
using MyApplication;

namespace InGame.MyCamera.Logic
{
    public class MoveStageLogic
    {
        private readonly CameraEntity _cameraEntity;
        private readonly MainCameraView _mainCameraView;

        public MoveStageLogic(CameraEntity cameraEntity, MainCameraView mainCameraView)
        {
            _cameraEntity = cameraEntity;
            _mainCameraView = mainCameraView;
        }

        public void MoveStage(StageArea area)
        {
            var cameraData = _cameraEntity.GetCameraSettingsData(area);
            if (cameraData==null)
            {
                return;
            }
            
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