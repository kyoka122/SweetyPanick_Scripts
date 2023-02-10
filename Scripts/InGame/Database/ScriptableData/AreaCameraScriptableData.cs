using System.Linq;
using MyApplication;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "AreaCameraScriptableData", menuName = "ScriptableObjects/AreaCameraScriptableData")]
    public class AreaCameraScriptableData : ScriptableObject
    {
        [SerializeField] private CameraData[] cameraData;

        public CameraData[] GetAllCameraData()
        {
            return cameraData;
        }
        
        public CameraData GetCameraData(StageArea area)
        {
            CameraData foundData=cameraData.FirstOrDefault(data => data.StageArea == area);

            if (foundData != null) return foundData;
            Debug.LogError($"Not Found Data.  area:{area}");
            return null;
        }
    }
}