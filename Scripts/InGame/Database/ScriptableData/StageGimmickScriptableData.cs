using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "StageGimmickScriptableData", menuName = "ScriptableObjects/StageGimmickScriptableData")]
    public class StageGimmickScriptableData:ScriptableObject
    {
        public float MoveFloorSpeed=>moveFloorSpeed;
        public Vector3 CrepeCameraShakeVelocity=>crepeCameraShakeVelocity;

        [SerializeField] private float moveFloorSpeed;
        [SerializeField] private Vector3 crepeCameraShakeVelocity;
    }
}