using UnityEngine;

namespace OutGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "ColateStageScriptableData", menuName = "ScriptableObjects/ColateStageScriptableData")]
    public class ColateStageScriptableData:ScriptableObject
    {
        public float ColateStagePrincessEnterTime => colateStagePrincessEnterTime;
        public float ColateStageMoveXVec => colateStageMoveXVec;
        public float TalkPartFadeInDuration => talkPartFadeInDuration;
        public float TalkPartFadeOutDuration => talkPartFadeOutDuration;

        [SerializeField] private float colateStagePrincessEnterTime = 3;
        [SerializeField] private float colateStageMoveXVec = 6;
        [SerializeField] private float talkPartFadeOutDuration = 0.5f;
        [SerializeField] private float talkPartFadeInDuration = 0.5f;
    }
}