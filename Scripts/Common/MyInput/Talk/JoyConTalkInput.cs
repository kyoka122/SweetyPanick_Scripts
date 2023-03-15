using MyApplication;
using Common.MyInput.Talk;
using UnityEngine;
using Utility;

namespace Common.MyInput.Talk
{
	/// <summary>
	/// 会話シーン用コントローラー入力
	/// </summary>
	public class JoyConTalkInput:BaseTalkInput
	{
		private readonly Joycon _joycon;
		
		//MEMO: Joyconの左右どちらでも取得可能
		private Joycon.Button _onNextKey;
		private Joycon.Button _onSkipKey;
		
		public JoyConTalkInput(int deviceId,Joycon joycon) : base(joycon.GetMyInputDeviceType(), deviceId)
		{
			_joycon = joycon;
			Init();
		}
		
		public JoyConTalkInput(MyInputDeviceType type, int deviceId, Joycon joycon) : base(type, deviceId)
		{
			_joycon = joycon;
			Init();
		}

		private void Init()
		{
			_onNextKey = _joycon.isLeft ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP;
			_onSkipKey = _joycon.isLeft ? Joycon.Button.DPAD_LEFT : Joycon.Button.DPAD_RIGHT;
			JoyconManager.Instance.updated += UpdateInput;
		}
		
		private void UpdateInput()
		{
			if (_joycon.GetButtonDown(_onNextKey))
			{
				next.OnNext(true);
			}
			if (_joycon.GetButtonUp(_onNextKey))
			{
				next.OnNext(false);
			}
			
			if (_joycon.GetButtonDown(_onSkipKey))
			{
				skip.OnNext(true);
			}
			if (_joycon.GetButtonUp(_onSkipKey))
			{
				skip.OnNext(false);
			}
		}
		
		public override void Dispose()
		{
			JoyconManager.Instance.updated -= UpdateInput;
			base.Dispose();
		}
	}
}