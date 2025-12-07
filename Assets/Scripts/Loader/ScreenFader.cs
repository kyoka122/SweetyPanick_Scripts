using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utility.TransitionFade;

namespace Loader
{
    /// <summary>
    /// Dissolveシェーダーを用いたフェード用クラス
    /// </summary>
    public class ScreenFader
    {
        private readonly Transition _transition;

        public ScreenFader(Image fadeImage)
        {
            Material fadeMaterial = new Material(fadeImage.material);
            fadeImage.material = fadeMaterial;
            _transition = new Transition(fadeMaterial, fadeImage, 1);
        }

        public ScreenFader(SpriteRenderer fadeSpriteRenderer)
        {
            Material fadeMaterial = new Material(fadeSpriteRenderer.material);
            fadeSpriteRenderer.material = fadeMaterial;
            _transition = new Transition(fadeMaterial, fadeSpriteRenderer, 1);
        }

        public async UniTask FadeIn(float duration, Ease ease, CancellationToken token)
        {
            try
            {
                _transition.FadeIn(duration, ease);
                await UniTask.WaitWhile(() => _transition.IsActiveFadeIn(), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                _transition.fadeTween.Kill();
                throw;
            }
        }

        public async UniTask FadeOut(float duration, Ease ease, CancellationToken token)
        {
            try
            {
                _transition.FadeOut(duration, ease);
                await UniTask.WaitWhile(() => _transition.IsActiveFadeOut(), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                _transition.fadeTween.Kill();
                throw;
            }
        }

        public async UniTask FadeInOut(float fadeInDuration, float fadeOutDuration, float fadingDuration, Ease ease,
            CancellationToken token, Action afterFadeInAction)
        {
            try
            {
                await FadeIn(fadeInDuration, ease, token);
                afterFadeInAction?.Invoke();
                await UniTask.Delay(TimeSpan.FromSeconds(fadingDuration), cancellationToken: token);
                await FadeOut(fadeOutDuration, ease, token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"FadeCanceled");
            }

        }

        /// <summary>
        /// FadeInする前の状態
        /// </summary>
        public void SetFadeInCondition()
        {
            _transition.TransitionFadeInCondition();
        }

        /// <summary>
        /// FadeOutする前の状態
        /// </summary>
        public void SetFadeOutCondition()
        {
            _transition.TransitionFadeOutCondition();
        }

        /// <summary>
        /// オブジェクトの破棄と同時にこのマテリアルも破棄する
        /// </summary>
        /// <returns></returns>
        public Material GetDisposeMaterial()
        {
            return _transition.GetDisposeMaterial();
        }
    }
}