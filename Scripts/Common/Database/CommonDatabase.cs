using System.Collections.Generic;
using System.Linq;
using InGame.Database;
using InGame.MyCamera.Interface;
using InGame.MyInput;
using MyApplication;

namespace InGame.Common.Database
{
    public class CommonDatabase
    {
        public CommonDatabase()
        {
            StaticDatabase.input = new List<ControllerNumData>();
            _cameraData = new List<CameraData>();
        }


        #region JoyconData

        public void SetControllerNumData(List<ControllerNumData> data)
        {
            StaticDatabase.input = new List<ControllerNumData>(data);
        }

        public BasePlayerInput GetControllerNumData(int playerNum)
        {
            return StaticDatabase.input.FirstOrDefault(num => num.playerNum == playerNum)?.playerInput;
        }

        public IReadOnlyList<ControllerNumData> GetAllControllerNumData()
        {
            return StaticDatabase.input;
        }

        #endregion

        #region CameraData

        private IReadOnlyCameraFunction _cameraFunction;

        public IReadOnlyCameraFunction GetReadOnlyCameraController()
        {
            return _cameraFunction;
        }

        public void SetReadOnlyCameraFunction(IReadOnlyCameraFunction cameraFunction)
        {
            _cameraFunction = cameraFunction;
        }
        
        
        
        private ICameraEvent _cameraEvent;

        public ICameraEvent GetCameraEvent()
        {
            return _cameraEvent;
        }

        public void SetCameraEvent(ICameraEvent cameraEvent)
        {
            _cameraEvent = cameraEvent;
        }

        

        private List<CameraData> _cameraData;

        public void SetCameraData(CameraData[] cameraData)
        {
            _cameraData = cameraData.ToList();
        }
        
        public void AddCameraData(CameraData cameraData)
        {
            _cameraData.Add(cameraData);
        }

        public CameraData GetCameraSettingsData(StageArea area)
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

        private static class StaticDatabase
        {
            // region PlayerInput
            public static List<ControllerNumData> input;

        }

    }
}