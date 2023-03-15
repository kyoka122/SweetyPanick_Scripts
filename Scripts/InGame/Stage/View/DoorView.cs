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
        public bool IsIgnoreKeyCheck => isIgnoreKeyCheck;
        public bool IsKeyDoor => isKeyDoor;
        public IObservable<DoorView> EnterDoorObserver=>_onEnterDoor;

        [SerializeField, Tooltip("デバッグ用。offにすると鍵無しでも入れるようになる")]
        private bool isIgnoreKeyCheck = false;
        [SerializeField] private bool isKeyDoor=false;
        
        private Subject<DoorView> _onEnterDoor;
        private Animator _animator;
        private string DoorAnimationName => isKeyDoor ? StageAnimationClipName.KeyDoorAnimation : StageAnimationClipName.DoorAnimation;

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
            return stateInfo.normalizedTime >= 1 &&
                   stateInfo.IsName(DoorAnimationName);
        }

    }
}