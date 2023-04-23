using MyApplication;
using UnityEngine;

namespace OutGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "ScoreSceneScriptableData", menuName = "ScriptableObjects/ScoreSceneScriptableData")]
    public class ScoreSceneScriptableData:ScriptableObject
    {
        public float BgmFadeOutDuration => bgmFadeOutDuration;
        public float BgmFadeInDuration => bgmFadeInDuration;
        public float ToScoreCountUpDelay => toScoreCountUpDelay;
        public float ScoreCountUpDuration => scoreCountUpDuration;
        public float BossDefeatBonusEnterDuration => bossDefeatBonusEnterDuration;
        public float BossDefeatBonusCountUpDuration => bossDefeatBonusCountUpDuration;
        public float BossDefeatBonusCountUpDelay => bossDefeatBonusCountUpDelay;
        public float BossDefeatBonusFadeInDelay => bossDefeatBonusFadeInDelay;
        public float CanOnTriggerFinishGameDelay => canOnTriggerFinishGameDelay;
        public float MedalEnterDelay => medalEnterDelay;
        public float MedalEnterDuration => medalEnterDuration;
        public float MedalInitScaleFactor => medalInitScaleFactor;
        public float ToTextDelay => toTextDelay;
        public int BossDefeatBonus => bossDefeatBonus;
        public MedalImages[] MedalImages=>medalImages;
        
        [SerializeField] private float toScoreCountUpDelay = 3;
        [SerializeField] private float bgmFadeOutDuration = 3;
        [SerializeField] private float bgmFadeInDuration = 2;
        [SerializeField] private float scoreCountUpDuration = 4;
        [SerializeField] private float bossDefeatBonusEnterDuration = 0.5f;
        [SerializeField] private float bossDefeatBonusCountUpDuration = 1;
        [SerializeField] private float bossDefeatBonusCountUpDelay = 1;
        [SerializeField] private float bossDefeatBonusFadeInDelay = 1;
        [SerializeField] private float canOnTriggerFinishGameDelay = 2;
        [SerializeField] private float medalEnterDelay = 1;
        [SerializeField] private float medalEnterDuration = 1;
        [SerializeField] private float medalInitScaleFactor = 1.3f;
        [SerializeField] private float toTextDelay = 2;
        [SerializeField] private int bossDefeatBonus = 5000;
        [SerializeField] private MedalImages[] medalImages;
    }
}