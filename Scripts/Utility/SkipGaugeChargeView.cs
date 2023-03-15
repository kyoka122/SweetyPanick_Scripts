using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    public class SkipGaugeChargeView:MonoBehaviour
    {
        [SerializeField] private Material material;
        [SerializeField] private MaskableGraphic[] graphics;
        [SerializeField] private Slider skipGauge;
        private IObservable<bool> _onSkip;
        
        private TweenerCore<float, float, FloatOptions> _skipGaugeTweenCore;
        private IDisposable _onSkipObserver;
        private bool _isPlayingSkipGaugeAnimation;


        public void Init(IObservable<bool> onSkip)
        {
            _onSkip = onSkip;
            skipGauge.value=0;
            SetObjectsAlphaColor(0);
        }
        
        public void ResetView(bool active)
        {
            SetObjectsAlphaColor(active ? 1 : 0);
            skipGauge.value = 0;
        }
        
        public void RegisterSkipObserver(Action onCompleteAction,Action onKilledAction,float gaugeDuration)
        {
            _onSkipObserver=_onSkip.Subscribe(on =>
            {
                switch (_isPlayingSkipGaugeAnimation)
                {
                    case false when on:
                        PlaySkipSliderAnimation(onCompleteAction,onKilledAction,gaugeDuration);
                        return;
                    case true when !on:
                        _skipGaugeTweenCore.Kill();
                        return;
                }
            });
        }

        public void ResetObserver(Action onCompleteAction,Action onKilledAction,float gaugeDuration)
        {
            _onSkipObserver.Dispose();
            RegisterSkipObserver(onCompleteAction,onKilledAction,gaugeDuration);
        }

        /// <summary>
        /// FadeIn
        /// </summary>
        /// <param name="duration"></param>
        public void PlayFadeInObjects(float duration)
        {
            DOTween.To(() => 0f, SetObjectsAlphaColor, 1,duration);
        }
        
        /// <summary>
        /// FadeIn
        /// </summary>
        /// <param name="duration"></param>
        public void PlayFadeOutObjects(float duration)
        {
            DOTween.To(() => 1f, SetObjectsAlphaColor, 0,duration);
        }

        private void PlaySkipSliderAnimation(Action onCompleteAction,Action onKilledAction,float duration)
        {
            SetObjectsAlphaColor(1);
            _skipGaugeTweenCore =
                DOTween.To(() => 0f, value => skipGauge.value = value, 1, duration)
                    .OnComplete(() =>
                    {
                        onCompleteAction?.Invoke();
                        Debug.Log($"OnSkip");
                    })
                    .OnKill(() =>
                    {
                        skipGauge.value = 0;
                        _isPlayingSkipGaugeAnimation = false;
                        onKilledAction?.Invoke();
                    });
            
            _isPlayingSkipGaugeAnimation = true;
        }

        private void SetObjectsAlphaColor(float alpha)
        {
            Color colorCache = material.color;
            material.color = new Color(colorCache.r, colorCache.g, colorCache.b, alpha);
            foreach (var skipGraphic in graphics)
            {
                colorCache = skipGraphic.color;
                skipGraphic.color = new Color(colorCache.r, colorCache.g, colorCache.b, alpha);
            }
        }
    }
}