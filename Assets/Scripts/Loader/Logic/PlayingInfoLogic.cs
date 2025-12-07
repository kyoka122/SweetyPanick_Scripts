using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Loader.Entity;
using Loader.View;
using MyApplication;

namespace Loader.Logic
{
    /// <summary>
    /// Load画面を出しているときに操作説明を表示する際のLogic
    /// </summary>
    public class PlayingInfoLogic
    {
        public PlayingInfoType currentPlayingInfoType { get; private set; }
        public bool isPlayingFade { get; private set; }
        public bool canFadeOut { get; private set; } = true;
        
        private readonly LoadEntity _loadEntity;
        private readonly PlayingInfoView _playingInfoView;
        private readonly LoadCameraView _loadCameraView;
        private CancellationTokenSource _tokenSource;
        
        public PlayingInfoLogic(LoadEntity loadEntity, PlayingInfoView playingInfoView, LoadCameraView loadCameraView)
        {
            _loadEntity = loadEntity;
            _playingInfoView = playingInfoView;
            _loadCameraView = loadCameraView;
            currentPlayingInfoType = PlayingInfoType.None;
        }

        public async UniTask PlayPlayingInfoFadeIn(PlayingInfoType type, float toFadeOutDurationMin,
            CancellationToken token)
        {
            isPlayingFade = true;
            currentPlayingInfoType = type;
            canFadeOut = false;
            _tokenSource = new CancellationTokenSource();
            _playingInfoView.InitOnFadeIn();

            await _playingInfoView.FadeInInfo(type,_loadEntity.PlayingInfoFadeInDuration,Ease.Unset,token);
            isPlayingFade = false;
            WaitActiveDuration(toFadeOutDurationMin,token).Forget();
        }
        
        private async UniTask WaitActiveDuration(float duration,CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            canFadeOut = true;
        }
        
        /// <summary>
        /// 説明画面を表示中であればフェードアウトさせる
        /// </summary>
        public async UniTask TryPlayPlayingInfoFadeOut(CancellationToken token)
        {
            isPlayingFade = true;
            _tokenSource.Cancel();

            await _playingInfoView.FadeOutInfo(currentPlayingInfoType, _loadEntity.PlayingInfoFadeOutDuration,
                Ease.Unset, token);
            _playingInfoView.UnloadTexture(currentPlayingInfoType);//MEMO: Textureのサイズが大きく負荷になるため、使用後にUnloadしておく。
            currentPlayingInfoType = PlayingInfoType.None;
            isPlayingFade = false;
        }
        
        
    }
}