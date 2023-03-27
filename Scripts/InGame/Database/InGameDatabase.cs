using System;
using System.Collections.Generic;
using System.Linq;
using MyApplication;
using InGame.Database.ScriptableData;
using UniRx;
using UnityEngine;

namespace InGame.Database
{
    public class InGameDatabase:IDisposable
    {
        public InGameDatabase()
        {
            _fixedSweetsSubject = new Subject<GimmickSweets>();
            _fixedSweets = new List<GimmickSweets>();

            _candyInStageData = new ReactiveProperty<CharacterUpdateableInStageData>();
            _fuInStageData = new ReactiveProperty<CharacterUpdateableInStageData>();
            _mashInStageData = new ReactiveProperty<CharacterUpdateableInStageData>();
            _kureInStageData = new ReactiveProperty<CharacterUpdateableInStageData>();
            _eachStagePlayerInstanceData = new List<EachStagePlayerInstanceData>();
        }
        
        ///////////////////////////////////////////////////////////////////////////
        /// 可変データに関しては毎回Cloneするようにする///////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        
        #region CharacterInstancePosition
        
        private readonly List<EachStagePlayerInstanceData> _eachStagePlayerInstanceData;

        public void AddPlayerInstanceData(EachStagePlayerInstanceData data)
        {
            Debug.Log($"AddInstanceDataType:{data}, area:{data.StageArea}");
            _eachStagePlayerInstanceData.Add(data);
        }

        public PlayerInstanceData GetPlayerInstanceData(StageArea type)
        {
            return _eachStagePlayerInstanceData
                .FirstOrDefault(data=>data.StageArea==type)?.PlayerInstanceData;
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
            var data = new BaseCharacterCommonStatus[]
            {
                GetCandyStatus(), GetMashStatus(), GetFuStatus(), GetKureStatus()
            };

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

        private PlayerUpdateableData _candyUpdateableData;
        private PlayerUpdateableData _fuUpdateableData;
        private PlayerUpdateableData _mashUpdateableData;
        private PlayerUpdateableData _kureUpdateableData;

        public PlayerUpdateableData[] GetAllPlayerUpdateableData()
        {
            return new[]{_candyUpdateableData, _mashUpdateableData, _fuUpdateableData,_kureUpdateableData};
        }
        
        public PlayerUpdateableData GetPlayerUpdateableData(PlayableCharacter type)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    return _candyUpdateableData;
                case PlayableCharacter.Mash:
                    return _mashUpdateableData;
                case PlayableCharacter.Fu:
                    return _fuUpdateableData;
                case PlayableCharacter.Kure:
                    return _kureUpdateableData;
                default:
                    Debug.LogError($"ArgumentOutOfRangeException. type{type}");
                    return null;
            }
        }

        public void SetPlayerUpdateableData(PlayableCharacter type,PlayerUpdateableData playerUpdateableData)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    _candyUpdateableData=playerUpdateableData;
                    break;
                case PlayableCharacter.Mash:
                    _mashUpdateableData=playerUpdateableData;
                    break;
                case PlayableCharacter.Fu:
                    _fuUpdateableData=playerUpdateableData;
                    break;
                case PlayableCharacter.Kure:
                    _kureUpdateableData=playerUpdateableData;
                    break;
                default:
                    Debug.LogError($"ArgumentOutOfRangeException. type{type}");
                    break;
            }
        }

        
        #endregion


        #region CharacterUpdateableInStageData


        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> CandyInStageData=>_candyInStageData;
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> FuInStageData=>_fuInStageData;
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> MashInStageData=>_mashInStageData;
        public IReadOnlyReactiveProperty<CharacterUpdateableInStageData> KureInStageData=>_kureInStageData;
        
        private readonly ReactiveProperty<CharacterUpdateableInStageData> _candyInStageData;
        private readonly ReactiveProperty<CharacterUpdateableInStageData> _fuInStageData;
        private readonly ReactiveProperty<CharacterUpdateableInStageData> _mashInStageData;
        private readonly ReactiveProperty<CharacterUpdateableInStageData> _kureInStageData;

        public Dictionary<PlayableCharacter,CharacterUpdateableInStageData> GetAllCharacterInStageData()
        {
            var data = new Dictionary<PlayableCharacter,CharacterUpdateableInStageData>
            {
                {PlayableCharacter.Candy,_candyInStageData.Value}, 
                {PlayableCharacter.Fu,_fuInStageData.Value},
                {PlayableCharacter.Mash,_mashInStageData.Value},
                {PlayableCharacter.Kure,_kureInStageData.Value}
            };

            return data;
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
                    Debug.LogError($"ArgumentOutOfRangeException:{nameof(type)}");
                    return new CharacterUpdateableInStageData();
            }
        }

        public void SetCharacterInStageData(PlayableCharacter type,CharacterUpdateableInStageData characterUpdateableInStageData)
        {
            switch (type)
            {
                case PlayableCharacter.Candy:
                    _candyInStageData.Value=characterUpdateableInStageData;
                    break;
                case PlayableCharacter.Mash:
                    _mashInStageData.Value=characterUpdateableInStageData;
                    break;
                case PlayableCharacter.Fu:
                    _fuInStageData.Value=characterUpdateableInStageData;
                    break;
                case PlayableCharacter.Kure:
                    _kureInStageData.Value=characterUpdateableInStageData;
                    break;
                default:
                    Debug.LogError($"ArgumentOutOfRangeException:{nameof(type)}");
                    break;
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

        #region Colate

        private ColateScriptableData _colateScriptableData;
        public void SetColateData(ColateScriptableData colateScriptableData)
        {
            _colateScriptableData = colateScriptableData;
        }

        public ColateScriptableData GetColateData()
        {
            return _colateScriptableData;
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
        
        
        
        private StageGimmickScriptableData _stageGimmickScriptableData;

        public StageGimmickScriptableData GetStageGimmickData()
        {
            return _stageGimmickScriptableData;
        }

        public void SetStageGimmickData(StageGimmickScriptableData stageGimmickScriptableData)
        {
            _stageGimmickScriptableData = stageGimmickScriptableData;
        }

        

        private AllStageData _allStageData;
        
        public AllStageData GetAllStageData()
        {
            return _allStageData;
        }

        public void SetAllStageData(AllStageData allStageData)
        {
            _allStageData = allStageData;
        }

        
        #endregion

        #region UIData

        private StageUIData _stageUIData;
        
        public StageUIData GetUIData()
        {
            return _stageUIData;
        }

        public void SetUIData(StageUIData stageUIScriptableData)
        {
            _stageUIData = stageUIScriptableData.Clone();
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

        public void Dispose()
        {
            _candyInStageData?.Dispose();
            _fuInStageData?.Dispose();
            _mashInStageData?.Dispose();
            _kureInStageData?.Dispose();
            _fixedSweetsSubject?.Dispose();
        }
    }
}