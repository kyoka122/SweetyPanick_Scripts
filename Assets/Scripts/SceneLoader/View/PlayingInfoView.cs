using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyApplication;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.SceneLoader.View
{
    public class PlayingInfoView:MonoBehaviour
    {
        [SerializeField] private Image punchInfoImage;
        [SerializeField] private Image fixInfoImage;
        [SerializeField] private Image damageInfoImage;
        
        
        public void Init()
        {
            InitOnFadeIn();
            //SetActive(false);
        }
        
        // public void SetActive(bool active)
        // {
        //     gameObject.SetActive(active);
        // }

        public void InitOnFadeIn()
        {
            punchInfoImage.color = new Color(1, 1, 1, 0);
            fixInfoImage.color = new Color(1, 1, 1, 0);
            damageInfoImage.color = new Color(1, 1, 1, 0);
        }
        
        /*public void InitOnFadeOut()
        {
            _punchInfoFader.SetFadeInCondition();
            _fixInfoFader.SetFadeInCondition();
            _damageInfoFader.SetFadeInCondition();
        }*/
        
        public UniTask FadeInInfo(PlayingInfoType type,float fadeDuration,Ease ease,CancellationToken token)
        {
            Image fadeImage = GetPlayInfoImage(type);
            return fadeImage.DOFade(1, fadeDuration)
                .SetEase(ease)
                .ToUniTask(cancellationToken: token);
        }
        
        public UniTask FadeOutInfo(PlayingInfoType type,float fadeDuration,Ease ease,CancellationToken token)
        {
            Image fadeImage = GetPlayInfoImage(type);
            return fadeImage.DOFade(0, fadeDuration)
                .SetEase(ease)
                .ToUniTask(cancellationToken: token);
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