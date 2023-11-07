using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Database;
using Common.MyCamera.Entity;
using Common.MyCamera.View;
using MyApplication;
using UnityEngine;

namespace Common.MyCamera.Logic
{
    public class SwitchCameraLogic
    {
        private readonly CameraEntity _cameraEntity;
        private readonly SubCameraView[] _subCameraViews;
        
        public SwitchCameraLogic(CameraEntity cameraEntity, SubCameraView[] subCameraViews)
        {
            _cameraEntity = cameraEntity;
            _subCameraViews = subCameraViews;
        }

        public void UpdateCameraPriority()
        {
            var characterUpdateableInStageData =_cameraEntity.GetCharacterUpdateableInStageData();
            if (characterUpdateableInStageData==null)
            {
                return;
            }
            var headPosX = characterUpdateableInStageData
                .Where(data=>data.Value.canTargetCamera)
                .Select(data => data.Value.transform)
                .Where(transformData => transformData!=null)
                .Max(transformData => transformData.position.x);
            SetPriority(headPosX);
        }
        
        private void SetPriority(float posX)
        {
            SubCameraView topPriorityView=_subCameraViews.
                FirstOrDefault(view => posX > view.LeftEdge && posX < view.RightEdge);
            
            foreach (var subCameraView in _subCameraViews)
            {
                if (subCameraView==topPriorityView)
                {
                    subCameraView.SetPriority(_cameraEntity.MaxPriority);
                    continue;
                }
                
                subCameraView.SetPriority(_cameraEntity.MinPriority);
            }
        }
    }
}