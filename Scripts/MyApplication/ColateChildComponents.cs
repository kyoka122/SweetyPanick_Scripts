using UnityEngine;

namespace MyApplication
{
    public class ColateChildComponents:MonoBehaviour
    {
        public Vector2 ToGroundRayPos => toGroundRayTransform.position;
        public Vector2 ToSideWallRayPos => toSideWallRayTransform.position;
        
        [SerializeField] private Transform toGroundRayTransform;
        [SerializeField] private Transform toSideWallRayTransform;
    }
}