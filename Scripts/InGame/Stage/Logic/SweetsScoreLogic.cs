using System;
using System.Collections.Generic;
using InGame.Stage.Entity;
using InGame.Stage.View;
using UniRx;

namespace InGame.Stage.Logic
{
    public class SweetsScoreLogic:IDisposable
    {
        private readonly ScoreEntity _scoreEntity;
        private readonly ISweets[] _sweets;
        private readonly List<IDisposable> _disposables;
        private readonly ScoreView _scoreView;

        public SweetsScoreLogic(ScoreEntity scoreEntity, ISweets[] sweets,ScoreView scoreView)
        {
            _scoreEntity = scoreEntity;
            _sweets = sweets;
            _scoreView = scoreView;
            _disposables = new List<IDisposable>();
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            foreach (var sweets in _sweets)
            {
                _disposables.Add(
                    sweets.onFix
                        .SkipLatestValueOnSubscribe()
                        .Subscribe(value =>
                        {
                            if (value)
                            {
                                _scoreEntity.AddSweetsScore(sweets.type);
                                return;
                            }

                            _scoreEntity.LoseSweetsScore(sweets.type);
                        }));
            }

            _scoreEntity.Score
                .Subscribe(score=> _scoreView.PlayScoreAnimation(score,_scoreEntity.ScoreCountUpDuration))
                .AddTo(_scoreView);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}