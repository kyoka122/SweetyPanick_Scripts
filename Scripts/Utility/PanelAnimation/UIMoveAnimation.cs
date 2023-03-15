using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Utility.PanelAnimation
{
    public class UIMoveAnimation:IPanelAnimation
    {
        public CancellationToken Token { get; }
        
        private readonly RectTransform _upTarget;
        private readonly RectTransform _downTarget;
        private readonly float _enterDuration;
        private readonly float _exitDuration;
        private readonly Vector2 _upTargetExitEndPos;
        private readonly Vector2 _downTargetExitEndPos;
        private readonly Vector2 _upTargetEnterEndPos;
        private readonly Vector2 _downTargetEnterEndPos;

        public UIMoveAnimation(RectTransform upTarget, RectTransform downTarget, float enterDuration, float exitDuration, Vector2 upTargetExitEndPos, Vector2 downTargetExitEndPos, Vector2 upTargetEnterEndPos, Vector2 downTargetEnterEndPos)
        {
            _upTarget = upTarget;
            _downTarget = downTarget;
            _enterDuration = enterDuration;
            _exitDuration = exitDuration;
            _upTargetExitEndPos = upTargetExitEndPos;
            _downTargetExitEndPos = downTargetExitEndPos;
            _upTargetEnterEndPos = upTargetEnterEndPos;
            _downTargetEnterEndPos = downTargetEnterEndPos;
            Token = upTarget.GetCancellationTokenOnDestroy();
        }

        public async UniTask Enter(CancellationToken token)
        {
            await DOTween.Sequence()
                .Append(_upTarget.DOLocalMove(_upTargetEnterEndPos, _enterDuration).SetEase(Ease.OutExpo))
                .Join(_downTarget.DOLocalMove(_downTargetEnterEndPos, _enterDuration).SetEase(Ease.OutBack))
                .OnPlay(() =>
                {
                    _upTarget.gameObject.SetActive(true);
                    _downTarget.gameObject.SetActive(true);
                })
                .ToUniTask(cancellationToken:token);
        }

        public async UniTask Exit(CancellationToken token)
        {
            await DOTween.Sequence()
                .Append(_upTarget.DOLocalMove(_upTargetExitEndPos, _exitDuration).SetEase(Ease.InExpo))
                .Join(_downTarget.DOLocalMove(_downTargetExitEndPos, _exitDuration).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    _upTarget.gameObject.SetActive(false);
                    _downTarget.gameObject.SetActive(false);
                })
                .ToUniTask(cancellationToken:token);
        }
    }
}