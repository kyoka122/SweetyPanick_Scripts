using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UniRx;
using UnityEngine;

namespace Utility.TransitionFade
{
    //MEMO: 参考：https://github.com/TripleAt/ShaderFade

    [Serializable]
    public class Transition : IUtilTransition
    {
        private float TransitionRate { get; set; }
        public bool IsFadeIn { get; private set; }
        public bool IsFadeOut { get; private set; }
        public Tween fadeTween { get; private set; }

        private static readonly int Property = Shader.PropertyToID("_CutOff");

        private readonly Material _transitionMaterial;
        private readonly Component _container;

        public delegate void Callback();

        private Callback _callbackComplete;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transitionMaterial"></param>
        /// <param name="container"></param>
        /// <param name="firstTransition"></param>
        public Transition(Material transitionMaterial, Component container, float firstTransition)
        {
            _transitionMaterial = transitionMaterial;
            _container = container;
            Init(firstTransition);
        }

        private void Init(float initTransition)
        {
            if (_transitionMaterial == null)
            {
                Debug.LogWarning("Transition.cs:マテリアルが設定されてない");
                return;
            }

            TransitionRate = initTransition;

            this.ObserveEveryValueChanged(x => TransitionRate).Subscribe(
                _ => { _transitionMaterial.SetFloat(Property, TransitionRate); }
            ).AddTo(_container);
        }
        
        public bool IsActiveFadeIn()
        {
            return IsFadeIn;
        }

        public bool IsActiveFadeOut()
        {
            return IsFadeOut;
        }

        public void TransitionFadeInCondition()
        {
            TransitionRate = 0;
        }
        
        public void TransitionFadeOutCondition()
        {
            TransitionRate = 1;
        }

        /// <summary>
        /// カットアウトさせる
        /// </summary>
        /// <param name="startVal"></param>
        /// <param name="endVal"></param>
        /// <param name="duration"></param>
        public IUtilTransition Fade(float startVal, float endVal, float duration)
        {
            TransitionRate = startVal;
            fadeTween = DOTween
                .To(() => TransitionRate, (x) => TransitionRate = x, endVal, duration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    IsFadeIn = false;
                    IsFadeOut = false;
                    _callbackComplete?.Invoke();
                });
            return this;
        }
        
        public IUtilTransition Fade(float startVal, float endVal, float duration,Ease ease)
        {
            TransitionRate = startVal;
            fadeTween = DOTween
                .To(() => TransitionRate, (x) => TransitionRate = x, endVal, duration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    IsFadeIn = false;
                    IsFadeOut = false;
                    _callbackComplete?.Invoke();
                });
            return this;
        }


        /// <summary>
        /// 完了
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IUtilTransition Complete(Callback action)
        {
            _callbackComplete = action;
            return this;
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        public IUtilTransition FadeOut(float duration,Ease easeMode)
        {
            IsFadeOut = true;
            return Fade(0, 1, duration,easeMode);
        }
        
        /// <summary>
        /// フェードアウト
        /// </summary>
        public IUtilTransition FadeOut(float duration)
        {
            IsFadeOut = true;
            return Fade(0, 1, duration,Ease.InOutSine);
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        public IUtilTransition FadeIn(float duration)
        {
            IsFadeIn = true;
            return Fade(1, 0, duration,Ease.InOutSine);

        }
        
        /// <summary>
        /// フェードイン
        /// </summary>
        public IUtilTransition FadeIn(float duration,Ease easeMode)
        {
            IsFadeIn = true;
            return Fade(1, 0, duration,easeMode);

        }
    }
}