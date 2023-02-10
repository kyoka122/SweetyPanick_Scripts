using MyInput;
using UniRx;
using UnityEngine.InputSystem;

namespace DebugInput
{
    //MEMO: 参考：https://nekojara.city/unity-input-system-actions
    public class DebugCharacterChanger
    {
        public IReadOnlyReactiveProperty<int> PlayerNum => _playerNum;
        public int playerNum => _playerNum.Value;
        
        private readonly ReactiveProperty<int> _playerNum;
        private DebugInputMap _gameInputMap;

        public DebugCharacterChanger()
        {
            // Input Actionインスタンス生成
            _gameInputMap = new DebugInputMap();
            _playerNum = new ReactiveProperty<int>(1);
            
            // Actionイベント登録
            _gameInputMap.Player.Player1.started += SwitchPlayer1;
            _gameInputMap.Player.Player2.started += SwitchPlayer2;
            _gameInputMap.Player.Player3.started += SwitchPlayer3;
            _gameInputMap.Player.Player4.started += SwitchPlayer4;

            // Input Actionを機能させるためには、
            // 有効化する必要がある
            _gameInputMap.Enable();
        }
        
        private void SwitchPlayer1(InputAction.CallbackContext context)
        {
            _playerNum.Value = 1;
        }

        private void SwitchPlayer2(InputAction.CallbackContext context)
        {
            _playerNum.Value = 2;
        }

        private void SwitchPlayer3(InputAction.CallbackContext context)
        {
            _playerNum.Value = 3;
        }

        private void SwitchPlayer4(InputAction.CallbackContext context)
        {
            _playerNum.Value = 4;
        }
    }
}