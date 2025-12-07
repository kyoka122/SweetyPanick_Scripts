using System.Collections.Generic;
using System.Linq;
using Common.Database.ScriptableData;
using InGame.Database;
using InGame.Database.ScriptableData;
using InGame.MyCamera.Interface;
using Common.MyInput.Player;
using MyApplication;

namespace InGame.Common.Database
{
    public class CommonDatabase
    {
        public CommonDatabase()
        {
            StaticDatabase.input = new List<ControllerNumData>();
            _cameraData = new List<CameraInitData>();
        }

        #region GameData
        
        
        private int _maxPlayerCount;
        public void SetMaxPlayerCount(int count)
        {
            _maxPlayerCount = count;
        }
        
        public int GetMaxPlayerCount()
        {
            return _maxPlayerCount;
        } 

        #endregion

        #region JoyconData

        public void SetAllControllerData(List<ControllerNumData> data)
        {
            StaticDatabase.input = new List<ControllerNumData>(data);
        }

        public BasePlayerInput GetControllerData(int playerNum)
        {
            return StaticDatabase.input.FirstOrDefault(num => num.playerNum == playerNum)?.playerInput;
        }

        public IReadOnlyList<ControllerNumData> GetAllControllerData()
        {
            return StaticDatabase.input;
        }
        
        #endregion

        #region CameraData

        private IReadOnlyCameraFunction _readOnlyCameraFunction;

        public IReadOnlyCameraFunction GetReadOnlyCameraController()
        {
            return _readOnlyCameraFunction;
        }

        public void SetReadOnlyCameraFunction(IReadOnlyCameraFunction cameraFunction)
        {
            _readOnlyCameraFunction = cameraFunction;
        }
        
        
        
        private ICameraActionable _cameraActionable;

        public ICameraActionable GetCameraEvent()
        {
            return _cameraActionable;
        }

        public void SetCameraEvent(ICameraActionable cameraActionable)
        {
            _cameraActionable = cameraActionable;
        }

        

        private List<CameraInitData> _cameraData;

        public void SetCameraInitData(CameraInitData[] cameraData)
        {
            _cameraData = cameraData.ToList();
        }
        
        public void AddCameraInitData(CameraInitData cameraInitData)
        {
            _cameraData.Add(cameraInitData);
        }

        public CameraInitData GetCameraInitData(StageArea area)
        {
            return _cameraData.FirstOrDefault(data => data.StageArea == area);
        }

        #endregion

        #region CharacterSettingsData

        private List<UseCharacterData> _useCharacterData;

        public IReadOnlyList<UseCharacterData> GetUseCharacterData()
        {
            return _useCharacterData;
        }

        public void SetUseCharacterData(List<UseCharacterData> useCharacterData)
        {
            _useCharacterData = new List<UseCharacterData>(useCharacterData);
        }

        #endregion

        #region KeyData

        private KeySpriteScriptableData _keySpriteScriptableData;
        
        public KeySpriteScriptableData GetKeySpriteScriptableData()
        {
            return _keySpriteScriptableData;
        }

        public void SetKeySpriteScriptableData(KeySpriteScriptableData keySpriteScriptableData)
        {
            _keySpriteScriptableData = keySpriteScriptableData;
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
        private static class StaticDatabase
        {
            //MEMO: PlayerInput(コントローラー)情報
            public static List<ControllerNumData> input;

        }

        public void Dispose()
        {
            StaticDatabase.input?.Clear();
            _cameraData?.Clear();
            _useCharacterData?.Clear();
        }
    }
}