using System.Linq;
using InGame.Colate.View;
using InGame.Player.View;
using InGame.Database.ScriptableData;
using UnityEngine;

namespace InGame.Database
{
    public class UIData
    {
        public Canvas Canvas { get; }
        public PlayerStatusView PlayerStatusView { get; }
        public Vector2[] PlayerStatusDataPos { get; }
        public ColateStatusView ColateStatusView { get; }
        public Vector2 ColateStatusDataPos { get; }
        

        public UIData(StageUIScriptableData stageUIScriptableData, Canvas canvas)
        {
            PlayerStatusView = stageUIScriptableData.PlayerStatusView;
            ColateStatusView = stageUIScriptableData.ColateStatusView;
            Canvas = canvas;
            PlayerStatusDataPos = stageUIScriptableData.StatusDataPositions
                .OrderBy(position => position.x)
                .ToArray();
            ColateStatusDataPos = ColateStatusDataPos;
        }

        public UIData Clone()
        {
            return MemberwiseClone() as UIData;
        }
    }
}