using InGame.MyInput;
using MyApplication;
using MyInput;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using Utility;

namespace OutGame.PlayerCustom.MyInput
{
	public class AnyDeviseCaseUnknownControllerInput : BaseCaseUnknownControllerInput
    {
        public override MyInputDeviceType DeviceType { get; }
        
        private InputDevice[] _devices;
        
        public AnyDeviseCaseUnknownControllerInput(MyInputDeviceType type,InputDevice newDevice)
        {
            DeviceType = type;
            Init(new []{newDevice});
        }
        
        public AnyDeviseCaseUnknownControllerInput(InputDevice[] newDevices)
        {
            Init(newDevices);
        }

        private void Init(InputDevice[] newDevices)
        {
            _devices = newDevices;
            var gameInputMap = new InputMap{devices = newDevices};

            gameInputMap.PlayerCustom.HorizontalMove.started += SetHorizontalMoveValue;
            gameInputMap.PlayerCustom.HorizontalMove.performed += SetHorizontalMoveValue;
            gameInputMap.PlayerCustom.HorizontalMove.canceled += SetHorizontalMoveValue;
            
            gameInputMap.PlayerCustom.VerticalMove.started += SetVerticalMoveValue;
            gameInputMap.PlayerCustom.VerticalMove.performed += SetVerticalMoveValue;
            gameInputMap.PlayerCustom.VerticalMove.canceled += SetVerticalMoveValue;
            
            gameInputMap.PlayerCustom.Next.started += OnNext;
            gameInputMap.PlayerCustom.Next.canceled += OffNext;
            
            gameInputMap.PlayerCustom.Back.started += OnBack;
            gameInputMap.PlayerCustom.Back.canceled += OffBack;
            
            gameInputMap.Enable();
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

        public override BasePlayerInput GetInGamePlayerInput()
        {
            Debug.Log($"Generate InGamePlayerInput: {_devices[0].name}");
            return new AnyDevicePlayerInput(_devices);
        }
    }
}