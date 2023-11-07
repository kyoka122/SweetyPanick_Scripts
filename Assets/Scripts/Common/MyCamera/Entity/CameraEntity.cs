using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Common.Database;
using InGame.Database;
using MyApplication;
using UniRx;
using UnityEngine;

namespace Common.MyCamera.Entity
{
    public class CameraEntity
    {
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> CandyUpdateableInStageData =>
            _inGameDatabase.CandyInStageData;

        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> MashUpdateableInStageData =>
            _inGameDatabase.MashInStageData;

        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> FuUpdateableInStageData =>
            _inGameDatabase.FuInStageData;

        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> KureUpdateableInStageData =>
            _inGameDatabase.KureInStageData;

        public int MaxPriority => 10;
        public int MinPriority => 0;

        public Dictionary<PlayableCharacter,CharacterUpdateableInStageData> GetCharacterUpdateableInStageData() =>
            _inGameDatabase.GetAllCharacterInStageData();

        public Dictionary<Transform, float> cameraWeightTarget { get; private set; }
        //public Dictionary<PlayableCharacter, bool> HadTargetGroup => _hadTargetGroup;
        
        public CameraInitData GetCameraInitData(StageArea area)=>_commonDatabase.GetCameraInitData(area);
        
        private readonly Dictionary<PlayableCharacter, bool> _hadTargetGroup;
        private readonly Dictionary<PlayableCharacter, Vector3> _characterPrevPos;
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public CameraEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _hadTargetGroup = new Dictionary<PlayableCharacter, bool>(4);
            _characterPrevPos = new Dictionary<PlayableCharacter, Vector3>(4);
            cameraWeightTarget = new Dictionary<Transform, float>(4);
            var playableCharacterArray = Enum.GetValues(typeof(PlayableCharacter));
            Debug.LogWarning($"playableCharacterArray.Length:{playableCharacterArray.Length}");
            for (int i = 1; i < playableCharacterArray.Length; i++)
            {
                _hadTargetGroup.Add((PlayableCharacter) i, false);
            }
        }

        public void InitTransform()
        {
            foreach (var inStageData in _inGameDatabase.GetAllCharacterInStageData())
            {
                _characterPrevPos.Add(inStageData.Key, inStageData.Value.transform.position);
            }
        }

        public bool GetHadTargetGroup(PlayableCharacter characterType)
        {
            return _hadTargetGroup[characterType];
        }
        
        public void SetCharacterHadTargetGroup(PlayableCharacter characterType)
        {
            _hadTargetGroup[characterType] = true;
        }
        
        public void RemoveCharacterHadTargetGroup(PlayableCharacter characterType)
        {
            _hadTargetGroup[characterType] = false;
        }

        public void SetCharacterPrevPos(PlayableCharacter characterType,Vector3 moveVec)
        {
            _characterPrevPos[characterType] = moveVec;
        }

        public Vector3 GetCharacterPrevPos(PlayableCharacter characterType)
        {
            return _characterPrevPos[characterType];
        }

        public void SetCameraWeightTarget(Transform transform,float value)
        {
            cameraWeightTarget[transform] = value;
        }
    }
}