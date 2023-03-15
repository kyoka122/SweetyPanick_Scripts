using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "StageGimmickScriptableData", menuName = "ScriptableObjects/StageGimmickScriptableData")]
    public class StageGimmickScriptableData:ScriptableObject
    {
        public int GimmickSweetsScore => gimmickSweetsScore;
        public int NormalSweetsScore => normalSweetsScore;
        public float MoveFloorSpeed=>moveFloorSpeed;
        public Vector3 CrepeCameraShakeVelocity=>crepeCameraShakeVelocity;

        [SerializeField,Tooltip("ガムを修復したら動く台")] private int gimmickSweetsScore=3000;
        [SerializeField,Tooltip("ガムを修復したら動く台")] private int normalSweetsScore=1000;
        [SerializeField,Tooltip("ガムを修復したら動く台")] private float moveFloorSpeed;
        [SerializeField] private Vector3 crepeCameraShakeVelocity;
    }
}