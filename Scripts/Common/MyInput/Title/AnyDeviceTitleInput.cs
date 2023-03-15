using MyApplication;
using UnityEngine.InputSystem;
using Utility;

namespace Common.MyInput.Title
{
    public class AnyDeviceTitleInput:BaseTitleInput
    {
        private InputMap _inputMap;

        public AnyDeviceTitleInput(InputDevice newDevice) : base(newDevice.GetMyInputDeviceType(), newDevice.deviceId)
        {
            Init(new[] {newDevice});
        }

        public AnyDeviceTitleInput(InputDevice[] newDevices) : base(newDevices[0].GetMyInputDeviceType(),newDevices[0].deviceId)
        {
            Init(newDevices);
        }
        
        public AnyDeviceTitleInput(MyInputDeviceType type, int deviceId, InputDevice newDevice) : base(type,
            deviceId)
        {
            Init(new[] {newDevice});
        }

        public AnyDeviceTitleInput(MyInputDeviceType type, int deviceId, InputDevice[] newDevices) : base(type,
            deviceId)
        {
            Init(newDevices);
        }

        private void Init(InputDevice[] newDevices)
        {
            _inputMap = new InputMap{devices = newDevices};

            _inputMap.Title.Scroll.started += SetScrollValue;
            _inputMap.Title.Scroll.performed += SetScrollValue;
            _inputMap.Title.Scroll.canceled += SetScrollValue;
            
            _inputMap.Title.Next.started += OnNext;
            _inputMap.Title.Next.canceled += OffNext;
            
            _inputMap.Title.Credit.started += SwitchCredit;
            
            _inputMap.Enable();
        }


        private void SetScrollValue(InputAction.CallbackContext context)
        {
            scrollValue.Value = context.ReadValue<float>();
        }

        private void OnNext(InputAction.CallbackContext context)
        {
            onNext.Value = true;
        }
        
        private void OffNext(InputAction.CallbackContext context)
        {
            onNext.Value = false;
        }
        
        private void SwitchCredit(InputAction.CallbackContext context)
        {
            onCredit.Value = !onCredit.Value;
        }

        public override void Dispose()
        {
            _inputMap.Dispose();
            base.Dispose();
        }
    }
}