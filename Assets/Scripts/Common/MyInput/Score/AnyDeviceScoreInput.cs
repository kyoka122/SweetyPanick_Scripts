using MyApplication;
using UnityEngine.InputSystem;
using Utility;

namespace Common.MyInput.Score
{
    public class AnyDeviceScoreInput:BaseScoreInput
    {
        private InputMap _inputMap;

        public AnyDeviceScoreInput(InputDevice newDevice) : base(newDevice.GetMyInputDeviceType(), newDevice.deviceId)
        {
            Init(new[] {newDevice});
        }

        public AnyDeviceScoreInput(InputDevice[] newDevices) : base(newDevices[0].GetMyInputDeviceType(),newDevices[0].deviceId)
        {
            Init(newDevices);
        }
        
        public AnyDeviceScoreInput(MyInputDeviceType type, int deviceId, InputDevice newDevice) : base(type,
            deviceId)
        {
            Init(new[] {newDevice});
        }

        public AnyDeviceScoreInput(MyInputDeviceType type, int deviceId, InputDevice[] newDevices) : base(type,
            deviceId)
        {
            Init(newDevices);
        }

        private void Init(InputDevice[] newDevices)
        {
            _inputMap = new InputMap{devices = newDevices};

            _inputMap.Score.Next.started += OnNext;
            _inputMap.Score.Next.canceled += OffNext;
            
            _inputMap.Enable();
        }

        private void OnNext(InputAction.CallbackContext context)
        {
            next.Value = true;
        }
        
        private void OffNext(InputAction.CallbackContext context)
        {
            next.Value = false;
        }

        public override void Dispose()
        {
            _inputMap.Dispose();
            base.Dispose();
        }
        
    }
}