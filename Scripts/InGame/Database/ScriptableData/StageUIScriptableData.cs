using InGame.Player.View;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "UIScriptableData", menuName = "ScriptableObjects/UIScriptableData")]
    public class StageUIScriptableData:ScriptableObject
    {
        public PlayerStatusView PlayerStatusView => playerStatusView;
        public Vector2[] StatusDataPositions => statusDataPositions;
        
        [SerializeField] private PlayerStatusView playerStatusView;
        [SerializeField] private Vector2[] statusDataPositions;
        
    }
}