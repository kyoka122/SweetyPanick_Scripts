using System.Collections.Generic;
using OutGame.PlayerCustom.MyInput;

namespace OutGame.PlayerCustom.Entity
{
    /// <summary>
    /// コントローラー登録前
    /// </summary>
    public class InputCaseUnknownControllerEntity
    {
        public IReadOnlyList<JoyConLeftInputCaseUnknownController> joyconLeftInputs => _joyconLeftInputs;
        public IReadOnlyList<JoyConRightInputCaseUnknownController> joyconRightInputs => _joyconRightInputs;
        private readonly List<JoyConLeftInputCaseUnknownController> _joyconLeftInputs;
        private readonly List<JoyConRightInputCaseUnknownController> _joyconRightInputs;
        
        public InputCaseUnknownControllerEntity()
        {
            List<Joycon> joycons= JoyconManager.Instance.j;
            _joyconLeftInputs = new List<JoyConLeftInputCaseUnknownController>();
            _joyconRightInputs = new List<JoyConRightInputCaseUnknownController>();

            foreach (var joycon in joycons)
            {
                if (joycon.isLeft)
                {
                    _joyconLeftInputs.Add(new JoyConLeftInputCaseUnknownController(joycon));
                }
                else
                {
                    _joyconRightInputs.Add(new JoyConRightInputCaseUnknownController(joycon));
                }
            }
        }
    }
}