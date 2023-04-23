using System.Collections.Generic;
using System.Linq;
using InGame.Common.Database;
using InGame.Database;
using Common.MyInput.Player;
using MyApplication;
using OutGame;
using OutGame.Database;
using Common.MyInput.PlayerCustom;
using UniRx;
using Unity;
using UnityEngine;

namespace OutGame.PlayerCustom.Entity
{
    public class InSceneDataEntity
    {
        public IReadOnlyReactiveProperty<PlayerCustomState> ChangedSettingsState => _settingsState;
        public IReadOnlyReactiveProperty<PlayerCustomState> HadFinishedPopUpWindow => _hadFinishedPopUpWindow;
        public PlayerCustomState PlayerCustomState => _settingsState.Value;
        public PlayerCustomState FinishedPopUpWindowState => _hadFinishedPopUpWindow.Value;
        public int MaxPlayerCount => _commonDatabase.GetMaxPlayerCount();

        /// <summary>
        /// コントローラー登録シーンで登録中のコントローラー
        /// </summary>
        public IReadOnlyList<BasePlayerCustomInput> SelectedControllers => _selectedControllers;
        
        /// <summary>
        /// コントローラー登録シーン終了後、データベースに保存したコントローラー
        /// </summary>ｓ
        public IReadOnlyList<BasePlayerCustomInput> RegisteredPlayerSelectController => _outGameDatabase.GetAllCharacterSelectController();
        
        /// <summary>
        /// 1~4まで
        /// </summary>
        public int selectingViewNum { get; private set; }= 1;
        public int selectingViewNumCache { get; private set; }= 0;
        public bool isPlayingPopAnimation { get; private set; }
        public PlayerCustomState PrevPlayerCustomState { get; private set; }
        public List<UseCharacterData> useCharacterData { get; private set; }

        private readonly List<BasePlayerCustomInput> _selectedControllers;


        private readonly ReactiveProperty<PlayerCustomState> _settingsState;
        private readonly ReactiveProperty<PlayerCustomState> _hadFinishedPopUpWindow;
        private readonly OutGameDatabase _outGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public InSceneDataEntity(OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _settingsState = new ReactiveProperty<PlayerCustomState>();
            _hadFinishedPopUpWindow = new ReactiveProperty<PlayerCustomState>();
            _selectedControllers = new List<BasePlayerCustomInput>();
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
            _commonDatabase.SetMaxPlayerCount(count);
        }
        
        public void SetController(BasePlayerCustomInput input)
        {
            _selectedControllers.Add(input);
        }

        public void SetControllerToDatabase()
        {
            _outGameDatabase.SetCharacterSelectController(_selectedControllers);
        }

        public void SetInGameControllerToDatabase()
        {
            var inGameControllers = _selectedControllers
                    .Select(controller => 
                        InputMakeHelper.GeneratePlayerCustomInput(controller.DeviceType,controller.DeviceId))
                    .ToList();
            
            var controllerNumData = new List<ControllerNumData>();
            for (int i = 0; i < inGameControllers.Count; i++)
            {
                controllerNumData.Add(new ControllerNumData(i + 1, inGameControllers[i]));
            }
            _commonDatabase.SetAllControllerData(controllerNumData);
        }

        public void CancelRegisteredController(BasePlayerCustomInput input)
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