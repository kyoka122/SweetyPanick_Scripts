using System;
using InGame.Common.Database.ScriptableData;
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
            RegisterTargetObserver();
    
            foreach (var inStageData in _cameraEntity.GetCharacterUpdateableInStageData())
            {
                TrySetTargetGroup(inStageData.Value, inStageData.Key);
            }
        }

        private void RegisterTargetObserver()
        {
            _cameraEntity.CandyUpdateableInStageData
                .Subscribe(property=> TrySetTargetGroup(property, PlayableCharacter.Candy))
                .AddTo(_mainCameraView);
            
            _cameraEntity.FuUpdateableInStageData
                .Subscribe(property=> TrySetTargetGroup(property, PlayableCharacter.Fu))
                .AddTo(_mainCameraView);
            
            _cameraEntity.MashUpdateableInStageData
                .Subscribe(property=> TrySetTargetGroup(property, PlayableCharacter.Mash))
                .AddTo(_mainCameraView);
            
            _cameraEntity.KureUpdateableInStageData
                .Subscribe(property=> TrySetTargetGroup(property, PlayableCharacter.Kure))
                .AddTo(_mainCameraView);
        }
        

        private void TrySetTargetGroup(CharacterUpdateableInStageData property,PlayableCharacter characterType)
        {
            if (property.transform==null)
            {
                return;
            }

            TryAddTargetGroup(property, characterType);
            TryRemoveTargetGroup(property, characterType);
        }

        private void TryAddTargetGroup(CharacterUpdateableInStageData property,PlayableCharacter characterType)
        {
            if (property.canTargetCamera && !_cameraEntity.GetHadTargetGroup(characterType))
            {
                if (_mainCameraView.TryAddTargetGroup(property.transform))
                {
                    _cameraEntity.SetCharacterHadTargetGroup(characterType);
                }
            }
        }

        private void TryRemoveTargetGroup(CharacterUpdateableInStageData property,PlayableCharacter characterType)
        {
            if (!property.canTargetCamera && _cameraEntity.GetHadTargetGroup(characterType))
            {
                if (_mainCameraView.TryRemoveTargetGroup(property.transform))
                {
                    _cameraEntity.RemoveCharacterHadTargetGroup(characterType);
                }
            }
        }
        
        public void SetCameraData(CameraData cameraData)
        {
            _mainCameraView.DoMoveXYPosition(cameraData.Position,cameraData.MoveDuration);
            _mainCameraView.DoSize(cameraData.Size,cameraData.MoveDuration);
        }
    }
}