using System;

namespace MyApplication
{
    public static class ScoreCalculator
    {
        public static int GetScore(SweetsType type, SweetsScoreType scoreType, int normalSweetsScore,
            int goldSweetsScore, int gimmickSweetsScore)
        {
            return type switch
            {
                SweetsType.Sweets => scoreType == SweetsScoreType.Gold
                    ? goldSweetsScore
                    : normalSweetsScore,
                SweetsType.GimmickSweets => gimmickSweetsScore,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}