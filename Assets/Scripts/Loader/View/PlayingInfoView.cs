using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyApplication;
using UnityEngine;
using UnityEngine.UI;

namespace Loader.View
{
    /// <summary>
    /// Load画面を出しているときにゲーム説明を表示する際のView
    /// </summary>
    public class PlayingInfoView : MonoBehaviour
    {
        [SerializeField] private Image punchInfoImage;
        [SerializeField] private Image fixInfoImage;
        [SerializeField] private Image damageInfoImage;


        public void Init()
        {
            InitOnFadeIn();
        }

        public void InitOnFadeIn()
        {
            punchInfoImage.color = new Color(1, 1, 1, 0);
            fixInfoImage.color = new Color(1, 1, 1, 0);
            damageInfoImage.color = new Color(1, 1, 1, 0);
            
            punchInfoImage.gameObject.SetActive(false);
            fixInfoImage.gameObject.SetActive(false);
            damageInfoImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// 指定のゲーム説明をフェードインさせる
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="ease"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public UniTask FadeInInfo(PlayingInfoType type, float fadeDuration, Ease ease, CancellationToken token)
        {
            Image fadeImage = GetPlayInfoImage(type);
            fadeImage.gameObject.SetActive(true);
            return fadeImage.DOFade(1, fadeDuration)
                .SetEase(ease)
                .ToUniTask(cancellationToken: token);
        }

        /// <summary>
        /// 指定のゲーム説明をフェードアウトさせる
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="ease"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public UniTask FadeOutInfo(PlayingInfoType type, float fadeDuration, Ease ease, CancellationToken token)
        {
            Image fadeImage = GetPlayInfoImage(type);
            return fadeImage.DOFade(0, fadeDuration)
                .SetEase(ease)
                .OnComplete(()=>fadeImage.gameObject.SetActive(false))
                .ToUniTask(cancellationToken: token);
        }

        /// <summary>
        /// 指定のゲーム説明をフェードアウトさせる
        /// </summary>
        /// <param name="type"></param>
        public void UnloadTexture(PlayingInfoType type)
        {
            Sprite sprite = GetPlayInfoImage(type).sprite;
            if (sprite != null)
            {
                Resources.UnloadAsset(sprite.texture);
            }
        }

        private Image GetPlayInfoImage(PlayingInfoType type)
        {
            return type switch
            {
                PlayingInfoType.Fix => fixInfoImage,
                PlayingInfoType.Damage => damageInfoImage,
                PlayingInfoType.Punch => punchInfoImage,
                _ => null,
            };
        }

        public void OnDestroy()
        {
            InitOnFadeIn();
        }
    }
}