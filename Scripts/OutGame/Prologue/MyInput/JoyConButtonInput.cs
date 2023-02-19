using System;
using UniRx;

namespace OutGame.Prologue.MyInput
{
	/// <summary>
	/// 会話シーン用コントローラー入力
	/// </summary>
	public class JoyConButtonInput:IButtonInput
	{
		public IObservable<bool> OnButton => _onButton;
		private readonly Joycon _joycon;
		
		//MEMO: Joyconの左右どちらでも取得可能
		private readonly Subject<bool> _onButton;
		private readonly Joycon.Button _onButtonType;
		
		public JoyConButtonInput(Joycon joycon)
		{
			_joycon = joycon;
			_onButton = new Subject<bool>();
			_onButtonType = joycon.isLeft ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP;
			JoyconManager.Instance.updated += UpdateInput;
		}

		private void UpdateInput()
		{
			if (_joycon.GetButtonDown(_onButtonType))
			{
				_onButton.OnNext(true);
			}

		}

		public void Dispose()
		{
			_onButton?.Dispose();
			JoyconManager.Instance.updated -= UpdateInput;
		}
	}
}