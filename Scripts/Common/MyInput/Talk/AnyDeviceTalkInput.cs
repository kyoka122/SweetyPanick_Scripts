using MyApplication;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Common.MyInput.Talk
{
    public class AnyDeviceTalkInput : BaseTalkInput
    {
        private InputMap _inputMap;
        
        public AnyDeviceTalkInput(InputDevice newDevice): base(newDevice.GetMyInputDeviceType(), newDevice.deviceId)
        {
            Init(new[] {newDevice});
        }

        public AnyDeviceTalkInput(InputDevice[] newDevices): base(newDevices[0].GetMyInputDeviceType(),newDevices[0].deviceId)
        {
            Init(newDevices);
        }
        public AnyDeviceTalkInput(MyInputDeviceType type, int deviceId,InputDevice newDevice): base(type, deviceId)
        {
            Init(new[] {newDevice});
        }

        public AnyDeviceTalkInput(MyInputDeviceType type,int deviceId,InputDevice[] newDevices): base(type, deviceId)
        {
            Init(newDevices);
        }

        private void Init(InputDevice[] newDevices)
        {
            _inputMap = new InputMap {devices = newDevices};

            _inputMap.Talk.Next.started += OnNext;
            _inputMap.Talk.Next.canceled += OffNext;
            _inputMap.Talk.Skip.started += OnSkip;
            _inputMap.Talk.Skip.canceled += OffSkip;
            _inputMap.Enable();
        }

        private void OnNext(InputAction.CallbackContext context)
        {
            next?.OnNext(true);
        }
        
        private void OffNext(InputAction.CallbackContext context)
        {
            next?.OnNext(false);
        }

        private void OnSkip(InputAction.CallbackContext context)
        {
            skip?.OnNext(true);
        }
        
        private void OffSkip(InputAction.CallbackContext context)
        {
            skip?.OnNext(false);
        }

        public override void Dispose()
        {
            _inputMap.Dispose();
            base.Dispose();
        }
    }
}