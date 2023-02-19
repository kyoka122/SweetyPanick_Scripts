using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Stage.View
{
    [RequireComponent(typeof(Animator))]
    public class DoorView:MonoBehaviour
    {
        public bool NeedToKey => needToKey;
        public IObservable<DoorView> EnterDoorObserver=>_onEnterDoor;
        
        [SerializeField] private bool needToKey=false;
        
        private Subject<DoorView> _onEnterDoor;
        private Animator _animator;

        private bool _isDoorOpen;

        public void Init()
        {
            _onEnterDoor = new Subject<DoorView>();
            _onEnterDoor.AddTo(this);
            _animator = GetComponent<Animator>();
        }

        public void InitAtStageMove()
        {
            _isDoorOpen = false;
            _animator.SetTrigger(StageAnimatorParameter.OnClose);
        }

        public async void TryEnterDoor()
        {
            if (_isDoorOpen)
            {
                return;
            }
            _isDoorOpen = true;
            _animator.SetTrigger(StageAnimatorParameter.OnOpen);
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            Debug.Log($"WaitStart");
            await UniTask.WaitUntil(HadFinishedOpenAnimation , cancellationToken:token);
            _onEnterDoor.OnNext(this);
        }

        private bool HadFinishedOpenAnimation()
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= 0 &&
                   stateInfo.IsName(StageAnimationClipName.DoorAnimation);
        }

    }
}