using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Loader.Entity;
using Loader.View;

namespace Loader.Logic
{
    /// <summary>
    /// ブラックスクリーンで遷移する時のLogic
    /// </summary>
    public class BlackFadeLogic
    {
        public bool isFadeInState { get; private set; }
        public bool isPlayingFade { get; private set; }

        private readonly LoadEntity _loadEntity;
        private readonly BlackFadeView _blackFadeView;
        private readonly LoadCameraView _loadCameraView;

        public BlackFadeLogic(LoadEntity loadEntity, BlackFadeView blackFadeView, LoadCameraView loadCameraView)
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
            isPlayingFade = true;
            _blackFadeView.InitOnFadeIn();
            _blackFadeView.SetActive(true);
            _loadCameraView.SetActive(true);
            await _blackFadeView.BlackScreenFader.FadeIn(_loadEntity.BlackScreenFadeInDuration, Ease.Unset, token);
            isFadeInState = true;
            isPlayingFade = false;
        }

        /// <summary>
        /// ブラックスクリーンによるフェードインが完了していればフェードアウトを開始する
        /// </summary>
        public async UniTask PlayBlackFadeOut(CancellationToken token)
        {
            isPlayingFade = true;
            await _blackFadeView.BlackScreenFader.FadeOut(_loadEntity.BlackScreenFadeOutDuration, Ease.Unset, token);
            _blackFadeView.SetActive(false);
            _loadCameraView.SetActive(false);
            isFadeInState = false;
            isPlayingFade = false;
        }
    }
}