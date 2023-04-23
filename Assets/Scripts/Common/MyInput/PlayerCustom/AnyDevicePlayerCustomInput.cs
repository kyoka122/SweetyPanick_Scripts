using MyApplication;
using UnityEngine.InputSystem;
using Utility;

namespace Common.MyInput.PlayerCustom
{
	public class AnyDevicePlayerCustomInput : BasePlayerCustomInput
    {
        private InputMap _inputMap;

        public AnyDevicePlayerCustomInput(InputDevice newDevice) : base(newDevice.GetMyInputDeviceType(), newDevice.deviceId)
        {
            Init(new[] {newDevice});
        }

        public AnyDevicePlayerCustomInput(InputDevice[] newDevices) : base(newDevices[0].GetMyInputDeviceType(),newDevices[0].deviceId)
        {
            Init(newDevices);
        }
        
        public AnyDevicePlayerCustomInput(MyInputDeviceType type, int deviceId, InputDevice newDevice) : base(type,
            deviceId)
        {
            Init(new[] {newDevice});
        }

        public AnyDevicePlayerCustomInput(MyInputDeviceType type, int deviceId, InputDevice[] newDevices) : base(type,
            deviceId)
        {
            Init(newDevices);
        }

        private void Init(InputDevice[] newDevices)
        {
            _inputMap = new InputMap{devices = newDevices};

            _inputMap.PlayerCustom.HorizontalMove.started += SetHorizontalMoveValue;
            _inputMap.PlayerCustom.HorizontalMove.performed += SetHorizontalMoveValue;
            _inputMap.PlayerCustom.HorizontalMove.canceled += SetHorizontalMoveValue;
            
            _inputMap.PlayerCustom.VerticalMove.started += SetVerticalMoveValue;
            _inputMap.PlayerCustom.VerticalMove.performed += SetVerticalMoveValue;
            _inputMap.PlayerCustom.VerticalMove.canceled += SetVerticalMoveValue;
            
            _inputMap.PlayerCustom.Next.started += OnNext;
            _inputMap.PlayerCustom.Next.canceled += OffNext;
            
            _inputMap.PlayerCustom.Back.started += OnBack;
            _inputMap.PlayerCustom.Back.canceled += OffBack;
            
            _inputMap.Enable();
        }

        private void SetHorizontalMoveValue(InputAction.CallbackContext context)
        {
            horizontalValue.Value = context.ReadValue<float>();
            horizontalDigitalMoveValue.Value =
                GetStickDigitalDirection(context.ReadValue<float>(), horizontalDigitalMoveValue.Value);
        }

        private void SetVerticalMoveValue(InputAction.CallbackContext context)
        {
            verticalValue.Value = context.ReadValue<float>();
            verticalDigitalMoveValue.Value =
                GetStickDigitalDirection(context.ReadValue<float>(), verticalDigitalMoveValue.Value);
        }

        private void OnNext(InputAction.CallbackContext context)
        {
            next.Value = true;
        }
        
        private void OffNext(InputAction.CallbackContext context)
        {
            next.Value = false;
        }
        
        private void OnBack(InputAction.CallbackContext context)
        {
            back.Value = true;
        }
        
        private void OffBack(InputAction.CallbackContext context)
        {
            back.Value = false;
        }

        public override void Dispose()
        {
            _inputMap.Dispose();
            base.Dispose();
        }
    }
}