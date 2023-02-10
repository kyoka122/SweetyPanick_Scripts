using System;
using System.Collections.Generic;
using System.Linq;
using InGame.MyCamera.Controller;
using MyApplication;
using InGame.Database.ScriptableData;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace InGame.Database
{
    public class InGameDatabase
    {
        public InGameDatabase()
        {
            _fixedSweetsSubject = new Subject<GimmickSweets>();
            _fixedSweets = new List<GimmickSweets>();

            _candyUpdateableData = new ReactiveProperty<PlayerUpdateableData>();
            _fuUpdateableData = new ReactiveProperty<PlayerUpdateableData>();
            _mashUpdateableData = new ReactiveProperty<PlayerUpdateableData>();
            _kureUpdateableData = new ReactiveProperty<PlayerUpdateableData>();
            
            _candyInStageData = new ReactiveProperty<CharacterUpdateableInStageData>();
            _fuInStageData = new ReactiveProperty<CharacterUpdateableInStageData>();
            _mashInStageData = new ReactiveProperty<CharacterUpdateableInStageData>();
            _kureInStageData = new ReactiveProperty<CharacterUpdateableInStageData>();
            _playerInstancePositions = new Dictionary<StageArea, PlayerInstancePositions>();
        }
        
        ///////////////////////////////////////////////////////////////////////////
        /// 可変データに関しては毎回Cloneするようにする///////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        
        #region CharacterInstancePosition
        
        private readonly Dictionary<StageArea,PlayerInstancePositions> _playerInstancePositions;

        public void AddPlayerInstancePositions(StageArea type,PlayerInstancePositions instancePositions)
        {
            _playerInstancePositions.Add(type,instancePositions);
        }

        public PlayerInstancePositions GetPlayerInstancePositions(StageArea type)
        {
            return _playerInstancePositions
                .FirstOrDefault(data=>data.Key==type).Value;
        }
        
        

        #endregion
        
        
        #region CharacterStatus

        private CandyStatus _candyStatus;
        private MashStatus _mashStatus;
        private FuStatus _fuStatus;
        private KureStatus _kureStatus;
        
        public CandyStatus GetCandyStatus()
        {
            return _candyStatus;
        }
        
        public void SetCandyStatus(CandyStatus candyStatus)
        {
            _candyStatus = candyStatus.Clone();
        }

        public MashStatus GetMashStatus()
        {
            return _mashStatus;
        }

        public void SetMashStatus(MashStatus mashStatus)
        {
            _mashStatus = mashStatus.Clone();
        }

        public FuStatus GetFuStatus()
        {
            return _fuStatus;
        }

        public void SetFuStatus(FuStatus fuStatus)
        {
            _fuStatus = fuStatus.Clone();
        }
        
        public KureStatus GetKureStatus()
        {
            return _kureStatus;
        }

        public void SetKureStatus(KureStatus kureStatus)
        {
            _kureStatus = kureStatus.Clone();
        }
        
        public BaseCharacterCommonStatus[] GetAllCharacterCommonStatus()
        {
            var data = new List<BaseCharacterCommonStatus>();
            data.Add(GetCandyStatus());
            data.Add(GetMashStatus());
            data.Add(GetFuStatus());
            data.Add(GetKureStatus());
            
            return data.Where(characterCommonStatus => characterCommonStatus!= null)
                .Where(registeredData => registeredData.characterType != PlayableCharacter.None)
                .ToArray();
        }
        
        public BaseCharacterCommonStatus GetCharacterCommonStatus(PlayableCharacter playableCharacter)
        {
            BaseCharacterCommonStatus selectedStatus = GetAllCharacterCommonStatus()
                .FirstOrDefault(parameter => parameter.characterType == playableCharacter);
            if (selectedStatus==null)
            {
                Debug.LogError($"None PlayerStatus");
            }

            return selectedStatus;
        }
        
        #endregion

        
        #region PlayerUpdateableData

        private readonly ReactiveProperty<PlayerUpdateableData> _candyUpdateableData;
        private readonly ReactiveProperty<PlayerUpdateableData> _fuUpdateableData;
        private readonly ReactiveProperty<PlayerUpdateableData> _mashUpdateableData;
        private readonly ReactiveProperty<PlayerUpdateableData> _kureUpdateableData;

        public IReadOnlyReactiveProperty<PlayerUpdateableData> GetPlayerUpdateableDataObserver(PlayableCharacter type)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    return _candyUpdateableData;
                case PlayableCharacter.Mash:
                    return _fuUpdateableData;
                case PlayableCharacter.Fu:
                    return _mashUpdateableData;
                case PlayableCharacter.Kure:
                    return _kureUpdateableData;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public PlayerUpdateableData GetPlayerUpdatedData(PlayableCharacter type)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    return _candyUpdateableData.Value;
                case PlayableCharacter.Mash:
                    return _mashUpdateableData.Value;
                case PlayableCharacter.Fu:
                    return _fuUpdateableData.Value;
                case PlayableCharacter.Kure:
                    return _kureUpdateableData.Value;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void SetPlayerUpdateableData(PlayableCharacter type,PlayerUpdateableData playerUpdateableData)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    _candyUpdateableData.Value=playerUpdateableData.Clone();
                    break;
                case PlayableCharacter.Mash:
                    _mashUpdateableData.Value=playerUpdateableData.Clone();
                    break;
                case PlayableCharacter.Fu:
                    _fuUpdateableData.Value=playerUpdateableData.Clone();
                    break;
                case PlayableCharacter.Kure:
                    _kureUpdateableData.Value=playerUpdateableData.Clone();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        
        #endregion


        #region CharacterUpdateableInStageData


        private readonly ReactiveProperty<CharacterUpdateableInStageData> _candyInStageData;
        private readonly ReactiveProperty<CharacterUpdateableInStageData> _fuInStageData;
        
        private readonly ReactiveProperty<CharacterUpdateableInStageData> _mashInStageData;
        
        private readonly ReactiveProperty<CharacterUpdateableInStageData> _kureInStageData;

        public CharacterUpdateableInStageData[] GetAllCharacterInStageData()
        {
            var data = new List<CharacterUpdateableInStageData>();
            data.Add(_candyInStageData.Value);
            data.Add(_fuInStageData.Value);
            data.Add(_mashInStageData.Value);
            data.Add(_kureInStageData.Value);
            
            return data
                .Where(characterInStageData => characterInStageData!= null)
                .ToArray();
        }
        
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> GetCharacterInStageDataObserver(PlayableCharacter type)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    return _candyInStageData;
                case PlayableCharacter.Fu:
                    return _mashInStageData;
                case PlayableCharacter.Mash:
                    return _fuInStageData;
                case PlayableCharacter.Kure:
                    return _kureInStageData;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public CharacterUpdateableInStageData GetCharacterInStageData(PlayableCharacter type)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    return _candyInStageData.Value;
                case PlayableCharacter.Mash:
                    return _mashInStageData.Value;
                case PlayableCharacter.Fu:
                    return _fuInStageData.Value;
                case PlayableCharacter.Kure:
                    return _kureInStageData.Value;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void SetCharacterInStageData(PlayableCharacter type,CharacterUpdateableInStageData characterUpdateableInStageData)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    _candyInStageData.Value=characterUpdateableInStageData.Clone();
                    break;
                case PlayableCharacter.Mash:
                    _mashInStageData.Value=characterUpdateableInStageData.Clone();
                    break;
                case PlayableCharacter.Fu:
                    _fuInStageData.Value=characterUpdateableInStageData.Clone();
                    break;
                case PlayableCharacter.Kure:
                    _kureInStageData.Value=characterUpdateableInStageData.Clone();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        #endregion
        
        #region CharacterConstData

        private CandyConstData _candyConstData;
        private MashConstData _mashConstData;
        private FuConstData _fuConstData;
        private KureConstData _kureConstData;
        
        public CandyConstData GetCandyConstData()
        {
            return _candyConstData;
        }
        
        public void SetCandyConstData(CandyConstData candyConstData)
        {
            _candyConstData = candyConstData;
        }

        public MashConstData GetMashConstData()
        {
            return _mashConstData;
        }

        public void SetMashConstData(MashConstData mashConstData)
        {
            _mashConstData = mashConstData;
        }

        public FuConstData GetFuConstData()
        {
            return _fuConstData;
        }

        public void SetFuConstData(FuConstData fuConstData)
        {
            _fuConstData = fuConstData;
        }
        
        public KureConstData GetKureConstData()
        {
            return _kureConstData;
        }

        public void SetKureConstData(KureConstData kureConstData)
        {
            _kureConstData = kureConstData;
        }
        #endregion

        #region CharacterCommonConstData
        
        private CharacterCommonConstData _candyCommonConstData;
        private CharacterCommonConstData _mashCommonConstData;
        private CharacterCommonConstData _fuCommonConstData;
        private CharacterCommonConstData _kureCommonConstData;
        
        public CharacterCommonConstData GetCommonCandyConstData()
        {
            return _candyCommonConstData;
        }
        
        public void SetCommonCandyConstData(CharacterCommonConstData candyCommonConstData)
        {
            _candyCommonConstData = candyCommonConstData;
        }

        public CharacterCommonConstData GetCommonMashConstData()
        {
            return _mashCommonConstData;
        }

        public void SetCommonMashConstData(CharacterCommonConstData mashCommonConstData)
        {
            _mashCommonConstData = mashCommonConstData;
        }

        public CharacterCommonConstData GetCommonFuConstData()
        {
            return _fuCommonConstData;
        }

        public void SetCommonFuConstData(CharacterCommonConstData fuCommonConstData)
        {
            _fuCommonConstData = fuCommonConstData;
        }
        
        public CharacterCommonConstData GetCommonKureConstData()
        {
            return _kureCommonConstData;
        }

        public void SetCommonKureConstData(CharacterCommonConstData kureCommonConstData)
        {
            _kureCommonConstData = kureCommonConstData;
        }
        
        public CharacterCommonConstData[] GetAllCharacterConstData()
        {
            var data = new List<CharacterCommonConstData>();
            data.Add(GetCommonCandyConstData());
            data.Add(GetCommonMashConstData());
            data.Add(GetCommonFuConstData());
            data.Add(GetCommonKureConstData());
            
            return data.Where(characterCommonStatus => characterCommonStatus!= null)
                .Where(registeredData => registeredData.CharacterType != PlayableCharacter.None)
                .ToArray();
        }
        
        public CharacterCommonConstData GetCharacterConstData(PlayableCharacter playableCharacter)
        {
            CharacterCommonConstData selectedStatus = GetAllCharacterConstData()
                .FirstOrDefault(parameter => parameter.CharacterType == playableCharacter);
            if (selectedStatus==null)
            {
                Debug.LogError($"None PlayerStatus");
            }

            return selectedStatus;
        }
        
        #endregion

        #region Enemy

        private EnemyScriptableData _enemyData;

        public EnemyScriptableData GetEnemyData()
        {
            return _enemyData;
        }
        
        public void SetEnemyData(EnemyScriptableData enemyScriptableData)
        {
            _enemyData = enemyScriptableData;
        }

        #endregion
        
        
        #region StageGimmick

        public IObservable<GimmickSweets> FixedSweetsObserver => _fixedSweetsSubject;

        private readonly Subject<GimmickSweets> _fixedSweetsSubject;
        private readonly List<GimmickSweets> _fixedSweets;

        public void SetFixedSweets(GimmickSweets gimmickSweets)
        {
            if (_fixedSweets.Contains(gimmickSweets))
            {
                return;
            }
            _fixedSweets.Add(gimmickSweets);
            _fixedSweetsSubject.OnNext(gimmickSweets);
        }

        #endregion

        #region UIData

        private UIData _uiData;
        
        public UIData GetUIData()
        {
            return _uiData;
        }

        public void SetUIData(UIData uiScriptableData)
        {
            _uiData = uiScriptableData.Clone();
        }
        
        #endregion

        
        #region StageSettings

        private StageSettingsScriptableData stageSettingsScriptableData;

        public StageSettingsScriptableData GetStageSettings()
        {
            return stageSettingsScriptableData;
        }
        public void SetStageSettings(StageSettingsScriptableData stageSettingsScriptableData)
        {
            this.stageSettingsScriptableData = stageSettingsScriptableData;
        }

        #endregion

        #region StageGimmicks

        private StageGimmickScriptableData _stageGimmickScriptableData;

        public StageGimmickScriptableData GetStageGimmickData()
        {
            return _stageGimmickScriptableData;
        }

        public void SetStageGimmickData(StageGimmickScriptableData stageGimmickScriptableData)
        {
            _stageGimmickScriptableData = stageGimmickScriptableData;
        }
        

        #endregion
        

        #region Load

        private SceneLoadData _sceneLoadData;
        
        public SceneLoadData GetSceneLoadData()
        {
            return _sceneLoadData;
        }

        public void SetSceneLoadData(SceneLoadData sceneLoadData)
        {
            _sceneLoadData = sceneLoadData;
        }

        #endregion
        
    }
}