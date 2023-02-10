using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Stage.View
{
    public class DoorView:MonoBehaviour
    {
        public IObservable<DoorView> EnterDoorObserver=>_onEnterDoor;
        
        private Subject<DoorView> _onEnterDoor;
        private Animator _animator;

        public void Init()
        {
            _onEnterDoor = new Subject<DoorView>();
            _onEnterDoor.AddTo(this);
            _animator = GetComponent<Animator>();
        }
        
        public async void EnterDoor()
        {
            _animator.SetTrigger(StageAnimatorParameter.OnOpen);
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitUntil(HadFinishedOpenAnimation , cancellationToken:token);
            _onEnterDoor.OnNext(this);
        }

        private bool HadFinishedOpenAnimation()
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName(StageAnimationClipName.DoorOpened);
        }

    }
}