using InGame.Colate.View;
using InGame.Player.View;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "UIScriptableData", menuName = "ScriptableObjects/UIScriptableData")]
    public class StageUIScriptableData:ScriptableObject
    {
        public PlayerStatusView PlayerStatusView => playerStatusView;
        public Vector2[] PlayerStatusDataPositions => playerStatusDataPositions;
        public ColateStatusView ColateStatusView => colateStatusView;
        public Vector2 ColateStatusDataPosition => colateStatusDataPositions;
        public float ScoreCountUpDuration => scoreCountUpDuration;
        public float CursorInterval => cursorInterval;
        public float ActiveKeyEffectDuration => activeKeyEffectDuration;
        public float PopScoreDistance => popScoreDistance;
        public float PopScoreEnterDuration => popScoreEnterDuration;
        public float PopScoreExitDuration => popScoreExitDuration;
        public float PopScoreRumblePower => popScoreRumblePower;
        public Color ScoreDownColor => scoreDownColor;
        public Color ScoreUpColor => scoreUpColor;


        [SerializeField] private PlayerStatusView playerStatusView;
        [SerializeField] private ColateStatusView colateStatusView;
        [SerializeField] private float scoreCountUpDuration = 0.3f;
        [SerializeField] private float popScoreDistance = 0.2f;
        [SerializeField] private float popScoreEnterDuration = 0.2f;
        [SerializeField] private float popScoreExitDuration = 0.4f;
        [SerializeField] private float popScoreRumblePower=0.2f;
        [SerializeField] private Color scoreDownColor;
        [SerializeField] private Color scoreUpColor;
        [SerializeField] private Vector2[] playerStatusDataPositions;
        [SerializeField] private Vector2 colateStatusDataPositions;
        [SerializeField] private float cursorInterval = 200;
        [SerializeField] private float activeKeyEffectDuration = 1;
    }
}