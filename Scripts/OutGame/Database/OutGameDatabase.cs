using System;
using System.Collections.Generic;
using System.Linq;
using MyApplication;
using OutGame.Database.ScriptableData;
using OutGame.PlayerCustom.MyInput;
using UniRx;

namespace OutGame.Database
{
    public class OutGameDatabase:IDisposable
    {
        public OutGameDatabase()
        {
            _playerInputs = new List<BaseCaseUnknownControllerInput>();
            _talkStageData = new ReactiveProperty<TalkPartPlayerMoveData>();
            _talkPartActionFinished = new ReactiveProperty<TalkPartActionType>();
        }
        
        #region PlayerCustomScene

        private PlayerCustomSceneScriptableData _playerCustomSceneData;
        private bool _canCharacterSelect;
        private int _maxPlayerCount;
        private List<BaseCaseUnknownControllerInput> _playerInputs;

        public void SetPlayerCustomSceneData(PlayerCustomSceneScriptableData data)
        {
            _playerCustomSceneData = data;
        }

        public PlayerCustomSceneScriptableData GetPlayerCustomSceneData()
        {
            return _playerCustomSceneData.Clone();
        }

        
        /*public void SetPlayerInput(BasePlayerInput playerInput)
        {
            _playerInputs.Add(playerInput);
        }
        
        public List<BasePlayerInput> GetBasePlayerInputs()
        {
            return _playerInputs.Select(input => input.Clone()).ToList();
        }
        
        
        public BasePlayerInput GetBasePlayerInput(int playerNum)
        {
            return _playerInputs.Select(input=>input.);
        }*/
        
        
        public void SetMaxPlayerCount(int count)
        {
            _maxPlayerCount = count;
        }
        
        public int GetMaxPlayerCount()
        {
            return _maxPlayerCount;
        } 
        

        
        private List<BaseCaseUnknownControllerInput> _unknownControllerInputs;
        
        public void AddCharacterSelectController(BaseCaseUnknownControllerInput newInput)
        {
            var duplicationData = _unknownControllerInputs.FirstOrDefault(input => input == newInput);
            if (duplicationData == null)
            {
                _unknownControllerInputs.Add(newInput);
            }
            else
            {
                _unknownControllerInputs.Remove(duplicationData);
                _unknownControllerInputs.Add(newInput);
            }
        }
        
        public void SetCharacterSelectController(List<BaseCaseUnknownControllerInput> data)
        {
            _unknownControllerInputs = new List<BaseCaseUnknownControllerInput>(data);
        }

        public IReadOnlyList<BaseCaseUnknownControllerInput> GetAllCharacterSelectController()
        {
            return _unknownControllerInputs;
        }
        
        
        
        #endregion

        #region BossStage

        private BossStageScriptableData _bossStageScriptableData;
        
        public void SetBossStageScriptableData(BossStageScriptableData bossStageScriptableData)
        {
            _bossStageScriptableData = bossStageScriptableData;
        }

        public BossStageScriptableData GetBossStageScriptableData()
        {
            return _bossStageScriptableData;
        }
        
        public IReadOnlyReactiveProperty<TalkPartPlayerMoveData> TalkStageData => _talkStageData;
        private readonly ReactiveProperty<TalkPartPlayerMoveData> _talkStageData;

        public void SetTalkObservableData(TalkPartPlayerMoveData talkPartPlayerMoveData)
        {
            _talkStageData.Value = talkPartPlayerMoveData;
        }
        
        public IReadOnlyReactiveProperty<TalkPartActionType> TalkPartActionFinished => _talkPartActionFinished;
        private readonly ReactiveProperty<TalkPartActionType> _talkPartActionFinished;

        public void SetTalkActionFinished(TalkPartActionType talkPartActionType)
        {
            _talkPartActionFinished.Value = talkPartActionType;
        }

        #endregion

        public void Dispose()
        {
            _talkStageData?.Dispose();
            _talkPartActionFinished?.Dispose();
        }
    }
}