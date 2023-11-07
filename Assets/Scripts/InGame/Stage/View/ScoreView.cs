using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace InGame.Stage.View
{
    public class ScoreView:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Slider scoreSlider;
        
        private int _prevScore = 0;
        private int _countUpScore = 0;
        private int _maxScore = 0;
        private TweenerCore<int, int, NoOptions> _animationTweenCore;

        public void Init(int score,int maxScore)
        {
            scoreText.text = score.ToString("D6");
            _maxScore = maxScore;
            SetSlider(score, _maxScore);
        }
        
        public void PlayScoreAnimation(int score,float duration)
        {
            if (_animationTweenCore is {active: true})
            {
                _animationTweenCore.Kill();
                _prevScore = _countUpScore;
            }

            _animationTweenCore =
                DOTween.To(() => _prevScore, upScore =>
                        {
                            _countUpScore = upScore;
                            scoreText.text = upScore.ToString("D6");
                            SetSlider(upScore, _maxScore);
                        },
                        score, duration)
                    .OnComplete(() =>
                    {
                        _prevScore = score;
                    });
        }

        private void SetSlider(float score,float maxScore)
        {
            scoreSlider.value = MyMathf.InRange(score / maxScore, 0, 1);
        }
    }
}