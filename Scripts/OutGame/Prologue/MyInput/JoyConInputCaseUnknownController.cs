using UniRx;
using UnityEngine;

namespace OutGame.Prologue.MyInput
{
	public class JoyConInputCaseUnknownController
	{
		public IReadOnlyReactiveProperty<bool> onNext => _onNext;
		
		private readonly Joycon _joyconRight;
		private readonly ReactiveProperty<bool> _onNext;
		public JoyConInputCaseUnknownController(Joycon joyconRight)
		{
			_onNext = new ReactiveProperty<bool>();
			_joyconRight = joyconRight;
			JoyconManager.Instance.RegisterDelegate(UpdateInput);
		}

		private void UpdateInput()
		{
			if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_RIGHT))
			{
				_onNext.Value = true;
			}

			if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_RIGHT))
			{
				_onNext.Value = false;
			}
		}

	}
}