using InGame.Database;
using InGame.MyCamera.Entity;
using InGame.MyCamera.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.MyCamera.Logic
{
    public class PlayingInStageLogic
    {
        private readonly CameraEntity _cameraEntity;
        private readonly MainCameraView _mainCameraView;

        public PlayingInStageLogic(CameraEntity cameraEntity, MainCameraView mainCameraView)
        {
            _cameraEntity = cameraEntity;
            _mainCameraView = mainCameraView;
            SetInDatabase();
            RegisterObserver();
        }

        private void SetInDatabase()
        {
            _cameraEntity.SetCameraFunctionInDatabase(_mainCameraView);
        }

        private void RegisterObserver()
        {
            _cameraEntity.candyUpdateableInStageData
                .Subscribe(property=>TrySetTargetGroup(property,PlayableCharacter.Candy))
                .AddTo(_mainCameraView);
            
            _cameraEntity.fuUpdateableInStageData
                .Subscribe(property=>TrySetTargetGroup(property,PlayableCharacter.Fu))
                .AddTo(_mainCameraView);
            
            _cameraEntity.mashUpdateableInStageData
                .Subscribe(property=>TrySetTargetGroup(property,PlayableCharacter.Mash))
                .AddTo(_mainCameraView);
            
            _cameraEntity.kureUpdateableInStageData
                .Subscribe(property=>TrySetTargetGroup(property,PlayableCharacter.Kure))
                .AddTo(_mainCameraView);
        }

        private void TrySetTargetGroup(CharacterUpdateableInStageData property,PlayableCharacter characterType)
        {
            if (property==null)
            {
                return;
            }
            if (property.transform==null)
            {
                return;
            }
            /*if (_cameraEntity.GetHadTargetGroup(characterType))
            {
                _cameraView.ChangeWeight(property.transform, property.nearnessFromTargetView);
                return;
            }*/

            if (_mainCameraView.TryAddTargetGroup(property.transform))
            {
                _cameraEntity.SetCharacterHadTargetGroup(characterType);
            }
            
        }

    }
}