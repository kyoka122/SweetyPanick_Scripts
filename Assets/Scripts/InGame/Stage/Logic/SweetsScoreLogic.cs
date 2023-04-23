using System;
using System.Collections.Generic;
using InGame.Stage.Entity;
using InGame.Stage.View;
using UniRx;
using Utility;

namespace InGame.Stage.Logic
{
    public class SweetsScoreLogic:IDisposable
    {
        private readonly ScoreEntity _scoreEntity;
        private readonly ISweets[] _sweets;
        private readonly List<IDisposable> _disposables;
        private readonly ScoreView _scoreView;

        public SweetsScoreLogic(ScoreEntity scoreEntity, ISweets[] sweets,ScoreView scoreView,
            PopTextGeneratorView popTextGeneratorView)
        {
            _scoreEntity = scoreEntity;
            _sweets = sweets;
            _scoreView = scoreView;
            _disposables = new List<IDisposable>();
            _scoreEntity.SetScoreTextPool(new ObjectPool<PopText>(popTextGeneratorView));
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            foreach (var sweets in _sweets)
            {
                _disposables.Add(
                    sweets.onFix
                        .SkipLatestValueOnSubscribe()
                        .Subscribe(async value =>
                        {
                            int score = _scoreEntity.GetScore(sweets.type, sweets.scoreType);
                            PopText popText = _scoreEntity.popTextPool.GetObject(sweets.GetScorePos());
                            
                            if (value)
                            {
                                _scoreEntity.AddSweetsScore(score);
                                popText.SetOutLineColor(_scoreEntity.ScoreUpColor);
                                await popText.Pop(score.ToString(),_scoreEntity.PopScoreDistance,_scoreEntity.PopScoreEnterDuration, 
                                    _scoreEntity.PopScoreExitDuration);
                            }
                            else
                            {
                                score *= -1;
                                _scoreEntity.AddSweetsScore(score);
                                popText.SetOutLineColor(_scoreEntity.ScoreDownColor);
                                await popText.PopRumble(score.ToString(),_scoreEntity.PopScoreDistance,_scoreEntity.PopScoreEnterDuration, 
                                    _scoreEntity.PopScoreExitDuration,_scoreEntity.PopScoreRumblePower);
                            }

                            _scoreEntity.popTextPool.ReleaseObject(popText);
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