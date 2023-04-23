using MyApplication;
using Utility;

namespace Common.MyInput.Score
{
    public class JoyconScoreInput:BaseScoreInput
    {
        private readonly Joycon _joycon;
        
        private Joycon.Button _rightKey;

        public JoyconScoreInput(int deviceId,Joycon joycon) : base(joycon.GetMyInputDeviceType(), deviceId)
        {
            _joycon = joycon;
            Init();
        }
        
        public JoyconScoreInput(MyInputDeviceType type, int deviceId,Joycon joycon) : base(type, deviceId)
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
            _rightKey = _joycon.isLeft ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP;
        }

        private void UpdateInput()
        {
            if (_joycon.GetButtonDown(_rightKey))
            {
                next.Value = true;
            }
            if (_joycon.GetButtonUp(_rightKey))
            {
                next.Value = false;
            }
        }
        
        public override void Dispose()
        {
            JoyconManager.Instance.updated -= UpdateInput;
            base.Dispose();
        }
    }
}