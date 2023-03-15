using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Utility.PanelAnimation
{
    public class PopAnimation:IPanelAnimation
    {
        public CancellationToken Token { get; }
        private readonly GameObject _target;
        private readonly float _popUpTime;
        private readonly float _popDownTime;

        public PopAnimation(GameObject target, float popUpTime, float popDownTime)
        {
            _target = target;
            _popUpTime = popUpTime;
            _popDownTime = popDownTime;
            Token = target.GetCancellationTokenOnDestroy();
        }

        /// <summary>
        /// ポップアップアニメーション
        /// </summary>
        public async UniTask Enter(CancellationToken token)
        {
            await DOTween.Sequence()
                .Append(_target.transform.DOScale(1, _popUpTime).SetEase(Ease.OutBack))
                .OnPlay(() =>
                {
                    _target.SetActive(true);
                })
                .ToUniTask(cancellationToken:token);
        }
        
        /// <summary>
        /// ポップアップアニメーション
        /// </summary>
        public async UniTask Exit(CancellationToken token)
        {
            await DOTween.Sequence()
                .Append(_target.transform.DOScale(Vector3.zero, _popDownTime).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    _target.SetActive(false);
                })
                .ToUniTask(cancellationToken:token);
        }
        
    }
}