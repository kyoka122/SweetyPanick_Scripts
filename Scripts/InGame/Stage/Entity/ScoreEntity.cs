using InGame.Database;
using MyApplication;
using UniRx;

namespace InGame.Stage.Entity
{
    public class ScoreEntity
    {
        public float ScoreCountUpDuration => _inGameDatabase.GetUIData().ScoreCountUpDuration;
        public IReadOnlyReactiveProperty<int> Score => _score;
        private readonly ReactiveProperty<int> _score ;
        private readonly InGameDatabase _inGameDatabase;

        public ScoreEntity(InGameDatabase inGameDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _score = new ReactiveProperty<int>();
        }

        public void AddSweetsScore(SweetsType type)
        {
            AllStageData data = _inGameDatabase.GetAllStageData();
            switch (type)
            {
                case SweetsType.Sweets:
                    data.score += _inGameDatabase.GetStageGimmickData().NormalSweetsScore;
                    break;
                case SweetsType.GimmickSweets:
                    data.score += _inGameDatabase.GetStageGimmickData().GimmickSweetsScore;
                    break;
            }

            _score.Value = data.score;
            _inGameDatabase.SetAllStageData(data);
        }

        public void LoseSweetsScore(SweetsType type)
        {
            AllStageData data = _inGameDatabase.GetAllStageData();
            switch (type)
            {
                case SweetsType.Sweets:
                    data.score -= _inGameDatabase.GetStageGimmickData().NormalSweetsScore;
                    break;
                case SweetsType.GimmickSweets:
                    data.score -= _inGameDatabase.GetStageGimmickData().GimmickSweetsScore;
                    break;
            }

            _score.Value = data.score;
            _inGameDatabase.SetAllStageData(data);
        }
    }
}