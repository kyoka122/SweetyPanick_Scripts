using System;
using InGame.Common.Database;
using InGame.Database;
using UniRx;
using UnityEngine;

namespace InGame.Stage.Entity
{
    public class StageGimmickEntity
    {
        public float MoveFloorSpeed => _inGameDatabase.GetStageGimmickData().MoveFloorSpeed;
        public Vector3 CrepeCameraShakeVelocity => _inGameDatabase.GetStageGimmickData().CrepeCameraShakeVelocity;
        public Action<Vector3> CameraShakeEvent => _commonDatabase.GetCameraEvent().ShakeWithVelocity;
   
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public StageGimmickEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }
        
    }
}