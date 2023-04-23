using System;
using InGame.Database;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;

namespace InGame.Stage.Entity
{
    public class ScoreEntity
    {
        public float ScoreCountUpDuration => _inGameDatabase.GetUIData().ScoreCountUpDuration;
        public float PopScoreDistance => _inGameDatabase.GetUIData().PopScoreDistance;
        public float PopScoreEnterDuration => _inGameDatabase.GetUIData().PopScoreEnterDuration;
        public float PopScoreExitDuration => _inGameDatabase.GetUIData().PopScoreExitDuration;
        public float PopScoreRumblePower => _inGameDatabase.GetUIData().PopScoreRumblePower;
        public Color ScoreDownColor => _inGameDatabase.GetUIData().ScoreDownColor;
        public Color ScoreUpColor => _inGameDatabase.GetUIData().ScoreUpColor;
        
        public ObjectPool<PopText> popTextPool { get; private set; }
        public IReadOnlyReactiveProperty<int> Score => _score;
        
        
        private readonly ReactiveProperty<int> _score ;
        private readonly InGameDatabase _inGameDatabase;
        private readonly bool _isLimitedChangeScore;

        public ScoreEntity(InGameDatabase inGameDatabase,StageArea stageArea)
        {
            _inGameDatabase = inGameDatabase;
            _score = new ReactiveProperty<int>(inGameDatabase.GetAllStageData().score);
            _isLimitedChangeScore = stageArea == StageArea.ColateStageFirst;
        }
        
        public int GetScore(SweetsType type,SweetsScoreType scoreType)
        {
            return type switch
            {
                SweetsType.Sweets => scoreType == SweetsScoreType.Gold
                    ? _inGameDatabase.GetStageGimmickData().GoldSweetsScore
                    : _inGameDatabase.GetStageGimmickData().NormalSweetsScore,
                SweetsType.GimmickSweets => _inGameDatabase.GetStageGimmickData().GimmickSweetsScore,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public void AddSweetsScore(int score)
        {
            if (_isLimitedChangeScore)
            {
                return;
            }

            AllStageData data = _inGameDatabase.GetAllStageData();
            data.score += score;
            _score.Value = data.score;
            _inGameDatabase.SetAllStageData(data);
        }

        public void SetScoreTextPool(ObjectPool<PopText> newPopTextPool)
        {
            popTextPool = newPopTextPool;
        }
    }
}