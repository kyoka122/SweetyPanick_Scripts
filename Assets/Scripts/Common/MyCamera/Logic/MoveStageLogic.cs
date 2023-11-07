using Cinemachine;
using Common.MyCamera.Entity;
using Common.MyCamera.View;
using MyApplication;
using UnityEngine;

namespace Common.MyCamera.Logic
{
    public class MoveStageLogic
    {
        private readonly CameraEntity _cameraEntity;
        private readonly MainCameraView _mainCameraView;
        private readonly CinemachineConfiner2D _cinemachineConfiner2D;
        private readonly bool _haveCinemachineConfinerInStage;

        public MoveStageLogic(CameraEntity cameraEntity, MainCameraView mainCameraView)
        {
            _cameraEntity = cameraEntity;
            _mainCameraView = mainCameraView;
            _haveCinemachineConfinerInStage = false;
        }
        public MoveStageLogic(CameraEntity cameraEntity, MainCameraView mainCameraView,CinemachineConfiner2D cinemachineConfiner2D)
        {
            _cameraEntity = cameraEntity;
            _mainCameraView = mainCameraView;
            _cinemachineConfiner2D = cinemachineConfiner2D;
            _haveCinemachineConfinerInStage = true;
        }

        public void MoveStage(StageArea area)
        {
            var cameraData = _cameraEntity.GetCameraInitData(area);
            if (cameraData==null)
            {
                Debug.LogWarning($"Not Find CameraData. area:{area}");
                return;
            }

            if (_haveCinemachineConfinerInStage)
            {
                _cinemachineConfiner2D.m_BoundingShape2D = cameraData.StageAreaCompositeCollider;
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