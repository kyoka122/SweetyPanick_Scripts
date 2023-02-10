using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Stage.View;
using UnityEngine;

namespace InGame.Stage.Logic
{
    public class HealAnimationLogic
    {
        private readonly HealAnimationView _healAnimationView;

        public HealAnimationLogic(HealAnimationView healAnimationView)
        {
            _healAnimationView = healAnimationView;
        }

        public async UniTask PlayHealAnimationTask()
        {
            //TODO: アニメーション実装後コメントアウト解除 ＆ Delay削除
            /*_healAnimationView.PlayHealAnimation();
            await UniTask.WaitUntil(() => !_healAnimationView.IsPlayingHealAnimation(),
                cancellationToken: tokenSource.Token);*/
            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: _healAnimationView.GetCancellationToken());

            Debug.Log($"HealAnimationFinish!");
        }
    }
}