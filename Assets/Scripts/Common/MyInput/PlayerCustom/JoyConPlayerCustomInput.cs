﻿using MyApplication;
using Utility;

namespace Common.MyInput.PlayerCustom
{
    public class JoyConPlayerCustomInput:BasePlayerCustomInput
    {
        private readonly Joycon _joycon;
        
        private Joycon.Button _upKey;
        private Joycon.Button _downKey;
        private Joycon.Button _rightKey;
        private Joycon.Button _leftKey;
        private int _horizontalPositiveNegativeReverse;
        private int _verticalPositiveNegativeReverse;

        public JoyConPlayerCustomInput(int deviceId,Joycon joycon) : base(joycon.GetMyInputDeviceType(), deviceId)
        {
            _joycon = joycon;
            Init();
        }
        
        public JoyConPlayerCustomInput(MyInputDeviceType type, int deviceId,Joycon joycon)
            : base(type, deviceId)
        {
            _joycon = joycon;
            Init();
        }

        private void Init()
        {
            SetKeyConfig();
            JoyconManager.Instance.updated += UpdateInput;
        }

        private void SetKeyConfig()
        {
            if (_joycon.isLeft)
            {
                _upKey = Joycon.Button.DPAD_RIGHT;
                _downKey = Joycon.Button.DPAD_LEFT;
                _rightKey = Joycon.Button.DPAD_DOWN;
                _leftKey = Joycon.Button.DPAD_UP;
                _horizontalPositiveNegativeReverse = -1;
                _verticalPositiveNegativeReverse = 1;
            }
            else
            {
                _upKey = Joycon.Button.DPAD_LEFT;
                _downKey = Joycon.Button.DPAD_RIGHT;
                _rightKey = Joycon.Button.DPAD_UP;
                _leftKey = Joycon.Button.DPAD_DOWN;
                _horizontalPositiveNegativeReverse = 1;
                _verticalPositiveNegativeReverse = -1;
            }
        }

        private void UpdateInput()
        {
            if (_joycon.GetButtonDown(_downKey))
            {
                back.Value = true;
            }
            if (_joycon.GetButtonUp(_downKey))
            {
                back.Value = false;
            }
            if (_joycon.GetButtonDown(_rightKey))
            {
                next.Value = true;
            }
            if (_joycon.GetButtonUp(_rightKey))
            {
                next.Value = false;
            }
            
            //MEMO: ↓スティック入力
            //MEMO: GetStick()[0]はスティックの左右の入力値
            float newHorizontalValue = _horizontalPositiveNegativeReverse * _joycon.GetStick()[1];
            float newVerticalValue = _verticalPositiveNegativeReverse * _joycon.GetStick()[0];
            
            horizontalValue.Value = newHorizontalValue;
            verticalValue.Value = newVerticalValue;
            horizontalDigitalMoveValue.Value=GetStickDigitalDirection(newHorizontalValue,horizontalDigitalMoveValue.Value);
            verticalDigitalMoveValue.Value = GetStickDigitalDirection(newVerticalValue, verticalDigitalMoveValue.Value);
        }
        
        public override void Dispose()
        {
            JoyconManager.Instance.updated -= UpdateInput;
            base.Dispose();
        }
    }
}