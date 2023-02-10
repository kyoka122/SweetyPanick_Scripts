using System;
using InGame.Common.Database;
using InGame.Database;
using InGame.Database.ScriptableData;
using InGame.MyCamera.Controller;
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

        public bool isCandyHadTargetGroup { get; private set; }
        public bool isMashHadTargetGroup { get; private set; }
        public bool isFuHadTargetGroup { get; private set; }
        public bool isKureHadTargetGroup { get; private set; }

        public CameraData GetCameraSettingsData(StageArea area)=>_commonDatabase.GetCameraSettingsData(area);
        
        public bool GetHadTargetGroup(PlayableCharacter characterType)
        {
            return characterType switch
            {
                PlayableCharacter.Candy => isCandyHadTargetGroup,
                PlayableCharacter.Mash => isMashHadTargetGroup,
                PlayableCharacter.Fu => isFuHadTargetGroup,
                PlayableCharacter.Kure => isKureHadTargetGroup,
                _ => throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null)
            };
        }
        
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;
        
        public CameraEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            GetObserver();
        }

        private void GetObserver()
        {
            candyUpdateableInStageData = _inGameDatabase.GetCharacterInStageDataObserver(PlayableCharacter.Candy);
            mashUpdateableInStageData = _inGameDatabase.GetCharacterInStageDataObserver(PlayableCharacter.Mash);
            fuUpdateableInStageData = _inGameDatabase.GetCharacterInStageDataObserver(PlayableCharacter.Fu);
            kureUpdateableInStageData = _inGameDatabase.GetCharacterInStageDataObserver(PlayableCharacter.Kure);
        }

        public void SetCameraFunctionInDatabase(IReadOnlyCameraFunction readOnlyFunction)
        {
            _commonDatabase.SetReadOnlyCameraFunction(readOnlyFunction);
        }

        public void SetCharacterHadTargetGroup(PlayableCharacter characterType)
        {
            switch (characterType)
            {
                case PlayableCharacter.Candy:
                    isCandyHadTargetGroup = true;
                    break;
                case PlayableCharacter.Mash:
                    isMashHadTargetGroup = true;
                    break;
                case PlayableCharacter.Fu:
                    isFuHadTargetGroup = true;
                    break;
                case PlayableCharacter.Kure:
                    isKureHadTargetGroup = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
            }
        }
        
    }
}