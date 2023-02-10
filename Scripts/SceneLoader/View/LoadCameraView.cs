using UnityEngine;

namespace InGame.SceneLoader.View
{
    public class LoadCameraView:MonoBehaviour
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

        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
        }
    }
}