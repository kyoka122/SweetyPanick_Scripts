using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace OutGame.Prologue.MyInput
{
    public class InputCaseUnknownController
    {
        public IReadOnlyList<JoyConInputCaseUnknownController> joyconInputs => _joyconInputs;
        private readonly List<JoyConInputCaseUnknownController> _joyconInputs;
        
        public InputCaseUnknownController()
        {
            List<Joycon> joycons= JoyconManager.Instance.j;
            _joyconInputs = new List<JoyConInputCaseUnknownController>();

            foreach (var joycon in joycons.Where(joycon => !joycon.isLeft))
            {
                _joyconInputs.Add(new JoyConInputCaseUnknownController(joycon));
            }
        }
    }
}