using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "StageGimmickScriptableData", menuName = "ScriptableObjects/StageGimmickScriptableData")]
    public class StageGimmickScriptableData:ScriptableObject
    {
        public float MoveFloorSpeed=>moveFloorSpeed;

        [SerializeField] private float moveFloorSpeed;
    }
}