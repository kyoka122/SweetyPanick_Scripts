using System;
using MyApplication;
using UniRx;

namespace Common.MyInput.Score
{
    public abstract class BaseScoreInput : IDisposable
    {
        public MyInputDeviceType DeviceType { get; }
        public int DeviceId { get; }
        
        public IReadOnlyReactiveProperty<bool> Next => next;
        protected readonly ReactiveProperty<bool> next;

        protected BaseScoreInput(MyInputDeviceType type, int deviceId)
        {
            DeviceType = type;
            DeviceId = deviceId;
            next = new ReactiveProperty<bool>();
        }

        public virtual void Dispose()
        {
            next?.Dispose();
        }
    }
}