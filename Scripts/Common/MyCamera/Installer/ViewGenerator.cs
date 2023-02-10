using InGame.MyCamera.View;
using UnityEngine;

namespace InGame.MyCamera.Installer
{
    public class ViewGenerator:MonoBehaviour
    {
        public MainCameraView InstanceCameraView(MainCameraView prefab)
        {
            return Instantiate(prefab);
        }
    }
}