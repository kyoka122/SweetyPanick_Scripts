using Cinemachine;
using UnityEngine;

namespace InGame.MyCamera.View
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class SubCameraView:MonoBehaviour
    {
        public float LeftEdge => leftEdge;
        public float RightEdge => rightEdge;
        
        [SerializeField,Tooltip("このカメラが優先される範囲（左端）")] private float leftEdge;
        [SerializeField,Tooltip("このカメラが優先される範囲（右端）")] private float rightEdge;
        
        private CinemachineVirtualCamera _cinemachineVirtualCamera;

        public void Init()
        {
            _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        public void SetPriority(int value)
        {
            _cinemachineVirtualCamera.Priority = value;
        }
    }
}