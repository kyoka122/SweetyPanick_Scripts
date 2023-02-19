/*using System;
using UniRx;

namespace OutGame.PlayerCustom.MyInput
{
    /// <summary>
    /// OutGame.PlayerCustom.MyInput.BasePlayerInputへ統合
    /// </summary>
    public abstract class BaseControllerUnKnownInput:IDisposable
    {
        public IReadOnlyReactiveProperty<bool> ControllerSet => controllerSet;
        public IReadOnlyReactiveProperty<bool> Next => next;
        public IReadOnlyReactiveProperty<bool> Back => back;

        protected readonly ReactiveProperty<bool> controllerSet;
        protected readonly ReactiveProperty<bool> next;
        protected readonly ReactiveProperty<bool> back;
        
        protected BaseControllerUnKnownInput()
        {
            controllerSet = new ReactiveProperty<bool>();
            next = new ReactiveProperty<bool>();
            back = new ReactiveProperty<bool>();
        }

        public virtual void Dispose()
        {
            controllerSet?.Dispose();
            next?.Dispose();
            back?.Dispose();
        }
    }
}*/