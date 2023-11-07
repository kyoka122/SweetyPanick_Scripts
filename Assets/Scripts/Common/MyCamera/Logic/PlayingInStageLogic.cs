using System.Collections.Generic;
using System.Linq;
using Common.Database.ScriptableData;
using InGame.Database;
using Common.MyCamera.Entity;
using Common.MyCamera.View;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;

namespace Common.MyCamera.Logic
{
    public class PlayingInStageLogic
    {
        private readonly CameraEntity _cameraEntity;
        private readonly MainCameraView _mainCameraView;

        private const float SignificantFiguresRate = 10000;

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

        public void LateInit()
        {
            _cameraEntity.InitTransform();
        }

        public void FixedUpdate()
        {
            Dictionary<PlayableCharacter, CharacterUpdateableInStageData> allPlayerUpdateableInStageData =
                _cameraEntity.GetCharacterUpdateableInStageData();
            SetWeightTarget(allPlayerUpdateableInStageData);
            for (int i = 0; i < allPlayerUpdateableInStageData.Count; i++)
            {
                if (!allPlayerUpdateableInStageData[(PlayableCharacter) i+1].canTargetCamera)
                {
                    continue;
                }
                Transform transform = allPlayerUpdateableInStageData[(PlayableCharacter) i+1].transform;
                float currentWeight = _mainCameraView.GetCameraTargetWeight(transform);
                int weightTmp = (int)((currentWeight + (_cameraEntity.cameraWeightTarget[transform] - currentWeight) / 100) * SignificantFiguresRate);
                
                float weight = weightTmp / SignificantFiguresRate;
                _mainCameraView.SetCameraTargetWeight(transform, MyMathf.InRange(weight,0,1));
            }
        }

        private void SetWeightTarget(Dictionary<PlayableCharacter, CharacterUpdateableInStageData> allPlayerUpdateableInStageData)
        {
            if (allPlayerUpdateableInStageData.Count==0)
            {
                return;
            }
            /*Vector3 posSum=Vector3.zero;
            foreach (var characterUpdateableInStageData in allPlayerUpdateableInStageData)
            {
                posSum += characterUpdateableInStageData.Value.transform.position;
            }*/
            //Vector3 centerPos = posSum / allPlayerUpdateableInStageData.Count;
            var moveValues=new Dictionary<Transform, float>(4);
            for (int i = 0; i < allPlayerUpdateableInStageData.Count; i++)
            {
                if (!allPlayerUpdateableInStageData[(PlayableCharacter) i+1].canTargetCamera)
                {
                    continue;
                }
                Transform transform = allPlayerUpdateableInStageData[(PlayableCharacter) i+1].transform;
                Vector3 currentPos = transform.position;
                Vector3 currentMoveVec = currentPos - _cameraEntity.GetCharacterPrevPos((PlayableCharacter) i + 1);
                moveValues.Add(transform, currentMoveVec.magnitude);
                
                _cameraEntity.SetCharacterPrevPos((PlayableCharacter) i + 1, currentPos);
            }

            float maxValue = moveValues.Max(data => data.Value);
            foreach (var moveValue in moveValues)
            {
                float rate = maxValue == 0 ? 0 : moveValue.Value / maxValue;
                float targetWeight = _mainCameraView.CinemaChineMinWeight + _mainCameraView.CinemaChineWeightRate * rate;
                _cameraEntity.SetCameraWeightTarget(moveValue.Key, targetWeight);
                //Debug.Log($"name:{moveValue.Key.gameObject.name}, moveValue:{moveValue.Value}, maxValue:{maxValue}, targetWeight:{targetWeight}");
            }
            // for (int i = 0; i < moveVecs.Count; i++)
            // {
            //     float targetWeight=0.5f + 1 - 0.5 * moveVecs[i] / maxValue;
            //     Debug.Log($"data:{moveVecs[i].Value}");
            //     Debug.Log($"type:{(PlayableCharacter) i+1}, Smallest[i]:{_mainCameraView.CinemaChineWeightOrderBySmallest[i]}");
            //     
            // }
            
            //全員のMoveVecを集めて、割合決める0.5~1
            
            // var sortedData=moveVecs.OrderBy(vec=>vec.Value.magnitude).ToArray();
            // for (int i = 0; i < sortedData.Length; i++)
            // {
            //     Debug.Log($"data:{sortedData[i].Value}");
            //     Debug.Log($"type:{(PlayableCharacter) i+1}, Smallest[i]:{_mainCameraView.CinemaChineWeightOrderBySmallest[i]}");
            //     _cameraEntity.SetCameraWeightTarget(sortedData[i].Key, _mainCameraView.CinemaChineWeightOrderBySmallest[i]);
            // }
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