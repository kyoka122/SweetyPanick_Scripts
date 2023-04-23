using MyApplication;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Common.MyInput.Player
{
    /// <summary>
    /// Joycon以外のデバイスのInput。2つで1対のコントローラーがあれば
    /// </summary>
    public class AnyDevicePlayerInput:BasePlayerInput
    {
        private InputMap _inputMap;
        
        public AnyDevicePlayerInput(InputDevice newDevice): base(newDevice.GetMyInputDeviceType(), newDevice.deviceId)
        {
            Init(new []{newDevice});
        }
        
        public AnyDevicePlayerInput(InputDevice[] newDevices): base(newDevices[0].GetMyInputDeviceType(),newDevices[0].deviceId)
        {
            Init(newDevices);
        }
        public AnyDevicePlayerInput(MyInputDeviceType type, int deviceId,InputDevice newDevice): base(type, deviceId)
        {
            Init(new []{newDevice});
        }
        
        public AnyDevicePlayerInput(MyInputDeviceType type, int deviceId,InputDevice[] newDevices): base(type, deviceId)
        {
            Init(newDevices);
        }

        private void Init(InputDevice[] newDevices)
        {
            _inputMap = new InputMap {devices = newDevices};
            
            if (newDevices[0].IsTDevice<Gamepad>())
            {
                rumbleEvent += Rumble;
                Debug.Log($"Set Gamepad To Rumble");
            }

            _inputMap.Player.Move.started += SetMoveValue;
            _inputMap.Player.Move.performed += SetMoveValue;
            _inputMap.Player.Move.canceled += SetMoveValue;

            _inputMap.Player.Next.started += OnNext;
            
            _inputMap.Player.Jump.started += OnJump;
            _inputMap.Player.Jump.canceled += OffJump;
            
            _inputMap.Player.Punch.started += OnPunch;
            _inputMap.Player.Punch.canceled += OffPunch;
            
            _inputMap.Player.Skill.started += OnSkill;
            _inputMap.Player.Skill.canceled += OffSkill;

            _inputMap.Player.Fix.started += OnFix;
            _inputMap.Player.Fix.canceled += OffFix;
            
            _inputMap.Player.OnPlayerSelect.started += OnPlayerSelector;
            _inputMap.Player.OnPlayerSelect.canceled += OffPlayerSelector;
            
            _inputMap.Player.PlayerSelectMove.started += MovePlayerSelector;
            _inputMap.Player.PlayerSelectMove.performed += MovePlayerSelector;
            _inputMap.Player.PlayerSelectMove.canceled += MovePlayerSelector;
            
            _inputMap.Enable();
        }

        private void SetMoveValue(InputAction.CallbackContext context)
        {
            move.Value = context.ReadValue<float>();
        }
        
        private void OnNext(InputAction.CallbackContext context)
        {
            next.OnNext(true);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            jump.Value = true;
        }
        
        private void OffJump(InputAction.CallbackContext context)
        {
            jump.Value = false;
        }

        private void OnPunch(InputAction.CallbackContext context)
        {
            punch.Value = true;
        }
        
        private void OffPunch(InputAction.CallbackContext context)
        {
            punch.Value = false;
        }
        
        private void OnSkill(InputAction.CallbackContext context)
        {
            //skill.Value = true;
        }
        
        private void OffSkill(InputAction.CallbackContext context)
        {
            //skill.Value = false;
        }
        
        private void OnFix(InputAction.CallbackContext context)
        {
            fix.Value = true;
        }
        
        private void OffFix(InputAction.CallbackContext context)
        {
            fix.Value = false;
        }
        
        private void OnPlayerSelector(InputAction.CallbackContext context)
        {
            //TODO: キャラ切り替え実装後コメントアウト解除
            //playerSelector.Value = true;
        }
        
        private void OffPlayerSelector(InputAction.CallbackContext context)
        {
            //TODO: キャラ切り替え実装後コメントアウト解除
            //playerSelector.Value = false;
        }

        private void MovePlayerSelector(InputAction.CallbackContext context)
        {
            if (!playerSelector.Value)
            {
                playerSelectDirection.Value = 0;
                return;
            }
            playerSelectDirection.Value = GetStickDigitalDirection(context.ReadValue<float>(),playerSelectDirection.Value);
        }

        private void Rumble()
        {
            Debug.Log($"Rumble!");
        }

        public override void Dispose()
        {
            _inputMap.Dispose();
            base.Dispose();
        }
    }
}