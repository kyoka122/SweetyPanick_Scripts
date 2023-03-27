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
        

        [SerializeField] private PlayerStatusView playerStatusView;
        [SerializeField] private ColateStatusView colateStatusView;
        [SerializeField] private float scoreCountUpDuration = 0.3f;
        [SerializeField] private Vector2[] playerStatusDataPositions;
        [SerializeField] private Vector2 colateStatusDataPositions;
        [SerializeField] private float cursorInterval = 200;
        [SerializeField] private float activeKeyEffectDuration = 1;

    }
}