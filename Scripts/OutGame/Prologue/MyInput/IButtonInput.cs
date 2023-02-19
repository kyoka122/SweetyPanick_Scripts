using System;
using UniRx;

namespace OutGame.Prologue.MyInput
{
    public interface IButtonInput
    {
        public IObservable<bool> OnButton { get; }
    }
}