using System.Collections.Generic;
using System.Linq;
using InGame.Common.Database;
using InGame.Database;
using MyApplication;
using OutGame;
using OutGame.Database;
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

        public IReadOnlyList<Joycon> RegisteredJoycons => registeredJoycons;
        public IReadOnlyList<JoyconNumData> RegisteredJoyconNumData => _commonDatabase.GetAllJoyconNumData();
        
        /// <summary>
        /// 1~4まで
        /// </summary>
        public int selectingViewNum { get; private set; }= 1;
        public int selectingViewNumCache { get; private set; }= 0;
        public bool isPlayingPopAnimation { get; private set; }
        public PlayerCustomState PrevPlayerCustomState { get; private set; }
        public List<UseCharacterData> useCharacterData { get; private set; }

        private List<Joycon> registeredJoycons;


        private readonly ReactiveProperty<PlayerCustomState> _settingsState;
        private readonly ReactiveProperty<PlayerCustomState> _hadFinishedPopUpWindow;
        private readonly OutGameDatabase _outGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public InSceneDataEntity(OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _settingsState = new ReactiveProperty<PlayerCustomState>();
            _hadFinishedPopUpWindow = new ReactiveProperty<PlayerCustomState>();
            registeredJoycons = new List<Joycon>();
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
        
        public void SetJoycon(Joycon joycon)
        {
            registeredJoycons.Add(joycon);
        }

        public void SetJoyconNumData()
        {
            Debug.Log($"MaxPlayerCount:{MaxPlayerCount}");
            
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                Debug.Log($"playerCount:{i+1}");
                Debug.Log($"JoyconManager.Instance.j.IndexOf(registeredJoycons[i * 2]:{JoyconManager.Instance.j.IndexOf(registeredJoycons[i * 2])}");
                Debug.Log($"JoyconManager.Instance.j.IndexOf(registeredJoycons[i * 2 + 1]):{JoyconManager.Instance.j.IndexOf(registeredJoycons[i * 2 + 1])}");
                var data = new JoyconNumData(i + 1, JoyconManager.Instance.j.IndexOf(registeredJoycons[i * 2]),
                    JoyconManager.Instance.j.IndexOf(registeredJoycons[i * 2 + 1]));
                _commonDatabase.SetJoyconNumData(data);
            }
        }

        public void CancelRegisteredController(Joycon joycon)
        {
            registeredJoycons.Remove(joycon);
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