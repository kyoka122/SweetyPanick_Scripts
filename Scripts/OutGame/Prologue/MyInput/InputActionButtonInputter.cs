using System;
using MyInput;
using UniRx;
using UnityEngine.InputSystem;

namespace OutGame.Prologue.MyInput
{
    public class InputActionButtonInputter:IButtonInput
    {
        public IObservable<bool> OnButton { get; }

        private Subject<bool> _onButton;

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
            var gameInputMap = new InputMap{devices = newDevices};
            
            gameInputMap.TalkEnter.Enter.started += OnEnter;
            
            gameInputMap.Enable();
        }
        
        private void OnEnter(InputAction.CallbackContext context)
        {
            _onButton.OnNext(true);
        }
    }
}