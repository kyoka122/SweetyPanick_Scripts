using System.Linq;
using UnityEditor;
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
        [SerializeField] private int[] stageMaxScore = new int[2];

        public int StageMaxScore(int stageNum) => stageMaxScore[stageNum];
        public int AllStageSumScore() => stageMaxScore.Sum();
#if UNITY_EDITOR
        public void SetStageMaxScore(int stageNum, int score)
        {
            if (stageNum <= 0 || stageNum > stageMaxScore.Length)
            {
                Debug.LogError($"Not Found {stageNum} StageIndex");
                return;
            }

            var serializedObject = new SerializedObject(this);
            serializedObject.Update();
            Debug.Log(score);
            SerializedProperty serializedProperty = serializedObject.FindProperty("stageMaxScore").GetArrayElementAtIndex(stageNum-1);
            serializedProperty.intValue = score;
            serializedObject.ApplyModifiedProperties();
            stageMaxScore[stageNum - 1] = score;
        }
#endif
    }
}