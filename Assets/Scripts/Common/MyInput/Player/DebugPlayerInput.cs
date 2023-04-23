/*using MyInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InGame.MyInput
{
    //MEMO: 参考：https://nekojara.city/unity-input-system-actions
    /// <summary>
    /// InGame.MyInput.AnyDevicePlayerInputに移動
    /// </summary>
    public class DebugPlayerInput:BasePlayerInput
    {
        private const float SelectorCanMoveValue = 0.9f;
        private const float SelectorNotMoveValue = 0.1f;

        public DebugPlayerInput()
        {
            // Input Actionインスタンス生成
            var gameInputMap = new DebugInputMap();
            
            // Actionイベント登録
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
            
            gameInputMap.Player.PlayerSelect.started += OnPlayerSelector;
            gameInputMap.Player.PlayerSelect.canceled += OffPlayerSelector;

            rumbleEvent += Rumble;

            // Input Actionを機能させるためには、
            // 有効化する必要がある
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
            playerSelector.Value = true;
        }
        
        private void OffPlayerSelector(InputAction.CallbackContext context)
        {
            playerSelector.Value = false;
        }
        
        private void MovePlayerSelector(InputAction.CallbackContext context)
        {
            if (!playerSelector.Value)
            {
                playerSelectDirection.Value = 0;
                return;
            }

            int moveDirection = 1;
            
            //MEMO: 右入力か左入力か
            moveDirection*=(int) Mathf.Sign(move.Value);

            //MEMO: 押し込み具合
            if (Mathf.Abs(move.Value) > SelectorCanMoveValue)
            {
                moveDirection *= 1;
            }
            else if (Mathf.Abs(move.Value) < SelectorNotMoveValue)
            {
                moveDirection *= 0;
            }
            else
            {
                moveDirection *= playerSelectDirection.Value;
            }

            playerSelectDirection.Value = moveDirection;
        }

        public void Rumble()
        {
            Debug.Log($"Rumble!");
        }
    }
}*/