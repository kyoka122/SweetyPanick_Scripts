using System;
using MyApplication;
using UniRx;

namespace Common.MyInput.Title
{
    public abstract class BaseTitleInput:IDisposable
    {
        public MyInputDeviceType DeviceType { get; }
        public int DeviceId { get; }
        public IReadOnlyReactiveProperty<float> ScrollValue => scrollValue;
        public IReadOnlyReactiveProperty<bool> OnNext => onNext;
        public IReadOnlyReactiveProperty<bool> OnCredit => onCredit;
        
        protected readonly ReactiveProperty<float> scrollValue;
        protected readonly ReactiveProperty<bool> onNext;
        protected readonly ReactiveProperty<bool> onCredit;
        
        
        protected BaseTitleInput(MyInputDeviceType type,int deviceId)
        {
            DeviceType = type;
            DeviceId = deviceId;
            scrollValue = new ReactiveProperty<float>();
            onNext = new ReactiveProperty<bool>();
            onCredit = new ReactiveProperty<bool>();
        }
        
        public virtual void Dispose()
        {
            scrollValue?.Dispose();
            onNext?.Dispose();
            onCredit?.Dispose();
        }
    }
}