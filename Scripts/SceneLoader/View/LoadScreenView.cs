using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace InGame.SceneLoader.View
{
    public class LoadScreenView:MonoBehaviour
    {
        public Material[] LoadScreenMaterials{ get; private set; }
        public ScreenFader LoadScreenFader { get; private set; }
        
        public RectTransform LoadBackgroundRectTransform => loadBackgroundRectTransform;
        public RectTransform WaveTransform => waveTransform;

        public float loadBackGroundXDefaultPosition { get; private set; }
        public float LoadBackGroundWidth => loadBackgroundRectTransform.sizeDelta.x;
        public float waveObjXDefaultPosition { get; private set; }
        public float WaveObjXDefaultWidth => waveTransform.sizeDelta.x;
       
        
        private Transform _loadBackGroundObjTransform;
        private Transform _waveObjTransform;

        [SerializeField] private Image loadDissolveFaderImage;
        [SerializeField] private RectTransform loadBackgroundRectTransform;
        [SerializeField] private RectTransform waveTransform;
        [SerializeField] private Material playersMaterial;//MEMO: プレイヤーをまとめてフェードしたいのでマテリアルを変更する
        [SerializeField] private Material loadUIMaterial;

        public void Init()
        {
            LoadScreenFader = new ScreenFader(loadDissolveFaderImage);
            LoadScreenMaterials = new[] {loadUIMaterial, playersMaterial};
            loadBackGroundXDefaultPosition = loadBackgroundRectTransform.localPosition.x;
            waveObjXDefaultPosition = waveTransform.localPosition.x;
            
            InitOnFadeIn();
            SetActiveLoadScreenParent(false);
        }
        
        public void SetActiveLoadScreenParent(bool active)
        {
            gameObject.SetActive(active);
        }
        
        public UniTask FadeInLoadObjects(Material[] materials,float fadeDuration,Ease ease,CancellationToken token)
        {
            return materials.DOSameFades(1, fadeDuration)
                .SetEase(ease)
                .ToUniTask(cancellationToken: token);
        }
        
        public UniTask FadeOutLoadObjects(Material[] materials,float fadeDuration,Ease ease,CancellationToken token)
        {
            return materials.DOSameFades(0, fadeDuration)
                .SetEase(ease)
                .ToUniTask(cancellationToken: token);
        }

        public void InitOnFadeIn()
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            FadeOutLoadObjects(LoadScreenMaterials, 0, Ease.Unset, token);
            LoadScreenFader.FadeOut(0, Ease.Unset, token).Forget();
        }
        
        public void OnDestroy()
        {
            if (LoadScreenFader?.GetDisposeMaterial()!=null)
            {
                Destroy(LoadScreenFader.GetDisposeMaterial());
            }

            InitOnFadeIn();
        }
    }
}