using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Common.Interface;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.View
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimatorView:MonoBehaviour,IAnimationCallback,IDisposable
    {
        public PlayableCharacter type { get; private set; }
        public IObservable<string> OnAnimationEvent=>_animationEventSubject;
        
        public CancellationToken thisToken { get; private set; }
        
        private Subject<string> _animationEventSubject;
        private Animator _animator;

        public virtual void Init(PlayableCharacter type)
        {
            this.type = type;
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

        public AnimatorStateInfo GetCurrentAnimationStateInfo()
        {
            return _animator.GetCurrentAnimatorStateInfo(0);
        }

        public bool IsPunching()
        {
            return GetCurrentAnimationName()== PlayerAnimationName.GetEachName(type,PlayerAnimationName.Punch);
        }

        public bool IsFixingSweets()
        {
            string animationName= GetCurrentAnimationName();
            return animationName == PlayerAnimationName.GetEachName(type, PlayerAnimationName.Fix);
        }
        
        public bool IsFixingGimmickSweets()
        {
            string animationName= GetCurrentAnimationName();
            return animationName == PlayerAnimationName.GetEachName(type, PlayerAnimationName.OnFix) ||
                   animationName == PlayerAnimationName.GetEachName(type, PlayerAnimationName.Fixing) ||
                   animationName == PlayerAnimationName.GetEachName(type, PlayerAnimationName.Fixed);
        }

        public bool IsUsingSkill()
        {
            return GetCurrentAnimationName()== PlayerAnimationName.GetEachName(type,PlayerAnimationName.Skill);
        }
        
        public bool IsJumping()
        {
            return GetCurrentAnimationName()== PlayerAnimationName.GetEachName(type,PlayerAnimationName.Jump);
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

        public void Rebind()
        {
            _animator.Rebind();
        }
        
        public void Pause()
        {
            _animator.speed = 0;
        }
        
        public virtual void CallbackAnimation(string animationClipName)
        {
            _animationEventSubject.OnNext(animationClipName);
        }
        
        public void SetAnimatorSpeed(float speed)
        {
            _animator.speed = speed;
        }
        

        public void Dispose()
        {
            _animationEventSubject?.Dispose();
        }
    }
}