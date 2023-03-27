using System.Linq;
using InGame.Colate.View;
using InGame.Player.View;
using InGame.Database.ScriptableData;
using UnityEngine;

namespace InGame.Database
{
    public class StageUIData
    {
        public Canvas Canvas { get; }
        public float ScoreCountUpDuration { get; }
        public PlayerStatusView PlayerStatusView { get; }
        public Vector2[] PlayerStatusDataPos { get; }
        public ColateStatusView ColateStatusView { get; }
        public Vector2 ColateStatusDataPos { get; }
        
        public GameObject Key { get; }
        public float ActiveKeyEffectDuration { get; }
        public GameObject TrailEffect { get; }

        public StageUIData(StageUIScriptableData stageUIScriptableData, Canvas canvas,GameObject key,GameObject trailEffect)
        {
            PlayerStatusView = stageUIScriptableData.PlayerStatusView;
            ScoreCountUpDuration = stageUIScriptableData.ScoreCountUpDuration;
            ColateStatusView = stageUIScriptableData.ColateStatusView;
            TrailEffect = trailEffect;
            ActiveKeyEffectDuration = stageUIScriptableData.ActiveKeyEffectDuration;
            Canvas = canvas;
            PlayerStatusDataPos = stageUIScriptableData.PlayerStatusDataPositions
                .OrderBy(position => position.x)
                .ToArray();
            ColateStatusDataPos = stageUIScriptableData.ColateStatusDataPosition;
            Key = key;
        }

        public StageUIData Clone()
        {
            return MemberwiseClone() as StageUIData;
        }
    }
}