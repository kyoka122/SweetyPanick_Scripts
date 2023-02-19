using MyInput;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace InGame.MyInput
{
    /// <summary>
    /// Joycon以外のデバイスのInput。2つで1対のコントローラーがあれば
    /// </summary>
    public class AnyDevicePlayerInput:BasePlayerInput
    {
        public AnyDevicePlayerInput(InputDevice newDevice)
        {
            Init(new []{newDevice});
        }
        
        public AnyDevicePlayerInput(InputDevice[] newDevices)
        {
            Init(newDevices);
        }

        private void Init(InputDevice[] newDevices)
        {
            var gameInputMap = new InputMap{devices = newDevices};
            
            if (newDevices[0].IsTDevice<Gamepad>())
            {
                rumbleEvent += Rumble;
                Debug.Log($"Set Gamepad To Rumble");
            }

            gameInputMap.Player.Move.started += SetMoveValue;
            gameInputMap.Player.Move.performed += SetMoveValue;
            gameInputMap.Player.Move.canceled += SetMoveValue;
            
            gameInputMap.Player.Move.started += MovePlayerSelector;
            gameInputMap.Player.Move.performed += MovePlayerSelector;
            gameInputMap.Player.Move.canceled += MovePlayerSelector;
            
            gameInputMap.Player.Jump.started += OnJump;
            gameInputMap.Player.Jump.canceled += OffJump;
            
            gameInputMap.Player.Punch.started += OnPunch;
            gameInputMap.Player.Punch.canceled += OffPunch;
            
            gameInputMap.Player.Skill.started += OnSkill;
            gameInputMap.Player.Skill.canceled += OffSkill;

            gameInputMap.Player.Fix.started += OnSkill;
            gameInputMap.Player.Skill.canceled += OffSkill;

            gameInputMap.Player.Fix.started += OnFix;
            gameInputMap.Player.Fix.canceled += OffFix;
            
            gameInputMap.Player.OnPlayerSelect.started += OnPlayerSelector;
            gameInputMap.Player.OnPlayerSelect.canceled += OffPlayerSelector;
            
            gameInputMap.Enable();
        }

        private void SetMoveValue(InputAction.CallbackContext context)
        {
            move.Value = context.ReadValue<float>();
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
            skill.Value = true;
        }
        
        private void OffSkill(InputAction.CallbackContext context)
        {
            skill.Value = false;
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
    }
}