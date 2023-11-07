using System;
using InGame.Database.ScriptableData;
using InGame.Stage.View;
using UnityEngine;
using Utility;

namespace MyApplication
{
    public class SceneSweetsCounter:MonoBehaviour
    {
        [SerializeField] private int stageNum;
        
        [SerializeField] private StageGimmickScriptableData stageGimmickScriptableData;
#if UNITY_EDITOR
        public void CountAndRegisterMaxSweetsScore()
        {
            ISweets[] sweets = GameObjectExtensions.FindInterface<ISweets>();
            int score = 0;
            foreach (var sweet in sweets)
            {
                if (!sweet.isActive||sweet.IsInitState!=FixState.Broken)
                {
                    continue;
                }
                score+=ScoreCalculator.GetScore(sweet.type, sweet.scoreType, stageGimmickScriptableData.NormalSweetsScore,
                    stageGimmickScriptableData.GoldSweetsScore, stageGimmickScriptableData.GimmickSweetsScore);
            }
            
            stageGimmickScriptableData.SetStageMaxScore(stageNum,score);
        }
#endif
    }
}