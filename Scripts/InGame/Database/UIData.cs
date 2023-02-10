using InGame.Player.View;
using InGame.Database.ScriptableData;
using UnityEngine;

namespace InGame.Database
{
    public class UIData
    {
        public Canvas Canvas { get; }
        public PlayerStatusView PlayerStatusView { get; }

        public Vector2[] StatusDataPos { get; }

        public UIData(StageUIScriptableData stageUIScriptableData, Canvas canvas)
        {
            PlayerStatusView = stageUIScriptableData.PlayerStatusView;
            Canvas = canvas;
            StatusDataPos = stageUIScriptableData.StatusDataPositions;
        }

        public UIData Clone()
        {
            return MemberwiseClone() as UIData;
        }
    }
}