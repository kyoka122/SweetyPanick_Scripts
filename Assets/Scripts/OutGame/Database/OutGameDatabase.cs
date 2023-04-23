using System;
using System.Collections.Generic;
using System.Linq;
using MyApplication;
using OutGame.Database.ScriptableData;
using Common.MyInput.PlayerCustom;
using TalkSystem;
using UniRx;

namespace OutGame.Database
{
    public class OutGameDatabase:IDisposable
    {
        public OutGameDatabase()
        {
            _talkStageData = new ReactiveProperty<TalkPartPlayerMoveData>();
            _talkPartActionFinished = new ReactiveProperty<TalkPartActionType>(TalkPartActionType.None);
        }

        #region TitleScene

        private TitleSceneScriptableData _titleSceneScriptableData;

        public void SetTitleSceneScriptableData(TitleSceneScriptableData titleSceneScriptableData)
        {
            _titleSceneScriptableData = titleSceneScriptableData;
        }
        
        public TitleSceneScriptableData GetTitleSceneScriptableData()
        {
            return _titleSceneScriptableData;
        }

        #endregion
        
        #region PlayerCustomScene

        private PlayerCustomSceneScriptableData _playerCustomSceneData;

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


        private List<BasePlayerCustomInput> _unknownControllerInputs;
        
        public void AddCharacterSelectController(BasePlayerCustomInput newInput)
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
        
        public void SetCharacterSelectController(List<BasePlayerCustomInput> data)
        {
            _unknownControllerInputs = new List<BasePlayerCustomInput>(data);
        }

        public IReadOnlyList<BasePlayerCustomInput> GetAllCharacterSelectController()
        {
            return _unknownControllerInputs;
        }
        
        
        
        #endregion

        #region BossStage

        private ColateStageScriptableData _colateStageScriptableData;
        
        public void SetColateStageScriptableData(ColateStageScriptableData colateStageScriptableData)
        {
            _colateStageScriptableData = colateStageScriptableData;
        }

        public ColateStageScriptableData GetColateStageScriptableData()
        {
            return _colateStageScriptableData;
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

        #region Dialog

        private DialogFaceSpriteScriptableData _dialogFaceSpriteScriptableData;

        public void SetDialogFaceSpriteScriptableData(DialogFaceSpriteScriptableData dialogFaceSpriteScriptableData)
        {
            _dialogFaceSpriteScriptableData = dialogFaceSpriteScriptableData;
        }
        
        public DialogFaceSpriteScriptableData GetDialogFaceSpriteScriptableData()
        {
            return _dialogFaceSpriteScriptableData;
        }

        #endregion

        #region TalkPart

        private TalkPartUIScriptableData _talkPartUIScriptableData;

        public void SetTalkPartUIScriptableData(TalkPartUIScriptableData  talkPartUIScriptableData)
        {
            _talkPartUIScriptableData = talkPartUIScriptableData;
        }
        
        public TalkPartUIScriptableData GetTalkPartUIScriptableData()
        {
            return  _talkPartUIScriptableData;
        }

        #endregion
        
        public void Dispose()
        {
            _talkStageData?.Dispose();
            _talkPartActionFinished?.Dispose();
        }
    }
}