using MyApplication;
using Utility;

namespace Common.MyInput.Title
{
    public class JoyconTitleInput:BaseTitleInput
    { 
        private readonly Joycon _joycon;
        
        private Joycon.Button _upKey;
        private Joycon.Button _rightKey;
        private int _verticalPositiveNegativeReverse;

        public JoyconTitleInput(int deviceId,Joycon joycon) : base(joycon.GetMyInputDeviceType(), deviceId)
        {
            _joycon = joycon;
            Init();
        }
        
        public JoyconTitleInput(MyInputDeviceType type, int deviceId,Joycon joycon) : base(type, deviceId)
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
                _rightKey = Joycon.Button.DPAD_DOWN;
                _verticalPositiveNegativeReverse = 1;
            }
            else
            {
                _upKey = Joycon.Button.DPAD_LEFT;
                _rightKey = Joycon.Button.DPAD_UP;
                _verticalPositiveNegativeReverse = -1;
            }
        }

        private void UpdateInput()
        {
            if (_joycon.GetButtonDown(_upKey))
            {
                onCredit.Value = !onCredit.Value;
            }
            if (_joycon.GetButtonDown(_rightKey))
            {
                onNext.Value = true;
            }
            if (_joycon.GetButtonUp(_rightKey))
            {
                onNext.Value = false;
            }
            
            //MEMO: ↓スティック入力
            //MEMO: GetStick()[0]はスティックの左右の入力値
            float newVerticalValue = _verticalPositiveNegativeReverse * _joycon.GetStick()[0];
            
            scrollValue.Value = newVerticalValue;
        }
        
        public override void Dispose()
        {
            JoyconManager.Instance.updated -= UpdateInput;
            base.Dispose();
        }
    }
}