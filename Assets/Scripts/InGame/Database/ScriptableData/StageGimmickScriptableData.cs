using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "StageGimmickScriptableData", menuName = "ScriptableObjects/StageGimmickScriptableData")]
    public class StageGimmickScriptableData:ScriptableObject
    {
        public int GimmickSweetsScore => gimmickSweetsScore;
        public int NormalSweetsScore => normalSweetsScore;
        public float CandyLightsParticleDuration=>candyLightsParticleDuration;
        public float MoveFloorSpeed=>moveFloorSpeed;
        public Vector3 CrepeCameraShakeVelocity=>crepeCameraShakeVelocity;
        public int GoldSweetsScore => goldSweetsScore;

        [SerializeField] private int goldSweetsScore = 10000;
        [SerializeField] private int gimmickSweetsScore = 3000;
        [SerializeField] private int normalSweetsScore = 1000;
        [SerializeField] private float candyLightsParticleDuration=1;
        [SerializeField,Tooltip("ガムを修復したら動く台のスピード")] private float moveFloorSpeed;
        [SerializeField] private Vector3 crepeCameraShakeVelocity;
        
    }
}