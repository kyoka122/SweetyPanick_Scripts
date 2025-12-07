using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Loader.View
{
    /// <summary>
    /// ゲームオーバーを表示して遷移する時のView
    /// </summary>
    public class GameOverView : MonoBehaviour
    {
        public float backGroundXDefaultPosition { get; private set; }
        public RectTransform BackgroundRectTransform => backgroundRectTransform;

        [SerializeField, Tooltip("マテリアルを自由に操作していい場合はそれぞれのフェード対象にこのマテリアルをアタッチ")]
        private Material gameOverMaterial;

        [SerializeField, Tooltip("colorを自由に操作していいフェード対象の場合はこの配列に追加")]
        private MaskableGraphic[] gameOverGraphics;

        [SerializeField] private RectTransform backgroundRectTransform;

        public void Init()
        {
            InitOnFadeIn();
            SetActive(false);
            backGroundXDefaultPosition = backgroundRectTransform.localPosition.x;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        /// <summary>
        /// フェードインされた状態にセットする
        /// </summary>
        public void InitOnFadeIn()
        {
            foreach (var gameOverGraphic in gameOverGraphics)
            {
                Color color = gameOverGraphic.color;
                gameOverGraphic.color = new Color(color.r, color.g, color.b, 0);
            }

            Color materialColor = gameOverMaterial.color;
            gameOverMaterial.color = new Color(materialColor.r, materialColor.g, materialColor.b, 0);
        }

        /// <summary>
        /// フェードインさせる
        /// </summary>
        public async UniTask FadeIn(float duration, CancellationToken token)
        {
            CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                this.GetCancellationTokenOnDestroy(), token).Token;
            await DOTween.Sequence()
                .Append(gameOverGraphics.DOSameFades(1, duration))
                .Join(gameOverMaterial.DOFade(1, duration))
                .ToUniTask(cancellationToken: linkedToken);
        }

        /// <summary>
        /// フェードアウトされた状態にセットする
        /// </summary>
        public void SetFadeOut()
        {
            foreach (var gameOverGraphic in gameOverGraphics)
            {
                Color color = gameOverGraphic.color;
                gameOverGraphic.color = new Color(color.r, color.g, color.b, 0);
            }

            Color materialColor = gameOverMaterial.color;
            gameOverMaterial.color = new Color(materialColor.r, materialColor.g, materialColor.b, 1);
        }

        private void OnDestroy()
        {
            InitOnFadeIn();
        }
    }
}