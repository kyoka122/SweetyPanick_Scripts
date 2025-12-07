using UnityEngine;

namespace Loader.View
{
    /// <summary>
    /// 画面遷移時、遷移用画面を移すためのCameraを管理するView
    /// </summary>
    public class LoadCameraView : MonoBehaviour
    {
        public float cameraWidth { get; private set; }
        private Camera _camera;

        public void Init()
        {
            _camera = GetComponent<Camera>();
            cameraWidth = _camera.pixelWidth;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}