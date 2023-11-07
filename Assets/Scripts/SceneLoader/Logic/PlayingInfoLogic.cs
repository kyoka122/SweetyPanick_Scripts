using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.SceneLoader.Entity;
using InGame.SceneLoader.View;
using MyApplication;
using UnityEngine;

namespace InGame.SceneLoader.Logic
{
    public class PlayingInfoLogic
    {
        public PlayingInfoType currentPlayingInfoType { get; private set; }
        public bool isFading { get; private set; }
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
            isFading = true;
            currentPlayingInfoType = type;
            canFadeOut = false;
            _tokenSource = new CancellationTokenSource();
            _playingInfoView.InitOnFadeIn();

            //MEMO: ↓フェード
            await _playingInfoView.FadeInInfo(type,_loadEntity.PlayingInfoFadeInDuration,Ease.Unset,token);
            isFading = false;
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
            isFading = true;
            _tokenSource.Cancel();

            Debug.Log($"StartFadeOut");
            await _playingInfoView.FadeOutInfo(currentPlayingInfoType, _loadEntity.PlayingInfoFadeOutDuration,
                Ease.Unset, token);
            currentPlayingInfoType = PlayingInfoType.None;//MEMO: Fade後でないとFade時の引数がNoneになる
            isFading = false;
        }
        
        
    }
}