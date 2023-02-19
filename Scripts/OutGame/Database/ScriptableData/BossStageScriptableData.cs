using UnityEngine;

namespace OutGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "BossSceneScriptableData", menuName = "ScriptableObjects/BossSceneScriptableData")]
    public class BossStageScriptableData:ScriptableObject
    {
        public float BossStagePrincessEnterTime => bossStagePrincessEnterTime;
        public float BossStageMoveXVec => bossStageMoveXVec;

        [SerializeField] private float bossStagePrincessEnterTime;
        [SerializeField] private float bossStageMoveXVec;
    }
}