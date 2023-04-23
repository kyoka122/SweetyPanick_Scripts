using System.Linq;
using Cinemachine;
using InGame.Common.Database;
using InGame.Database;
using InGame.MyCamera.Controller;
using InGame.MyCamera.Entity;
using InGame.MyCamera.Logic;
using InGame.MyCamera.View;
using UnityEngine;

namespace InGame.MyCamera.Installer
{
    public class CameraInstaller:MonoBehaviour
    {
        /// <summary>
        /// キャラクター追跡をしたり、カメラの設定が切り替わる場合、かつシーン内でステージを移動する場合ののInstaller
        /// </summary>
        /// <param name="inGameDatabase"></param>
        /// <param name="commonDatabase"></param>
        /// <param name="targetGroup"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public CameraController InstallMoveAndMultiStageCamera(InGameDatabase inGameDatabase,CommonDatabase commonDatabase,
            CinemachineTargetGroup targetGroup,CinemachineImpulseSource cinemachineImpulseSource,Camera camera,
            CinemachineConfiner2D cinemachineConfiner2D)
        {
            var cameraEntity = new CameraEntity(inGameDatabase,commonDatabase);
            
            var cameraView = FindObjectOfType<MainCameraView>();
            cameraView.Init(targetGroup,cinemachineImpulseSource,camera);
            
            var subCameraViews = FindObjectsOfType<SubCameraView>();
            foreach (var subCameraView in subCameraViews)
            {
                subCameraView.Init();
            }
            
            var playingInStageLogic = new PlayingInStageLogic(cameraEntity,cameraView);
            var moveStageLogic = new MoveStageLogic(cameraEntity, cameraView,cinemachineConfiner2D);
            var switchCameraLogic = new SwitchCameraLogic(cameraEntity,
                subCameraViews.OrderBy(view => view.LeftEdge).ToArray());
            
            var cameraController = new CameraController(playingInStageLogic,moveStageLogic,switchCameraLogic);
            
            //MEMO: 複数のCameraViewが必要になった際はDatabaseの構造を変える
            commonDatabase.SetReadOnlyCameraFunction(cameraView);
            commonDatabase.SetCameraEvent(cameraView);
            
            return cameraController;
        }
        
        /// <summary>
        /// キャラクター追跡をしたり、カメラの設定が切り替わる場合のInstaller
        /// </summary>
        /// <param name="inGameDatabase"></param>
        /// <param name="commonDatabase"></param>
        /// <param name="targetGroup"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public CameraController InstallMoveCamera(InGameDatabase inGameDatabase,CommonDatabase commonDatabase,
            CinemachineTargetGroup targetGroup,CinemachineImpulseSource cinemachineImpulseSource,Camera camera)
        {
            var cameraEntity = new CameraEntity(inGameDatabase,commonDatabase);
            
            var cameraView = FindObjectOfType<MainCameraView>();
            cameraView.Init(targetGroup,cinemachineImpulseSource,camera);
            
            var subCameraViews = FindObjectsOfType<SubCameraView>();
            foreach (var subCameraView in subCameraViews)
            {
                subCameraView.Init();
            }
            
            var playingInStageLogic = new PlayingInStageLogic(cameraEntity,cameraView);
            var moveStageLogic = new MoveStageLogic(cameraEntity, cameraView);
            var switchCameraLogic = new SwitchCameraLogic(cameraEntity,
                subCameraViews.OrderBy(view => view.LeftEdge).ToArray());
            
            var cameraController = new CameraController(playingInStageLogic,moveStageLogic,switchCameraLogic);
            
            //MEMO: 複数のCameraViewが必要になった際はDatabaseの構造を変える
            commonDatabase.SetReadOnlyCameraFunction(cameraView);
            commonDatabase.SetCameraEvent(cameraView);
            
            return cameraController;
        }
        
        /// <summary>
        /// キャラクター追跡がない場合のInstaller
        /// </summary>
        /// <param name="inGameDatabase"></param>
        /// <param name="commonDatabase"></param>
        /// <param name="camera"></param>
        public CameraController InstallMoveCamera(InGameDatabase inGameDatabase,CommonDatabase commonDatabase,Camera camera)
        {
            var cameraView = FindObjectOfType<MainCameraView>();
            cameraView.Init(camera);
            
            var subCameraViews = FindObjectsOfType<SubCameraView>();
            foreach (var subCameraView in subCameraViews)
            {
                subCameraView.Init();
            }

            var cameraEntity = new CameraEntity(inGameDatabase,commonDatabase);
            var playingInStageLogic = new PlayingInStageLogic(cameraEntity,cameraView);
            
            var cameraController = new CameraController(playingInStageLogic);
            
            //MEMO: 複数のCameraViewが必要になった際はDatabaseの構造を変える
            commonDatabase.SetReadOnlyCameraFunction(cameraView);
            commonDatabase.SetCameraEvent(cameraView);

            return cameraController;
        }
        
        /// <summary>
        /// ゲーム中完全にカメラ固定の場合のInstaller (UIのみのシーンなど)
        /// </summary>
        /// <param name="inGameDatabase"></param>
        /// <param name="commonDatabase"></param>
        /// <param name="camera"></param>
        public CameraController InstallConstCamera(InGameDatabase inGameDatabase,CommonDatabase commonDatabase,Camera camera)
        {
            var cameraView = FindObjectOfType<MainCameraView>();
            cameraView.Init(camera);
            
            var cameraController = new CameraController();
            
            //MEMO: 複数のCameraViewが必要になった際はDatabaseの構造を変える
            commonDatabase.SetReadOnlyCameraFunction(cameraView);
            commonDatabase.SetCameraEvent(cameraView);

            return cameraController;
        }
    }
}