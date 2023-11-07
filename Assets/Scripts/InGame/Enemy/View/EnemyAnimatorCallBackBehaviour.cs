using System;
using Common.Interface;
using UniRx;
using UnityEngine;

namespace InGame.Enemy.View
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimatorCallBackBehaviour:MonoBehaviour,IDisposable
    {
        public IObservable<string> OnAnimationEvent => _onAnimationEvent;
        private Subject<string> _onAnimationEvent;

        public void Init()
        {
            _onAnimationEvent = new Subject<string>();
        }
        
        public void CallbackAnimation(string animationClipName)
        {
            _onAnimationEvent.OnNext(animationClipName);
        }

        public void Dispose()
        {
            _onAnimationEvent.Dispose();
        }
    }
}