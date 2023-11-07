using Cinemachine;
using UnityEngine;

namespace Common.MyCamera.View
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class SubCameraView:MonoBehaviour
    {
        public float LeftEdge => leftEdge;
        public float RightEdge => rightEdge;
        
        [SerializeField,Tooltip("適応範囲(左端)(world座標)")] private float leftEdge;
        [SerializeField,Tooltip("適応範囲(右端)(world座標)")] private float rightEdge;
        
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