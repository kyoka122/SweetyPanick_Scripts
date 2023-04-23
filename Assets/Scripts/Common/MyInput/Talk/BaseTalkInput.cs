using System;
using MyApplication;
using UniRx;

namespace Common.MyInput.Talk
{
    public abstract class BaseTalkInput:IDisposable
    {
        public MyInputDeviceType DeviceType { get; }
        public int DeviceId { get; }
        
        public IObservable<bool> Next => next;
        public IObservable<bool> Skip => skip;

        protected readonly Subject<bool> next;
        protected readonly Subject<bool> skip;

        protected BaseTalkInput(MyInputDeviceType type,int deviceId)
        {
            DeviceType = type;
            DeviceId = deviceId;
            next = new Subject<bool>();
            skip = new Subject<bool>();
        }
        
        public virtual void Dispose()
        {
            next?.Dispose();
            skip?.Dispose();
        }
    }
}