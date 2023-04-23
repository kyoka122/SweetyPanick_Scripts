using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace OutGame.PlayerCustom.View
{
    public class FromMessageWindowRecieverView:MonoBehaviour
    {
        private Subject<bool> _moveNextScene;
        //private const float CheerDialogDuration = 2f;
        public void Init(Subject<bool> moveNextScene)
        {
            _moveNextScene = moveNextScene;
        }

        public async void SendCheerMessageEvent()
        {
            //await UniTask.Delay(TimeSpan.FromSeconds(CheerDialogDuration), cancellationToken: this.GetCancellationTokenOnDestroy());
            _moveNextScene.OnNext(true);
        }
    }
}