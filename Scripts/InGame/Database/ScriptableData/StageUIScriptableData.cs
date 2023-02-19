using InGame.Colate.View;
using InGame.Player.View;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "UIScriptableData", menuName = "ScriptableObjects/UIScriptableData")]
    public class StageUIScriptableData:ScriptableObject
    {
        public PlayerStatusView PlayerStatusView => playerStatusView;
        public Vector2[] StatusDataPositions => statusDataPositions;
        
        public ColateStatusView ColateStatusView => colateStatusView;
        public Vector2 ColateStatusPosition => colateStatusPositions;
        
        [SerializeField] private PlayerStatusView playerStatusView;
        [SerializeField] private ColateStatusView colateStatusView;
        [SerializeField] private Vector2[] statusDataPositions;
        [SerializeField] private Vector2 colateStatusPositions;
        
    }
}