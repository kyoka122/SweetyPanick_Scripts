using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Loader.View
{
    /// <summary>
    /// キャラを表示するタイプのLoad画面で遷移する時View
    /// </summary>
    public class LoadScreenView : MonoBehaviour
    {
        public Material[] LoadScreenMaterials { get; private set; }
        public ScreenFader LoadScreenFader { get; private set; }

        public RectTransform LoadBackgroundRectTransform => loadBackgroundRectTransform;
        public RectTransform WaveTransform => waveTransform;

        public float loadBackGroundXDefaultPosition { get; private set; }
        public float LoadBackGroundWidth => loadBackgroundRectTransform.sizeDelta.x;
        public float waveObjXDefaultPosition { get; private set; }

        private Transform _loadBackGroundObjTransform;
        private Transform _waveObjTransform;

        [SerializeField] private Image loadDissolveFaderImage;
        [SerializeField] private RectTransform loadBackgroundRectTransform;
        [SerializeField] private RectTransform waveTransform;
        [SerializeField] private Material playersMaterial; //MEMO: プレイヤーをまとめてフェードしたいのでマテリアルを変更する
        [SerializeField] private Material loadUIMaterial;

        public void Init()
        {
            LoadScreenFader = new ScreenFader(loadDissolveFaderImage);
            LoadScreenMaterials = new[] { loadUIMaterial, playersMaterial };
            loadBackGroundXDefaultPosition = loadBackgroundRectTransform.localPosition.x;
            waveObjXDefaultPosition = waveTransform.localPosition.x;

            InitOnFadeIn();
            SetActiveLoadScreenParent(false);
        }

        /// <summary>
        /// Load画面全体の親オブジェクトのActive切り替え
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveLoadScreenParent(bool active)
        {
            gameObject.SetActive(active);
        }

        /// <summary>
        /// フェードインさせる
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="ease"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public UniTask FadeInLoadObjects(Material[] materials, float fadeDuration, Ease ease, CancellationToken token)
        {
            return materials.DOSameFades(1, fadeDuration)
                .SetEase(ease)
                .ToUniTask(cancellationToken: token);
        }

        /// <summary>
        /// フェードアウトさせる
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="ease"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public UniTask FadeOutLoadObjects(Material[] materials, float fadeDuration, Ease ease, CancellationToken token)
        {
            return materials.DOSameFades(0, fadeDuration)
                .SetEase(ease)
                .ToUniTask(cancellationToken: token);
        }

        /// <summary>
        /// フェードインされた状態にセットする
        /// </summary>
        public void InitOnFadeIn()
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            FadeOutLoadObjects(LoadScreenMaterials, 0, Ease.Unset, token);
            LoadScreenFader.FadeOut(0, Ease.Unset, token).Forget();
        }

        public void OnDestroy()
        {
            if (LoadScreenFader?.GetDisposeMaterial() != null)
            {
                Destroy(LoadScreenFader.GetDisposeMaterial());
            }

            InitOnFadeIn();
        }
    }
}