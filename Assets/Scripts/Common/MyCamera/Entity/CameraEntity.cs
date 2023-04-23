using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.MyCamera.Entity
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
        //public Dictionary<PlayableCharacter, bool> HadTargetGroup => _hadTargetGroup;
        
        public CameraInitData GetCameraInitData(StageArea area)=>_commonDatabase.GetCameraInitData(area);
        
        private readonly Dictionary<PlayableCharacter, bool> _hadTargetGroup;
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public CameraEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _hadTargetGroup = new Dictionary<PlayableCharacter, bool>();
            var playableCharacterArray = Enum.GetValues(typeof(PlayableCharacter));
            for (int i = 0; i < playableCharacterArray.Length; i++)
            {
                _hadTargetGroup.Add((PlayableCharacter) i, false);
            }
        }
        
        /*public void SetCameraFunctionInDatabase(IReadOnlyCameraFunction readOnlyFunction)
        {
            _commonDatabase.SetReadOnlyCameraFunction(readOnlyFunction);
            _commonDatabase.SetReadOnlyCameraFunction();
        }*/

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


    }
}