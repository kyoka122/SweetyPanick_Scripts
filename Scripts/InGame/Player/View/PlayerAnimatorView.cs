using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Common.Interface;
using UniRx;
using UnityEngine;

namespace InGame.Player.View
{
    public class PlayerAnimatorView:MonoBehaviour,IAnimationCallback
    {
        public IObservable<string> OnAnimationEvent=>_animationEventSubject;
        
        public CancellationToken thisToken { get; private set; }
        
        private Subject<string> _animationEventSubject;
        private Animator _animator;

        public void Init()
        {
            _animationEventSubject = new Subject<string>();
            _animator = GetComponent<Animator>();
            thisToken = this.GetCancellationTokenOnDestroy();
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
        
        public void PlayFloatAnimation(string animationName, float value)
        {
            _animator.SetFloat(animationName, value);
        }

        public void PlayBoolAnimation(string animationName, bool on)
        {
            _animator.SetBool(animationName, on);
        }

        public void PlayTriggerAnimation(string animationName)
        {
            _animator.SetTrigger(animationName);
        }

        public void ResetAllParameter()
        {
            
        }
        
        public void CallbackAnimation(string animationClipName)
        {
            _animationEventSubject.OnNext(animationClipName);
        }
        
        public void SetAnimatorSpeed(float speed)
        {
            _animator.speed = speed;
        }
    }
}