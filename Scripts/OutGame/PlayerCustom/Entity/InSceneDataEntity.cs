using System.Collections.Generic;
using System.Linq;
using InGame.Common.Database;
using InGame.Database;
using InGame.MyInput;
using MyApplication;
using OutGame;
using OutGame.Database;
using OutGame.PlayerCustom.MyInput;
using UniRx;
using Unity;
using UnityEngine;

namespace OutGame.PlayerCustom.Entity
{
    public class InSceneDataEntity
    {
        public IReadOnlyReactiveProperty<PlayerCustomState> changedSettingsState => _settingsState;
        public IReadOnlyReactiveProperty<PlayerCustomState> hadFinishedPopUpWindow => _hadFinishedPopUpWindow;
        public PlayerCustomState PlayerCustomState => _settingsState.Value;
        public PlayerCustomState finishedPopUpWindowState => _hadFinishedPopUpWindow.Value;
        public int MaxPlayerCount => _outGameDatabase.GetMaxPlayerCount();

        /// <summary>
        /// コントローラー登録シーンで登録中のコントローラー
        /// </summary>
        public IReadOnlyList<BaseCaseUnknownControllerInput> SelectedControllers => _selectedControllers;
        
        /// <summary>
        /// コントローラー登録シーン終了後、データベースに保存したコントローラー
        /// </summary>ｓ
        public IReadOnlyList<BaseCaseUnknownControllerInput> RegisteredPlayerSelectController => _outGameDatabase.GetAllCharacterSelectController();
        
        /// <summary>
        /// 1~4まで
        /// </summary>
        public int selectingViewNum { get; private set; }= 1;
        public int selectingViewNumCache { get; private set; }= 0;
        public bool isPlayingPopAnimation { get; private set; }
        public PlayerCustomState PrevPlayerCustomState { get; private set; }
        public List<UseCharacterData> useCharacterData { get; private set; }

        private readonly List<BaseCaseUnknownControllerInput> _selectedControllers;


        private readonly ReactiveProperty<PlayerCustomState> _settingsState;
        private readonly ReactiveProperty<PlayerCustomState> _hadFinishedPopUpWindow;
        private readonly OutGameDatabase _outGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public InSceneDataEntity(OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _settingsState = new ReactiveProperty<PlayerCustomState>();
            _hadFinishedPopUpWindow = new ReactiveProperty<PlayerCustomState>();
            _selectedControllers = new List<BaseCaseUnknownControllerInput>();
            useCharacterData = new List<UseCharacterData>();
            _outGameDatabase = outGameDatabase;
            _commonDatabase = commonDatabase;
        }
        
        public void SetSelectingPlayerCountView(int num)
        {
            selectingViewNum = num;
        }
        
        public void SetSelectingPlayerCountViewCache(int num)
        {
            selectingViewNumCache = num;
        }
        
        public void SetSettingsState(PlayerCustomState state)
        {
            PrevPlayerCustomState = _settingsState.Value;
            _settingsState.Value = state;
        }
        
        public void SetHadFinishedPopUpWindow(PlayerCustomState state)
        {
            _hadFinishedPopUpWindow.Value = state;
        }

        public void SetIsPlayingPopAnimation(bool on)
        {
            isPlayingPopAnimation = on;
        }
        
        public void SetMaxPlayerCount(int count)
        {
            _outGameDatabase.SetMaxPlayerCount(count);
        }
        
        public void SetController(BaseCaseUnknownControllerInput input)
        {
            _selectedControllers.Add(input);
        }

        public void SetControllerToDatabase()
        {
            _outGameDatabase.SetCharacterSelectController(_selectedControllers);
        }

        public void SetInGameControllerToDatabase()
        {
            var inGameControllers =
                _selectedControllers.Select(controller => controller.GetInGamePlayerInput()).ToList();

            var controllerNumData = new List<ControllerNumData>();
            for (int i = 0; i < inGameControllers.Count; i++)
            {
                controllerNumData.Add(new ControllerNumData(i + 1, inGameControllers[i]));
            }
            _commonDatabase.SetControllerNumData(controllerNumData);
        }

        public void CancelRegisteredController(BaseCaseUnknownControllerInput input)
        {
            _selectedControllers.Remove(input);
        }
        
        public void CancelAllRegisteredController()
        {
            _selectedControllers.Clear();
        }

        public void SetUseCharacter(int playerNum,PlayableCharacter type)
        {
            var duplication = useCharacterData.FirstOrDefault(data => data.playerNum == playerNum);
            if (duplication!=null)
            {
                useCharacterData.Remove(duplication);
            }
            useCharacterData.Add(new UseCharacterData(playerNum,type));
        }
        
        public void CancelUseCharacter(int playerNum)
        {
            var removeData = useCharacterData.FirstOrDefault(data => data.playerNum == playerNum);
            if (removeData!=null)
            {
                useCharacterData.Remove(removeData);
            }
        }
        
        public void SetUseCharacterToDatabase()
        {
            _commonDatabase.SetUseCharacterData(useCharacterData);
        }
    }
}