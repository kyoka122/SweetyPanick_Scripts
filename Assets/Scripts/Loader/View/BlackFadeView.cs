using UnityEngine;
using UnityEngine.UI;

namespace Loader.View
{
    /// <summary>
    /// ブラックスクリーンで遷移する時のView
    /// </summary>
    public class BlackFadeView : MonoBehaviour
    {
        public ScreenFader BlackScreenFader { get; private set; }

        [SerializeField] private Image blackFadeImage;

        public void Init()
        {
            BlackScreenFader = new ScreenFader(blackFadeImage);
            InitOnFadeIn();
            SetActive(false);
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
            BlackScreenFader.SetFadeOutCondition();
        }

        public void OnDestroy()
        {
            if (BlackScreenFader?.GetDisposeMaterial() != null)
            {
                Destroy(BlackScreenFader.GetDisposeMaterial());
            }

            InitOnFadeIn();
        }

    }
}