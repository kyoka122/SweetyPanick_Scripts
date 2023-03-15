using UnityEngine;
using UnityEngine.UI;

namespace InGame.SceneLoader.View
{
    public class BlackFadeView:MonoBehaviour
    {
        [SerializeField] private Image blackFadeImage;

        public ScreenFader BlackScreenFader { get; private set; }
        
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

        public void InitOnFadeIn()
        {
            BlackScreenFader.SetFadeOutCondition();
        }
        
        public void OnDestroy()
        {
            if (BlackScreenFader?.GetDisposeMaterial()!=null)
            {
                Destroy(BlackScreenFader.GetDisposeMaterial());
            }
            InitOnFadeIn();
        }

    }
}