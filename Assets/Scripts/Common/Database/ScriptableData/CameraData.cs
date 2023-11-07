using UnityEngine;

namespace Common.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "CameraData", menuName = "ScriptableObjects/CameraData")]
    public class CameraData:ScriptableObject
    {
        public Vector2 Position => position;
        public float Size => size;
        public float MoveDuration => moveDuration;
        
        [SerializeField] private Vector2 position;
        [SerializeField] private float size;
        [SerializeField] private float moveDuration;
    }
}