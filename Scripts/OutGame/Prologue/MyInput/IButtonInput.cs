using System;
using UniRx;

namespace OutGame.Prologue.MyInput
{
    public interface IButtonInput:IDisposable
    {
        public IObservable<bool> OnButton { get; }
    }
}