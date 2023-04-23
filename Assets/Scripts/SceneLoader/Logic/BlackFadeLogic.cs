using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.SceneLoader.Entity;
using InGame.SceneLoader.View;
using UnityEngine;

namespace InGame.SceneLoader.Logic
{
    public class BlackFadeLogic
    {
        public bool isFadeIn { get; private set; }
        public bool isFading { get; private set; }
        
        private readonly LoadEntity _loadEntity;
        private readonly BlackFadeView _blackFadeView;
        private readonly LoadCameraView _loadCameraView;

        public BlackFadeLogic(LoadEntity loadEntity,BlackFadeView blackFadeView,LoadCameraView loadCameraView)
        {
            _loadEntity = loadEntity;
            _blackFadeView = blackFadeView;
            _loadCameraView = loadCameraView;
        }

        /// <summary>
        /// ブラックスクリーンによるフェードインをしていなければフェードイン演出を開始する
        /// </summary>
        public async UniTask PlayFadeIn(CancellationToken token)
        {
            isFading = true;
            _blackFadeView.InitOnFadeIn();
            _blackFadeView.SetActive(true);
            _loadCameraView.SetActive(true);
            await _blackFadeView.BlackScreenFader.FadeIn(_loadEntity.BlackScreenFadeInDuration,Ease.Unset,token);
            isFadeIn = true;
            isFading = false;
        }
        
        /// <summary>
        /// ブラックスクリーンによるフェードインをしていなければフェードイン演出を開始する
        /// </summary>
        public async UniTask PlayFadeIn(Vector2 fadeCenter,CancellationToken token)
        {
            isFading = true;
            _blackFadeView.SetActive(true);
            _loadCameraView.SetActive(true);
            await _blackFadeView.BlackScreenFader.FadeIn(_loadEntity.BlackScreenFadeInDuration,Ease.Unset,token);
            isFadeIn = true;
            isFading = false;
        }
        
        
        /// <summary>
        /// ブラックスクリーンによるフェードインが完了していればフェードアウトを開始する
        /// </summary>
        public async UniTask TryPlayBlackFadeOut(CancellationToken token)
        {
            isFading = true;
            await _blackFadeView.BlackScreenFader.FadeOut(_loadEntity.BlackScreenFadeOutDuration,Ease.Unset,token);
            _blackFadeView.SetActive(false);
            _loadCameraView.SetActive(false);
            isFadeIn = false;
            isFading = false;
        }
    }
}