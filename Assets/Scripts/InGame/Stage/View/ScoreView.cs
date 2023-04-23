using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace InGame.Stage.View
{
    public class ScoreView:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        private int _prevScore = 0;
        private int _countUpScore = 0;
        private TweenerCore<int, int, NoOptions> _animationTweenCore;

        public void Init(int score)
        {
            scoreText.text = score.ToString("D6");
        }
        
        public void PlayScoreAnimation(int score,float duration)
        {
            if (_animationTweenCore is {active: true})
            {
                _animationTweenCore.Kill();
                _prevScore = _countUpScore;
            }
            _animationTweenCore = DOTween.To(() => _prevScore, upScore =>
                {
                    _countUpScore = upScore;
                    scoreText.text = upScore.ToString("D6");
                },
                score, duration);
        }
    }
}