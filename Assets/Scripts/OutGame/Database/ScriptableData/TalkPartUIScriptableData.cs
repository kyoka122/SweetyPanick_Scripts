using UnityEngine;

namespace OutGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "TalkPartUIScriptableData", menuName = "ScriptableObjects/TalkPartUIScriptableData")]
    public class TalkPartUIScriptableData:ScriptableObject
    {
        public float SkipGaugeDuration => skipGaugeDuration;
        public float SkipFadeInDuration => skipFadeInDuration;
        public float SkipFadeOutDuration => skipFadeOutDuration;
        public float ToSkipFadeInTime => toSkipFadeInTime;
        
        [SerializeField] private float skipGaugeDuration = 3;
        [SerializeField] private float skipFadeInDuration = 1.5f;
        [SerializeField] private float skipFadeOutDuration = 1;
        [SerializeField] private float toSkipFadeInTime = 2;
    }
}