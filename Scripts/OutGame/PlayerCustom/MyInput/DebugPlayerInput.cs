using MyInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OutGame.MyInput
{
    //MEMO: 参考：https://nekojara.city/unity-input-system-actions
    
    //TODO: 後回し
    public class DebugPlayerInput:BasePlayerInput
    {
        private const float SelectorCanMoveValue = 0.9f;
        private const float SelectorNotMoveValue = 0.1f;

        public DebugPlayerInput()
        {
            // Input Actionインスタンス生成
            var gameInputMap = new DebugInputMap();
            
            // Actionイベント登録
          

            // Input Actionを機能させるためには、
            // 有効化する必要がある
            gameInputMap.Enable();
        }

        /*private void SetMoveValue(InputAction.CallbackContext context)
        {
            horizontalCommand.Value = context.ReadValue<float>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            verticalCommand.Value = true;
        }
        
        private void OffJump(InputAction.CallbackContext context)
        {
            verticalCommand.Value = false;
        }

        private void OnPunch(InputAction.CallbackContext context)
        {
            _downCommand.Value = true;
        }
        
        private void OffPunch(InputAction.CallbackContext context)
        {
            _downCommand.Value = false;
        }
        
        private void OnSkill(InputAction.CallbackContext context)
        {
            rightControllerSet.Value = true;
        }
        
        private void OffSkill(InputAction.CallbackContext context)
        {
            rightControllerSet.Value = false;
        }
        
        private void OnFix(InputAction.CallbackContext context)
        {
            next.Value = true;
        }
        
        private void OffFix(InputAction.CallbackContext context)
        {
            next.Value = false;
        }
        
        private void OnPlayerSelector(InputAction.CallbackContext context)
        {
            back.Value = true;
        }
        
        private void OffPlayerSelector(InputAction.CallbackContext context)
        {
            back.Value = false;
        }*/
        

        private int SetStickDirection(float stickValue)
        {
            int moveDirection = 1;

            //MEMO: スティックが右入力か左入力か
            moveDirection *= (int) Mathf.Sign(stickValue);


            //MEMO: スティックの押し込み具合
            /*if (Mathf.Abs(horizontalCommand.Value) > SelectorCanMoveValue)
            {
                moveDirection *= 1;
            }*/
            if (Mathf.Abs(stickValue) < SelectorNotMoveValue)
            {
                moveDirection *= 0;
            }

            return moveDirection;
        }
    }
}