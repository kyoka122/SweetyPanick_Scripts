using System;
using UniRx;

namespace OutGame.PlayerCustom.MyInput
{
    public class JoyConRightInputCaseUnknownController:IDisposable
    {
        public IReadOnlyReactiveProperty<bool> RightControllerSet => rightControllerSet;
        public IReadOnlyReactiveProperty<bool> Next => next;
        public IReadOnlyReactiveProperty<bool> Back => back;
        
        public readonly Joycon _joyconRight;
    
        protected readonly ReactiveProperty<bool> rightControllerSet;
        protected readonly ReactiveProperty<bool> next;
        protected readonly ReactiveProperty<bool> back;

        public JoyConRightInputCaseUnknownController(Joycon joyconRight)
        {
            _joyconRight = joyconRight;
            rightControllerSet = new ReactiveProperty<bool>();
            next = new ReactiveProperty<bool>();
            back = new ReactiveProperty<bool>();
            JoyconManager.Instance.updated += UpdateInput;
        }

        private void UpdateInput()
        {
            
            if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                back.Value = true;
            }
            if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_DOWN))
            {
                back.Value = false;
            }
            if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_RIGHT))
            {
                next.Value = true;
            }
            if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_RIGHT))
            {
                next.Value = false;
            }
            if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_RIGHT))
            {
                rightControllerSet.Value = true;
            }
            if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_RIGHT))
            {
                rightControllerSet.Value = false;
            }

        }

        public void Dispose()
        {
            rightControllerSet?.Dispose();
            next?.Dispose();
            back?.Dispose();
            JoyconManager.Instance.updated -= UpdateInput;
        }
    }
}