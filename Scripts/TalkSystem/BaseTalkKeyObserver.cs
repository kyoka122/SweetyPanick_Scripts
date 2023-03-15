using System;
using UniRx;

namespace TalkSystem
{
    public abstract class BaseTalkKeyObserver:IDisposable
    {
        public IObservable<bool> OnNext=>onNext;
        public IObservable<bool> OnSkip=>onSkip;

        protected readonly Subject<bool> onNext;
        protected readonly Subject<bool> onSkip;

        protected BaseTalkKeyObserver()
        {
            onNext = new Subject<bool>();
            onSkip = new Subject<bool>();
        }

        public virtual void Dispose()
        {
            onNext?.Dispose();
            onSkip?.Dispose();
        }
    }
}