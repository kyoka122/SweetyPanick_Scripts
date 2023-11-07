using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Common.View
{
    public class CallbackAnimatorView:MonoBehaviour
    {
        private Subject<string> _onCallBack;
        private Animator _animator;
        
        public void Init()
        {
            _animator = GetComponent<Animator>();
        }


        private void OnEnable()
        {
            _onCallBack = new Subject<string>();
        }

        public IObservable<string> GetCallbackObserver()
        {
            return _onCallBack;
        }
        
        public void SetTrigger(string animationName)
        {
            _animator.SetTrigger(animationName);
        }
        
        public void SetBool(string animationName,bool on)
        {
            _animator.SetBool(animationName,on);
        }

        //MEMO: Animation用 Callback インスペクターより設定
        public void OnCallBack(string callbackName)
        {
            _onCallBack?.OnNext(callbackName);
        }

        private void OnDestroy()
        {
            _onCallBack?.Dispose();
        }

        public string GetCurrentAnimationName()
        {
            AnimationClip firstClip = _animator.GetCurrentAnimatorClipInfo(0)
                .FirstOrDefault().clip;
            if (firstClip == null)
            {
                Debug.LogWarning($"None Clip");
                return null;
            }

            return firstClip.name;
        }

        public AnimatorStateInfo GetCurrentAnimationStateInfo()
        {
            return _animator.GetCurrentAnimatorStateInfo(0);
        }
        
        public void Pause()
        {
            _animator.speed = 0;
        }

        public void ResetAnimator()
        {
            _animator.Rebind();
        }
        
        private void OnDisable()
        {
            _onCallBack?.Dispose();
            _onCallBack = null;
        }

        public void DisposeCallbackObservable()
        {
            _onCallBack?.Dispose();
            _onCallBack = null;
        }
        
    }
}