using System.Collections.Generic;
using System.Linq;
using InGame.Database;
using InGame.MyCamera.Controller;
using MyApplication;

namespace InGame.Common.Database
{
    public class CommonDatabase
    {
        #region JoyconData

        public CommonDatabase()
        {
            StaticDatabase._joyconNums = new List<JoyconNumData>();
            _cameraData = new List<CameraData>();
        }

        public void SetJoyconNumData(JoyconNumData data)
        {
            var duplicationData = StaticDatabase._joyconNums.FirstOrDefault(num => num.playerNum == data.playerNum);
            if (duplicationData == null)
            {
                StaticDatabase._joyconNums.Add(data);
            }
            else
            {
                StaticDatabase._joyconNums.Remove(duplicationData);
                StaticDatabase._joyconNums.Add(data);
            }
        }


        public void SetJoyconNumData(List<JoyconNumData> data)
        {
            StaticDatabase._joyconNums = new List<JoyconNumData>(data);
        }

        public JoyconNumData GetJoyconNumData(int playerNum)
        {
            return StaticDatabase._joyconNums.FirstOrDefault(num => num.playerNum == playerNum);
        }

        public IReadOnlyList<JoyconNumData> GetAllJoyconNumData()
        {
            return StaticDatabase._joyconNums;
        }

        #endregion

        #region CameraData

        private IReadOnlyCameraFunction _cameraFunction;

        public IReadOnlyCameraFunction GetIReadOnlyCameraController()
        {
            return _cameraFunction;
        }

        public void SetReadOnlyCameraFunction(IReadOnlyCameraFunction cameraFunction)
        {
            _cameraFunction = cameraFunction;
        }


        private List<CameraData> _cameraData;

        public void SetCameraData(CameraData[] cameraData)
        {
            _cameraData = cameraData.ToList();
        }
        
        public bool TryAddCameraData(CameraData cameraData)
        {
            if (_cameraData.FirstOrDefault(data => data.StageArea == cameraData.StageArea) != null)
            {
                return false;
            }

            _cameraData.Add(cameraData);
            return true;
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
            public static List<JoyconNumData> _joyconNums;

        }
    }
}