using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using MyApplication;
using UniRx;

namespace InGame.MyCamera.Entity
{
    public class CameraEntity
    {
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> candyUpdateableInStageData;
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> mashUpdateableInStageData;
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> fuUpdateableInStageData;
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> kureUpdateableInStageData;
        
        public Dictionary<PlayableCharacter, bool> HadTargetGroup => _hadTargetGroup;
        
        public CameraData GetCameraSettingsData(StageArea area)=>_commonDatabase.GetCameraSettingsData(area);
        
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
            GetObserver();
        }

        private void GetObserver()
        {
            candyUpdateableInStageData = _inGameDatabase.GetCharacterInStageDataObserver(PlayableCharacter.Candy);
            mashUpdateableInStageData = _inGameDatabase.GetCharacterInStageDataObserver(PlayableCharacter.Mash);
            fuUpdateableInStageData = _inGameDatabase.GetCharacterInStageDataObserver(PlayableCharacter.Fu);
            kureUpdateableInStageData = _inGameDatabase.GetCharacterInStageDataObserver(PlayableCharacter.Kure);
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