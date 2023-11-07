using Common.MyCamera.View;
using UnityEngine;

namespace Common.MyCamera.Installer
{
    public class ViewGenerator:MonoBehaviour
    {
        public MainCameraView InstanceCameraView(MainCameraView prefab)
        {
            return Instantiate(prefab);
        }
    }
}