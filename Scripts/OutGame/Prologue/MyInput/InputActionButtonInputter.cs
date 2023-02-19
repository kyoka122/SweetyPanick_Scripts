using System;
using MyInput;
using UniRx;
using UnityEngine.InputSystem;

namespace OutGame.Prologue.MyInput
{
    public class InputActionButtonInputter:IButtonInput
    {
        public IObservable<bool> OnButton => _onButton;

        private Subject<bool> _onButton;
        private InputMap _inputMap;

        public InputActionButtonInputter(InputDevice newDevice)
        {
            Init(new[]{newDevice});
        }
        
        /// <param name="newDevices">デバイスが対になっている場合は配列にして引数にいれる</param>
        public InputActionButtonInputter(InputDevice[] newDevices)
        {
            Init(newDevices);
        }

        private void Init(InputDevice[] newDevices)
        {
            _onButton = new Subject<bool>();
            _inputMap = new InputMap{devices = newDevices};
            
            _inputMap.TalkEnter.Enter.started += OnEnter;
            
            _inputMap.Enable();
        }
        
        private void OnEnter(InputAction.CallbackContext context)
        {
            _onButton.OnNext(true);
        }

        public void Dispose()
        {
            _inputMap.TalkEnter.Enter.started -= OnEnter;
            _onButton?.Dispose();
        }
    }
}