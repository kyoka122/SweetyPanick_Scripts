using System.Collections.Generic;
using OutGame.Database.ScriptableData;
using OutGame.MyInput;

namespace OutGame.Database
{
    public class OutGameDatabase
    {
        #region PlayerCustomScene

        private PlayerCustomSceneScriptableData _playerCustomSceneData;
        private bool _canCharacterSelect;
        private int _maxPlayerCount;
        private List<BasePlayerInput> _playerInputs;

      
        
        public OutGameDatabase()
        {
            _playerInputs = new List<BasePlayerInput>();
        }

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
        

        /*public void SetCanCharacterSelect(bool on)
        {
            _canCharacterSelect = on;
        }

        public bool GetCanCharacterSelect()
        {
            return _canCharacterSelect;
        }

        public void SetJoyConPlayerInput(JoyConPlayerInput joyConPlayerInput)
        {
            if ()
            {
                
            }
            joyConPlayerInputs.Add();
        }

        public bool GetJoyConPlayerInput()
        {
            return _canCharacterSelect;
        }*/

        #endregion
    }
}